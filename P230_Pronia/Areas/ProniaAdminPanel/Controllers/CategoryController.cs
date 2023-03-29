using Microsoft.AspNetCore.Mvc;
using P230_Pronia.DAL;
using P230_Pronia.Entities;

namespace P230_Pronia.Areas.ProniaAdminPanel.Controllers
{
    [Area("ProniaAdminPanel")]
    public class CategoryController : Controller
    {
        readonly ProniaDbContext _context;

        public CategoryController(ProniaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> category = _context.Categories.AsEnumerable();
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Name", "You cannot duplicate category name");
                return View();
            }
            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id <= 0) return NotFound();
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int id, Category edited)
        {
            if (id != edited.Id) return BadRequest();
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            category.Name = edited.Name;
            var duplicate = _context.Categories.Any(c => c.Name == edited.Name);
            if (duplicate)
            {
                ModelState.AddModelError("Name", "तुस श्रेणी दे नांऽ गी डुप्लिकेट नेईं करी सकदे");
                return View();
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Detail(int id)
        {
            if (id <= 0) return NotFound();
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }
        public IActionResult Delete(int id)
        {
            if (id <= 0) return NotFound();
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Delete(int id, Category deleted)
        {
            if (id <= 0) return NotFound();
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();

            if (category.Id == deleted.Id)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
    }
}
