package com.example.do_an_di_dong.ui.pos;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.CartItem;
import com.example.do_an_di_dong.model.CheckoutRequest;
import com.example.do_an_di_dong.model.CheckoutResult;
import com.example.do_an_di_dong.model.CreateQrRequest;
import com.example.do_an_di_dong.model.CreateQrResponse;
import com.example.do_an_di_dong.model.Customer;
import com.example.do_an_di_dong.model.PayOSStatusResponse;
import com.example.do_an_di_dong.model.Product;
import com.example.do_an_di_dong.model.Promotion;

import java.util.ArrayList;
import java.util.List;

public class PosViewModel extends AndroidViewModel {

    private final DataRepository repository;
    private final MutableLiveData<List<CartItem>> cartLiveData = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<Double> rawTotalAmountLiveData = new MutableLiveData<>(0.0);
    private final MutableLiveData<Double> finalPayableAmountLiveData = new MutableLiveData<>(0.0);
    private final MutableLiveData<Double> totalDiscountLiveData = new MutableLiveData<>(0.0);
    private final MutableLiveData<Integer> totalQuantityLiveData = new MutableLiveData<>(0);
    private final MutableLiveData<List<Product>> suggestionsLiveData = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<CheckoutResult> checkoutSuccessLiveData = new MutableLiveData<>();
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);

    private final MutableLiveData<Customer> selectedCustomerLiveData = new MutableLiveData<>(null);
    private final MutableLiveData<Promotion> selectedPromotionLiveData = new MutableLiveData<>(null);
    private final MutableLiveData<Boolean> usePointsLiveData = new MutableLiveData<>(false);

    private final List<CartItem> cartItems = new ArrayList<>();

    public PosViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<CartItem>> getCart() { return cartLiveData; }
    public LiveData<Double> getRawTotalAmount() { return rawTotalAmountLiveData; }
    public LiveData<Double> getFinalPayableAmount() { return finalPayableAmountLiveData; }
    public LiveData<Double> getTotalDiscount() { return totalDiscountLiveData; }
    public LiveData<Integer> getTotalQuantity() { return totalQuantityLiveData; }
    public LiveData<List<Product>> getSuggestions() { return suggestionsLiveData; }
    public LiveData<CheckoutResult> getCheckoutSuccess() { return checkoutSuccessLiveData; }
    public LiveData<String> getError() { return errorLiveData; }
    public LiveData<Boolean> getLoading() { return loadingLiveData; }

    public LiveData<Customer> getSelectedCustomer() { return selectedCustomerLiveData; }
    public LiveData<Promotion> getSelectedPromotion() { return selectedPromotionLiveData; }
    public LiveData<Boolean> getUsePoints() { return usePointsLiveData; }

    public void selectCustomer(Customer customer) {
        selectedCustomerLiveData.setValue(customer);
        recalculate();
    }

    public void clearCustomer() {
        selectedCustomerLiveData.setValue(null);
        usePointsLiveData.setValue(false);
        recalculate();
    }

    public void selectPromotion(Promotion promo) {
        selectedPromotionLiveData.setValue(promo);
        recalculate();
    }

    public void clearPromotion() {
        selectedPromotionLiveData.setValue(null);
        recalculate();
    }

    public void setUsePoints(boolean use) {
        usePointsLiveData.setValue(use);
        recalculate();
    }

    public void searchProducts(String keyword) {
        if (keyword == null || keyword.trim().isEmpty()) {
            suggestionsLiveData.setValue(new ArrayList<>());
            return;
        }

        repository.searchProductsSimple(keyword.trim(), new DataRepository.ApiCallback<List<Product>>() {
            @Override
            public void onSuccess(List<Product> result) {
                suggestionsLiveData.postValue(result != null ? result : new ArrayList<>());
            }

            @Override
            public void onError(String message) {
                suggestionsLiveData.postValue(new ArrayList<>());
            }
        });
    }

    public void addProductToCart(Product product) {
        if (product == null) return;

        for (CartItem item : cartItems) {
            if (item.getProductId() == product.getProductId()) {
                item.setQuantity(item.getQuantity() + 1);
                recalculate();
                return;
            }
        }

        cartItems.add(new CartItem(product.getProductId(), product.getName(), product.getSku(), product.getPrice(), 1));
        recalculate();
    }

    public void addByBarcode(String barcode) {
        if (barcode == null || barcode.trim().isEmpty()) return;

        loadingLiveData.setValue(true);
        repository.searchProductsSimple(barcode.trim(), new DataRepository.ApiCallback<List<Product>>() {
            @Override
            public void onSuccess(List<Product> result) {
                loadingLiveData.postValue(false);
                if (result != null && !result.isEmpty()) {
                    addProductToCart(result.get(0));
                } else {
                    errorLiveData.postValue("Không tìm thấy sản phẩm với mã: " + barcode);
                }
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
            }
        });
    }

    public void updateItemQuantity(CartItem item, int newQty) {
        for (CartItem ci : cartItems) {
            if (ci.getProductId() == item.getProductId()) {
                ci.setQuantity(newQty);
                break;
            }
        }
        recalculate();
    }

    public void removeItem(CartItem item) {
        cartItems.removeIf(ci -> ci.getProductId() == item.getProductId());
        recalculate();
    }

    public void clearCart() {
        cartItems.clear();
        clearCustomer();
        clearPromotion();
        recalculate();
    }

    public void recalculate() {
        double rawTotal = 0;
        int count = 0;
        for (CartItem ci : cartItems) {
            rawTotal += ci.getSubtotal();
            count += ci.getQuantity();
        }

        double discount = 0;

        // Promotion discount
        Promotion promo = selectedPromotionLiveData.getValue();
        if (promo != null && rawTotal >= promo.getMinOrderValue()) {
            discount += promo.getDiscountAmount();
        }

        // Customer VIP / Loyalty points discount (1 point = 1,000 VND)
        Customer customer = selectedCustomerLiveData.getValue();
        if (customer != null && Boolean.TRUE.equals(usePointsLiveData.getValue())) {
            double pointDiscount = customer.getLoyaltyPoints() * 1000.0;
            discount += Math.min(pointDiscount, rawTotal - discount);
        }

        double payable = Math.max(0, rawTotal - discount);

        cartLiveData.setValue(new ArrayList<>(cartItems));
        rawTotalAmountLiveData.setValue(rawTotal);
        totalDiscountLiveData.setValue(discount);
        finalPayableAmountLiveData.setValue(payable);
        totalQuantityLiveData.setValue(count);
    }

    public void createPayOSLink(CreateQrRequest request, DataRepository.ApiCallback<CreateQrResponse> callback) {
        repository.createPayOSLink(request, callback);
    }

    public void checkPayOSStatus(long orderCode, DataRepository.ApiCallback<PayOSStatusResponse> callback) {
        repository.checkPayOSStatus(orderCode, callback);
    }

    public void checkout(String paymentMethod, DataRepository.ApiCallback<CheckoutResult> callback) {
        if (cartItems.isEmpty()) {
            if (callback != null) callback.onError("Giỏ hàng đang trống!");
            return;
        }

        double total = finalPayableAmountLiveData.getValue() != null ? finalPayableAmountLiveData.getValue() : 0.0;
        Customer customer = selectedCustomerLiveData.getValue();
        Integer customerId = (customer != null && customer.getCustomerId() > 0) ? customer.getCustomerId() : 1;

        // 10,000 VND = 1 loyalty point earned
        int pointsEarned = (int) Math.floor(total / 10000.0);

        int currentUserId = com.example.do_an_di_dong.util.UserSessionManager.getCurrentUserId(getApplication());
        int currentBranchId = com.example.do_an_di_dong.util.UserSessionManager.getCurrentBranchId(getApplication());

        CheckoutRequest request = new CheckoutRequest(customerId, total, paymentMethod, new ArrayList<>(cartItems));
        request.setUserId(currentUserId);
        request.setBranchId(currentBranchId);
        request.setPointsEarned(pointsEarned);

        if (Boolean.TRUE.equals(usePointsLiveData.getValue()) && customer != null) {
            request.setPointsUsed(customer.getLoyaltyPoints());
        }

        loadingLiveData.setValue(true);
        repository.checkoutOrder(request, new DataRepository.ApiCallback<CheckoutResult>() {
            @Override
            public void onSuccess(CheckoutResult result) {
                loadingLiveData.postValue(false);
                if (result != null && result.isSuccess()) {
                    checkoutSuccessLiveData.postValue(result);
                    if (callback != null) callback.onSuccess(result);
                } else {
                    String msg = result != null && result.getMessage() != null ? result.getMessage() : "Thanh toán thất bại từ hệ thống";
                    errorLiveData.postValue(msg);
                    if (callback != null) callback.onError(msg);
                }
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
                if (callback != null) callback.onError(message);
            }
        });
    }
}
