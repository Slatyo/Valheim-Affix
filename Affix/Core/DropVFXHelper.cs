namespace Affix.Core
{
    /// <summary>
    /// Helper to add VFX to dropped items via Spark (when available).
    /// Falls back gracefully if Spark is not loaded.
    /// </summary>
    public static class DropVFXHelper
    {
        private static bool? _sparkAvailable;

        /// <summary>
        /// Checks if Spark is loaded.
        /// </summary>
        public static bool IsSparkAvailable
        {
            get
            {
                if (!_sparkAvailable.HasValue)
                {
                    _sparkAvailable = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.slatyo.spark");
                }
                return _sparkAvailable.Value;
            }
        }

        /// <summary>
        /// Adds rarity-based VFX to a dropped item.
        /// </summary>
        /// <param name="itemDrop">The dropped item in the world.</param>
        /// <param name="rarity">Rarity tier (0=Common, 1=Uncommon, 2=Rare, 3=Epic, 4=Legendary).</param>
        public static void AddRarityEffect(ItemDrop itemDrop, Rarity rarity)
        {
            if (itemDrop == null)
            {
                Plugin.Log?.LogDebug("DropVFXHelper: itemDrop is null");
                return;
            }

            if (!IsSparkAvailable)
            {
                Plugin.Log?.LogDebug("DropVFXHelper: Spark not available");
                return;
            }

            // Skip Common items
            if (rarity == Rarity.Common)
            {
                Plugin.Log?.LogDebug("DropVFXHelper: Skipping Common rarity");
                return;
            }

            Plugin.Log?.LogInfo($"DropVFXHelper: Adding VFX for {rarity} item: {itemDrop.name}");

            try
            {
                SparkDropBridge.AddRarityGlow(itemDrop, (int)rarity);
                Plugin.Log?.LogDebug($"DropVFXHelper: Added effect for {rarity}");
            }
            catch (System.Exception ex)
            {
                Plugin.Log?.LogWarning($"Drop VFX failed: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    /// <summary>
    /// Bridge to Spark API. Separated to avoid type loading issues when Spark isn't available.
    /// </summary>
    internal static class SparkDropBridge
    {
        public static void AddRarityGlow(ItemDrop itemDrop, int rarity)
        {
            Spark.API.SparkDrop.AddRarityGlow(itemDrop, rarity);
        }
    }
}
