namespace Affix.DropTables
{
    /// <summary>
    /// Test drop tables for development.
    /// </summary>
    public static class TestDropTables
    {
        /// <summary>
        /// Registers test drop tables.
        /// </summary>
        public static void Register()
        {
            // Test table: Greydwarf drops SwordSilver 100%
            DropTable.Register("Greydwarf", new DropTableConfig
            {
                MinDrops = 1,
                MaxDrops = 1,
                Entries = new System.Collections.Generic.List<DropEntry>
                {
                    new DropEntry("SwordSilver", chance: 1.0f, rarity: RarityRoll.Weighted)
                }
            });

            Plugin.Log?.LogInfo("Registered test drop table: Greydwarf -> SwordSilver (100%)");
        }
    }
}
