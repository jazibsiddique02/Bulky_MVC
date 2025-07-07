using BulyWebRazor_Temp.Data;
using BulyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BulyWebRazor_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {

        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Category category { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult OnGet(int id)
        {


            category = _db.Categories.Find(id);

            return Page();
        }

        public IActionResult OnPost()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _db.Attach(category).State = EntityState.Modified;
            try
            {
                _db.SaveChanges();
                TempData["success"] = "Category updated successfully";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!categoryExists(category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToPage("Index");
        }

        private bool categoryExists(int id)
        {
            return _db.Categories.Any(e => e.Id == id);
        }
    }
}
