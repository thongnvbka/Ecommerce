using Library.DbContext.Entities;

namespace Library.Models
{
    public class UserState
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int? OfficeId { get; set; }
        public string OfficeName { get; set; }
        public string OfficeIdPath { get; set; }
        public string OfficeAddress { get; set; }
        public byte? OfficeType { get; set; }
        public int? TitleId { get; set; }
        public string TitleName { get; set; }
        public string SessionId { get; set; }
        /// <summary>
        /// 0: Nhân viên, 1: Trưởng đơn vị, 2: Quản lý, 3: Giao hàng
        /// </summary>
        public byte? Type { get; set; }
        public string Avatar { get; set; }
        public string Culture { get; set; }
        /// <summary>
        /// Exports a short string list of Id, Email, Name separated by |
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("|", UserId.ToString(), UserName, FullName, OfficeId.ToString(), OfficeName, OfficeType, TitleId.ToString(), TitleName, SessionId, Type, OfficeIdPath, OfficeAddress, Avatar, Culture);
        }


        /// <summary>
        /// Imports Id, Email and Name from a | separated string
        /// </summary>
        /// <param name="itemString"></param>
        public bool FromString(string itemString)
        {
            if (string.IsNullOrEmpty(itemString))
                return false;

            string[] strings = itemString.Split('|');
            if (strings.Length < 3)
                return false;

            UserId = int.Parse(strings[0]);
            UserName = strings[1];
            FullName = strings[2];

            int officeId;
            OfficeId = int.TryParse(strings[3], out officeId) ? officeId : 0;

            OfficeName = strings[4];

            byte officeType;
            OfficeType = byte.TryParse(strings[5], out officeType) ? officeType : (byte)0;

            int titleId;
            TitleId = int.TryParse(strings[6], out titleId) ? titleId : 0;

            TitleName = strings[7];
            SessionId = strings[8];

            byte type;
            Type = byte.TryParse(strings[9], out type) ? type : (byte)0;

            OfficeIdPath = strings[10];
            OfficeAddress = strings[11];
            Avatar = strings[12];
            Culture = strings[13];
            return true;
        }

        /// <summary>
        /// Populates the AppUserState properties from a
        /// User instance
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userPosition"></param>
        /// <param name="office"></param>
        public void FromUser(User user, UserPosition userPosition, Office office)
        {
            UserId = user.Id;
            UserName = user.UserName;
            FullName = user.FullName;
            TitleId = userPosition?.TitleId;
            TitleName = userPosition?.TitleName;
            OfficeId = userPosition?.OfficeId;
            OfficeName = userPosition?.OfficeName;
            OfficeIdPath = userPosition?.OfficeIdPath;
            OfficeType = office?.Type;
            OfficeAddress = office?.Address;
            Type = userPosition?.Type;
            Avatar = user.Avatar;
            Culture = user.Culture;
        }

        /// <summary>
        /// Determines if the user is logged in
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(FullName))
                return true;

            return false;
        }
    }
}
