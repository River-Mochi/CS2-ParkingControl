// <copyright file="ManualNoParkingToolSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Lets the player add/remove No Parking on one side of an existing road.

namespace ParkingControl
{
    using Colossal.Mathematics;
    using Game.Common;
    using Game.Tools;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using UnityEngine.InputSystem;

    public sealed partial class NoParkingRoadToolSystem : ToolBaseSystem
    {
        internal const string kToolId = "ParkingControl.NoParking";

        private Game.Prefabs.PrefabBase? m_ToolPrefab;
        private Game.Audio.AudioManager m_AudioManager = null!;
        private EntityQuery m_SoundQuery;

        private bool m_IsAdding;
        private bool m_IsRemoving;

        private Entity m_PreviewRoad;
        private bool m_PreviewRightSide;
        private bool m_PreviewRemoving;

        private Entity m_HighlightedRoad;
        private bool m_OwnsRoadHighlight;

        public override string toolID => kToolId;

        internal bool IsToolActive => Enabled;
        internal Entity PreviewRoad => m_PreviewRoad;
        internal bool PreviewRightSide => m_PreviewRightSide;
        internal bool PreviewRemoving => m_PreviewRemoving;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_AudioManager =
                World.GetOrCreateSystemManaged<Game.Audio.AudioManager>();

            m_SoundQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Prefabs.ToolUXSoundSettingsData>()
                .Build();

            ClearPreview();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            applyAction.shouldBeEnabled = true;
            secondaryApplyAction.shouldBeEnabled = true;

            // RMB is the native Secondary Apply action for removing a ban.
            // Escape is handled manually in OnUpdate().
            cancelAction.shouldBeEnabled = false;

            m_IsAdding = false;
            m_IsRemoving = false;

            requireNet = Game.Net.Layer.Road;
            allowUnderground = false;

            ClearPreview();

#if DEBUG
            CS2Shared.RiverMochi.LogUtils.Info(
                $"{Mod.ModTag} [RoadTool] ACTIVE: " +
                $"activeTool={m_ToolSystem.activeTool?.toolID ?? "(null)"}.");
#endif
        }

        protected override void OnStopRunning()
        {
            applyAction.shouldBeEnabled = false;
            secondaryApplyAction.shouldBeEnabled = false;
            cancelAction.shouldBeEnabled = false;

            m_IsAdding = false;
            m_IsRemoving = false;

            requireNet = Game.Net.Layer.None;
            allowUnderground = false;

            ClearPreview();

#if DEBUG
            CS2Shared.RiverMochi.LogUtils.Info(
                $"{Mod.ModTag} [RoadTool] INACTIVE.");
#endif

            base.OnStopRunning();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            bool escapePressed = false;

            try
            {
                Keyboard? keyboard = Keyboard.current;
                escapePressed =
                    keyboard != null &&
                    keyboard.escapeKey.wasPressedThisFrame;
            }
            catch
            {
            }

            if (escapePressed)
            {
                m_IsAdding = false;
                m_IsRemoving = false;
                ClearPreview();

                if (m_ToolSystem.activeTool == this)
                {
                    m_ToolSystem.activeTool = m_DefaultToolSystem;
                }

                return inputDeps;
            }

            bool hasRoadSide =
                TryGetRoadSideUnderCursor(
                    out Entity road,
                    out bool rightSide);

            if (!hasRoadSide)
            {
                // A drag starts only when the mouse button is first pressed
                // over a valid road side. This prevents the UI tile click
                // that activates the tool from accidentally painting a road.
                if (applyAction.WasReleasedThisFrame())
                {
                    m_IsAdding = false;
                }

                if (secondaryApplyAction.WasReleasedThisFrame())
                {
                    m_IsRemoving = false;
                }

                ClearPreview();
                return inputDeps;
            }

            m_PreviewRoad = road;
            m_PreviewRightSide = rightSide;
            m_PreviewRemoving = IsManualBanSet(road, rightSide);

            // Keep the current phase-1 preview: a cyan side strip plus the
            // game's normal Highlighted outline while an add is available.
            UpdateRoadHighlight(
                road,
                shouldHighlight: !m_PreviewRemoving);

            // Start add-paint only when LMB is initially pressed over a valid
            // road side. While held, every newly hovered unbanned road side
            // is changed immediately.
            if (applyAction.WasPressedThisFrame())
            {
                m_IsAdding = true;
                m_IsRemoving = false;
            }

            if (applyAction.WasReleasedThisFrame())
            {
                m_IsAdding = false;
            }

            // Same behavior for RMB removal using ToolBaseSystem's native
            // Secondary Apply action, rather than reading Mouse.current.
            if (secondaryApplyAction.WasPressedThisFrame())
            {
                m_IsRemoving = true;
                m_IsAdding = false;
            }

            if (secondaryApplyAction.WasReleasedThisFrame())
            {
                m_IsRemoving = false;
            }

            if (m_IsAdding &&
                applyAction.IsPressed() &&
                !m_PreviewRemoving)
            {
                if (SetManualBan(
                        road,
                        rightSide,
                        banned: true))
                {
                    PlayNetBuildSound();
                }

                m_PreviewRemoving = true;
                ClearRoadHighlight();
                return inputDeps;
            }

            if (m_IsRemoving &&
                secondaryApplyAction.IsPressed() &&
                m_PreviewRemoving)
            {
                if (SetManualBan(
                        road,
                        rightSide,
                        banned: false))
                {
                    PlayNetBuildSound();
                }

                m_PreviewRemoving = false;

                UpdateRoadHighlight(
                    road,
                    shouldHighlight: true);
            }

            return inputDeps;
        }

