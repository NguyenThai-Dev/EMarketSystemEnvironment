namespace EMarket.Modules.SalesModule.DTOs
{
    public class CreateQrResponseDTO
    {
        public bool Success { get; set; }
        public string CheckoutUrl { get; set; }
        public string QrCode { get; set; }
        public long OrderCode { get; set; } // Thêm để map với SignalR
        public string Message { get; set; }
    }
}