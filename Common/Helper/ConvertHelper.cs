using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Common.Helper
{
    public static class ConvertHelper
    {
        /// <summary>
        /// Lấy ra kiểu chuyển đổi
        /// </summary>
        /// <param name="type">Kiểu</param>
        public static TypeConverter GetTypeConverter(Type type)
        {
            TypeConverter result;

            if (type == typeof(List<int>))
            {
                result = new GenericListTypeConverter<int>();
            }
            else if (type == typeof(List<decimal>))
            {
                result = new GenericListTypeConverter<decimal>();
            }
            else if (type == typeof(List<string>))
            {
                result = new GenericListTypeConverter<string>();
            }
            else
            {
                result = TypeDescriptor.GetConverter(type);
            }

            return result;
        }

        /// <summary>
        /// Chuyển đổi 1 giá trị thành 1 kiểu khác
        /// </summary>
        /// <param name="value">Giá trị cần chuyển đổi</param>
        /// <param name="destinationType">Kiểu dữ liệu mới cần chuyển đổi</param>
        /// <returns>Giá trị đã chuyển đổi</returns>
        public static object To(this object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Chuyển đổi 1 giá trị thành 1 kiểu khác
        /// </summary>
        /// <param name="value">Giá trị cần chuyển đổi</param>
        /// <param name="destinationType">Kiểu dữ liệu mới cần chuyển đổi</param>
        /// <param name="culture">Culture</param>
        /// <returns>Giá trị đã chuyển đổi</returns>
        public static object To(this object value, Type destinationType, CultureInfo culture)
        {
            object result = null;
            if (value != null)
            {
                var sourceType = value.GetType();
                var destinationConverter = GetTypeConverter(destinationType);
                var sourceConverter = GetTypeConverter(sourceType);

                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                {
                    result = destinationConverter.ConvertFrom(null, culture, value);
                }
                else if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                {
                    result = sourceConverter.ConvertTo(null, culture, value, destinationType);
                }
                else if (destinationType.IsEnum && value is int)
                {
                    result = Enum.ToObject(destinationType, (int)value);
                }
                else if (!destinationType.IsInstanceOfType(value))
                {
                    result = Convert.ChangeType(value, destinationType, culture);
                }
                else
                {
                    result = value;
                }
            }
            return result;
        }

        /// <summary>
        /// Chuyển đổi 1 giá trị thành 1 kiểu khác
        /// </summary>
        /// <param name="value">Giá trị cần chuyển đổi</param>
        /// <typeparam name="T">Kiểu dữ liệu mới cần chuyển đổi</typeparam>
        /// <returns>Giá trị đã chuyển đổi</returns>
        public static T To<T>(this object value)
        {
            return (T)To(value, typeof(T));
        }

        /// <summary>
        /// Chuyển đổi một giá trị về kiểu boolean
        /// </summary>
        /// <param name="data">Giá trị cần chuyển đổi</param>
        /// <returns>Boolean</returns>
        public static bool ConvertBoolean(object data)
        {
            bool result;
            if (data == null)
            {
                result = false;
            }
            else if (data is bool)
            {
                result = (bool)data;
            }
            else if (data is string)
            {
                var inputStr = data.ToString().Trim();
                var falseStrs = new[] { "false", "0" };

                result = !falseStrs.Any(falseStr => String.Equals(falseStr, inputStr, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                result = true;
            }
            return result;
        }
    }
}
