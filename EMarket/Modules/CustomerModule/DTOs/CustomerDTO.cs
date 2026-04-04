using System;

namespace EMarket.Modules.CustomerModule.DTOs
{
    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CustomerType { get; set; }
        public int PointBalance { get; set; }
        public int PointEarnedTotal { get; set; }
        public string UserImg { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerCreateDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CustomerType { get; set; }
        public string UserImg { get; set; }
    }

    public class CustomerUpdateDTO : CustomerCreateDTO
    {
        public int CustomerId { get; set; }
    }
}