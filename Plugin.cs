using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace GanExtendDisplay
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("evilmask.elinplugins.modoptions", BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "ExtendDisplay";
        public const string PluginName = "ExtendDisplay";
        public const string PluginVersion = "1.1.0";

        internal static ManualLogSource Log { get; private set; }

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo("Initializing Gan Extend Display configuration.");
            ModConfig.Bind(Config);
            ModOptionsIntegration.TryRegister();
        }

        private void Start()
        {
            _harmony = new Harmony(PluginGuid);
            PatchIfEnabled(ModConfig.CharacterDisplay, typeof(CharacterHoverPatch));
            PatchIfEnabled(ModConfig.ThingDisplay, typeof(ThingHoverPatch));
            PatchIfEnabled(ModConfig.InteractDisplay, typeof(InteractionPatch));
            PatchIfEnabled(ModConfig.NotificationDisplay, typeof(NotificationPatch));
            PatchIfEnabled(ModConfig.EnchantDisplay, typeof(EnchantPatch));
            Log.LogInfo("Gan Extend Display patches applied.");
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
                ModState.AltHeld = true;

            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                ModState.AltHeld = false;
                if (!ModState.ExtendedDisplayLocked)
                    ModState.ExtendedDisplayActive = false;
            }

            if (!ModState.AltHeld)
            {
                if (ModState.ExtendedDisplayLocked)
                    ModState.ExtendedDisplayActive = true;
                return;
            }

            ModState.ExtendedDisplayActive = true;

            if (Input.GetKeyUp(KeyCode.CapsLock))
                ModState.ExtendedDisplayLocked = !ModState.ExtendedDisplayLocked;

            if (Input.GetKeyUp(KeyCode.End))
                ModState.Enabled = !ModState.Enabled;

            if (Input.GetKeyUp(KeyCode.Home))
                EClass.debug.showExtra = !EClass.debug.showExtra;
        }

        private void PatchIfEnabled(DisplayToggle toggle, System.Type patchType)
        {
            if (toggle != null && toggle.IsPatched)
                _harmony.PatchAll(patchType);
        }
    }
}