        public override Game.Prefabs.PrefabBase? GetPrefab()
        {
            return m_ToolPrefab;
        }

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

        public override void InitializeRaycast()
        {
            base.InitializeRaycast();

            m_ToolRaycastSystem.typeMask = TypeMask.Net;
            m_ToolRaycastSystem.netLayerMask = Game.Net.Layer.Road;
        }

        private bool TryGetRoadSideUnderCursor(
            out Entity road,
            out bool rightSide)
        {
            road = Entity.Null;
            rightSide = false;

            if (!GetRaycastResult(
                    out Entity hitOwner,
                    out RaycastHit hit))
            {
                return false;
            }

            road = ResolveRoadEntity(hitOwner);

            // At intersections the raycast owner can be a node while the
            // concrete hit entity is the road edge.
            if (road == Entity.Null &&
                hit.m_HitEntity != Entity.Null)
            {
                road = ResolveRoadEntity(hit.m_HitEntity);
            }

            if (road == Entity.Null ||
                !EntityManager.Exists(road) ||
                !EntityManager.HasComponent<Game.Net.Road>(road) ||
                !EntityManager.HasComponent<Game.Net.Curve>(road) ||
                !EntityManager.HasBuffer<Game.Net.SubLane>(road))
            {
                road = Entity.Null;
                return false;
            }

            Game.Net.Curve roadCurve =
                EntityManager.GetComponentData<Game.Net.Curve>(road);

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

            float2 leftDirection =
                MathUtils.Left(tangent);

            float cursorLateral =
                math.dot(
                    hit.m_HitPosition.xz - roadPosition.xz,
                    leftDirection);

            bool cursorOnGeometricLeft =
                cursorLateral >= 0f;

            DynamicBuffer<Game.Net.SubLane> subLanes =
                EntityManager.GetBuffer<Game.Net.SubLane>(
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
                        out Game.Net.ParkingLane parkingLane,
                        out Game.Net.Curve laneCurve))
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

                bool laneOnGeometricLeft =
                    laneLateral >= 0f;

