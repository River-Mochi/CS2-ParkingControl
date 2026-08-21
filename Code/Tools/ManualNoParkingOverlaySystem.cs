// <copyright file="ManualNoParkingOverlaySystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Draws Manual No Parking side previews and add/remove road perimeters.

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
            public NativeArray<Bezier4x3> AvailablePerimeterCurves;

            [ReadOnly]
            public NativeArray<Bezier4x3> AppliedPerimeterCurves;

            public Color AvailableOutlineColor;
            public Color AvailableFillColor;
            public Color AppliedOutlineColor;
            public Color AppliedFillColor;
            public Color AvailablePerimeterColor;
            public Color AppliedPerimeterColor;

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

                for (int index = 0; index < AvailablePerimeterCurves.Length; index++)
                {
                    OverlayBuffer.DrawCurve(
                        AvailablePerimeterColor,
                        AvailablePerimeterCurves[index],
                        kRoadPerimeterWidth);
                }

                for (int index = 0; index < AppliedPerimeterCurves.Length; index++)
                {
                    OverlayBuffer.DrawCurve(
                        AppliedPerimeterColor,
                        AppliedPerimeterCurves[index],
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
                    out Color availablePerimeterColor,
                    out Color appliedPerimeterColor))
            {
                return;
            }

            NativeList<Bezier4x3> availableCurves =
                new(Allocator.TempJob);

            NativeList<Bezier4x3> appliedCurves =
                new(Allocator.TempJob);

            NativeList<Bezier4x3> availablePerimeterCurves =
                new(Allocator.TempJob);

            NativeList<Bezier4x3> appliedPerimeterCurves =
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
                        availablePerimeterCurves,
                        appliedPerimeterCurves);
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
                        availablePerimeterCurves,
                        appliedPerimeterCurves);
                }

                if (availableCurves.Length == 0 &&
                    appliedCurves.Length == 0 &&
                    availablePerimeterCurves.Length == 0 &&
                    appliedPerimeterCurves.Length == 0)
                {
                    availableCurves.Dispose();
                    appliedCurves.Dispose();
                    availablePerimeterCurves.Dispose();
                    appliedPerimeterCurves.Dispose();
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
                        AvailablePerimeterCurves = availablePerimeterCurves.AsArray(),
                        AppliedPerimeterCurves = appliedPerimeterCurves.AsArray(),
                        AvailableOutlineColor = availableOutline,
                        AvailableFillColor = availableFill,
                        AppliedOutlineColor = appliedOutline,
                        AppliedFillColor = appliedFill,
                        AvailablePerimeterColor = availablePerimeterColor,
                        AppliedPerimeterColor = appliedPerimeterColor,
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

                JobHandle disposeAvailablePerimeter =
                    availablePerimeterCurves.Dispose(drawHandle);

                JobHandle disposeAppliedPerimeter =
                    appliedPerimeterCurves.Dispose(drawHandle);

               JobHandle disposeSideCurves =
                    JobHandle.CombineDependencies(
                        disposeAvailable,
                        disposeApplied);

               JobHandle disposePerimeterCurves =
                    JobHandle.CombineDependencies(
                        disposeAvailablePerimeter,
                        disposeAppliedPerimeter);

                Dependency =
                    JobHandle.CombineDependencies(
                        disposeSideCurves,
                        disposePerimeterCurves);
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

                if (availablePerimeterCurves.IsCreated)
                {
                    availablePerimeterCurves.Dispose();
                }

                if (appliedPerimeterCurves.IsCreated)
                {
                    appliedPerimeterCurves.Dispose();
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
            NativeList<Bezier4x3> availablePerimeterCurves,
            NativeList<Bezier4x3> appliedPerimeterCurves)
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
                m_PerimeterRoads.Contains(road) ||
                !EntityManager.HasComponent<Game.Net.EdgeGeometry>(road))
            {
                return;
            }

            Game.Net.EdgeGeometry edgeGeometry =
                EntityManager.GetComponentData<Game.Net.EdgeGeometry>(road);

            NativeList<Bezier4x3> targetPerimeter =
                removing
                    ? appliedPerimeterCurves
                    : availablePerimeterCurves;

            targetPerimeter.Add(edgeGeometry.m_Start.m_Left);
            targetPerimeter.Add(edgeGeometry.m_Start.m_Right);
            targetPerimeter.Add(edgeGeometry.m_End.m_Left);
            targetPerimeter.Add(edgeGeometry.m_End.m_Right);

            // Close the road perimeter at both ends. EdgeGeometry's start
            // half uses .a at the start node; the end half uses .d at the end.
            targetPerimeter.Add(
                MakeLineCurve(
                    edgeGeometry.m_Start.m_Left.a,
                    edgeGeometry.m_Start.m_Right.a));

            targetPerimeter.Add(
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
            out Color availablePerimeterColor,
            out Color appliedPerimeterColor)
        {
            availableOutline = default;
            availableFill = default;
            appliedOutline = default;
            appliedFill = default;
            availablePerimeterColor = default;
            appliedPerimeterColor = default;

            if (m_RenderSettingsQuery.IsEmptyIgnoreFilter)
            {
                return false;
            }

            Game.Prefabs.RenderingSettingsData renderingSettings =
                m_RenderSettingsQuery
                    .GetSingleton<Game.Prefabs.RenderingSettingsData>();

            // Available side follows the game's current HoveredColor. If
            // Mochi's Hover Colors mod changes that global value, PC follows it too.
            availableOutline = renderingSettings.m_HoveredColor;
            availableFill = renderingSettings.m_HoveredColor;
            availableOutline.a = kSideOutlineAlpha;
            availableFill.a = kSideFillAlpha;

            // Applied side uses the game's WarningColor rather than a PC-only
            // hard-coded red. This matches the same warning palette Hover
            // Colors mod uses for its recommended bulldozer behavior.
            appliedOutline = renderingSettings.m_WarningColor;
            appliedFill = renderingSettings.m_WarningColor;
            appliedOutline.a = kSideOutlineAlpha;
            appliedFill.a = kSideFillAlpha;

            // Available road perimeter follows current OwnerColor.
            availablePerimeterColor = renderingSettings.m_OwnerColor;

            // Already-applied road perimeter matches the WarningColor strip.
            appliedPerimeterColor = renderingSettings.m_WarningColor;

            return availableOutline.a > 0f ||
                appliedOutline.a > 0f ||
                availablePerimeterColor.a > 0f ||
                appliedPerimeterColor.a > 0f;
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
