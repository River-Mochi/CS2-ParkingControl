// <copyright file="NoParkingRoadOverlaySystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Draws a side-specific preview for the No Parking road tool.

namespace ParkingControl
{
    using Colossal.Mathematics;
    using Game;
    using Game.Net;
    using Game.Rendering;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using UnityEngine;

    public sealed partial class NoParkingRoadOverlaySystem : GameSystemBase
    {
        private const float kPreviewWidth = 3.8f;

        private static readonly Color s_AddColor =
            new(0.10f, 0.80f, 1.00f, 0.60f);

        private static readonly Color s_RemoveColor =
            new(1.00f, 0.15f, 0.15f, 0.60f);

        private OverlayRenderSystem m_OverlayRenderSystem = null!;
        private RenderingSystem m_RenderingSystem = null!;
        private NoParkingRoadToolSystem m_ToolSystem = null!;

        [BurstCompile]
        private struct DrawPreviewJob : IJob
        {
            [ReadOnly]
            public NativeArray<Bezier4x3> Curves;

            public OverlayRenderSystem.Buffer OverlayBuffer;
            public Color PreviewColor;

            public void Execute()
            {
                for (int index = 0; index < Curves.Length; index++)
                {
                    OverlayBuffer.DrawCurve(
                        PreviewColor,
                        Curves[index],
                        kPreviewWidth);
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
                !EntityManager.HasBuffer<SubLane>(road))
            {
                return;
            }

            bool rightSide = m_ToolSystem.PreviewRightSide;

            DynamicBuffer<SubLane> subLanes =
                EntityManager.GetBuffer<SubLane>(
                    road,
                    isReadOnly: true);

            NativeList<Bezier4x3> curves =
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
                            out ParkingLane parkingLane,
                            out Curve curve))
                    {
                        continue;
                    }

                    bool laneRightSide =
                        (parkingLane.m_Flags &
                            ParkingLaneFlags.RightSide) != 0;

                    if (laneRightSide == rightSide)
                    {
                        curves.Add(curve.m_Bezier);
                    }
                }

                if (curves.Length == 0)
                {
                    curves.Dispose();
                    return;
                }

                Color previewColor =
                    m_ToolSystem.PreviewRemoving
                        ? s_RemoveColor
                        : s_AddColor;

                OverlayRenderSystem.Buffer buffer =
                    m_OverlayRenderSystem.GetBuffer(
                        out JobHandle dependencies);

                JobHandle drawHandle =
                    new DrawPreviewJob
                    {
                        Curves = curves.AsArray(),
                        OverlayBuffer = buffer,
                        PreviewColor = previewColor,
                    }
                    .Schedule(
                        JobHandle.CombineDependencies(
                            Dependency,
                            dependencies));

                m_OverlayRenderSystem.AddBufferWriter(drawHandle);
                Dependency = curves.Dispose(drawHandle);
            }
            catch
            {
                if (curves.IsCreated)
                {
                    curves.Dispose();
                }

                throw;
            }
        }
    }
}
