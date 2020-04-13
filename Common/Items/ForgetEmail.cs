using System;

namespace Common.Items
{
    public class ForgetEmail
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public int SystemId { get; set; }
        public DateTime SendDate { get; set; }
        public string Link { get; set; }

        public override string ToString()
        {
            return string.Join("|", Email, Code, SystemId, SendDate.ToString());
        }

        public bool FromString(string itemString)
        {
            itemString = PasswordEncrypt.PasswordEncrypt.Base64Decode(itemString);

            if (string.IsNullOrEmpty(itemString))
                return false;

            string[] strings = itemString.Split('|');
            if (strings.Length < 3)
                return false;

            Email = strings[0];
            Code = strings[1];
            SystemId = int.Parse(strings[2]);
            SendDate = DateTime.Parse(strings[3]);

            return true;
        }

        public ForgetEmail() { }

        public ForgetEmail(string email, int systemId, DateTime sendDate, string domain)
        {
            this.Email = email;
            this.Code = PasswordEncrypt.PasswordEncrypt.GeneratePassword(20);
            this.SystemId = systemId;
            this.SendDate = sendDate;
            this.Link = domain + "/vi/lay-lai-mat-khau-" + PasswordEncrypt.PasswordEncrypt.Base64Encode(this.ToString());
        }
    }
}
