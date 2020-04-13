using System;
using System.Collections.Generic;
using System.Linq;
using Common.Helper;
using Library.DbContext.Entities;

namespace Library.UnitOfWork
{
    public interface ISettings
    {
    }

    public class SettingProvider<TSettings> where TSettings : ISettings, new()
    {
        private TSettings _settings;
        private readonly UnitOfWork _unitOfWork;

        public SettingProvider()
        {
            _unitOfWork = new UnitOfWork();
        }

        public SettingProvider(string region)
        {
            _unitOfWork = new UnitOfWork();
            Region = region;
        }

        /// <summary>
        /// Lấy hoặc thiết lập loại cấu hình
        /// </summary>
        public TSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    BuildSetting();
                }
                return _settings;
            }
            protected set { _settings = value; }
        }

        public string Region { get; set; }

        private void BuildSetting()
        {
            Settings = Activator.CreateInstance<TSettings>();

            // lấy ra những thuộc tính cho phép ghi
            var properties = typeof(TSettings).GetProperties()
                        .Where(prop => prop.CanWrite && prop.CanRead)
                        .Select(
                            prop =>
                            new
                            {
                                prop,
                                setting = _unitOfWork.SettingRepo.GetSettingValueByKey<string>(BuildKey(typeof(TSettings).Name, prop.Name))
                            }
                        )
                        .Where(set => !(string.IsNullOrEmpty(set.setting))
                                      && ConvertHelper.GetTypeConverter(set.prop.PropertyType).CanConvertFrom(typeof(string))
                                      && ConvertHelper.GetTypeConverter(set.prop.PropertyType).IsValid(set.setting))
                        .Select(
                            set =>
                            new
                            {
                                set.prop,
                                value = ConvertHelper.GetTypeConverter(set.prop.PropertyType).ConvertFromInvariantString(set.setting)
                            }
                        );

            // gán giá trị cho các thuộc tính
            properties.ToList().ForEach(p => p.prop.SetValue(Settings, p.value, null));
        }

        /// <summary>
        /// Lưu các cấu hình
        /// </summary>
        public void SaveSettings()
        {
            if (_settings == null)
                return;

            SaveSettings(_settings);
        }

        /// <summary>
        /// Lưu các cấu hình liên quan đến loại cấu hình được truyền vào
        /// </summary>
        /// <param name="settings">Loại cấu hình</param>
        public void SaveSettings(TSettings settings)
        {
            Save(settings);
            Settings = settings;
        }

        /// <summary>
        /// Lưu cấu hình được truyền vào
        /// </summary>
        /// <param name="settings">Loại cấu hình</param>
        private void Save(TSettings settings)
        {
            var properties = typeof(TSettings).GetProperties().Where(prop => prop.CanWrite && prop.CanRead)
                .Where(prop => ConvertHelper.GetTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)));

            foreach (var prop in properties)
            {
                var key = BuildKey(typeof(TSettings).Name, prop.Name);
                dynamic value = prop.GetValue(settings, null);
                _unitOfWork.SettingRepo.SetSetting(key, value ?? "", false);
            }

            _unitOfWork.SettingRepo.Save();
        }

        /// <summary>
        /// Xóa các cấu hình liên quan đến loại cấu hình được truyền vào
        /// </summary>
        public void DeleteSettings()
        {
            var properties = typeof(TSettings).GetProperties();

            var settingList = new List<Setting>();
            foreach (var prop in properties)
            {
                var key = BuildKey(typeof(TSettings).Name, prop.Name);
                var setting = _unitOfWork.SettingRepo.Get(key);
                if (setting != null)
                {
                    settingList.Add(setting);
                }
            }
            _unitOfWork.SettingRepo.DeleteSetting(settingList);
        }

        private string BuildKey(string className, string propName)
        {
            return $"{className}.{propName}{(string.IsNullOrWhiteSpace(Region) ? string.Empty : $".{Region.ToLower()}")}";
        }
    }
}
