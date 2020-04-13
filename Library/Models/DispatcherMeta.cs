using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Foolproof;

namespace Library.Models
{
    public class DispatcherMeta
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "Note must not be longer than 500 characters")]
        public string Note { get; set; }

        public int? ToWarehouseId { get; set; }

        [Required(ErrorMessage = "Shipping partners are required")]
        public byte TransportPartnerId { get; set; }

        [Required(ErrorMessage = "Shipping method is required")]
        public byte TransportMethodId { get; set; }

        [StringLength(300, ErrorMessage = "Contact name can not more than 300 characters")]
        public string ContactName { get; set; }

        [StringLength(20, ErrorMessage = "Contact phone number can not be more than 20 characters")]
        public string ContactPhone { get; set; }

        /// <summary>
        /// 0: Theo cân nặng, 1: Theo Thể tích
        /// </summary>
        [Required(ErrorMessage = "Form of payment is required")]
        public byte PriceType { get; set; }

        [Required(ErrorMessage = "Cost of purchase is required")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Latches of weight or volume are required")]
        public decimal Value { get; set; }

        public DateTime? ForcastDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public byte Status { get; set; }

        public int? EntrepotId { get; set; }

        public string EntrepotName { get; set; }

        [Required(ErrorMessage = "Package is required")]
        public List<DispatcherDetailMeta> Wallets { get; set; }
    }

    public class DispatcherDetailMeta
    {
        public int Id { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        public string WalletCode { get; set; }

        [StringLength(500, ErrorMessage = "Description can not be longer than 500 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Giá trị điều chỉnh là cân nặng hoặc là thể tích
        /// </summary>
        [DisplayName("Value adjustment")]
        public decimal Value { get; set; }

        [StringLength(500, ErrorMessage = "Note must not be longer than 500 characters")]
        public string Note { get; set; }
        public int? DispatcherId { get; set; }
    }

    public class ValueUpdateMeta
    {
        [Required(ErrorMessage = "Item details of the package are required")]
        public int DispatcherDetailId { get; set; }

        [Required(ErrorMessage = "Transport key value can not be blank")]
        public decimal Value { get; set; }
    }

    public class DescriptionUpdateMeta
    {
        [Required(ErrorMessage = "Item details of the package are required")]
        public int DispatcherDetailId { get; set; }

        public string Description { get; set; }
    }
}
