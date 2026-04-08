$(document).ready(function () {

    // BƯỚC 1: Đăng ký hàm xử lý TRƯỚC
    SignalRManager.onPaymentSuccess(function (payload) {
        console.log("🎯 Đã nhảy vào callback tại trang POS!");
        if (typeof POS !== 'undefined' && POS.Payment) {
            POS.Payment.simulateSuccess();
        }
    });

    // BƯỚC 2: Khởi tạo Hub SAU
    SignalRManager.init();
});