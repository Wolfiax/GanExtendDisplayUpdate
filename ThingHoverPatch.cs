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
            string raritySymbol = GetRaritySymbol(thing.rarity);
            Color rarityColor = GetRarityColor(thing.rarity);
            string rarity = raritySymbol.TagColor(rarityColor);
            string price = ("¤ " + thing.GetPrice(CurrencyType.Money, false, PriceType.Default, null)).TagSize(14);
            string lockLevel = thing.c_lockLv > 0 ? ("Lock.Lv." + thing.c_lockLv.ToString().TagColor(Color.yellow)) : string.Empty;

            return DisplayText.JoinNonEmpty(" ",
                ownership,
                (rarity + " Lv." + thing.LV).TagColor(rarityColor),
                thing.material.GetName(),
                originalText,
                price,
                lockLevel);
        }

        private static string GetRaritySymbol(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Crude: return "x";
                case Rarity.Superior: return "△";
                case Rarity.Legendary: return "◇";
                case Rarity.Mythical: return "☆";
                case Rarity.Artifact: return "★";
                default: return string.Empty;
            }
        }

        private static Color GetRarityColor(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Crude: return DisplayText.RarityCrude;
                case Rarity.Superior: return DisplayText.RaritySuperior;
                case Rarity.Legendary: return DisplayText.RarityLegendary;
                case Rarity.Mythical: return DisplayText.RarityMythical;
                case Rarity.Artifact: return DisplayText.RarityArtifact;
                default: return DisplayText.RarityNormal;
            }
        }
    }
}
