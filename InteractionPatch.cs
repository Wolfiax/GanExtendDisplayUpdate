using HarmonyLib;

namespace GanExtendDisplay
{
    internal static class InteractionPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WidgetMouseover), nameof(WidgetMouseover.Show))]
        private static void AppendZoneDangerLevel(ref string s)
        {
            if (!ModState.Enabled || !ModConfig.InteractDisplay.IsEnabled)
                return;

            PointTarget mouseTarget = EMono.scene.mouseTarget;
            if (mouseTarget.target is Zone zone)
                s += " Lv." + zone.DangerLv;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BaseTaskHarvest), nameof(BaseTaskHarvest.GetText))]
        private static void AppendHarvestTaskInfo(BaseTaskHarvest __instance, ref string __result)
        {
            if (!ModState.Enabled || !ModConfig.InteractDisplay.IsEnabled)
                return;

            __result = InteractionTextFormatter.AppendHarvestTaskInfo(__instance, __result);
        }
    }

    internal static class InteractionTextFormatter
    {
        public static string AppendHarvestTaskInfo(BaseTaskHarvest task, string originalText)
        {
            // Keep harvest/object hover focused on the hovered target.
            // Do not prepend the currently equipped tool name; it makes tree/ore hover text
            // look like the equipment itself is being hovered.
            string levelText = task.IsTooHard
                ? $"Lv:{task.toolLv}<{task.reqLv}"
                : $"Lv:{task.toolLv} >= {task.reqLv}";

            string growthText = string.Empty;
            if (task.pos.cell.growth != null)
            {
                int currentStage = task.pos.cell.growth.stage.idx;
                int harvestStage = task.pos.cell.growth.HarvestStage;
                growthText = (currentStage > harvestStage && harvestStage > 0) ? "Withering:" : "Growth:";
                growthText += currentStage;
                if (harvestStage > 0)
                    growthText += "/" + harvestStage;
            }

            return originalText + "\n" + DisplayText.JoinNonEmpty(" ", levelText, growthText);
        }
    }
}
