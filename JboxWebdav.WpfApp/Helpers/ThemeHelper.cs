using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JboxWebdav.WpfApp.Helpers
{
    public static class ThemeHelper
    {
        static PaletteHelper _paletteHelper;

        public static void ApplyBase(object isDark)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            theme.SetBaseTheme((bool)isDark ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
        }

        public static void ChangeHue(object obj)
        {
            var hue = StringToColor(obj.ToString());
            _paletteHelper = new PaletteHelper();
            _paletteHelper.ChangePrimaryColor(hue);
        }
        public static Color StringToColor(string colorStr)
        {
            TypeConverter cc = TypeDescriptor.GetConverter(typeof(Color));
            var result = (Color)cc.ConvertFromString(colorStr);
            return result;
        }

    }
    public static class PaletteHelperExtensions
    {
        public static void ChangePrimaryColor(this PaletteHelper paletteHelper, Color color)
        {
            ITheme theme = paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(color.Lighten(), theme.PrimaryLight.ForegroundColor);
            theme.PrimaryMid = new ColorPair(color, theme.PrimaryMid.ForegroundColor);
            theme.PrimaryDark = new ColorPair(color.Darken(), theme.PrimaryDark.ForegroundColor);
            
            if (theme is Theme internalTheme)
            {
                internalTheme.ColorAdjustment = new ColorAdjustment() { Colors = ColorSelection.All, DesiredContrastRatio = 4.5f, Contrast = Contrast.Medium };
            }
            paletteHelper.SetTheme(theme);
        }

        public static void ChangeSecondaryColor(this PaletteHelper paletteHelper, Color color)
        {
            ITheme theme = paletteHelper.GetTheme();

            theme.SecondaryLight = new ColorPair(color.Lighten(), theme.SecondaryLight.ForegroundColor);
            theme.SecondaryMid = new ColorPair(color, theme.SecondaryMid.ForegroundColor);
            theme.SecondaryDark = new ColorPair(color.Darken(), theme.SecondaryDark.ForegroundColor);

            paletteHelper.SetTheme(theme);
        }
    }

}
