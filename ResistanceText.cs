using UnityEngine;

namespace GanExtendDisplay
{
    internal static class ResistanceText
    {
        public static string Colorize(string name, int id)
        {
            return name.TagColor(ColorFor(id));
        }

        private static Color ColorFor(int id)
        {
            switch (id)
            {
                case 950: return new Color(1f, 0.5f, 0.2f);   // Fire
                case 951: return new Color(0.2f, 0.5f, 1f);   // Cold
                case 952: return new Color(1f, 1f, 0.2f);     // Lightning
                case 953: return new Color(0.5f, 0.2f, 0.5f); // Darkness
                case 954: return new Color(0.5f, 0.2f, 1f);   // Mind/Mesmerize
                case 955: return new Color(0.2f, 1f, 0.2f);   // Poison
                case 956: return new Color(1f, 0.2f, 0.2f);   // Nether/Hell
                case 957: return new Color(1f, 0.5f, 0.5f);   // Sound
                case 958: return new Color(0.2f, 1f, 0.6f);   // Nerve
                case 959: return new Color(0.6f, 0.3f, 0.2f); // Chaos
                case 960: return new Color(1f, 1f, 0.5f);     // Holy
                case 961: return new Color(0.2f, 0.2f, 1f);   // Magic
                case 962: return new Color(0.2f, 0.2f, 1f);   // Ether
                case 963: return new Color(0.2f, 1f, 0.2f);   // Acid
                case 964: return new Color(1f, 0.2f, 0.2f);   // Bleed
                case 965: return new Color(0.2f, 0.2f, 1f);   // Impact
                case 970: return new Color(0.6f, 0.3f, 0.2f); // Corruption
                case 971: return new Color(0.5f, 0.5f, 0.5f); // Damage
                case 972: return new Color(0.2f, 0.2f, 0.2f); // Curse
                default: return Color.white;
            }
        }
    }
}
