namespace EMarket.Modules.ProductModule.DTOs
{
    public class ProductCategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CanBeDeleted { get; set; }
    }
}