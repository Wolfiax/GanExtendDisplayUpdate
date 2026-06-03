using System;
using BepInEx.Configuration;
using EvilMask.Elin.ModOptions;
using EvilMask.Elin.ModOptions.UI;

namespace GanExtendDisplay
{
    internal static class ModOptionsIntegration
    {
        private const string TabId = "ExtDisplay.Tab";
        private static bool _registered;

        public static void TryRegister()
        {
            if (_registered)
                return;

            try
            {
                ModOptionController controller = ModOptionController.Register(
                    Plugin.PluginGuid,
                    "Extend Display",
                    new object[] { TabId });

                SetTranslations(controller);
                controller.SetPreBuildWithXml(BuildXml());
                controller.OnBuildUI += OnBuildUI;

                _registered = true;
                Plugin.Log?.LogInfo("Gan Extend Display registered with Mod Options.");
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogWarning("Gan Extend Display could not register with Mod Options. The normal BepInEx config file will still work. " + ex.Message);
            }
        }

        private static void SetTranslations(ModOptionController controller)
        {
            T(controller, TabId, "Extend Display");
            T(controller, "ExtDisplay.Sec.Affected", "Affected Display");
            T(controller, "ExtDisplay.Sec.Chara", "Character Display Lines");
            T(controller, "ExtDisplay.Keep", "Keep (always visible)");
            T(controller, "ExtDisplay.Hide", "Hide (Alt to reveal)");
            T(controller, "ExtDisplay.Disable", "Disable (never shown)");
            T(controller, "ExtDisplay.PCFac", "PC faction only");
            T(controller, "ExtDisplay.IPL", "Items Per Line (0 = no limit):");
            T(controller, "ExtDisplay.CharaNote", "Character line changes take effect immediately.");

            T(controller, "ExtDisplay.F.Chara", "Character Display");
            T(controller, "ExtDisplay.F.Thing", "Thing Display (Ground Items)");
            T(controller, "ExtDisplay.F.Interact", "Interact Display (Harvesting)");
            T(controller, "ExtDisplay.F.Notif", "Notification UI Display");
            T(controller, "ExtDisplay.F.Enchant", "Enchant Display (Equipment / DNA)");
            T(controller, "ExtDisplay.F.AffectedNote", "Choose which tooltip groups Gan Extend Display is allowed to modify.");

            T(controller, "ExtDisplay.L.L1", "Line 1: Sex, Age, Race, Job, AI, Armor Skill, Attack Style");
            T(controller, "ExtDisplay.L.L2", "Line 2: HP, DV, PV, Speed");
            T(controller, "ExtDisplay.L.L3", "Line 3: SP, Hunger, Works / Hobbies");
            T(controller, "ExtDisplay.L.L4", "Line 4: MP, Weight, EXP");
            T(controller, "ExtDisplay.L.Res", "Resist Line: Elemental Resistances");
            T(controller, "ExtDisplay.L.Att", "Attributes Line: STR CON DEX PER LRN WIL MAG CHR");
            T(controller, "ExtDisplay.L.AffGift", "Affinity and Favorite Gift Line");
            T(controller, "ExtDisplay.L.Cond", "Conditions Line: Buffs / Debuffs / Diseases");
            T(controller, "ExtDisplay.L.Act", "Act Line: Active Abilities");
            T(controller, "ExtDisplay.L.Fea", "Feat Line: Passive Traits");
        }

        private static void T(ModOptionController controller, string key, string english)
        {
            controller.SetTranslation(key, english, english);
        }

        private static string BuildXml()
        {
            return @"<config>
  <topic>ExtDisplay.Sec.Affected</topic>
  <text align=""left"">ExtDisplay.F.AffectedNote</text>

  <topic>ExtDisplay.F.Chara</topic>
  <one_choice id=""dd_charaDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>

  <topic>ExtDisplay.F.Thing</topic>
  <one_choice id=""dd_thingDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>

  <topic>ExtDisplay.F.Interact</topic>
  <one_choice id=""dd_interactDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>

  <topic>ExtDisplay.F.Notif</topic>
  <one_choice id=""dd_notifDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>

  <topic>ExtDisplay.F.Enchant</topic>
  <one_choice id=""dd_enchantDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>

  <topic>ExtDisplay.Sec.Chara</topic>
  <text align=""left"">ExtDisplay.CharaNote</text>
" + LineXml("ExtDisplay.L.L1", "dd_l1", "tg_l1pcf", "in_l1sz") + @"
" + LineXml("ExtDisplay.L.L2", "dd_l2", "tg_l2pcf", "in_l2sz") + @"
" + LineXml("ExtDisplay.L.L3", "dd_l3", "tg_l3pcf", "in_l3sz") + @"
" + LineXml("ExtDisplay.L.L4", "dd_l4", "tg_l4pcf", "in_l4sz") + @"
" + LineXml("ExtDisplay.L.Res", "dd_lRes", "tg_lRespcf", "in_lRessz") + @"
" + LineXml("ExtDisplay.L.Att", "dd_lAtt", "tg_lAttpcf", "in_lAttsz") + @"
" + LineXml("ExtDisplay.L.AffGift", "dd_lAffGift", "tg_lAffGiftpcf", "in_lAffGiftsz") + @"
" + LineXml("ExtDisplay.L.Cond", "dd_lCond", "tg_lCondpcf", "in_lCondsz") + @"
" + LineXml("ExtDisplay.L.Act", "dd_lAct", "tg_lActpcf", "in_lActsz") + @"
  <text align=""left"">ExtDisplay.IPL</text>
  <input id=""in_lActipl"" width=""40%""/>
" + LineXml("ExtDisplay.L.Fea", "dd_lFea", "tg_lFeapcf", "in_lFeasz") + @"
  <text align=""left"">ExtDisplay.IPL</text>
  <input id=""in_lFeaipl"" width=""40%""/>
  <vlayout height=""30""/>
</config>";
        }

