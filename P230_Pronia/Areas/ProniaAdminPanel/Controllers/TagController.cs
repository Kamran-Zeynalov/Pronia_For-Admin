using Microsoft.AspNetCore.Mvc;
using P230_Pronia.DAL;
using P230_Pronia.Entities;

namespace P230_Pronia.Areas.ProniaAdminPanel.Controllers
{
    [Area("ProniaAdminPanel")]
    public class TagController : Controller
    {
        readonly ProniaDbContext _context;

        public TagController(ProniaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Tag> Tag = _context.Tags.AsEnumerable();
            return View(Tag);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Tag newTag)
        {
            var ErrorMessage = _context.Tags.FirstOrDefault(c => c.Name == newTag.Name);
            if (!ModelState.IsValid && ErrorMessage != null)
            {
                ModelState.AddModelError("Name", $"This '{ErrorMessage.Name}' Tag Already Exist");
                return View();
            }
            _context.Tags.Add(newTag);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (id <= 0) return NotFound();
            Tag? Tag = _context.Tags.FirstOrDefault(c => c.Id == id);
            if (Tag == null) return NotFound();
            return View(Tag);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int id, Tag edited)
        {
            if (id != edited.Id) return BadRequest();
            Tag? Tag = _context.Tags.FirstOrDefault(c => c.Id == id);
            if (Tag == null) return NotFound();
            Tag.Name = edited.Name;
            var duplicate = _context.Tags.Any(c => c.Name == edited.Name);
            if (duplicate)
            {
                ModelState.AddModelError("Name", $"This '{edited.Name}' Tag is now available");
                return View();
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Detail(int id)
        {
            if (id <= 0) return NotFound();
            Tag? Tag = _context.Tags.FirstOrDefault(c => c.Id == id);
            if (Tag == null) return NotFound();
            return View(Tag);
        }
        public IActionResult Delete(int id)
        {
            if (id <= 0) return NotFound();
            Tag? Tag = _context.Tags.FirstOrDefault(c => c.Id == id);
            if (Tag == null) return NotFound();
            return View(Tag);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Delete(int id, Tag deleted)
        {
            if (id <= 0) return NotFound();
            Tag? Tag = _context.Tags.FirstOrDefault(c => c.Id == id);
            if (Tag == null) return NotFound();

            if (Tag.Id == deleted.Id)
            {
                _context.Tags.Remove(Tag);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(Tag);
        }
    }
}
