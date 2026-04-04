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
}