using System.Collections.Generic;

namespace Affix.DropTables
{
    /// <summary>
    /// Configuration for a drop table.
    /// </summary>
    public class DropTableConfig
    {
        /// <summary>
        /// Minimum number of items to drop.
        /// </summary>
        public int MinDrops { get; set; } = 1;

        /// <summary>
        /// Maximum number of items to drop.
        /// </summary>
        public int MaxDrops { get; set; } = 1;

        /// <summary>
        /// Name of another drop table to inherit from.
        /// Inherited entries are combined with this table's entries.
        /// </summary>
        public string Inherits { get; set; }

        /// <summary>
        /// The drop entries in this table.
        /// </summary>
        public List<DropEntry> Entries { get; set; } = new();

        /// <summary>
        /// Bonus max drops per additional player (for multiplayer scaling).
        /// </summary>
        public int BonusDropsPerPlayer { get; set; } = 0;
    }
}
