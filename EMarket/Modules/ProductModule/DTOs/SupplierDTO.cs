namespace EMarket.Modules.ProductModule.DTOs
{
    public class SupplierDTO
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string AddressUrl { get; set; }
        public string ContactPerson { get; set; }
        public bool CanBeDeleted { get; set; }
    }
}