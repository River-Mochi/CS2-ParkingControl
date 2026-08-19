// <copyright file="NoParkingRoadToolSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Lets the player toggle No Parking on one side of an existing road.

namespace ParkingControl
{
    using Colossal.Mathematics;
    using Game.Common;
    using Game.Net;
    using Game.Tools;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;

    public sealed partial class NoParkingRoadToolSystem : ToolBaseSystem
    {
        internal const string kToolId = "ParkingControl.NoParking";

        private Game.Prefabs.PrefabBase? m_ToolPrefab;
        private Entity m_PreviewRoad;
        private bool m_PreviewRightSide;
        private bool m_PreviewRemoving;

        /// <inheritdoc/>
        public override string toolID => kToolId;

        /// <summary>
        /// Gets whether this tool is currently active.
        /// </summary>
        internal bool IsToolActive => Enabled;

        /// <summary>
        /// Gets the road currently previewed by the tool.
        /// </summary>
        internal Entity PreviewRoad => m_PreviewRoad;

        /// <summary>
        /// Gets whether the preview represents the parking lane's right side.
        /// </summary>
        internal bool PreviewRightSide => m_PreviewRightSide;

        /// <summary>
        /// Gets whether clicking will remove an existing manual ban.
        /// </summary>
        internal bool PreviewRemoving => m_PreviewRemoving;

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();

            ClearPreview();
        }

        /// <inheritdoc/>
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            applyAction.shouldBeEnabled = true;
            secondaryApplyAction.shouldBeEnabled = false;
            cancelAction.shouldBeEnabled = true;

            requireNet = Layer.Road;
            allowUnderground = false;

