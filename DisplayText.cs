using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GanExtendDisplay
{
    internal static class DisplayText
    {
        public static readonly Color RarityCrude = new Color(0.44705883f, 0.21176471f, 0.21176471f);
        public static readonly Color RarityNormal = new Color(0.4f, 0.4f, 0.4f);
        public static readonly Color RaritySuperior = new Color(0f, 0.6745098f, 1f);
        public static readonly Color RarityLegendary = new Color(0.9137255f, 0.4745098f, 1f);
        public static readonly Color RarityMythical = new Color(1f, 0.89411765f, 0.3137255f);
        public static readonly Color RarityArtifact = new Color(1f, 0.15686275f, 0.15686275f);

        public static string Size(string text, int size)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.TagSize(size);
        }

        public static string Color(string text, Color color)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.TagColor(color);
        }

        public static string JoinNonEmpty(string separator, params string[] values)
        {
            return string.Join(separator, values.Where(v => !string.IsNullOrWhiteSpace(v)).ToArray());
        }

        public static string WrapLines(IReadOnlyList<string> values, int itemsPerLine)
        {
            if (values == null || values.Count == 0)
                return string.Empty;

            var builder = new StringBuilder();
            for (int i = 0; i < values.Count; i++)
            {
                if (i > 0)
                    builder.Append((i % itemsPerLine == 0) ? System.Environment.NewLine : " ");

                builder.Append(values[i]);
            }
            return builder.ToString();
        }
    }
}
