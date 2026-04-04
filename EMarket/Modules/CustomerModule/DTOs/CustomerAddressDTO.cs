namespace EMarket.Modules.CustomerModule.DTOs
{
    public class CustomerAddressDTO
    {
        public int AddressId { get; set; }
        public int CustomerId { get; set; }
        public string FullAddress { get; set; }
        public string AddressUrl { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CustomerAddressCreateDTO
    {
        public int CustomerId { get; set; }
        public string FullAddress { get; set; }
        public string AddressUrl { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CustomerAddressUpdateDTO : CustomerAddressCreateDTO
    {
        public int AddressId { get; set; }
    }
}