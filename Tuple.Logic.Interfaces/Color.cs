using Windows.UI.Xaml.Media;

namespace Tuple.Logic.Interfaces
{
    public enum Color
    {
        Orange, // old red
        Purple, // old blue
        Green // old green
    }

    public static class ColorExtensions
    {
        private static readonly SolidColorBrush Orange;
        private static readonly SolidColorBrush Purple;
        private static readonly SolidColorBrush Green;

        static ColorExtensions()
        {
            Orange = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 83, 0));
            Purple = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 145, 0, 145));
            Green = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 166, 0));
        }

        public static SolidColorBrush GetBrush(this Color color)
        {
            switch (color)
            {
                case Color.Orange:
                    return Orange;
                case Color.Purple:
                    return Purple;
                case Color.Green:
                    return Green;
                default:
                    return null;
            }
        }
    }
}
