using Library.DbContext.Entities;

namespace Library.Models
{
    public class CustomerState
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public byte LevelId { get; set; }
        public string LevelName { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string SessionId { get; set; }
        public decimal BalanceAvalible { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Culture { get; set; }
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        /// <summary>
        /// Exports a short string list of Id, Email, Name separated by |
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("|", Id.ToString(), Email, FullName, Phone, Avatar, SessionId, LevelId, LevelName, Culture, string.Format("{0:####0}", BalanceAvalible), Password, Address, WarehouseId.ToString(), WarehouseName);
        }


        /// <summary>
        /// Imports Id, Email and Name from a | separated string
        /// </summary>
        /// <param name="itemString"></param>
        public bool FromString(string itemString)
        {
            if (string.IsNullOrEmpty(itemString))
                return false;

            var strings = itemString.Split('|');
            if (strings.Length < 3)
                return false;

            Id = int.Parse(strings[0]);
            Email = strings[1];
            FullName = strings[2];
            Phone = strings[3];
            Avatar = strings[4];
            SessionId = strings[5];
            LevelId = byte.Parse(strings[6]);
            LevelName = strings[7];
            Culture = strings[8];
            BalanceAvalible = decimal.Parse(strings[9]);
            Password = strings[10];
            Address = strings[11];

            int warehouseId;
            WarehouseId = int.TryParse(strings[12], out warehouseId) ? warehouseId : -1;
            WarehouseName = strings[13];
            return true;
        }

        /// <summary>
        /// Populates the AppUserState properties from a
        /// User instance
        /// </summary>
        /// <param name="customer"></param>
        public void FromUser(Customer customer)
        {
            Id = customer.Id;
            Email = customer.Email;
            FullName = customer.FullName;
            Phone = customer.Phone;
            Avatar = customer.Avatar;
            LevelId = customer.LevelId;
            LevelName = customer.LevelName;
            BalanceAvalible = customer.BalanceAvalible;
            Password = customer.Password;
            Culture = customer.CountryId;
            Address = customer.Address;
            WarehouseId = customer.WarehouseId;
            WarehouseName = customer.WarehouseName;
        }

        /// <summary>
        /// Determines if the user is logged in
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(FullName))
                return true;

            return false;
        }
    }
}
