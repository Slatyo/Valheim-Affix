using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using Affix.Core;
using Affix.Patches;
using AffixDropTable = Affix.DropTables.DropTable;

namespace Affix
{
    /// <summary>
    /// Affix - Drop tables, rarity tiers, and random stat generation for Valheim gear.
    /// Uses Prime for stat modifiers and Veneer for UI integration.
    /// </summary>
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInDependency("com.slatyo.prime")]
    [BepInDependency("com.slatyo.veneer")]
    [BepInDependency("com.slatyo.munin", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.slatyo.affix";
        public const string PluginName = "Affix";
        public const string PluginVersion = "1.0.0";

        /// <summary>
        /// Logger instance for Affix.
        /// </summary>
        public static ManualLogSource Log { get; private set; }

        /// <summary>
        /// Plugin instance.
        /// </summary>
        public static Plugin Instance { get; private set; }

        private Harmony _harmony;
        private AffixTooltipProvider _tooltipProvider;
        private AffixSlotVisualProvider _slotVisualProvider;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            Log.LogInfo($"{PluginName} v{PluginVersion} is loading...");

            // Register default affixes
            DefaultAffixes.RegisterAll();
            Log.LogInfo($"Registered {AffixRegistry.Count} affixes");

            // Register test drop tables
            Affix.DropTables.TestDropTables.Register();

            // Add localizations
            AddLocalizations();

            // Initialize Harmony patches
            _harmony = new Harmony(PluginGUID);
            _harmony.PatchAll();

            // Register Veneer providers
            _tooltipProvider = new AffixTooltipProvider();
            Veneer.Components.Composite.VeneerTooltip.RegisterProvider(_tooltipProvider);

            _slotVisualProvider = new AffixSlotVisualProvider();
            Veneer.Components.Specialized.VeneerItemSlot.RegisterVisualProvider(_slotVisualProvider);

            // Register Munin commands (if Munin is loaded)
            RegisterCommands();

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded successfully");
        }

        private void OnDestroy()
        {
            UnregisterCommands();

            // Unregister Veneer providers
            if (_tooltipProvider != null)
            {
                Veneer.Components.Composite.VeneerTooltip.UnregisterProvider(_tooltipProvider);
                _tooltipProvider = null;
            }

            if (_slotVisualProvider != null)
            {
                Veneer.Components.Specialized.VeneerItemSlot.UnregisterVisualProvider(_slotVisualProvider);
                _slotVisualProvider = null;
            }

            _harmony?.UnpatchSelf();
            AffixRegistry.Clear();
            AffixDropTable.Clear();
            Patches.AffixSpawnQueue.Clear();
        }

        private void RegisterCommands()
        {
            try
            {
                // Check if Munin is loaded before registering
                if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.slatyo.munin"))
                {
                    Commands.AffixCommands.Register();
                }
            }
            catch (System.Exception ex)
            {
                Log.LogWarning($"Failed to register Munin commands: {ex.Message}");
            }
        }

        private void UnregisterCommands()
        {
            try
            {
                if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.slatyo.munin"))
                {
                    Commands.AffixCommands.Unregister();
                }
            }
            catch { }
        }

        private void AddLocalizations()
        {
            var loc = LocalizationManager.Instance.GetLocalization();

            loc.AddTranslation("English", new System.Collections.Generic.Dictionary<string, string>
            {
                // Rarity names
                { "affix_rarity_common", "Common" },
                { "affix_rarity_uncommon", "Uncommon" },
                { "affix_rarity_rare", "Rare" },
                { "affix_rarity_epic", "Epic" },
                { "affix_rarity_legendary", "Legendary" },
            });
        }
    }
}
