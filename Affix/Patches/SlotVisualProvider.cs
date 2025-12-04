using Affix.Core;
using Veneer.Components.Specialized;

namespace Affix.Patches
{
    /// <summary>
    /// Provides affix-based border colors to Veneer item slots.
    /// </summary>
    public class AffixSlotVisualProvider : IItemSlotVisualProvider
    {
        /// <summary>
        /// Priority is negative so affix visuals take precedence.
        /// </summary>
        public int Priority => -100;

        /// <summary>
        /// Sets the border color based on item affix rarity.
        /// </summary>
        public void ModifyVisuals(ItemSlotVisualContext context)
        {
            var item = context.Item;
            if (item == null || !AffixData.HasAffixes(item))
                return;

            var rarity = AffixData.GetRarity(item);

            // Map Affix rarity to Veneer rarity tier (1-5)
            // Rarity enum: Common=0, Uncommon=1, Rare=2, Epic=3, Legendary=4
            context.RarityTier = (int)rarity + 1;
        }
    }
}
