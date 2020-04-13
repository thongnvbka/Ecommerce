using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Library.UnitOfWork;
using Newtonsoft.Json;

namespace Library.Settings
{
    public class NotifySetting : ISettings
    {
        public bool IsFollow { get; set; }

        public string UsersString { get; set; }

        public List<UserNotify> Users
        {
            get
            {
                if (string.IsNullOrWhiteSpace(UsersString))
                {
                    return new List<UserNotify>();
                }
                try
                {
                    return JsonConvert.DeserializeObject<List<UserNotify>>(UsersString);
                }
                catch
                {
                    return new List<UserNotify>();
                }
            }
        }
    }

    public class UserNotify
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public bool IsNotify { get; set; }
    }
}