        private static string LineXml(string titleId, string dropdownId, string toggleId, string inputId)
        {
            return $@"  <topic>{titleId}</topic>
  <hlayout align=""left"">
    <one_choice id=""{dropdownId}"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""{toggleId}"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <input id=""{inputId}"" width=""40%""/>
  </hlayout>";
        }

        private static void OnBuildUI(OptionUIBuilder builder)
        {
            BindDropdown(builder, "dd_charaDisp", ModConfig.CharacterDisplay.Entry);
            BindDropdown(builder, "dd_thingDisp", ModConfig.ThingDisplay.Entry);
            BindDropdown(builder, "dd_interactDisp", ModConfig.InteractDisplay.Entry);
            BindDropdown(builder, "dd_notifDisp", ModConfig.NotificationDisplay.Entry);
            BindDropdown(builder, "dd_enchantDisp", ModConfig.EnchantDisplay.Entry);

            BindLine(builder, "dd_l1", "tg_l1pcf", "in_l1sz", ModConfig.CharacterLine1);
            BindLine(builder, "dd_l2", "tg_l2pcf", "in_l2sz", ModConfig.CharacterLine2);
            BindLine(builder, "dd_l3", "tg_l3pcf", "in_l3sz", ModConfig.CharacterLine3);
            BindLine(builder, "dd_l4", "tg_l4pcf", "in_l4sz", ModConfig.CharacterLine4);
            BindLine(builder, "dd_lRes", "tg_lRespcf", "in_lRessz", ModConfig.CharacterResistances);
            BindLine(builder, "dd_lAtt", "tg_lAttpcf", "in_lAttsz", ModConfig.CharacterAttributes);
            BindCombinedAffinityGift(builder);
            BindLine(builder, "dd_lCond", "tg_lCondpcf", "in_lCondsz", ModConfig.CharacterConditions);
            BindLine(builder, "dd_lAct", "tg_lActpcf", "in_lActsz", ModConfig.CharacterActions);
            BindLine(builder, "dd_lFea", "tg_lFeapcf", "in_lFeasz", ModConfig.CharacterFeats);
        }

        private static void BindCombinedAffinityGift(OptionUIBuilder builder)
        {
            BindDropdown(builder, "dd_lAffGift", ModConfig.CharacterAffinity.DisplayModeEntry);
            BindToggle(builder, "tg_lAffGiftpcf", ModConfig.CharacterAffinity.PcFactionOnlyEntry);
            BindInputInt(builder, "in_lAffGiftsz", ModConfig.CharacterAffinity.FontSizeEntry, 8, 40);

            ModConfig.CharacterFavoriteGift.DisplayModeEntry.Value = ModConfig.CharacterAffinity.DisplayModeEntry.Value;
            ModConfig.CharacterFavoriteGift.PcFactionOnlyEntry.Value = ModConfig.CharacterAffinity.PcFactionOnlyEntry.Value;
            ModConfig.CharacterFavoriteGift.FontSizeEntry.Value = ModConfig.CharacterAffinity.FontSizeEntry.Value;
        }

        private static void BindLine(OptionUIBuilder builder, string dropdownId, string toggleId, string inputId, CharacterLineSettings settings)
        {
            BindDropdown(builder, dropdownId, settings.DisplayModeEntry);
            BindToggle(builder, toggleId, settings.PcFactionOnlyEntry);
            BindInputInt(builder, inputId, settings.FontSizeEntry, 8, 40);
        }

        private static void BindDropdown(OptionUIBuilder builder, string id, ConfigEntry<string> entry)
        {
            OptDropdown dropdown = builder.GetPreBuild<OptDropdown>(id);
            if (dropdown == null)
                return;

            dropdown.Value = ToIndex(entry.Value);
            dropdown.OnValueChanged += index => entry.Value = FromIndex(index);
        }

        private static void BindToggle(OptionUIBuilder builder, string id, ConfigEntry<bool> entry)
        {
            OptToggle toggle = builder.GetPreBuild<OptToggle>(id);
            if (toggle == null)
                return;

            toggle.Checked = entry.Value;
            toggle.OnValueChanged += value => entry.Value = value;
        }

        private static void BindInputInt(OptionUIBuilder builder, string id, ConfigEntry<int> entry, int min, int max)
        {
            OptInput input = builder.GetPreBuild<OptInput>(id);
            if (input == null)
                return;

            input.Text = entry.Value.ToString();
            input.OnValueChanged += text =>
            {
                if (!int.TryParse(text, out int value))
                    return;

                if (value < min)
                    value = min;
                if (value > max)
                    value = max;

                entry.Value = value;
            };
        }

        private static int ToIndex(string mode)
        {
            if (string.Equals(mode, "Hide", StringComparison.OrdinalIgnoreCase))
                return 1;
            if (string.Equals(mode, "Disable", StringComparison.OrdinalIgnoreCase))
                return 2;
            return 0;
        }

        private static string FromIndex(int index)
        {
            switch (index)
            {
                case 1:
                    return "Hide";
                case 2:
                    return "Disable";
                default:
                    return "Keep";
            }
        }
    }
}
