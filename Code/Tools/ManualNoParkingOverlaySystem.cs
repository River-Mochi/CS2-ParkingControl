// <copyright file="ManualNoParkingOverlaySystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Draws Manual No Parking side previews and the available-action road perimeter.

namespace ParkingControl
{
    using System.Collections.Generic;
    using Colossal.Mathematics;
    using Game;
    using Game.Rendering;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using UnityEngine;

    public sealed partial class ManualNoParkingOverlaySystem : GameSystemBase
    {
        private const float kSidePreviewWidth = 3.8f;
        private const float kSideOutlineWidth = 0.12f;
        private const float kRoadPerimeterWidth = 0.30f;
        private const float kSideFillAlpha = 0.40f;
        private const float kSideOutlineAlpha = 0.78f;

        private readonly HashSet<Entity> m_PerimeterRoads = new();

        private OverlayRenderSystem m_OverlayRenderSystem = null!;
        private RenderingSystem m_RenderingSystem = null!;
        private ManualNoParkingToolSystem m_ToolSystem = null!;
        private EntityQuery m_RenderSettingsQuery;

        [BurstCompile]
        private struct DrawPreviewJob : IJob
        {
            [ReadOnly]
            public NativeArray<Bezier4x3> AvailableSideCurves;

            [ReadOnly]
            public NativeArray<Bezier4x3> AppliedSideCurves;

            [ReadOnly]
            public NativeArray<Bezier4x3> PerimeterCurves;

            public Color AvailableOutlineColor;
            public Color AvailableFillColor;
            public Color AppliedOutlineColor;
            public Color AppliedFillColor;
            public Color PerimeterColor;

            public OverlayRenderSystem.Buffer OverlayBuffer;

