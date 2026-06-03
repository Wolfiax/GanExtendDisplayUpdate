using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GanExtendDisplay
{
    internal static class CharacterHoverFormatter
    {
        private static readonly int[] AttributeIds = { 70, 71, 72, 73, 74, 75, 76, 77 };
        private static readonly string[] AttributeFallbackNames = { "STR", "END", "DEX", "PER", "LER", "WIL", "MAG", "CHA" };
        private static readonly int[] ResistanceIds = { 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 970, 971, 972 };

        public static string AppendMainHoverText(Chara character, string originalText)
        {
            string result = AddHeaderBadges(character, CleanOriginalHoverText(originalText));

            AppendLine(ref result, ModConfig.CharacterLine1, character, BuildVitalsLine);
            AppendLine(ref result, ModConfig.CharacterLine2, character, BuildSummaryLine);
            AppendLine(ref result, ModConfig.CharacterLine3, character, BuildAttributeLine);
            AppendLine(ref result, ModConfig.CharacterLine4, character, BuildResistanceLine);
            AppendAffinityGiftLine(ref result, character);

            return result;
        }

        public static string BuildSecondaryHoverText(Chara character)
        {
            string text = string.Empty;

            if (EClass.debug.showExtra)
                text += Environment.NewLine + BuildDebugLine(character);

            if (ModConfig.CharacterConditions.ShouldShow(character))
            {
                string conditionLine = BuildConditionLine(character);
                if (!string.IsNullOrEmpty(conditionLine))
                    text += Environment.NewLine + DisplayText.Size(conditionLine, ModConfig.CharacterConditions.FontSize);
            }

            if (ModConfig.CharacterActions.ShouldShow(character))
            {
                string actions = BuildActionLine(character);
                if (!string.IsNullOrEmpty(actions))
                    text += Environment.NewLine + DisplayText.Size(actions, ModConfig.CharacterActions.FontSize);
            }

            if (ModConfig.CharacterFeats.ShouldShow(character))
            {
                string feats = BuildFeatLine(character);
                if (!string.IsNullOrEmpty(feats))
                    text += Environment.NewLine + DisplayText.Color(DisplayText.Size(feats, ModConfig.CharacterFeats.FontSize), new Color(0.95f, 0.75f, 1f));
            }

            return text;
        }


        private static string CleanOriginalHoverText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string result = text;

            result = Regex.Replace(result, @"(?m)^.*\bFood\s*:[^\r\n]*(?:\r?\n)?", string.Empty);
            result = Regex.Replace(result, @"(?m)^\s*Game\s+Game\s*(?:\r?\n)?", string.Empty);
            result = Regex.Replace(result, @"(?m)^.*\bWeight\s*:[^\r\n]*(?:\r?\n)?", string.Empty);
            result = Regex.Replace(result, @"(?mi)^.*\bEXP\s*[:：][^\r\n]*(?:\r?\n)?", string.Empty);
            result = Regex.Replace(result, @"(?mi)^.*\bExperience\s*[:：][^\r\n]*(?:\r?\n)?", string.Empty);

            return result.TrimEnd();
        }

        private static void AppendLine(ref string result, CharacterLineSettings settings, Chara character, Func<Chara, string> builder)
        {
            if (!settings.ShouldShow(character))
                return;

            string line = builder(character);
            if (string.IsNullOrWhiteSpace(line))
                return;

            result += Environment.NewLine + DisplayText.Size(line, settings.FontSize);
        }

        private static string AddHeaderBadges(Chara character, string text)
        {
            text = AddAffinityBadge(character, text);
            text = AddRarityBadge(character, text);
            return BuildLevelBadge(character) + text;
        }

        private static string AddAffinityBadge(Chara character, string text)
        {
            int affinity = character._affinity;
            if (affinity <= 0)
                return text;

            string heart = affinity > 74 ? "♥" : "♡";
            heart = heart.TagSize(Mathf.Clamp(affinity / 10 + 10, 12, 20))
                         .TagColor(new Color(0.5f, Math.Min(1f, 0.5f + affinity / 100f), 0.5f));
            return " " + heart + " " + text;
        }

        private static string AddRarityBadge(Chara character, string text)
        {
            string badge = GetRaritySymbol(character.rarity);
            Color color = GetRarityColor(character.rarity);
            return badge.TagColor(color) + " " + text;
        }

        private static string BuildLevelBadge(Chara character)
        {
            int comparisonIndex = 2;
            if (character.LV >= EClass.pc.LV * 5)
                comparisonIndex = 0;
            else if (character.LV >= EClass.pc.LV * 2)
                comparisonIndex = 1;
            else if (character.LV <= EClass.pc.LV / 4)
                comparisonIndex = 4;
            else if (character.LV <= EClass.pc.LV / 2)
                comparisonIndex = 3;

            Color color = EClass.Colors.gradientLVComparison.Evaluate(0.25f * comparisonIndex);
            string danger = comparisonIndex == 0 ? " ☠ ".TagSize(30).TagColor(color) : string.Empty;
            return (" Lv." + character.LV).TagColor(color) + danger;
        }

        private static string BuildIdentityLine(Chara character)
        {
            string identity = $" {Lang._gender(character.bio.gender)} {character.bio.TextAge(character)} [{character.race.GetName()} {character.job.GetName()} {character.tactics.source.GetName()}]";
            string style = $" {character.elements.GetOrCreateElement(character.GetArmorSkill()).Name} {("style" + character.body.GetAttackStyle()).lang()}";
            return identity + style;
        }

        private static string BuildCombatLine(Chara character)
        {
            return DisplayText.JoinNonEmpty(" ",
                BuildPointValue("HP", character.hp, character.MaxHP, Color.red),
                $"DV:{character.DV}",
                $"PV:{character.PV}",
                BuildSpeedValue(character));
        }

        private static string BuildPointValue(string label, int current, int max, Color labelColor)
        {
            Color valueColor = current > max * 0.2f ? new Color(0.73f, 1f, 0.82f) : new Color(1f, 0.67f, 0.67f);
            return label.TagColor(labelColor) + ":" + ($"{current}/{max}").TagColor(valueColor);
        }

        private static string BuildReflectedPointValue(string label, Chara character, Color labelColor, string[] currentPaths, string[] maxPaths)
        {
            int current = ReflectionReader.IntMember(character, currentPaths);
            int max = ReflectionReader.IntMember(character, maxPaths);

            if (current < 0 && max < 0)
                return string.Empty;

            if (current < 0)
                current = 0;

            if (max <= 0)
                max = current;

            return BuildPointValue(label, current, max, labelColor);
        }

        private static string BuildReflectedPointValue(string label, Chara character, Color labelColor, params string[] memberPaths)
        {
            if (memberPaths == null || memberPaths.Length == 0)
                return string.Empty;

            int splitIndex = memberPaths.Length / 2;
            if (splitIndex <= 0)
                splitIndex = 1;

            string[] currentPaths = memberPaths.Take(splitIndex).ToArray();
            string[] maxPaths = memberPaths.Skip(splitIndex).ToArray();
            return BuildReflectedPointValue(label, character, labelColor, currentPaths, maxPaths);
        }

        private static string BuildSpeedValue(Chara character)
        {
            int speed = ReflectionReader.IntMember(character, "Speed", "speed");
            if (speed < 0)
                return string.Empty;

            object speedElement = GetElement(character, 79);
            string speedName = ReflectionReader.StringMember(speedElement, "Name", "name") ?? "Speed";
            return speedName + ":" + speed;
        }

        private static string BuildLifestyleLine(Chara character)
        {
            string sp = BuildReflectedPointValue("SP", character, Color.green, new[] { "stamina.value", "Stamina.value", "SP", "sp" }, new[] { "stamina.max", "Stamina.max", "MaxSP", "maxSP" });
            string hunger = character.hunger != null ? $"{character.hunger.name}:{character.hunger.value}/{character.hunger.max}" : string.Empty;
            string works = string.Join(", ", character.ListWorks().Select(work => work.Name).ToArray());
            string hobbies = string.Join(", ", character.ListHobbies().Select(hobby => hobby.Name).ToArray());

            return DisplayText.JoinNonEmpty(" ",
                sp,
                hunger,
                string.IsNullOrWhiteSpace(works) ? string.Empty : "Work:" + works,
                string.IsNullOrWhiteSpace(hobbies) ? string.Empty : "Hobby:" + hobbies);
        }

        private static string BuildResourceExperienceLine(Chara character)
        {
            return DisplayText.JoinNonEmpty(" ",
                BuildReflectedPointValue("MP", character, Color.blue, new[] { "mana.value", "MP", "mp" }, new[] { "mana.max", "MaxMP", "maxMP" }),
                BuildWeightLine(character),
                BuildExperienceLine(character));
        }

        private static string BuildWeightLine(Chara character)
        {
            object inventory = ReflectionReader.Member(character, "things")
                               ?? ReflectionReader.Member(character, "Things")
                               ?? ReflectionReader.Member(character, "inventory")
                               ?? ReflectionReader.Member(character, "Inventory");

            int current = FirstValidInt(
                ReflectionReader.IntMember(inventory, "ChildrenWeight", "ChildrenAndSelfWeight", "Weight", "weight"),
                ReflectionReader.IntFromValue(ReflectionReader.InvokeNoArgs(inventory, "GetWeight", "get_ChildrenWeight", "get_ChildrenAndSelfWeight")),
                ReflectionReader.IntMember(character, "things.ChildrenWeight", "things.ChildrenAndSelfWeight", "inventory.ChildrenWeight", "inventory.ChildrenAndSelfWeight")
            );

            int max = FirstValidInt(
                ReflectionReader.IntMember(character, "WeightLimit", "weightLimit", "MaxWeight", "maxWeight"),
                ReflectionReader.IntFromValue(ReflectionReader.InvokeNoArgs(character, "GetWeightLimit", "get_WeightLimit")),
                ReflectionReader.IntMember(inventory, "WeightLimit", "MaxWeight", "maxWeight")
            );

            if (current < 0 && max < 0)
                return string.Empty;

            if (current < 0)
                current = 0;

            if (max < 0)
                return "Weight:" + FormatWeight(current);

            return "Weight:" + FormatWeight(current) + "/" + FormatWeight(max);
        }

        private static int FirstValidInt(params int[] values)
        {
            foreach (int value in values)
            {
                if (value >= 0)
                    return value;
            }
            return -1;
        }

        private static string FormatWeight(int value)
        {
            if (value >= 1000)
                return (value / 1000f).ToString("0.#");

            return value.ToString();
        }

        private static string BuildExperienceLine(Chara character)
        {
            return "EXP:" + character.exp + "/" + character.ExpToNext;
        }

        private static string BuildAttributeLine(Chara character)
        {
            var values = new List<string>();
            for (int i = 0; i < AttributeIds.Length; i++)
            {
                object element = GetElement(character, AttributeIds[i]);
                string name = ReflectionReader.StringMember(element, "Name", "name") ?? AttributeFallbackNames[i];
                int value = ReflectionReader.IntMember(element, "Value", "value", "vBase", "Base");
                if (value >= 0)
                    values.Add(name + ":" + value);
            }
            return string.Join(" ", values.ToArray());
        }

        private static string BuildResistanceLine(Chara character)
        {
            var values = new List<string>();
            foreach (int id in ResistanceIds)
            {
                object element = GetElement(character, id);
                int value = ReflectionReader.IntMember(element, "Value", "value", "vBase", "Base");
                if (element == null || value <= 0)
                    continue;

                string name = ReflectionReader.StringMember(element, "Name", "name") ?? ("Res" + id);
                values.Add(ResistanceText.Colorize(name, id) + ":" + value);
            }
            return DisplayText.WrapLines(values, 5);
        }

        private static void AppendAffinityGiftLine(ref string result, Chara character)
        {
            bool showAffinity = ModConfig.CharacterAffinity.ShouldShow(character);
            bool showGift = ModConfig.CharacterFavoriteGift.ShouldShow(character);

            if (!showAffinity && !showGift)
                return;

            string affinity = showAffinity ? BuildAffinityText(character) : string.Empty;
            string gift = showGift ? BuildFavoriteGiftText(character) : string.Empty;

            if (string.IsNullOrWhiteSpace(affinity) && string.IsNullOrWhiteSpace(gift))
                return;

            string line;
            if (string.IsNullOrWhiteSpace(affinity))
                line = "Gift: " + gift;
            else if (string.IsNullOrWhiteSpace(gift))
                line = affinity;
            else
                line = affinity + " | Gift: " + gift;

            int fontSize = Math.Max(ModConfig.CharacterAffinity.FontSize, ModConfig.CharacterFavoriteGift.FontSize);
            result += Environment.NewLine + DisplayText.Size(line, fontSize);
        }

        private static string BuildAffinityText(Chara character)
        {
            int affinity = ReflectionReader.IntMember(character, "_affinity", "affinity", "Affinity");
            if (affinity < 0)
                affinity = 0;

            string heart = affinity > 74 ? "♥" : "♡";
            Color color = affinity > 74 ? new Color(1f, 0.45f, 0.65f) : new Color(0.8f, 0.8f, 0.8f);
            return heart.TagColor(color) + " " + affinity;
        }

        private static string BuildFavoriteGiftText(Chara character)
        {
            try
            {
                object category = character.GetFavCat();
                object food = character.GetFavFood();

                string categoryName = ReflectionReader.StringFromValue(ReflectionReader.InvokeNoArgs(category, "GetName"))
                    ?? ReflectionReader.StringMember(category, "Name", "name", "id")
                    ?? string.Empty;

                string foodName = ReflectionReader.StringFromValue(ReflectionReader.InvokeNoArgs(food, "GetName"))
                    ?? ReflectionReader.StringMember(food, "Name", "name", "id")
                    ?? string.Empty;

                if (string.IsNullOrWhiteSpace(categoryName) && string.IsNullOrWhiteSpace(foodName))
                    return string.Empty;

                if (string.IsNullOrWhiteSpace(categoryName))
                    return foodName;

                if (string.IsNullOrWhiteSpace(foodName))
                    return categoryName;

                return categoryName + " / " + foodName;
            }
            catch (Exception ex)
            {
                Plugin.Log.LogDebug("Favorite gift lookup failed: " + ex.Message);
                return string.Empty;
            }
        }


        private static string BuildVitalsLine(Chara character)
        {
            return DisplayText.JoinNonEmpty(" ",
                BuildPointValue("HP", character.hp, character.MaxHP, Color.red),
                BuildReflectedPointValue("SP", character, Color.green, new[] { "stamina.value", "Stamina.value", "SP", "sp" }, new[] { "stamina.max", "Stamina.max", "MaxSP", "maxSP" }),
                BuildReflectedPointValue("MP", character, Color.blue, new[] { "mana.value", "MP", "mp" }, new[] { "mana.max", "MaxMP", "maxMP" }),
                "DV:" + character.DV,
                "PV:" + character.PV,
                BuildSpeedValue(character));
        }

        private static string BuildSummaryLine(Chara character)
        {
            string hunger = character.hunger != null ? "Food:" + character.hunger.value + "/" + character.hunger.max : string.Empty;
            string works = string.Join(", ", character.ListWorks().Select(work => work.Name).ToArray());
            string hobbies = string.Join(", ", character.ListHobbies().Select(hobby => hobby.Name).ToArray());

            return DisplayText.JoinNonEmpty(" ",
                hunger,
                BuildWeightLine(character),
                BuildExperienceLine(character),
                string.IsNullOrWhiteSpace(works) ? string.Empty : "Work:" + works,
                string.IsNullOrWhiteSpace(hobbies) ? string.Empty : "Hobby:" + hobbies);
        }

        private static string BuildDebugLine(Chara character)
        {
            string tactics = character.source.tactics.IsEmpty(EClass.sources.tactics.map.TryGetValue(character.id)?.id ?? EClass.sources.tactics.map.TryGetValue(character.job.id)?.id ?? "predator");
            return "Global:" + character.IsGlobal + " AI:" + character.ai + " " + tactics + Environment.NewLine + character.uid + character.IsMinion + "/" + character.c_uidMaster + "/" + character.master + " dir:" + character.dir + " skin:" + character.idSkin;
        }

        private static string BuildConditionLine(Chara character)
        {
            IEnumerable<BaseStats> stats = character.conditions.Concat(!character.IsPCFaction ? new BaseStats[0] : new BaseStats[] { character.hunger, character.stamina });
            var values = new List<string>();

            foreach (BaseStats stat in stats)
            {
                string phase = stat.GetPhaseStr();
                if (phase.IsEmpty() || phase == "#")
                    continue;

                Color color = Color.white;
                switch (stat.source.group)
                {
                    case "Bad":
                    case "Debuff":
                    case "Disease":
                        color = EClass.Colors.colorDebuff;
                        break;
                    case "Buff":
                        color = EClass.Colors.colorBuff;
                        break;
                }

                string text = phase + "(" + stat.GetValue() + ")";
                if (character.resistCon != null && character.resistCon.ContainsKey(stat.id))
                    text += "{" + character.resistCon[stat.id] + "}";

                values.Add(text.TagColor(color));
            }

            return values.Count == 0 ? string.Empty : "Conditions: " + string.Join(", ", values.ToArray());
        }

        private static string BuildActionLine(Chara character)
        {
            var actions = new List<string>();
            foreach (ActList.Item item in character.ability.list.items)
            {
                string parentName = null;
                if (!string.IsNullOrWhiteSpace(item.act.source.aliasParent))
                {
                    string aliasParentElement = Element.GetName(item.act.source.aliasParent);
                    if (aliasParentElement != null)
                        parentName = "(" + aliasParentElement + ")";
                }
                actions.Add(item.act.Name + parentName);
            }
            return string.Join(", ", actions.ToArray());
        }

        private static string BuildFeatLine(Chara character)
        {
            var feats = new List<string>();
            object elements = ReflectionReader.Member(character, "elements");
            object listObject = ReflectionReader.Member(elements, "list") ?? ReflectionReader.Member(elements, "dict") ?? ReflectionReader.Member(elements, "map") ?? elements;

            IEnumerable enumerable = listObject as IEnumerable;
            if (enumerable == null)
                return string.Empty;

            foreach (object entry in enumerable)
            {
                object element = ReflectionReader.Member(entry, "Value") ?? entry;
                object source = ReflectionReader.Member(element, "source");
                string category = ReflectionReader.StringMember(source, "category", "Category");
                string categorySub = ReflectionReader.StringMember(source, "categorySub", "CategorySub");
                if (category != "feat" && categorySub != "feat")
                    continue;

                int value = ReflectionReader.IntMember(element, "Value", "value");
                if (value == 0)
                    continue;

                string name = ReflectionReader.StringMember(element, "Name", "name");
                if (!string.IsNullOrWhiteSpace(name))
                    feats.Add(name + (value > 0 ? "(" + value + ")" : string.Empty));
            }

            return feats.Count == 0 ? string.Empty : "Feats: " + string.Join(", ", feats.Distinct().ToArray());
        }

        private static object GetElement(Chara character, int id)
        {
            object elements = ReflectionReader.Member(character, "elements");
            return ReflectionReader.InvokeOneInt(elements, id, "GetOrCreateElement", "GetElement", "Get");
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
