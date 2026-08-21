// <copyright file="NoParkingRoadOverlaySystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Draws side-specific No Parking previews using the actual parking
// lane curve plus the road's EdgeGeometry for an available-action perimeter.

namespace ParkingControl
{
    using Colossal.Mathematics;
    using Game;
    using Game.Rendering;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using UnityEngine;

    public sealed partial class NoParkingRoadOverlaySystem : GameSystemBase
    {
        private const float kSidePreviewWidth = 3.8f;
        private const float kSideOutlineWidth = 0.14f;
        private const float kRoadPerimeterWidth = 0.32f;

        // Available side: cyan targeting strip.
        private static readonly Color s_AvailableFill =
            new(0.10f, 0.80f, 1.00f, 0.50f);

        private static readonly Color s_AvailableOutline =
            new(0.10f, 0.80f, 1.00f, 0.78f);

        // Already banned side: RMB will remove No Parking.
        private static readonly Color s_AppliedFill =
            new(1.00f, 0.18f, 0.12f, 0.50f);

        private static readonly Color s_AppliedOutline =
            new(1.00f, 0.18f, 0.12f, 0.78f);

        private OverlayRenderSystem m_OverlayRenderSystem = null!;
        private RenderingSystem m_RenderingSystem = null!;
        private NoParkingRoadToolSystem m_ToolSystem = null!;
        private EntityQuery m_RenderSettingsQuery;

        [BurstCompile]
        private struct DrawPreviewJob : IJob
        {
            [ReadOnly]
            public NativeArray<Bezier4x3> SideCurves;

            [ReadOnly]
            public NativeArray<Bezier4x3> PerimeterCurves;

            public Color SideOutlineColor;
            public Color SideFillColor;
            public Color PerimeterColor;
            public bool DrawPerimeter;

            public OverlayRenderSystem.Buffer OverlayBuffer;