            ClearPreview();
        }

        /// <inheritdoc/>
        protected override void OnStopRunning()
        {
            applyAction.shouldBeEnabled = false;
            secondaryApplyAction.shouldBeEnabled = false;
            cancelAction.shouldBeEnabled = false;

            requireNet = Layer.None;
            allowUnderground = false;

            ClearPreview();

            base.OnStopRunning();
        }

        /// <inheritdoc/>
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (cancelAction.WasPressedThisFrame())
            {
                ClearPreview();

                if (m_ToolSystem.activeTool == this)
                {
                    m_ToolSystem.activeTool = m_DefaultToolSystem;
                }

                return inputDeps;
            }

            if (!TryGetRoadSideUnderCursor(
                    out Entity road,
                    out bool rightSide))
            {
                ClearPreview();
                return inputDeps;
            }

            m_PreviewRoad = road;
            m_PreviewRightSide = rightSide;
            m_PreviewRemoving = IsManualBanSet(road, rightSide);

            if (!applyAction.WasPressedThisFrame())
            {
                return inputDeps;
            }

            ToggleManualBan(road, rightSide);

            // Refresh immediately so the overlay changes cyan -> red,
            // or red -> cyan, in the same interaction.
            m_PreviewRemoving = IsManualBanSet(road, rightSide);

            return inputDeps;
        }

        /// <inheritdoc/>
        public override Game.Prefabs.PrefabBase? GetPrefab()
        {
            return m_ToolPrefab;
        }

        /// <inheritdoc/>
        public override bool TrySetPrefab(Game.Prefabs.PrefabBase prefab)
        {
            if (prefab == null ||
                !string.Equals(
                    prefab.name,
                    kToolId,
                    System.StringComparison.Ordinal))
            {
                return false;
            }

            m_ToolPrefab = prefab;
            return true;
        }

        /// <inheritdoc/>
        public override void InitializeRaycast()
        {
            base.InitializeRaycast();

            m_ToolRaycastSystem.typeMask = TypeMask.Net;
            m_ToolRaycastSystem.netLayerMask = Layer.Road;
        }

        private bool TryGetRoadSideUnderCursor(
            out Entity road,
            out bool rightSide)
        {
            road = Entity.Null;
            rightSide = false;

            if (!GetRaycastResult(
                    out Entity hitEntity,
                    out RaycastHit hit))
            {
                return false;
            }

            road = ResolveRoadEntity(hitEntity);
            if (road == Entity.Null ||
                !EntityManager.Exists(road) ||
                !EntityManager.HasComponent<Road>(road) ||
                !EntityManager.HasComponent<Curve>(road) ||
                !EntityManager.HasBuffer<SubLane>(road))
            {
                road = Entity.Null;
                return false;
            }

            Curve roadCurve =
                EntityManager.GetComponentData<Curve>(road);

            float curvePosition =
                math.clamp(hit.m_CurvePosition, 0f, 1f);

            float3 roadPosition =
                MathUtils.Position(
                    roadCurve.m_Bezier,
                    curvePosition);

            float2 tangent =
                math.normalizesafe(
                    MathUtils.Tangent(
                        roadCurve.m_Bezier,
                        curvePosition).xz,
                    new float2(0f, 1f));

            float2 leftDirection = MathUtils.Left(tangent);

            float cursorLateral =
                math.dot(
                    hit.m_HitPosition.xz - roadPosition.xz,
                    leftDirection);

            bool cursorOnGeometricLeft = cursorLateral >= 0f;

            DynamicBuffer<SubLane> subLanes =
                EntityManager.GetBuffer<SubLane>(
                    road,
                    isReadOnly: true);

            bool found = false;
            float bestDistanceSq = float.MaxValue;

            for (int index = 0; index < subLanes.Length; index++)
            {
                Entity lane = subLanes[index].m_SubLane;

                if (!TryGetEligibleParkingLane(
                        EntityManager,
                        road,
                        lane,
                        out ParkingLane parkingLane,
                        out Curve laneCurve))
                {
                    continue;
                }

                float3 lanePosition =
                    MathUtils.Position(
                        laneCurve.m_Bezier,
                        curvePosition);

                float laneLateral =
                    math.dot(
                        lanePosition.xz - roadPosition.xz,
                        leftDirection);

                bool laneOnGeometricLeft = laneLateral >= 0f;

                if (laneOnGeometricLeft != cursorOnGeometricLeft)
                {
                    continue;
                }

                float distanceSq =
                    math.lengthsq(
                        lanePosition.xz -
                        hit.m_HitPosition.xz);

                if (distanceSq >= bestDistanceSq)
                {
                    continue;
                }

                bestDistanceSq = distanceSq;
                rightSide =
                    (parkingLane.m_Flags &
                        ParkingLaneFlags.RightSide) != 0;

                found = true;
            }

            if (!found)
            {
                road = Entity.Null;
            }

            return found;
        }

        private Entity ResolveRoadEntity(Entity entity)
        {
            if (entity == Entity.Null ||
                !EntityManager.Exists(entity))
            {
                return Entity.Null;
            }

            if (EntityManager.HasComponent<Road>(entity))
            {
                return entity;
            }

            if (EntityManager.HasComponent<Owner>(entity))
            {
                Entity owner =
                    EntityManager.GetComponentData<Owner>(entity).m_Owner;

                if (owner != Entity.Null &&
                    EntityManager.Exists(owner) &&
                    EntityManager.HasComponent<Road>(owner))
                {
                    return owner;
                }
            }

            return Entity.Null;
        }

        private bool IsManualBanSet(
            Entity road,
            bool rightSide)
        {
            return EntityManager.HasComponent<ManualRoadParkingBan>(road) &&
                EntityManager
                    .GetComponentData<ManualRoadParkingBan>(road)
                    .IsBanned(rightSide);
        }

        private void ToggleManualBan(
            Entity road,
            bool rightSide)
        {
            ManualRoadParkingBan ban =
                EntityManager.HasComponent<ManualRoadParkingBan>(road)
                    ? EntityManager
                        .GetComponentData<ManualRoadParkingBan>(road)
                    : default;

            bool newValue = !ban.IsBanned(rightSide);
            ban.SetBanned(rightSide, newValue);

            if (ban.IsEmpty)
            {
                if (EntityManager.HasComponent<ManualRoadParkingBan>(road))
                {
                    EntityManager.RemoveComponent<ManualRoadParkingBan>(road);
                }
            }
            else if (EntityManager.HasComponent<ManualRoadParkingBan>(road))
            {
                EntityManager.SetComponentData(road, ban);
            }
            else
            {
                EntityManager.AddComponentData(road, ban);
            }

            // Only this road needs immediate reconciliation.
            NoStreetParkingSystem.RequestRoadReconcile(road);
            ParkingStatusCache.MarkDirty();

#if DEBUG
            string side = rightSide ? "Right" : "Left";
            string state = newValue ? "BAN" : "ALLOW";

            CS2Shared.RiverMochi.LogUtils.Info(
                $"{Mod.ModTag} [RoadTool] {state}: " +
                $"road={road.Index}:{road.Version}, side={side}.");
#endif
        }

        private void ClearPreview()
        {
            m_PreviewRoad = Entity.Null;
            m_PreviewRightSide = false;
            m_PreviewRemoving = false;
        }

        /// <summary>
        /// Checks whether one sublane is an ordinary car-parking lane belonging
        /// to the supplied road.
        /// </summary>
        internal static bool TryGetEligibleParkingLane(
        EntityManager entityManager,
        Entity road,
        Entity lane,
        out Game.Net.ParkingLane parkingLane,
        out Game.Net.Curve curve)
        {
            parkingLane = default;
            curve = default;

            if (lane == Entity.Null ||
                !entityManager.Exists(lane) ||
                entityManager.HasComponent<Deleted>(lane) ||
                entityManager.HasComponent<Temp>(lane) ||
                !entityManager.HasComponent<Game.Net.ParkingLane>(lane) ||
                !entityManager.HasComponent<Owner>(lane) ||
                !entityManager.HasComponent<Game.Prefabs.PrefabRef>(lane) ||
                !entityManager.HasComponent<Game.Net.Curve>(lane)
            {
                return false;
            }

            Owner owner =
                entityManager.GetComponentData<Owner>(lane);

            if (owner.m_Owner != road)
            {
                return false;
            }

            parkingLane =
                entityManager.GetComponentData<Game.Net.ParkingLane>(lane);

            if ((parkingLane.m_Flags &
                (Game.Net.ParkingLaneFlags.VirtualLane |
                    Game.Net.ParkingLaneFlags.SpecialVehicles)) != 0)
            {
                return false;
            }

            Game.Prefabs.PrefabRef prefabRef =
                entityManager.GetComponentData<Game.Prefabs.PrefabRef>(lane);

            if (prefabRef.m_Prefab == Entity.Null ||
                !entityManager.Exists(prefabRef.m_Prefab) ||
                !entityManager.HasComponent<Game.Prefabs.ParkingLaneData>(
                    prefabRef.m_Prefab)
            {
                return false;
            }

            Game.Prefabs.ParkingLaneData parkingLaneData =
                entityManager.GetComponentData<Game.Prefabs.ParkingLaneData>(
                    prefabRef.m_Prefab);

            if ((parkingLaneData.m_RoadTypes & Game.Net.RoadTypes.Car) == 0)
            {
                return false;
            }

            curve = entityManager.GetComponentData<Game.Net.Curve>(lane);
            return true;
        }
    }
}
