using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace MithrilCubeWpf.Prism.Converter
{
    /*
        xmlns:pc="clr-namespace:MithrilCubeWpf.Prism.Converter;assembly=MithrilCubeWpf"

        <UserControl.Resources>
            <pc:InverseBooleanConverter x:Key="InverseBooleanConverter"/>
        </UserControl.Resources>

        <TextBox x:Name="textBox" IsReadOnly="{Binding IsEnableEditButton, Converter={StaticResource InverseBooleanConverter}}"/>
     */

    /// <summary>
    /// フィールドと逆の真偽値にしてBindするためのコンバータ
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
