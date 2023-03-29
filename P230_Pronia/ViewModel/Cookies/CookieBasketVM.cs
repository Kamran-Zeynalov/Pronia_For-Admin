namespace P230_Pronia.ViewModel.Cookies
{
    public class CookieBasketVM
    {
        public List<CookieBasketItemVM> CookieBasketItemVMs { get; set; }
        public decimal TotalPrice { get; set; }

        public CookieBasketVM()
        {
            CookieBasketItemVMs = new();
        }
    }
}
