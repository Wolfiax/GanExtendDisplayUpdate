using System;
using BepInEx.Configuration;

namespace GanExtendDisplay
{
    internal enum DisplayMode
    {
        Keep,
        Hide,
        Disable
    }

    internal sealed class DisplayToggle
    {
        private readonly ConfigEntry<string> _entry;

        public DisplayToggle(ConfigEntry<string> entry)
        {
            _entry = entry;
        }

        public ConfigEntry<string> Entry => _entry;

        public bool IsEnabled
        {
            get
            {
                DisplayMode mode = Parse(_entry.Value);
                return mode == DisplayMode.Keep || (mode == DisplayMode.Hide && ModState.ExtendedDisplayActive);
            }
        }

        public bool IsPatched => Parse(_entry.Value) != DisplayMode.Disable;

        private static DisplayMode Parse(string value)
        {
            return Enum.TryParse(value, true, out DisplayMode mode) ? mode : DisplayMode.Keep;
        }
    }

    internal sealed class CharacterLineSettings
    {
        private readonly DisplayToggle _toggle;
        private readonly ConfigEntry<bool> _pcFactionOnly;
        private readonly ConfigEntry<int> _fontSize;

        public CharacterLineSettings(ConfigEntry<string> displayMode, ConfigEntry<bool> pcFactionOnly, ConfigEntry<int> fontSize)
        {
            _toggle = new DisplayToggle(displayMode);
            _pcFactionOnly = pcFactionOnly;
            _fontSize = fontSize;
        }

        public bool ShouldShow(Chara character)
        {
            return _toggle.IsEnabled && (!_pcFactionOnly.Value || character.IsPCFaction);
        }

        public int FontSize => _fontSize.Value;

        public ConfigEntry<string> DisplayModeEntry => _toggle.Entry;

        public ConfigEntry<bool> PcFactionOnlyEntry => _pcFactionOnly;

        public ConfigEntry<int> FontSizeEntry => _fontSize;
    }

    internal static class ModConfig
    {
        private static readonly AcceptableValueList<string> DisplayOptions = new AcceptableValueList<string>("Keep", "Hide", "Disable");

        public static DisplayToggle CharacterDisplay { get; private set; }
        public static DisplayToggle ThingDisplay { get; private set; }
        public static DisplayToggle InteractDisplay { get; private set; }
        public static DisplayToggle NotificationDisplay { get; private set; }
        public static DisplayToggle EnchantDisplay { get; private set; }

        public static CharacterLineSettings CharacterLine1 { get; private set; }
        public static CharacterLineSettings CharacterLine2 { get; private set; }
        public static CharacterLineSettings CharacterLine3 { get; private set; }
        public static CharacterLineSettings CharacterLine4 { get; private set; }
        public static CharacterLineSettings CharacterExperience { get; private set; }
        public static CharacterLineSettings CharacterAttributes { get; private set; }
        public static CharacterLineSettings CharacterResistances { get; private set; }
        public static CharacterLineSettings CharacterAffinity { get; private set; }
        public static CharacterLineSettings CharacterFavoriteGift { get; private set; }
        public static CharacterLineSettings CharacterConditions { get; private set; }
        public static CharacterLineSettings CharacterActions { get; private set; }
        public static CharacterLineSettings CharacterFeats { get; private set; }

        public static void Bind(ConfigFile config)
        {
            CharacterDisplay = new DisplayToggle(BindDisplay(config, "Affected Display", "Character Display", "Keep", "Extra character hover information."));
            ThingDisplay = new DisplayToggle(BindDisplay(config, "Affected Display", "Thing Display", "Keep", "Extra item hover information."));
            InteractDisplay = new DisplayToggle(BindDisplay(config, "Affected Display", "Interact Display", "Keep", "Extra interaction hover information."));
            NotificationDisplay = new DisplayToggle(BindDisplay(config, "Affected Display", "Notification UI Display", "Keep", "Extra notification UI values."));
            EnchantDisplay = new DisplayToggle(BindDisplay(config, "Affected Display", "Enchant Display", "Keep", "Extra enchantment and DNA information."));

            CharacterLine1 = BindCharacterLine(config, "Display Line1", "Keep", false, 18, "Sex, age, race, job, AI, armor skill, and attack style.");
            CharacterLine2 = BindCharacterLine(config, "Display Line2", "Keep", false, 18, "HP, MP, SP, DV, PV, and speed.");
            CharacterLine3 = BindCharacterLine(config, "Display Line3", "Keep", false, 18, "SP, hunger, current work, and current hobbies.");
            CharacterLine4 = BindCharacterLine(config, "Display Line4", "Keep", false, 18, "MP, carried weight versus limit, and experience progress.");
            CharacterExperience = BindCharacterLine(config, "Display Line EXP", "Disable", false, 16, "Legacy standalone EXP line. Ignored by the rewritten formatter because Line4 already includes EXP.");
            CharacterResistances = BindCharacterLine(config, "Display Line Resist", "Keep", false, 14, "Elemental and status resistances.");
            CharacterAttributes = BindCharacterLine(config, "Display Line Attributes", "Keep", false, 14, "Strength, Endurance, Dexterity, Perception, Learning, Will, Magic, and Charisma.");
            CharacterAffinity = BindCharacterLine(config, "Display Line Affinity", "Keep", false, 14, "Relationship/affinity value and heart indicator.");
            CharacterFavoriteGift = BindCharacterLine(config, "Display Line Favorite Gift", "Keep", false, 14, "Favorite gift category and favorite food item.");
            CharacterConditions = BindCharacterLine(config, "Display Line Conditions", "Keep", false, 14, "Active buffs, debuffs, diseases, hunger, stamina, and condition resistance values.");
            CharacterActions = BindCharacterLine(config, "Display Line Act", "Hide", false, 14, "Active combat abilities.");
            CharacterFeats = BindCharacterLine(config, "Display Line Feat", "Hide", false, 14, "Passive traits and talents shown separately from active abilities.");
        }

        private static ConfigEntry<string> BindDisplay(ConfigFile config, string section, string key, string defaultValue, string description)
        {
            return config.Bind(section, key, defaultValue, new ConfigDescription(description + " Options: Keep, Hide, Disable.", DisplayOptions));
        }

        private static CharacterLineSettings BindCharacterLine(ConfigFile config, string key, string defaultMode, bool defaultPcFactionOnly, int defaultFontSize, string description)
        {
            const string section = "Extend Character Display";
            ConfigEntry<string> display = BindDisplay(config, section, key, defaultMode, description);
            ConfigEntry<bool> pcOnly = config.Bind(section, key + " PCFactionOnly", defaultPcFactionOnly, "Only show this line for the player faction.");
            ConfigEntry<int> size = config.Bind(section, key + " Size", defaultFontSize, "Font size for this line.");
            return new CharacterLineSettings(display, pcOnly, size);
        }
    }
}
