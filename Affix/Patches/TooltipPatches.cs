using System.Text;
using Affix.Core;
using Veneer.Components.Composite;

namespace Affix.Patches
{
    /// <summary>
    /// Provides affix information to Veneer tooltips via the tooltip provider system.
    /// </summary>
    public class AffixTooltipProvider : IItemTooltipProvider
    {
        /// <summary>
        /// Priority is negative so affix info is prepended before other content.
        /// </summary>
        public int Priority => -100;

        /// <summary>
        /// Modifies item tooltip to include affix information.
        /// </summary>
        public void ModifyTooltip(ItemTooltipContext context)
        {
            var item = context.Item;
            if (item == null || !AffixData.HasAffixes(item))
                return;

            var rarity = AffixData.GetRarity(item);
            var rarityConfig = RarityManager.GetConfig(rarity);

            // Update tooltip rarity tier for border color
            context.Tooltip.RarityTier = (int)rarity + 1; // +1 because Veneer uses 1-5, Rarity enum is 0-4

            // Update title with rarity color
            var rarityColor = RarityManager.GetColor(rarity);
            context.Tooltip.TitleColor = rarityColor;

            // Build affix section to prepend to body
            var affixText = GenerateAffixText(item, rarity);
            if (!string.IsNullOrEmpty(affixText))
            {
                // Prepend affix info to existing body with a blank line
                if (!string.IsNullOrEmpty(context.Tooltip.Body))
                {
                    context.Tooltip.Body = affixText + "\n\n" + context.Tooltip.Body;
                }
                else
                {
                    context.Tooltip.Body = affixText;
                }
            }

            // Update subtitle to show rarity name
            var rarityName = Localization.instance?.Localize(rarityConfig.DisplayName) ?? rarity.ToString();
            if (!string.IsNullOrEmpty(context.Tooltip.Subtitle))
            {
                context.Tooltip.Subtitle = $"{rarityName} | {context.Tooltip.Subtitle}";
            }
            else
            {
                context.Tooltip.Subtitle = rarityName;
            }
        }

        private static string GenerateAffixText(ItemDrop.ItemData item, Rarity rarity)
        {
            var sb = new StringBuilder();
            var rarityColor = RarityManager.GetColorHex(rarity);

            var affixes = AffixData.GetAffixes(item);
            foreach (var affix in affixes)
            {
                if (affix.Definition == null) continue;

                var description = affix.GetFormattedDescription();
                sb.AppendLine($"<color={rarityColor}>◆</color> {description}");
            }

            // Add unique ability for Legendary items
            var uniqueAbility = AffixData.GetUniqueAbility(item);
            if (uniqueAbility != null)
            {
                sb.AppendLine(); // Blank line before unique ability
                sb.AppendLine($"<color=#FF8000>★ {uniqueAbility.DisplayName}</color>");
                sb.AppendLine($"<color=#CCCCCC>{uniqueAbility.Description}</color>");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
