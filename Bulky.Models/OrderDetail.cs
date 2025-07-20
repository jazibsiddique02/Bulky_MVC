using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class OrderDetail
    {
        // In 1 order header, there are multiple order details, each coresponding to 1 item bought, its quantity and price.
        // Each item of cart is an order detail
        public int Id { get; set; }

        public int OrderHeaderId { get; set; }

        [ValidateNever]
        [ForeignKey("OrderHeaderId")]
        public OrderHeader OrderHeader { get; set; }

        public int ProductId { get; set; }
        [ValidateNever]
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int Count { get; set; }
        public double Price { get; set; }
    }
}
