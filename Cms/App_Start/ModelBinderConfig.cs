using System;
using System.Web.Mvc;
using Common.Binders;

namespace Cms
{
    public static class ModelBinderConfig
    {
        public static void Register(ModelBinderDictionary binder)
        {
            binder.Add(typeof(string[]), new StringArrayBinder());
            binder.Add(typeof(int[]), new IntArrayBinder());
            binder.Add(typeof(double[]), new DoubleArrayBinder());

            binder.Add(typeof(long), new LongModelBinder());
            binder.Add(typeof(long?), new LongModelBinder());
            binder.Add(typeof(int), new IntegerModelBinder());
            binder.Add(typeof(int?), new IntegerModelBinder());
            binder.Add(typeof(double), new DoubleModelBinder());
            binder.Add(typeof(double?), new DoubleModelBinder());
            binder.Add(typeof(decimal), new DecimalModelBinder());
            binder.Add(typeof(decimal?), new DecimalModelBinder());
            binder.Add(typeof(bool), new BooleanModelBinder());
            binder.Add(typeof(bool?), new BooleanModelBinder());
            binder.Add(typeof(DateTime), new DateModelBinder());
            binder.Add(typeof(DateTime?), new DateModelBinder());
        }
    }
}