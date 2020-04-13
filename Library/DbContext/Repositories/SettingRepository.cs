using System;
using System.Collections.Generic;
using System.Linq;
using Common.Helper;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class SettingRepository : Repository<Setting>
    {
        public SettingRepository(ProjectXContext context) : base(context)
        {
        }
        public void Create(Setting setting, bool clearCache = true)
        {
            if (setting == null)
            {
                throw new ArgumentNullException("setting");
            }
            if (Db.Settings.Any(s => s.SettingKey.Equals(setting.SettingKey.ToLowerInvariant())))
            {
                throw new Exception(string.Format("Configured with key ({0}) !", setting.SettingKey));
            }
            Db.Settings.Add(setting);
            Db.SaveChanges();
        }

        public bool Exist(string settingKey)
        {
            return Db.Settings.Any(s => s.SettingKey.Equals(settingKey));
        }

        public void Update(Setting setting, string oldSettingKey, bool clearCache = true)
        {
            if (setting == null)
            {
                throw new ArgumentNullException("setting");
            }
            if (Db.Settings.Any(s => s.SettingKey.Equals(setting.SettingKey.ToLowerInvariant()) && !s.SettingKey.Equals(oldSettingKey.ToLowerInvariant())))
            {
                throw new Exception(string.Format("Configured with key ({0}) already exists!", setting.SettingKey));
            }
            Db.SaveChanges();
        }

        /// <summary>
        /// Xóa cấu hình
        /// </summary>
        /// <param name="setting">Entity cấu hình</param>
        /// <exception cref="ArgumentNullException">Ném ngoại lệ khi entity cấu hình truyền vào bị null</exception>
        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException("setting");
            }
            Db.Settings.Remove(setting);
            Db.SaveChanges();
        }

        /// <summary>
        /// Xóa cấu hình
        /// </summary>
        /// <param name="settings">Entity cấu hình</param>
        /// <exception cref="ArgumentNullException">Ném ngoại lệ khi entity cấu hình truyền vào bị null</exception>
        public virtual void DeleteSetting(IEnumerable<Setting> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            Db.Settings.RemoveRange(settings);
            Db.SaveChanges();
        }

        /// <summary>
        /// Lấy ra cấu hình theo id
        /// </summary>
        /// <param name="id">Id của cấu hình</param>
        /// <returns>Entity cấu hình</returns>
        public Setting Get(int id)
        {
            Setting setting = null;
            if (id > 0)
            {
                setting = Db.Settings.Find(id);
            }
            return setting;
        }

        /// <summary>
        /// Lấy ra cấu hình theo key
        /// </summary>
        /// <param name="key">Key của cấu hình</param>
        /// <returns>Entity cấu hình</returns>
        public Setting Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            key = key.Trim().ToLowerInvariant();
            var result = Db.Settings.SingleOrDefault(s => s.SettingKey.Equals(key));
            return result;
        }

        /// <summary>
        /// Lấy ra tất cả các cấu hình
        /// </summary>
        /// <returns>Danh sách các cấu hình dạng từ điển</returns>
        public IDictionary<string, KeyValuePair<int, string>> GetAllSettings()
        {
            var settings = Db.Settings.AsNoTracking().OrderBy(s => s.SettingKey).ToList();
            //format: <key, <id, value>>
            var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
            foreach (var s in settings)
            {
                var resourceName = s.SettingKey.ToLowerInvariant();
                if (!dictionary.ContainsKey(resourceName))
                    dictionary.Add(resourceName, new KeyValuePair<int, string>(s.Id, s.SettingValue));
            }
            return dictionary;
        }

        /// <summary>
        /// Lấy ra giá trị cấu hình theo key
        /// </summary>
        /// <param name="key">Key của cấu hình</param>
        /// <param name="defaultValue">Giá trị mặc định</param>
        /// <typeparam name="T">Kiểu của giá trị cấu hình</typeparam>
        /// <returns>Giá trị của cấu hình</returns>
        public T GetSettingValueByKey<T>(string key, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(key))
            {
                return defaultValue;
            }
            key = key.Trim().ToLowerInvariant();
            var settings = GetAllSettings();
            if (settings.ContainsKey(key))
            {
                return settings[key].Value.To<T>();
            }

            return defaultValue;
        }

        /// <summary>
        /// Gán giá trị cho cấu hình
        /// </summary>
        /// <param name="key">Key của cấu hình</param>
        /// <param name="value">Giá trị cần gán</param>
        /// <param name="saveChanges"></param>
        /// <typeparam name="T">Kiểu của giá trị</typeparam>
        /// <exception cref="ArgumentNullException">Ném ngoại lệ khi entity cấu hình truyền vào bị null</exception>
        public virtual void SetSetting<T>(string key, T value, bool saveChanges = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
            Setting setting;
            var valueStr = ConvertHelper.GetTypeConverter(typeof(T)).ConvertToInvariantString(value);
            if (settings.ContainsKey(key))
            {
                if (!settings[key].Value.Equals(valueStr))
                {
                    setting = Get(settings[key].Key);
                    setting.SettingValue = valueStr;
                }
            }
            else
            {
                setting = new Setting
                {
                    SettingKey = key,
                    SettingValue = valueStr,
                };
                Db.Settings.Add(setting);
            }

            if (saveChanges)
            {
                Db.SaveChanges();
            }
        }
    }
}
