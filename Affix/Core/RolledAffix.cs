namespace Affix.Core
{
    /// <summary>
    /// Represents an affix that has been rolled on an item with a specific value.
    /// </summary>
    public class RolledAffix
    {
        /// <summary>
        /// The affix definition this roll is based on.
        /// </summary>
        public AffixDefinition Definition { get; set; }

        /// <summary>
        /// The rolled value for this affix.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// The rarity tier this was rolled at.
        /// </summary>
        public Rarity Tier { get; set; }

        /// <summary>
        /// Gets the formatted description with the rolled value.
        /// </summary>
        public string GetFormattedDescription()
        {
            if (Definition == null) return string.Empty;

            var displayValue = Definition.Type == ModifierType.Percent
                ? (Value * 100f).ToString("F0") + "%"
                : Value.ToString("F0");

            return string.Format(Definition.Description, displayValue);
        }

        /// <summary>
        /// Serializes this affix to a string for storage.
        /// Format: "affixId:value"
        /// </summary>
        public string Serialize()
        {
            return $"{Definition.Id}:{Value:F2}";
        }

        /// <summary>
        /// Attempts to deserialize a rolled affix from a string.
        /// </summary>
        public static RolledAffix Deserialize(string data, Rarity tier)
        {
            if (string.IsNullOrEmpty(data)) return null;

            var parts = data.Split(':');
            if (parts.Length != 2) return null;

            var affixId = parts[0];
            if (!float.TryParse(parts[1], out var value)) return null;

            var definition = AffixRegistry.Get(affixId);
            if (definition == null) return null;

            return new RolledAffix
            {
                Definition = definition,
                Value = value,
                Tier = tier
            };
        }
    }
}
