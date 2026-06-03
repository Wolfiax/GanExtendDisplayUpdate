using HarmonyLib;

namespace GanExtendDisplay
{
    internal static class CharacterHoverPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Chara), nameof(Chara.GetHoverText))]
        private static void AppendCharacterHoverText(Chara __instance, ref string __result)
        {
            if (!ModState.Enabled || !ModConfig.CharacterDisplay.IsEnabled)
                return;

            __result = CharacterHoverFormatter.AppendMainHoverText(__instance, __result);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Chara), nameof(Chara.GetHoverText2))]
        private static bool ReplaceSecondaryHoverText(Chara __instance, ref string __result)
        {
            if (!ModState.Enabled || !ModConfig.CharacterDisplay.IsEnabled)
                return true;

            __result = CharacterHoverFormatter.BuildSecondaryHoverText(__instance);
            return false;
        }
    }
}
