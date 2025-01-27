using BestStoreMVC.Models;
using BestStoreMVC.Repositories;
using BestStoreMVC.Services;
using BestStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BestStoreMVC.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _imagesPath;
        private readonly int _pageSize = 5;

        public ProductService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public ProductsFormViewModel GetFilteredProducts(int pageIndex, string search, string column, string orderBy)
        {
            var query = _unitOfWork.Products.GetAll().AsQueryable();

            query = ApplySearch(query, search);
            query = ApplySorting(query, column, orderBy);
            var (paginatedItems, totalPages) = ApplyPagination(query, pageIndex);

            return new ProductsFormViewModel
            {
                Items = paginatedItems,
                TotalPages = totalPages,
                PageIndex = pageIndex,
                Search = search,
                Column = column,
                OrderBy = orderBy,
            };
        }

        public List<Product> GetNewestProduct()
        {
            var products = _unitOfWork.Products.GetAll().OrderByDescending(p => p.Id).Take(4).ToList();
            return products;
        }

        public Product GetProductById(int id)
        {
            return _unitOfWork.Products.GetById(id);
        }

        public void CreateProduct(CreateProductFormViewModel model)
        {
            var imageName = SaveImage(model.ImageFile);

            Product product = new Product()
            {
                Name = model.Name,
                Brand = model.Brand,
                Category = model.Category,
                Price = model.Price,
                Description = model.Description,
                ImageFileName = imageName,
            };

            _unitOfWork.Products.Create(product);
            _unitOfWork.Complete();
        }

        public void UpdateProduct(int id, UpdateProductFormViewModel model)
        {
            var product = _unitOfWork.Products.GetById(id);
            if (product == null) throw new Exception("Product not found!");

            product.Name = model.Name;
            product.Brand = model.Brand;
            product.Category = model.Category;
            product.Price = model.Price;
            product.Description = model.Description;

            if (model.ImageFile != null)
            {
                DeleteImage(product.ImageFileName);

                product.ImageFileName = SaveImage(model.ImageFile);
            }

            _unitOfWork.Products.Update(product);
            _unitOfWork.Complete();
        }



        public void DeleteProduct(int id)
        {
            var product = _unitOfWork.Products.GetById(id);

            if (product == null)
                throw new Exception("Product not found!");

            DeleteImage(product.ImageFileName);

            _unitOfWork.Products.Delete(id);
            _unitOfWork.Complete();
        }

        private string SaveImage(IFormFile image)
        {
            var imageName = DateTime.Now.ToString("yyyMMddHHmmssfff") + Path.GetExtension(image.FileName);
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "products", imageName);

            using (var stream = System.IO.File.Create(imagePath))
            {
                image.CopyTo(stream);
            }

            return imageName;
        }

        private void DeleteImage(string imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName)) return;

            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "products", imageFileName);

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        private IQueryable<Product> ApplySearch(IQueryable<Product> query, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
            }
            return query;
        }

        private IQueryable<Product> ApplySorting(IQueryable<Product> query, string column, string orderBy)
        {
            string[] validColumns = { "Id", "Name", "Brand", "Category", "Price", "CreatedAt" };
            string[] validOrderBy = { "desc", "asc" };

            if (!validColumns.Contains(column)) column = "Id";
            if (!validOrderBy.Contains(orderBy)) orderBy = "desc";

            return column switch
            {
                "Name" => orderBy == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "Brand" => orderBy == "asc" ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand),
                "Category" => orderBy == "asc" ? query.OrderBy(p => p.Category) : query.OrderByDescending(p => p.Category),
                "Price" => orderBy == "asc" ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                "CreatedAt" => orderBy == "asc" ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                _ => orderBy == "asc" ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id),
            };
        }

        private (List<Product> PaginatedItems, int TotalPages) ApplyPagination(IQueryable<Product> query, int pageIndex)
        {
            if (pageIndex < 1) pageIndex = 1;

            int totalCount = query.Count();
            int totalPages = (int)Math.Ceiling(totalCount / (double)_pageSize);
            var paginatedItems = query.Skip((pageIndex - 1) * _pageSize).Take(_pageSize).ToList();

            return (paginatedItems, totalPages);
        }
    }
}