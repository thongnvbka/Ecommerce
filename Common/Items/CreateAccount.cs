using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Items
{
    public class CreateAccount
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public int SystemId { get; set; }
        public DateTime SendDate { get; set; }
        public int UserId { get; set; }
        public string Link { get; set; }

        public override string ToString()
        {
            return string.Join("|", Email, Code, SystemId, SendDate.ToString(), UserId);
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
            UserId = int.Parse(strings[4]);

            return true;
        }

        public CreateAccount() { }

        public CreateAccount(string email, int systemId, DateTime sendDate, string domain, int userId)
        {
            this.Email = email;
            this.Code = PasswordEncrypt.PasswordEncrypt.GeneratePassword(20);
            this.SystemId = systemId;
            this.SendDate = sendDate;
            this.Link = "https://" + domain + "/vi/CreateAccount-" + PasswordEncrypt.PasswordEncrypt.Base64Encode(this.ToString());
            this.UserId = userId;
        }
    }
}
