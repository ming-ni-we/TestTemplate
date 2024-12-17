using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TestItemTemplate.Models;
using TestItemTemplate.ViewModels.Pages;
using Wpf.Ui.Appearance;

namespace TestItemTemplate.Helpers
{
    internal class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not String enumString)
            {
                throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
            }

            if (!Enum.IsDefined(typeof(ApplicationTheme), value))
            {
                throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum");
            }

            var enumValue = Enum.Parse(typeof(ApplicationTheme), enumString);

            return enumValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not String enumString)
            {
                throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
            }

            return Enum.Parse(typeof(ApplicationTheme), enumString);
        }
    }


    public class DataGridColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var settingVm = App.GetService<SettingsViewModel>();
            ItemState state = (ItemState)value;
            if (state == ItemState.测试通过)
            {
                return new SolidColorBrush(Color.FromRgb(0x67, 0xc2, 0x3a));
            }
            else if (state == ItemState.未测试)
            {
                return settingVm.CurrentTheme;
                /*                return new SolidColorBrush(Color.FromRgb(0x0, 0x0, 0x0));*/
            }
            else if (state == ItemState.测试失败)
            {
                return new SolidColorBrush(Color.FromRgb(0xff, 0x0, 0x0));
            }
            return new SolidColorBrush(Color.FromRgb(0xff, 0x0, 0x0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class CardColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var settingVm = App.GetService<SettingsViewModel>();
            if (value.ToString() == "PASS")
            {
                return new SolidColorBrush(Color.FromRgb(0x67, 0xc2, 0x3a));
            }
            else if (value.ToString() == "等待测试")
            {
                return settingVm.CurrentTheme;
                /*                return new SolidColorBrush(Color.FromRgb(0x0, 0x0, 0x0));*/
            }
            else if (value.ToString() == "测试中")
            {
                return new SolidColorBrush(Color.FromRgb(0xE6, 0xA2, 0x3C));
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(0xff, 0x0, 0x0));
            }


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
