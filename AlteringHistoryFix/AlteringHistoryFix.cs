using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;

namespace AlteringHistoryFix
{
    public class AlertingHistoryFix : ThreadingExtensionBase, IUserMod
    {
        public string Name => "Altering History Fix";

        public string Description => "Fixes Phantom Buildings caused by Altering History.";

        public static bool GameLoaded;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            try
            {
                if (GameLoaded && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
                    Input.GetKeyUp(KeyCode.F5))
                {
                    FixBuilding();
                }
            }
            catch (Exception e)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, $"[Altering History Fix] {e.Message} - {e.StackTrace}");
            }
        }

        private static void FixBuilding()
        {
            int counter = 0;

            for (ushort buildingId = 0; buildingId < BuildingManager.instance.m_buildings.m_buffer.Length; buildingId++)
            {
                var building = BuildingManager.instance.m_buildings.m_buffer[buildingId];

                if (building.m_flags.IsFlagSet(Building.Flags.Historical) &&
                    !building.m_flags.IsFlagSet(Building.Flags.Created))
                {
                    SimulationManager.instance.AddAction(() =>
                    {
                        BuildingManager.instance.ReleaseBuilding(buildingId);
                        counter += 1;
                    });
                }
            }

            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, $"[Altering History Fix] Fixed {counter} phantom buildings.");
        }
    }

    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode == LoadMode.NewAsset || mode == LoadMode.LoadAsset || mode == LoadMode.NewMap ||
                mode == LoadMode.LoadMap || mode == LoadMode.NewTheme || mode == LoadMode.LoadTheme)
                return;

            AlertingHistoryFix.GameLoaded = true;
        }
    }
}
