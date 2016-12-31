﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace anime_downloader.Classes
{
    // http://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    var fi = value.GetType().GetField(value.ToString());
                    if (fi != null)
                    {
                        var attributes =
                            (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        return attributes.Length > 0 && !string.IsNullOrEmpty(attributes[0].Description)
                            ? attributes[0].Description
                            : value.ToString();
                    }
                }

                return string.Empty;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    ///     Joins an array of strings delimited by a comma.
    /// </summary>
    public class StringJoinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumerable = value as IEnumerable<string> ?? new List<string>();
            return string.Join(", ", enumerable.Where(s => !string.IsNullOrEmpty(s)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string ?? "";
            return str.Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    /// <summary>
    ///     Returns opposite of the bool value.
    /// </summary>
    public class NotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !System.Convert.ToBoolean(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !System.Convert.ToBoolean(value);
        }
    }

    /// <summary>
    ///     Returns if value is equal to first given parameter.
    /// </summary>
    public class StringCompareConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString(value).ToLower().Equals(System.Convert.ToString(parameter).ToLower());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }

    /// <summary>
    ///     Returns if value is NOT equal to first given parameter.
    /// </summary>
    public class StringCompareConverterNot : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!System.Convert.ToString(value).Equals(System.Convert.ToString(parameter)))
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!System.Convert.ToBoolean(value))
                return parameter;
            return null;
        }
    }

    public class StringCompareVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Convert.ToString(value).Equals(System.Convert.ToString(parameter)))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Returns a symbol from the bool value representing true and false
    /// </summary>
    public class BooleanSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? "✓" : "✗";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString(value).Equals("✓");
        }
    }

    /// <summary>
    ///     Returns a color from the bool value repesenting true and false
    /// </summary>
    public class BooleanColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? "Green" : "Red";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString(value).Equals("Green");
        }
    }

    /// <summary>
    ///     Returns an opacity from the bool value.
    /// </summary>
    public class BooleanOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? 1.0 : 0.4;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Abs(System.Convert.ToDouble(value) - 1.0) < 0.01;
        }
    }

    /// <summary>
    ///     Returns an opacity from the bool value.
    /// </summary>
    public class BooleanOpacityConverterNot : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? 0.4 : 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Abs(System.Convert.ToDouble(value) - 1.0) < 0.01;
        }
    }

    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility) value == Visibility.Visible;
        }
    }

    public class StringLengthVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString(value).Length > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility) value == Visibility.Visible;
        }
    }

    // Just the opposite of the above
    public class StringLengthVisibilityConverterNot : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString(value).Length > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility) value == Visibility.Collapsed;
        }
    }

    public class IsNullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EpisodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = System.Convert.ToString(value);
            return val.Equals("0") || val.Equals("") ? "??" : val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SynposisSnipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var synopsis = value as string;
            if (synopsis == null)
                return "";
            synopsis = synopsis.Replace("&ndash;", "-");
            synopsis = new[]
                {
                    @"\[/?[a-zA-Z0-9=]*\]", // Style tags [b]
                    @"(/)?&\w+;(br)?", // Unescaped html 
                    @"<br />", @"&#[0-9]+;", // HTML break tag
                    @"\([a-zA-Z]+:[a-zA-Z\s-]+\)", // Source tag
                    @"[E|e]pisode(s)? [0-9]{1,}(-[0-9]{1,})?(&|,)?.+(\n|$)",
                    // Random information about episode previewing),
                    @"\n$" // Unneeded linebreak at the end  
                }
                .Aggregate(synopsis, (current, pattern) =>
                    Regex.Replace(current, pattern, ""));
            synopsis = Regex.Replace(synopsis, @"[\r\n]{2,}", "\n"); // Multiple linebreaks
            return synopsis;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringEqualsConverter : IValueConverter
    {
        // CurrentlyChecked
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString(value).Equals(System.Convert.ToString(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReverseVisibilityConverter : IValueConverter
    {
        // CurrentlyChecked
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility) value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringIsEmptyOrNullConverter : IValueConverter
    {
        // CurrentlyChecked
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(System.Convert.ToString(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}