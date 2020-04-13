using Library.DbContext.Entities;
using Library.ViewModels.Items;
using System.Collections.Generic;

namespace Library.ViewModels.Ship
{
    public class RequestShipDetailModel
    {
        public RequestShip RequestShipDetail { get; set; }
        public List<OrderPackageItem> ListPackage { get; set; }
    }
}