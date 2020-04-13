using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Foolproof;

namespace Library.Models
{
    public class OrderRefundMeta
    {
        public int Id { get; set; } // Id (Primary key)

        [Required(ErrorMessage = "Order is required")]
        public int OrderId { get; set; } // OrderId

        public byte Mode { get; set; } // Mode

        public string Note { get; set; } // Note

        [Required(ErrorMessage = "Amount of money is required to enter")]
        public decimal AmountActual { get; set; } // AmountActual

        public List<OrderRefundDetailMeta> Items { get; set; }
    }

    public class OrderRefundDetailMeta
    {
        public int Id { get; set; } // Id (Primary key)
        public int OrderDetailCountingId { get; set; } // OrderDetailId
        public int QuantityLose { get; set; } // QuantityLose
        public string Note { get; set; } // Note
        public string NotePrivate { get; set; } // Note
    }
}
