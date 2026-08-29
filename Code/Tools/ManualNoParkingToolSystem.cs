// <copyright file="ManualNoParkingToolSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Purpose: Lets the player add/remove No Parking on one side of existing roads.

using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Common;
using Game.Tools;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace ParkingControl
{

    public sealed partial class ManualNoParkingToolSystem : ToolBaseSystem
    {
        internal const string kToolId = "ParkingControl.NoParking";

        internal readonly struct RoadSideSelection
        {
            internal RoadSideSelection(
                Entity road,
                bool rightSide,
                bool removing)
            {
                Road = road;
                RightSide = rightSide;
                Removing = removing;
            }

            internal Entity Road { get; }

            internal bool RightSide { get; }

            internal bool Removing { get; }
        }

        private enum SelectionMode
        {
            None,
            Add,
            Remove,
        }

        private readonly List<RoadSideSelection> m_SelectedSides = new(32);

        private Game.Prefabs.PrefabBase? m_ToolPrefab;
        private Game.Audio.AudioManager m_AudioManager = null!;
        private EntityQuery m_SoundQuery;

        private SelectionMode m_SelectionMode;

        private Entity m_PreviewRoad;
        private bool m_PreviewRightSide;
        private bool m_PreviewRemoving;

        public override string toolID => kToolId;

        internal bool IsToolActive => Enabled;

        internal Entity PreviewRoad => m_PreviewRoad;

        internal bool PreviewRightSide => m_PreviewRightSide;

        internal bool PreviewRemoving => m_PreviewRemoving;

        internal IReadOnlyList<RoadSideSelection> SelectedSides => m_SelectedSides;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_AudioManager =
                World.GetOrCreateSystemManaged<Game.Audio.AudioManager>();

            m_SoundQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Prefabs.ToolUXSoundSettingsData>()
                .Build();

            ClearSelection();
            ClearPreview();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            applyAction.shouldBeEnabled = true;
            secondaryApplyAction.shouldBeEnabled = true;

            // RMB is native Secondary Apply. Escape stays explicit below.
            cancelAction.shouldBeEnabled = false;

            requireNet = Game.Net.Layer.Road;
            allowUnderground = false;

            ClearSelection();
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

            requireNet = Game.Net.Layer.None;
            allowUnderground = false;

            ClearSelection();
            ClearPreview();

#if DEBUG
            CS2Shared.RiverMochi.LogUtils.Info(
                $"{Mod.ModTag} [RoadTool] INACTIVE.");
#endif

            base.OnStopRunning();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (WasEscapePressed())
            {
                ClearSelection();
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

            bool currentBanned =
                hasRoadSide &&
                IsManualBanSet(road, rightSide);

            if (hasRoadSide)
            {
                m_PreviewRoad = road;
                m_PreviewRightSide = rightSide;
                m_PreviewRemoving = currentBanned;
            }
            else
            {
                ClearPreview();
            }

            // Start a new drag selection.
            if (applyAction.WasPressedThisFrame())
            {
                BeginSelection(SelectionMode.Add);

                if (hasRoadSide && !currentBanned)
                {
                    AddSelection(road, rightSide, removing: false);
                }
            }
            else if (secondaryApplyAction.WasPressedThisFrame())
            {
                BeginSelection(SelectionMode.Remove);

                if (hasRoadSide && currentBanned)
                {
                    AddSelection(road, rightSide, removing: true);
                }
            }

            // Keep collecting road sides while the mouse button stays held.
            if (m_SelectionMode == SelectionMode.Add &&
                applyAction.IsPressed() &&
                hasRoadSide &&
                !currentBanned)
            {
                AddSelection(road, rightSide, removing: false);
            }
            else if (m_SelectionMode == SelectionMode.Remove &&
                secondaryApplyAction.IsPressed() &&
                hasRoadSide &&
                currentBanned)
            {
                AddSelection(road, rightSide, removing: true);
            }

            // LMB release commits everything already collected.
            if (m_SelectionMode == SelectionMode.Add &&
                applyAction.WasReleasedThisFrame())
            {
                // Release can be over an intersection, gap, or not eligible road.
                if (hasRoadSide && !currentBanned)
                {
                    AddSelection(road, rightSide, removing: false);
                }

                ApplySelection(banned: true);
                ClearSelection();
                return inputDeps;
            }

            // RMB release commits everything already collected.
            if (m_SelectionMode == SelectionMode.Remove &&
                secondaryApplyAction.WasReleasedThisFrame())
            {
                // Release can be over an intersection, gap, or ineligible road.
                if (hasRoadSide && currentBanned)
                {
                    AddSelection(road, rightSide, removing: true);
                }

                ApplySelection(banned: false);
                ClearSelection();
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

        internal bool IsSelected(
            Entity road,
            bool rightSide)
        {
            for (int index = 0; index < m_SelectedSides.Count; index++)
            {
                RoadSideSelection selection = m_SelectedSides[index];

                if (selection.Road == road &&
                    selection.RightSide == rightSide)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool WasEscapePressed()
        {
            try
            {
                Keyboard? keyboard = Keyboard.current;

                return keyboard != null &&
                    keyboard.escapeKey.wasPressedThisFrame;
            }
            catch
            {
                return false;
            }
        }

        private void BeginSelection(SelectionMode mode)
        {
            m_SelectedSides.Clear();
            m_SelectionMode = mode;
        }

        private void ClearSelection()
        {
            m_SelectedSides.Clear();
            m_SelectionMode = SelectionMode.None;
        }

        private void AddSelection(
            Entity road,
            bool rightSide,
            bool removing)
        {
            if (IsSelected(road, rightSide))
            {
                return;
            }

            m_SelectedSides.Add(
                new RoadSideSelection(
                    road,
                    rightSide,
                    removing));

            PlaySelectSound();
        }

        private void ApplySelection(bool banned)
        {
            int changed = 0;

            for (int index = 0; index < m_SelectedSides.Count; index++)
            {
                RoadSideSelection selection = m_SelectedSides[index];

                if (selection.Road == Entity.Null ||
                    !EntityManager.Exists(selection.Road))
                {
                    continue;
                }

                if (SetManualBan(
                        selection.Road,
                        selection.RightSide,
                        banned))
                {
                    changed++;
                }
            }

            if (changed == 0)
            {
                return;
            }

            ParkingStatusCache.MarkDirty();

            // Keep the existing add/remove confirmation sound for this pass.
            // We can give RMB its own native sound after the audio test.
            PlayNetBuildSound();

#if DEBUG
            CS2Shared.RiverMochi.LogUtils.Info(
                $"{Mod.ModTag} [RoadTool] Applied " +
                $"{(banned ? "BAN" : "ALLOW")} to {changed} road side(s).");
#endif
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

        private void PlaySelectSound()
        {
            PlaySound(
                settings => settings.m_SelectEntitySound);
        }

        private void PlayNetBuildSound()
        {
            PlaySound(
                settings => settings.m_NetBuildSound);
        }

        private void PlaySound(
            System.Func<Game.Prefabs.ToolUXSoundSettingsData, Entity> selector)
        {
            if (m_SoundQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            Game.Prefabs.ToolUXSoundSettingsData soundSettings =
                m_SoundQuery.GetSingleton<Game.Prefabs.ToolUXSoundSettingsData>();

            Entity sound = selector(soundSettings);

            if (sound != Entity.Null)
            {
                m_AudioManager.PlayUISound(sound);
            }
        }

        private void ClearPreview()
        {
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
