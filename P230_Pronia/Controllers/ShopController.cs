using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using P230_Pronia.DAL;
using P230_Pronia.Entities;
using P230_Pronia.ViewModel.Cookies;
using System.Drawing;
using System.Net;
using System.Numerics;

namespace P230_Pronia.Controllers
{
    public class ShopController : Controller
    {
        private readonly ProniaDbContext _context;

        public ShopController(ProniaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Plant> plants = _context.Plants.Include(P => P.PlantImages)
                .Include(P => P.PlantDeliveryInformation)
                .Include(P => P.PlantCategories)
                .ThenInclude(P => P.Category)
                .Include(P => P.PlantTags)
                .ThenInclude(P => P.Tag)
                .ToList();

            ViewBag.RelatedProducts = _context.Plants
                .Include(p => p.PlantImages)
                .Take(12)
                .ToList();

            return View(plants);
        }

        public IActionResult Index1(int id)
        {
            if (id == 0) return NotFound();
            Plant? plant = _context.Plants.Include(P => P.PlantImages)
                .Include(P => P.PlantDeliveryInformation)
                .Include(P => P.PlantCategories)
                .ThenInclude(P => P.Category)
                .Include(P => P.PlantTags)
                .ThenInclude(P => P.Tag).FirstOrDefault(P => P.Id == id);

            List<int> categoryIds = plant.PlantCategories.Select(pc => pc.Category.Id).ToList();
            List<Plant> relatedproducts = _context.Plants
                 .Include(rp => rp.PlantImages)
                 .Include(rp => rp.PlantCategories)
                 .ThenInclude(rp => rp.Category)
                 .Where(rp => rp.PlantCategories.Any(pc => categoryIds.Contains(pc.Category.Id)))
                 .Where(rp => rp.Id != plant.Id)
                 .ToList();
            ViewBag.RelatedProducts = relatedproducts;

            if (plant is null) return NotFound();
            return View(plant);
        }

        public IActionResult AddBasket(int id)
        {
            if (id <= 0) return NotFound();
            Plant? plant = _context.Plants.FirstOrDefault(p => p.Id == id);
            if (plant == null) return NotFound();
            var cookies = HttpContext.Request.Cookies["basket"];
            CookieBasketVM basket = new CookieBasketVM();
            if (cookies is null)
            {
                CookieBasketItemVM item = new()
                {
                    Id = plant.Id,
                    Count = 1,
                    Price = plant.Price,
                };
                basket.CookieBasketItemVMs.Add(item);
                basket.TotalPrice = plant.Price;
            }
            else
            {
                basket = JsonConvert.DeserializeObject<CookieBasketVM>(cookies);
                CookieBasketItemVM existed = basket.CookieBasketItemVMs.Find(cki => cki.Id == id);

                if (existed is null)
                {
                    CookieBasketItemVM item = new()
                    {
                        Id = plant.Id,
                        Count = 1,
                        Price = plant.Price,
                    };
                    basket.CookieBasketItemVMs.Add(item);
                    basket.TotalPrice += item.Price;
                }
                else
                {
                    existed.Count++;
                    basket.TotalPrice += existed.Price;
                }
            }

            var basketStr = JsonConvert.SerializeObject(basket);
            HttpContext.Response.Cookies.Append("basket", basketStr);
            return RedirectToAction("Index", "Shop");
        }

        public IActionResult ShowBasket()
        {
            var cookie = HttpContext.Request.Cookies["basket"];
            return Json(JsonConvert.DeserializeObject<CookieBasketVM>(cookie));
        }

        public IActionResult RemoveBasket(int id)
        {
            var cookies = HttpContext.Request.Cookies["basket"];
            CookieBasketVM basket = new CookieBasketVM();
            basket = JsonConvert.DeserializeObject<CookieBasketVM>(cookies);
            CookieBasketItemVM item = basket.CookieBasketItemVMs.Find(cbi => cbi.Id == id);
            if (item is not null)
            {
                basket.CookieBasketItemVMs.Remove(item);
                basket.TotalPrice -= item.Count * item.Price;
                var basketStr = JsonConvert.SerializeObject(basket);
                HttpContext.Response.Cookies.Append("basket", basketStr);
            }
            return RedirectToAction("Index", "Shop");
        }
    }
}
