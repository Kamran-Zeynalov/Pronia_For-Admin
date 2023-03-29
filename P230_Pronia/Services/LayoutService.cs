using Newtonsoft.Json;
using P230_Pronia.DAL;
using P230_Pronia.Entities;
using P230_Pronia.ViewModel.Cookies;

namespace P230_Pronia.Services
{
    public class LayoutService
    {
        readonly ProniaDbContext _context;
        readonly IHttpContextAccessor _accessor;

        public LayoutService(ProniaDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public CookieBasketVM GetBasket()
        {
            var cookie = _accessor.HttpContext.Request.Cookies["basket"];
            CookieBasketVM basket = new();
            if (cookie is not null)
            {
                basket = JsonConvert.DeserializeObject<CookieBasketVM>(cookie);
                foreach (CookieBasketItemVM item in basket.CookieBasketItemVMs)
                {
                    Plant? plant = _context.Plants
                        .FirstOrDefault(p => p.Id == item.Id);
                    if (plant is null)
                    {
                        basket.CookieBasketItemVMs.Remove(item);
                        basket.TotalPrice -= item.Count * item.Price;
                    }
                }
            }
            return basket;
        }
        public List<Plant> GetPlants()
        {
            List<Plant> plant = _context.Plants.ToList();
            return plant;
        }
    }
}
