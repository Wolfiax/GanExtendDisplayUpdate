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
                    "ExtDisplay.Tab",
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
            // ModOptions v0.23.x overload:
            // SetTranslation(id, en, jp, cn)
            T(controller, TabId, "Extend Display", "拡張表示", "扩展显示");

            T(controller, "ExtDisplay.Sec.Affected", "Affected Display", "表示対象", "影响的显示");
            T(controller, "ExtDisplay.Sec.Chara", "Character Display Lines", "キャラクター表示行", "角色显示行");

            T(controller, "ExtDisplay.Keep", "Keep (always visible)", "常に表示", "保持显示");
            T(controller, "ExtDisplay.Hide", "Hide (Alt to reveal)", "隠す（Altで表示）", "隐藏（按 Alt 显示）");
            T(controller, "ExtDisplay.Disable", "Disable (never shown)", "無効", "禁用");

            T(controller, "ExtDisplay.PCFac", "PC Faction Only", "PC陣営のみ", "仅玩家阵营");
            T(controller, "ExtDisplay.IPL", "Items Per Line (0 = no limit)", "1行あたりの項目数（0 = 制限なし）", "每行项目数（0 = 无限制）");
            T(controller, "ExtDisplay.CharaNote", "Character line changes take effect immediately.", "キャラクター表示行の変更はすぐに反映されます。", "角色显示行设置会立即生效。");

            T(controller, "ExtDisplay.F.Chara", "Character Display", "キャラクター表示", "角色显示");
            T(controller, "ExtDisplay.F.Thing", "Thing Display (Ground Items)", "アイテム表示（地面アイテム）", "物品显示（地面物品）");
            T(controller, "ExtDisplay.F.Interact", "Interact Display (Harvesting)", "インタラクト表示（採集）", "互动显示（采集）");
            T(controller, "ExtDisplay.F.Notif", "Notification UI Display", "通知UI表示", "通知界面显示");
            T(controller, "ExtDisplay.F.Enchant", "Enchant Display (Equipment / DNA)", "エンチャント表示（装備 / DNA）", "附魔显示（装备 / DNA）");
            T(controller, "ExtDisplay.F.AffectedNote", "Choose which tooltip groups Gan Extend Display is allowed to modify.", "Gan Extend Display が変更できるツールチップの種類を選択します。", "选择 Gan Extend Display 可以修改哪些提示信息。");

            T(controller, "ExtDisplay.ItemPrice", "Show Item Price", "アイテム価格を表示", "显示物品价格");
            T(controller, "ExtDisplay.ItemRarity", "Show Item Rarity", "アイテム希少度を表示", "显示物品稀有度");
            T(controller, "ExtDisplay.LockLevel", "Show Lock Level", "ロックレベルを表示", "显示锁等级");

            T(controller, "ExtDisplay.L.RaceClass", "Gender / Race / Class", "性別 / 種族 / 職業", "性别 / 种族 / 职业");
            T(controller, "ExtDisplay.L.L1", "Vitals", "主要ステータス", "主要数值");
            T(controller, "ExtDisplay.L.L2", "Food / Weight / EXP", "空腹度 / 重量 / 経験値", "饱食度 / 重量 / 经验");
            T(controller, "ExtDisplay.L.L3", "Reserved Line", "予約行", "预留行");
            T(controller, "ExtDisplay.L.L4", "Reserved Line", "予約行", "预留行");
            T(controller, "ExtDisplay.L.Res", "Resistances", "耐性", "抗性");
            T(controller, "ExtDisplay.L.Att", "Attributes", "能力値", "属性");
            T(controller, "ExtDisplay.L.AffGift", "Affinity + Favorite Gift", "好感度 + 好きな贈り物", "好感度 + 喜欢的礼物");
            T(controller, "ExtDisplay.L.Cond", "Conditions", "状態", "状态");
            T(controller, "ExtDisplay.L.Act", "Abilities", "アビリティ", "能力");
            T(controller, "ExtDisplay.L.Fea", "Feats", "フィート", "特质");
        }

        private static void T(ModOptionController controller, string id, string english, string japanese, string chinese)
        {
            controller.SetTranslation(id, english, japanese, chinese);
        }

        private static string BuildXml()
        {
            return @"<config>
  <topic>ExtDisplay.Sec.Affected</topic>
  <text align=""left"">ExtDisplay.F.AffectedNote</text>

  <topic>ExtDisplay.F.Chara</topic>
  " + DisplayDropdownXml("dd_charaDisp") + @"

  <topic>ExtDisplay.F.Thing</topic>
  " + DisplayDropdownXml("dd_thingDisp") + @"

  <hlayout align=""left"">
    <toggle id=""tg_itemPrice"" width=""33%"">
      <contentId>ExtDisplay.ItemPrice</contentId>
    </toggle>
    <toggle id=""tg_itemRarity"" width=""33%"">
      <contentId>ExtDisplay.ItemRarity</contentId>
    </toggle>
    <toggle id=""tg_lockLevel"" width=""34%"">
      <contentId>ExtDisplay.LockLevel</contentId>
    </toggle>
  </hlayout>

  <topic>ExtDisplay.F.Interact</topic>
  " + DisplayDropdownXml("dd_interactDisp") + @"

  <topic>ExtDisplay.F.Notif</topic>
  " + DisplayDropdownXml("dd_notifDisp") + @"

  <topic>ExtDisplay.F.Enchant</topic>
  " + DisplayDropdownXml("dd_enchantDisp") + @"

  <topic>ExtDisplay.Sec.Chara</topic>
  <text align=""left"">ExtDisplay.CharaNote</text>
" + LineXml("ExtDisplay.L.RaceClass", "dd_lRaceClass", "tg_lRaceClasspcf", "in_lRaceClasssz") + @"
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

        private static string DisplayDropdownXml(string id)
        {
            return $@"<one_choice id=""{id}"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>";
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
            BindToggle(builder, "tg_itemPrice", ModConfig.ShowItemPrice);
            BindToggle(builder, "tg_itemRarity", ModConfig.ShowItemRarity);
            BindToggle(builder, "tg_lockLevel", ModConfig.ShowLockLevel);
            BindDropdown(builder, "dd_interactDisp", ModConfig.InteractDisplay.Entry);
            BindDropdown(builder, "dd_notifDisp", ModConfig.NotificationDisplay.Entry);
            BindDropdown(builder, "dd_enchantDisp", ModConfig.EnchantDisplay.Entry);

            BindLine(builder, "dd_lRaceClass", "tg_lRaceClasspcf", "in_lRaceClasssz", ModConfig.CharacterRaceClass);
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
