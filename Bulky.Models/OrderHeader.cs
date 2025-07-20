using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{

    // This class contains all details relating to the purchase, the user, and the overall total. In 1 order header, there are multiple order details, each coresponding to 1 item bought,its quantity and price
    public class OrderHeader
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        [ValidateNever]
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime ShippingDate { get; set; }

        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }   

        public string? PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateOnly PaymentDueDate { get; set; }


        // when a user tries to make payment, this is generated.
        public string? SessionId { get; set; }

        // when a payment is successful,this id is genereated.
        public string? PaymentIntentId { get; set; }  // unique id from Stripe

        [Required]
        public string Name { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

    }
}
