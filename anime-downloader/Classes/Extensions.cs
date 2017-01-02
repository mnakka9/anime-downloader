﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace anime_downloader.Classes
{
    public static class Extensions
    {
        public static void AddSorted<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            var i = 0;
            while (i < list.Count && comparer.Compare(list[i], item) < 0)
                i++;

            list.Insert(i, item);
        }

        public static string OnlyLettersAndSpace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsLetter(c) || !char.IsWhiteSpace(c))
                .ToArray());
        }

        // http://www.pavey.me/2015/04/aspnet-c-extension-method-to-get-enum.html
        public static string Description(this Enum value)
        {
            // variables  
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            // return  
            return attributes.Length == 0 ? value.ToString() : ((DescriptionAttribute) attributes[0]).Description;
        }
    }
}