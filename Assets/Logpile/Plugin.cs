/*using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Bindito.Core;
using TimberApi.ConsoleSystem;
using TimberApi.DependencyContainerSystem;
using TimberApi.ModSystem;
using Timberborn.Warehouses;

using HarmonyLib;

namespace TinyLogpile
{
    //-------------------- Timber API 0.5.0 ------------------------------
    public class Plugin : IModEntrypoint
    {
        public const string PluginGuid = "knattetobbert.tinylogpile2";
        public const string PluginName = "Tinylogpile";
        public const string PluginVersion = "1.0.7";
        
        public void Entry(IMod mod, IConsoleWriter consoleWriter)
        {
            //Log = Logger;
            
            //Log.LogInfo($"Loaded {PluginName} Version: {PluginVersion}!");
            new Harmony(PluginGuid).PatchAll();
            //consoleWriter.Log("Logpile Patched",LogInfo);
            
        }
    }
    

    
    [HarmonyPatch(typeof(LogPileStockpileVisualizer), "InitializeLogStack", new Type[] { typeof(int) , typeof(int) })]
    public class FixInitialStackPatch
    {
        static bool Prefix(LogPileStockpileVisualizer __instance, int x, int y)
        {
            var BuildingComp = __instance.gameObject.GetComponent<Stockpile>().name.Replace("(Clone)", "");
            var BuildingnameName = BuildingComp.Split(".");

            bool Returnvalue = true;
            if (x == 0 & y == 0)
            {
                //Plugin.Log.LogInfo($"*Right Stack startup X: {x} y: {y} Model: {BuildingnameName[0]}");
                return Returnvalue;
            }
            if (x == 0 & y == 1)
            {
                //Plugin.Log.LogInfo($"*Right Stack startup X: {x} y: {y} Model: {BuildingnameName[0]}");
                return Returnvalue;
            }
            if (BuildingnameName[0] == "TinyLogPile" | BuildingnameName[0] == "TinyMetalLogPile")
            {
                //Plugin.Log.LogInfo($" Wrong Stack Should place X: {x} y: {y} Model: {BuildingnameName[0]}");
                return !Returnvalue;
            }
            else
            {
                //Plugin.Log.LogInfo($" Wrong Stack Should place X: {x} y: {y} Model: {BuildingnameName[0]}");
                return Returnvalue;
            }
        }
    }
}*/