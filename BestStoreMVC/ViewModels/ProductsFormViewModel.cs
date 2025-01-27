using BestStoreMVC.Models;

namespace BestStoreMVC.ViewModels
{
    public class ProductsFormViewModel
    {
        public IEnumerable<Product> Items { get; set; }
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public string? Search { get; set; }
        public string? Column {  get; set; }
        public string? OrderBy { get; set; }
    }
}
