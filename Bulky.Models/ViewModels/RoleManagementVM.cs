using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class RoleManagementVM
    {
        public ApplicationUser ApplicationUser { get; set; }

        public string? CurrentRole { get; set; }


        [ValidateNever]
        public IEnumerable<SelectListItem> Roles { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Companies { get; set; }

        //[Required(ErrorMessage ="Please Select an Option.")]
        public string? CurrentCompany { get; set; }
    }
}