            public void Execute()
            {
                for (int index = 0; index < AvailableSideCurves.Length; index++)
                {
                    OverlayBuffer.DrawCurve(
                        AvailableOutlineColor,
                        AvailableFillColor,
                        kSideOutlineWidth,
                        (OverlayRenderSystem.StyleFlags)0,
                        AvailableSideCurves[index],
                        kSidePreviewWidth,
                        new float2(0.25f, 0.25f));
                }

                for (int index = 0; index < AppliedSideCurves.Length; index++)
                {
                    OverlayBuffer.DrawCurve(
                        AppliedOutlineColor,
                        AppliedFillColor,
                        kSideOutlineWidth,
                        (OverlayRenderSystem.StyleFlags)0,
                        AppliedSideCurves[index],
                        kSidePreviewWidth,
                        new float2(0.25f, 0.25f));
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

        protected override void OnCreate()
        {
            base.OnCreate();

            m_OverlayRenderSystem =
                World.GetOrCreateSystemManaged<OverlayRenderSystem>();

            m_RenderingSystem =
                World.GetOrCreateSystemManaged<RenderingSystem>();

            m_ToolSystem =
                World.GetOrCreateSystemManaged<ManualNoParkingToolSystem>();

            m_RenderSettingsQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Prefabs.RenderingSettingsData>()
                .Build();
        }

        protected override void OnUpdate()
        {
            if (!m_ToolSystem.IsToolActive ||
                m_RenderingSystem.hideOverlay)
            {
                return;
            }

            IReadOnlyList<ManualNoParkingToolSystem.RoadSideSelection> selections =
                m_ToolSystem.SelectedSides;

            Entity previewRoad = m_ToolSystem.PreviewRoad;

            if (previewRoad == Entity.Null &&
                selections.Count == 0)
            {
                return;
            }

            if (!TryGetPreviewColors(
                    out Color availableOutline,
                    out Color availableFill,
                    out Color appliedOutline,
                    out Color appliedFill,
                    out Color perimeterColor))
            {
                return;
            }

            NativeList<Bezier4x3> availableCurves =
                new(Allocator.TempJob);

            NativeList<Bezier4x3> appliedCurves =
                new(Allocator.TempJob);

            NativeList<Bezier4x3> perimeterCurves =
                new(Allocator.TempJob);

            m_PerimeterRoads.Clear();

            try
            {
                for (int index = 0; index < selections.Count; index++)
                {
                    ManualNoParkingToolSystem.RoadSideSelection selection =
                        selections[index];

                    AppendRoadSide(
                        selection.Road,
                        selection.RightSide,
                        selection.Removing,
                        availableCurves,
                        appliedCurves,
                        perimeterCurves);
                }

                if (previewRoad != Entity.Null &&
                    EntityManager.Exists(previewRoad) &&
                    !m_ToolSystem.IsSelected(
                        previewRoad,
                        m_ToolSystem.PreviewRightSide))
                {
                    AppendRoadSide(
                        previewRoad,
                        m_ToolSystem.PreviewRightSide,
                        m_ToolSystem.PreviewRemoving,
                        availableCurves,
                        appliedCurves,
                        perimeterCurves);
                }

                if (availableCurves.Length == 0 &&
                    appliedCurves.Length == 0 &&
                    perimeterCurves.Length == 0)
                {
                    availableCurves.Dispose();
                    appliedCurves.Dispose();
                    perimeterCurves.Dispose();
                    return;
                }

                OverlayRenderSystem.Buffer buffer =
                    m_OverlayRenderSystem.GetBuffer(
                        out JobHandle dependencies);

                JobHandle drawHandle =
                    new DrawPreviewJob
                    {
                        AvailableSideCurves = availableCurves.AsArray(),
                        AppliedSideCurves = appliedCurves.AsArray(),
                        PerimeterCurves = perimeterCurves.AsArray(),
                        AvailableOutlineColor = availableOutline,
                        AvailableFillColor = availableFill,
                        AppliedOutlineColor = appliedOutline,
                        AppliedFillColor = appliedFill,
                        PerimeterColor = perimeterColor,
                        OverlayBuffer = buffer,
                    }
                    .Schedule(
                        JobHandle.CombineDependencies(
                            Dependency,
                            dependencies));

                m_OverlayRenderSystem.AddBufferWriter(drawHandle);

                JobHandle disposeAvailable =
                    availableCurves.Dispose(drawHandle);

                JobHandle disposeApplied =
                    appliedCurves.Dispose(drawHandle);

                JobHandle disposePerimeter =
                    perimeterCurves.Dispose(drawHandle);

                Dependency =
                    JobHandle.CombineDependencies(
                        disposeAvailable,
                        disposeApplied,
                        disposePerimeter);
            }
            catch
            {
                if (availableCurves.IsCreated)
                {
                    availableCurves.Dispose();
                }

                if (appliedCurves.IsCreated)
                {
                    appliedCurves.Dispose();
                }

                if (perimeterCurves.IsCreated)
                {
                    perimeterCurves.Dispose();
                }

                throw;
            }
        }

        private void AppendRoadSide(
            Entity road,
            bool rightSide,
            bool removing,
            NativeList<Bezier4x3> availableCurves,
            NativeList<Bezier4x3> appliedCurves,
            NativeList<Bezier4x3> perimeterCurves)
        {
            if (road == Entity.Null ||
                !EntityManager.Exists(road) ||
                !EntityManager.HasBuffer<Game.Net.SubLane>(road))
            {
                return;
            }

            DynamicBuffer<Game.Net.SubLane> subLanes =
                EntityManager.GetBuffer<Game.Net.SubLane>(
                    road,
                    isReadOnly: true);

            bool foundSide = false;

            for (int index = 0; index < subLanes.Length; index++)
            {
                Entity lane = subLanes[index].m_SubLane;

                if (!ManualNoParkingToolSystem.TryGetEligibleParkingLane(
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

                if (laneRightSide != rightSide)
                {
                    continue;
                }

                foundSide = true;

                if (removing)
                {
                    appliedCurves.Add(curve.m_Bezier);
                }
                else
                {
                    availableCurves.Add(curve.m_Bezier);
                }
            }

            if (!foundSide ||
                removing ||
                m_PerimeterRoads.Contains(road) ||
                !EntityManager.HasComponent<Game.Net.EdgeGeometry>(road))
            {
                return;
            }

            Game.Net.EdgeGeometry edgeGeometry =
                EntityManager.GetComponentData<Game.Net.EdgeGeometry>(road);

            perimeterCurves.Add(edgeGeometry.m_Start.m_Left);
            perimeterCurves.Add(edgeGeometry.m_Start.m_Right);
            perimeterCurves.Add(edgeGeometry.m_End.m_Left);
            perimeterCurves.Add(edgeGeometry.m_End.m_Right);

            // Close the road perimeter at both ends. EdgeGeometry's start
            // half uses .a at the start node; the end half uses .d at the end.
            perimeterCurves.Add(
                MakeLineCurve(
                    edgeGeometry.m_Start.m_Left.a,
                    edgeGeometry.m_Start.m_Right.a));

            perimeterCurves.Add(
                MakeLineCurve(
                    edgeGeometry.m_End.m_Left.d,
                    edgeGeometry.m_End.m_Right.d));

            m_PerimeterRoads.Add(road);
        }

        private bool TryGetPreviewColors(
            out Color availableOutline,
            out Color availableFill,
            out Color appliedOutline,
            out Color appliedFill,
            out Color perimeterColor)
        {
            availableOutline = default;
            availableFill = default;
            appliedOutline = default;
            appliedFill = default;
            perimeterColor = default;

            if (m_RenderSettingsQuery.IsEmptyIgnoreFilter)
            {
                return false;
            }

            Game.Prefabs.RenderingSettingsData renderingSettings =
                m_RenderSettingsQuery
                    .GetSingleton<Game.Prefabs.RenderingSettingsData>();

            // Available side follows the game's current HoveredColor. If
            // Hover Colors changes that global value, PC follows it too.
            availableOutline = renderingSettings.m_HoveredColor;
            availableFill = renderingSettings.m_HoveredColor;
            availableOutline.a = kSideOutlineAlpha;
            availableFill.a = kSideFillAlpha;

            // Applied side uses the game's WarningColor rather than a PC-only
            // hard-coded red. This matches the same warning palette Hover
            // Colors uses for its recommended bulldozer behavior.
            appliedOutline = renderingSettings.m_WarningColor;
            appliedFill = renderingSettings.m_WarningColor;
            appliedOutline.a = kSideOutlineAlpha;
            appliedFill.a = kSideFillAlpha;

            // "Can apply here" road perimeter follows current OwnerColor.
            perimeterColor = renderingSettings.m_OwnerColor;

            return availableOutline.a > 0f ||
                appliedOutline.a > 0f ||
                perimeterColor.a > 0f;
        }

        private static Bezier4x3 MakeLineCurve(
            float3 start,
            float3 end)
        {
            Bezier4x3 curve = default;

            curve.a = start;
            curve.b = math.lerp(start, end, 1f / 3f);
            curve.c = math.lerp(start, end, 2f / 3f);
            curve.d = end;

            return curve;
        }
    }
}
