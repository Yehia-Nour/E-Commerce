using BestStoreMVC.Models;
using BestStoreMVC.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BestStoreMVC.Services
{
    public interface IProductService
    {
        ProductsFormViewModel GetFilteredProducts(int pageIndex, string search, string column, string orderBy);
        public List<Product> GetNewestProduct();
        Product GetProductById(int id);
        void CreateProduct(CreateProductFormViewModel model);
        void UpdateProduct(int id, UpdateProductFormViewModel model);
        void DeleteProduct(int id);
    }
}
