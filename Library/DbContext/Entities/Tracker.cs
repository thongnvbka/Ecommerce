using System;

namespace Library.DbContext.Entities
{
    // Tracker
    
    public partial class Tracker
    {
        public long Id { get; set; } // Id (Primary key)
        public string Browser { get; set; } // Browser (length: 20)
        public string Version { get; set; } // Version (length: 50)
        public string Os { get; set; } // OS (length: 15)
        public string PageUrl { get; set; } // PageUrl (length: 400)
        public string UrlReferrer { get; set; } // UrlReferrer (length: 450)
        public string SessionId { get; set; } // SessionID (length: 30)
        public string Ip { get; set; } // IP (length: 20)
        public DateTime InTime { get; set; } // InTime
        public string Country { get; set; } // Country (length: 20)
        public string City { get; set; } // City (length: 20)
        public bool IsMobileDevice { get; set; } // IsMobileDevice
        public string MobileDeviceManufacturer { get; set; } // MobileDeviceManufacturer (length: 20)
        public byte WebsiteId { get; set; } // WebsiteId
        public byte Day { get; set; } // Day
        public byte Month { get; set; } // Month
        public byte Quater { get; set; } // Quater
        public short Year { get; set; } // Year

        public Tracker()
        {
            InTime = DateTime.Now;
            IsMobileDevice = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
