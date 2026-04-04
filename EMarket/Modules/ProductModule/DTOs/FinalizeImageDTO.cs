using System.Collections.Generic;

namespace EMarket.Modules.ProductModule.DTOs
{
    public class FinalizeImageDTO
    {
        public int ProductId { get; set; }
        public List<string> Files { get; set; }
    }
}