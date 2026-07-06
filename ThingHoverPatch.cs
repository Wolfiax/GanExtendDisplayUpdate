using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace GanExtendDisplay
{
    internal static class ThingHoverPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Thing), nameof(Thing.GetHoverText))]
        private static void AppendThingHoverText(Thing __instance, ref string __result)
        {
            if (!ModState.Enabled || !ModConfig.ThingDisplay.IsEnabled || __instance.isChara)
                return;

            __result = ThingHoverFormatter.Format(__instance, __result);
        }
    }

    internal static class ThingHoverFormatter
    {
        public static string Format(Thing thing, string originalText)
        {
            string ownership = thing.isNPCProperty ? "(x)" : string.Empty;

            string header = DisplayText.JoinNonEmpty(" ",
                ownership,
                "Lv." + thing.LV,
                thing.material.GetName(),
                originalText);

            string rarityLine = ModConfig.ShowItemRarity.Value
                ? ("Rarity: " + GetRarityName(thing.rarity)).TagColor(GetRarityTextColor(thing.rarity))
                : string.Empty;

            string priceLine = ModConfig.ShowItemPrice.Value
                ? BuildPriceLine(thing)
                : string.Empty;

            return DisplayText.JoinNonEmpty("\n\n",
                header,
                rarityLine,
                priceLine);
        }

        private static string BuildPriceLine(Thing thing)
        {
            int unitPrice = thing.GetPrice(CurrencyType.Money, false, PriceType.Default, null);
            int stackCount = GetStackCount(thing);
            int containerPrice = unitPrice * stackCount;

            int contentsPrice = GetContainerContentsPrice(thing);

            if (contentsPrice > 0)
            {
                int totalPrice = containerPrice + contentsPrice;
                string lockText = ModConfig.ShowLockLevel.Value && thing.c_lockLv > 0
                    ? " | Lock Lv." + thing.c_lockLv
                    : string.Empty;

                return ("Price: " + totalPrice.ToString("N0") + " oren"
                    + lockText
                    + "\n(Container: " + containerPrice.ToString("N0")
                    + " | Contents: " + contentsPrice.ToString("N0") + ")")
                    .TagColor(new Color(1.00f, 0.85f, 0.25f));
            }

            string lockTextSingle = ModConfig.ShowLockLevel.Value && thing.c_lockLv > 0
                ? " | Lock Lv." + thing.c_lockLv
                : string.Empty;

            if (stackCount > 1)
            {
                return ("Price: " + containerPrice.ToString("N0") + " oren"
                    + lockTextSingle
                    + " (" + unitPrice.ToString("N0") + " x " + stackCount + ")")
                    .TagColor(new Color(1.00f, 0.85f, 0.25f));
            }

            return ("Price: " + containerPrice.ToString("N0") + " oren" + lockTextSingle)
                .TagColor(new Color(1.00f, 0.85f, 0.25f));
        }

        private static int GetContainerContentsPrice(Thing thing)
        {
            int total = 0;

            // Try the most common container/content member names.
            object contents =
                ReflectionReader.Member(thing, "things")
                ?? ReflectionReader.Member(thing, "Things")
                ?? ReflectionReader.Member(thing, "children")
                ?? ReflectionReader.Member(thing, "Children")
                ?? ReflectionReader.Member(thing, "items")
                ?? ReflectionReader.Member(thing, "Items")
                ?? ReflectionReader.Member(thing, "inventory")
                ?? ReflectionReader.Member(thing, "Inventory");

            if (contents == null)
                return 0;

            IEnumerable enumerable = contents as IEnumerable;

            if (enumerable == null)
            {
                object list =
                    ReflectionReader.Member(contents, "list")
                    ?? ReflectionReader.Member(contents, "items")
                    ?? ReflectionReader.Member(contents, "children")
                    ?? ReflectionReader.Member(contents, "values")
                    ?? ReflectionReader.Member(contents, "Values");

                enumerable = list as IEnumerable;
            }

            if (enumerable == null)
                return 0;

            foreach (object entry in enumerable)
            {
                Thing item = entry as Thing;

                if (item == null)
                {
                    item = ReflectionReader.Member(entry, "Value") as Thing
                           ?? ReflectionReader.Member(entry, "value") as Thing
                           ?? ReflectionReader.Member(entry, "Item") as Thing
                           ?? ReflectionReader.Member(entry, "item") as Thing;
                }

                if (item == null || item == thing)
                    continue;

                int itemUnitPrice = item.GetPrice(CurrencyType.Money, false, PriceType.Default, null);
                int itemCount = GetStackCount(item);
                total += itemUnitPrice * itemCount;
            }

            return total;
        }

        private static int GetStackCount(Thing thing)
        {
            int count = ReflectionReader.IntMember(
                thing,
                "Num",
                "num",
                "count",
                "Count",
                "stack",
                "Stack",
                "amount",
                "Amount");

            if (count <= 0)
                count = 1;

            return count;
        }

        private static string GetRarityName(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Superior:
                    return "Superior";

                case Rarity.Legendary:
                    return "Miracle";

                case Rarity.Mythical:
                    return "Godly";

                case Rarity.Artifact:
                    return "Unique";

                default:
                    return "Standard";
            }
        }

        private static Color GetRarityTextColor(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Superior:
                    return new Color(0.20f, 1.00f, 0.20f); // Green

                case Rarity.Legendary:
                    return new Color(0.30f, 0.60f, 1.00f); // Blue

                case Rarity.Mythical:
                    return new Color(1.00f, 0.25f, 0.25f); // Red

                case Rarity.Artifact:
                    return new Color(0.75f, 0.35f, 1.00f); // Purple

                default:
                    return Color.white; // Standard
            }
        }
    }
}
