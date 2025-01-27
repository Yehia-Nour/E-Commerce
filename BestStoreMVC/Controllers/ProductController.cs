using BestStoreMVC.Repositories;
using BestStoreMVC.Services;
using BestStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index(int pageIndex = 1,
            string? search = null,
            string? column = null,
            string? orderBy = null)
        {
            var productsFormViewModel = _productService.GetFilteredProducts(pageIndex, search, column, orderBy);

            return View(productsFormViewModel);
        }


        public IActionResult Create(CreateProductFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                _productService.CreateProduct(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Update(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null) return NotFound();

            var viewModel = new UpdateProductFormViewModel
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Update(int id, UpdateProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = id;
                ViewData["ImageFileName"] = model.ImageFile?.FileName ?? "";
                return View(model);
            }

            _productService.UpdateProduct(id, model);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            try
            {
                _productService.DeleteProduct(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return RedirectToAction("Index");
            }
        }

    }
}
