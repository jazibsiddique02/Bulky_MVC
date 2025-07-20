using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
    public static class StaticDetails
    {
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";



        // all status options for order
        public const string StatusPending = "Pending";
        public const string StatusInProcess = "InProcess";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusCompleted = "Completed";
        public const string StatusRefunded = "Refunded";
        public const string StatusApproved = "Approved";



        // all status options for payment
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";
        public const string PaymentStatusDelayedPayment = "ApproveForDelayedPayment";


        // Session Key
        public const string SessionCart = "SessionShoppingCart";

    }
}