                if (laneOnGeometricLeft !=
                    cursorOnGeometricLeft)
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
                        Game.Net.ParkingLaneFlags.RightSide) != 0;

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

            if (EntityManager.HasComponent<Game.Net.Road>(entity))
            {
                return entity;
            }

            if (EntityManager.HasComponent<Owner>(entity))
            {
                Entity owner =
                    EntityManager.GetComponentData<Owner>(entity).m_Owner;

                if (owner != Entity.Null &&
                    EntityManager.Exists(owner) &&
                    EntityManager.HasComponent<Game.Net.Road>(owner))
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

        private bool SetManualBan(
            Entity road,
            bool rightSide,
            bool banned)
        {
            bool hasComponent =
                EntityManager.HasComponent<ManualRoadParkingBan>(road);

            ManualRoadParkingBan manualBan =
                hasComponent
                    ? EntityManager
                        .GetComponentData<ManualRoadParkingBan>(road)
                    : default;

            if (manualBan.IsBanned(rightSide) == banned)
            {
                return false;
            }

            manualBan.SetBanned(
                rightSide,
                banned);

            if (manualBan.IsEmpty)
            {
                if (hasComponent)
                {
                    EntityManager.RemoveComponent<ManualRoadParkingBan>(road);
                }
            }
            else if (hasComponent)
            {
                EntityManager.SetComponentData(
                    road,
                    manualBan);
            }
            else
            {
                EntityManager.AddComponentData(
                    road,
                    manualBan);
            }

            NoStreetParkingSystem.RequestRoadReconcile(road);
            ParkingStatusCache.MarkDirty();

#if DEBUG
            string side =
                rightSide ? "Right" : "Left";

            string state =
                banned ? "BAN" : "ALLOW";

            CS2Shared.RiverMochi.LogUtils.Info(
                $"{Mod.ModTag} [RoadTool] {state}: " +
                $"road={road.Index}:{road.Version}, side={side}.");
#endif

            return true;
        }

        private void PlayNetBuildSound()
        {
            if (m_SoundQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            Game.Prefabs.ToolUXSoundSettingsData soundSettings =
                m_SoundQuery.GetSingleton<Game.Prefabs.ToolUXSoundSettingsData>();

            if (soundSettings.m_NetBuildSound != Entity.Null)
            {
                m_AudioManager.PlayUISound(
                    soundSettings.m_NetBuildSound);
            }
        }

        private void UpdateRoadHighlight(
            Entity road,
            bool shouldHighlight)
        {
            if (!shouldHighlight)
            {
                ClearRoadHighlight();
                return;
            }

            if (road == m_HighlightedRoad)
            {
                return;
            }

            ClearRoadHighlight();

            if (road == Entity.Null ||
                !EntityManager.Exists(road))
            {
                return;
            }

            m_HighlightedRoad = road;

            // Never remove a Highlighted component PC did not add.
            if (EntityManager.HasComponent<Highlighted>(road))
            {
                m_OwnsRoadHighlight = false;
                return;
            }

            EntityManager.AddComponent<Highlighted>(road);
            m_OwnsRoadHighlight = true;

            MarkBatchesUpdated(road);
        }

        private void ClearRoadHighlight()
        {
            if (m_HighlightedRoad != Entity.Null &&
                m_OwnsRoadHighlight &&
                EntityManager.Exists(m_HighlightedRoad) &&
                EntityManager.HasComponent<Highlighted>(
                    m_HighlightedRoad))
            {
                EntityManager.RemoveComponent<Highlighted>(
                    m_HighlightedRoad);

                MarkBatchesUpdated(m_HighlightedRoad);
            }

            m_HighlightedRoad = Entity.Null;
            m_OwnsRoadHighlight = false;
        }

        private void MarkBatchesUpdated(Entity road)
        {
            if (!EntityManager.HasComponent<BatchesUpdated>(road))
            {
                EntityManager.AddComponent<BatchesUpdated>(road);
            }
        }

        private void ClearPreview()
        {
            ClearRoadHighlight();

            m_PreviewRoad = Entity.Null;
            m_PreviewRightSide = false;
            m_PreviewRemoving = false;
        }

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
                !entityManager.HasComponent<Game.Net.Curve>(lane))
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
                    prefabRef.m_Prefab))
            {
                return false;
            }

            Game.Prefabs.ParkingLaneData parkingLaneData =
                entityManager.GetComponentData<Game.Prefabs.ParkingLaneData>(
                    prefabRef.m_Prefab);

            if ((parkingLaneData.m_RoadTypes &
                    Game.Net.RoadTypes.Car) == 0)
            {
                return false;
            }

            curve =
                entityManager.GetComponentData<Game.Net.Curve>(lane);

            return true;
        }
    }
}
