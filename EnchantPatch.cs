using System;
using HarmonyLib;

namespace GanExtendDisplay
{
    internal static class EnchantPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DNA), nameof(DNA.WriteNote))]
        private static bool ReplaceDnaNote(DNA __instance, UINote n)
        {
            if (!ModState.Enabled || !ModConfig.EnchantDisplay.IsEnabled)
                return true;

            EnchantDisplayClass.DNA_WriteNote_Prefix(__instance, n);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Element), nameof(Element.AddEncNote))]
        private static bool ReplaceEnchantNote(Element __instance, UINote n, Card Card, ElementContainer.NoteMode mode = ElementContainer.NoteMode.Default, Func<Element, string, string> funcText = null, Action<UINote, Element> onAddNote = null)
        {
            if (!ModState.Enabled || !ModConfig.EnchantDisplay.IsEnabled)
                return true;

            EnchantDisplayClass.Enchant_AddEncNote_Prefix(__instance, n, Card, mode, funcText, onAddNote);
            return false;
        }
    }
}
