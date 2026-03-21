using UnityEngine;

namespace Enums
{
    public enum ColorType
    {
        None = 0,
        Red = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4
    }

    public static class ColorTypeExtensions
    {
        public static Color ToColor(this ColorType colorType)
        {
            return colorType switch
            {
                ColorType.Red => Color.red,
                ColorType.Blue => Color.blue,
                ColorType.Green => Color.green,
                ColorType.Yellow => Color.yellow,
                _ => Color.white
            };
        }
    }
}