            public void Execute()
            {
                for (int index = 0; index < SideCurves.Length; index++)
                {
                    // Node Controller uses this richer DrawCurve overload:
                    // separate outline/fill colors and width. It gives the
                    // target strip a cleaner edge without hiding the road.
                    OverlayBuffer.DrawCurve(
                        SideOutlineColor,
                        SideFillColor,
                        kSideOutlineWidth,
                        (OverlayRenderSystem.StyleFlags)0,
                        SideCurves[index],
                        kSidePreviewWidth,
                        new float2(0.25f, 0.25f));
                }

                if (!DrawPerimeter)
                {
                    return;
                }

                for (int index = 0; index < PerimeterCurves.Length; index++)
                {
                    OverlayBuffer.DrawCurve(
                        PerimeterColor,
                        PerimeterCurves[index],
                        kRoadPerimeterWidth);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();

            m_OverlayRenderSystem =
                World.GetOrCreateSystemManaged<OverlayRenderSystem>();

            m_RenderingSystem =
                World.GetOrCreateSystemManaged<RenderingSystem>();

            m_ToolSystem =
                World.GetOrCreateSystemManaged<NoParkingRoadToolSystem>();

            m_RenderSettingsQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Prefabs.RenderingSettingsData>()
                .Build();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (!m_ToolSystem.IsToolActive ||
                m_RenderingSystem.hideOverlay)
            {
                return;
            }

            Entity road = m_ToolSystem.PreviewRoad;

            if (road == Entity.Null ||
                !EntityManager.Exists(road) ||
                !EntityManager.HasBuffer<Game.Net.SubLane>(road))
            {
                return;
            }

            bool rightSide = m_ToolSystem.PreviewRightSide;
            bool alreadyBanned = m_ToolSystem.PreviewRemoving;

            DynamicBuffer<Game.Net.SubLane> subLanes =
                EntityManager.GetBuffer<Game.Net.SubLane>(
                    road,
                    isReadOnly: true);

            NativeList<Bezier4x3> sideCurves =
                new(Allocator.TempJob);

            NativeList<Bezier4x3> perimeterCurves =
                new(Allocator.TempJob);

            try
            {
                for (int index = 0; index < subLanes.Length; index++)
                {
                    Entity lane = subLanes[index].m_SubLane;

                    if (!NoParkingRoadToolSystem.TryGetEligibleParkingLane(
                            EntityManager,
                            road,
                            lane,
                            out Game.Net.ParkingLane parkingLane,
                            out Game.Net.Curve curve))
                    {
                        continue;
                    }

                    bool laneRightSide =
                        (parkingLane.m_Flags &
                            Game.Net.ParkingLaneFlags.RightSide) != 0;

                    if (laneRightSide == rightSide)
                    {
                        sideCurves.Add(curve.m_Bezier);
                    }
                }

                if (sideCurves.Length == 0)
                {
                    sideCurves.Dispose();
                    perimeterCurves.Dispose();
                    return;
                }

                bool drawPerimeter = false;
                Color perimeterColor = default;

                // The perimeter means "No Parking can be added here".
                // Once already applied, the perimeter disappears and the
                // target strip turns red instead.
                if (!alreadyBanned &&
                    EntityManager.HasComponent<Game.Net.EdgeGeometry>(road) &&
                    TryGetOwnerColor(out perimeterColor))
                {
                    Game.Net.EdgeGeometry edgeGeometry =
                        EntityManager.GetComponentData<Game.Net.EdgeGeometry>(road);

                    // EdgeGeometry exposes the real road boundaries as
                    // left/right Beziers for the start and end halves.
                    // This follows curves, asymmetric roads, and custom
                    // Road Builder geometry better than a center-line guess.
                    perimeterCurves.Add(edgeGeometry.m_Start.m_Left);
                    perimeterCurves.Add(edgeGeometry.m_Start.m_Right);
                    perimeterCurves.Add(edgeGeometry.m_End.m_Left);
                    perimeterCurves.Add(edgeGeometry.m_End.m_Right);

                    drawPerimeter = true;
                }

                Color sideFill =
                    alreadyBanned
                        ? s_AppliedFill
                        : s_AvailableFill;

                Color sideOutline =
                    alreadyBanned
                        ? s_AppliedOutline
                        : s_AvailableOutline;

                OverlayRenderSystem.Buffer buffer =
                    m_OverlayRenderSystem.GetBuffer(
                        out JobHandle dependencies);

                JobHandle drawHandle =
                    new DrawPreviewJob
                    {
                        SideCurves = sideCurves.AsArray(),
                        PerimeterCurves = perimeterCurves.AsArray(),
                        SideOutlineColor = sideOutline,
                        SideFillColor = sideFill,
                        PerimeterColor = perimeterColor,
                        DrawPerimeter = drawPerimeter,
                        OverlayBuffer = buffer,
                    }
                    .Schedule(
                        JobHandle.CombineDependencies(
                            Dependency,
                            dependencies));

                m_OverlayRenderSystem.AddBufferWriter(drawHandle);

                JobHandle disposeSide =
                    sideCurves.Dispose(drawHandle);

                JobHandle disposePerimeter =
                    perimeterCurves.Dispose(drawHandle);

                Dependency =
                    JobHandle.CombineDependencies(
                        disposeSide,
                        disposePerimeter);
            }
            catch
            {
                if (sideCurves.IsCreated)
                {
                    sideCurves.Dispose();
                }

                if (perimeterCurves.IsCreated)
                {
                    perimeterCurves.Dispose();
                }

                throw;
            }
        }

        private bool TryGetOwnerColor(out Color ownerColor)
        {
            ownerColor = default;

            if (m_RenderSettingsQuery.IsEmptyIgnoreFilter)
            {
                return false;
            }

            Game.Prefabs.RenderingSettingsData renderingSettings =
                m_RenderSettingsQuery
                    .GetSingleton<Game.Prefabs.RenderingSettingsData>();

            ownerColor = renderingSettings.m_OwnerColor;
            return ownerColor.a > 0f;
        }
    }
}
