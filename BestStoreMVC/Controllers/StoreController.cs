using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    public class StoreController : Controller
    {
        private readonly IProductService _productService;

        public StoreController(IProductService productService)
        {
            _productService = productService;
        }
        public IActionResult Index(int pageIndex = 1,
            string? search = null,
            string? column = null,
            string? orderBy = null)
        {
            orderBy = "desc";
            var productsFormViewModel = _productService.GetFilteredProducts(pageIndex, search, column, orderBy);

            return View(productsFormViewModel);
        }

        public IActionResult Details(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Store");
            }

            return View(product);
        }
    }
}