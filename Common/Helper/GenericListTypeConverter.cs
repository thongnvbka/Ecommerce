using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Common.Helper
{
    /// <summary> 
    /// <para>1 class cho phép convert dữ liệu từ 1 List thành string có dạng : abc, def, hig và ngược lại.</para>
    /// </summary>
    public class GenericListTypeConverter<T> : TypeConverter
    {
        protected readonly TypeConverter TypeConverter;

        /// <summary>
        /// Khởi tạo
        /// </summary>
        /// <exception cref="InvalidOperationException">Ném ra ngoại lệ khi không tìm thấy kiểu chuyển đổi tương ứng với kiểu dữ liệu truyền vào</exception>
        public GenericListTypeConverter()
        {
            TypeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (TypeConverter == null)
                throw new InvalidOperationException("No type converter exists for type " + typeof(T).FullName);
        }


        protected virtual string[] GetStringArray(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                var result = input.Split(',');
                Array.ForEach(result, s => s.Trim());
                return result;
            }

            return new string[0];
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                var items = GetStringArray(sourceType.ToString());
                return (items.Any());
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var items = GetStringArray(value as string);
                var result = new List<T>();
                Array.ForEach(items, s =>
                {
                    var item = TypeConverter.ConvertFromInvariantString(s);
                    if (item != null)
                    {
                        result.Add((T)item);
                    }
                });

                return result;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var result = string.Empty;
                if (value != null)
                {
                    for (var i = 0; i < ((IList<T>)value).Count; i++)
                    {
                        var str1 = Convert.ToString(((IList<T>)value)[i], CultureInfo.InvariantCulture);
                        result += str1;
                        if (i != ((IList<T>)value).Count - 1)
                        {
                            result += ",";
                        }
                    }
                }
                return result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
