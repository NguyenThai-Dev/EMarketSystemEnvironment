using System.Collections.Generic;

namespace EMarket.Modules.ProductModule.DTOs
{
    public class ProductImportResult
    {
        public bool Success { get; set; }
        public int TotalRows { get; set; }
        public int ImportedRows { get; set; }
        public byte[] ErrorReport { get; set; }

        public string ErrorToken { get; set; }

    }

    public class ProductImportRow
    {
        public int RowNumber { get; set; }

        public ProductDTO Product { get; set; }

        public string ThumbnailUrl { get; set; }

        public List<string> ImageUrls { get; set; }
    }

    public class ProductExistingCheckResult
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
    }
}