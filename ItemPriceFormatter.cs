using System;
using UnityEngine;

namespace GanExtendDisplay
{
    internal static class ItemPriceFormatter
    {
        public static string AppendPrice(Thing thing, string originalText)
        {
            if (thing == null)
                return originalText ?? string.Empty;

            string text = originalText ?? string.Empty;

            if (text.IndexOf("Price:", StringComparison.OrdinalIgnoreCase) >= 0)
                return text;

            int price = GetBestPrice(thing);
            if (price <= 0)
                return text;

            return text.TrimEnd()
                   + Environment.NewLine
                   + DisplayText.Size("Price: " + FormatPrice(price) + " oren", 13)
                       .TagColor(new Color(1f, 0.86f, 0.35f));
        }

        private static int GetBestPrice(Thing thing)
        {
            
            
            try
            {
                int directPrice = thing.GetPrice(CurrencyType.Money, false, PriceType.Default, null);
                if (directPrice > 0)
                    return directPrice;
            }
            catch
            {
            }

            
            int fieldPrice = FirstValidInt(
                ReflectionReader.IntMember(thing, "price", "Price", "value", "Value", "cost", "Cost"),
                ReflectionReader.IntMember(thing, "source.price", "source.Price", "source.value", "source.Value", "source.cost", "source.Cost"),
                ReflectionReader.IntMember(thing, "trait.price", "trait.Price")
            );

            if (fieldPrice > 0)
                return fieldPrice;

            object source = ReflectionReader.Member(thing, "source") ?? ReflectionReader.Member(thing, "Source");
            return FirstValidInt(
                ReflectionReader.IntMember(source, "price", "Price", "value", "Value", "cost", "Cost")
            );
        }


        private static string FormatPrice(int price)
        {
            return price.ToString("N0");
        }

        private static int FirstValidInt(params int[] values)
        {
            foreach (int value in values)
            {
                if (value > 0)
                    return value;
            }

            return -1;
        }
    }
}
