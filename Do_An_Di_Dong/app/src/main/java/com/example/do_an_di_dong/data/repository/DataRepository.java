package com.example.do_an_di_dong.data.repository;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.util.Log;

import com.example.do_an_di_dong.data.api.ApiClient;
import com.example.do_an_di_dong.data.api.ApiService;
import com.example.do_an_di_dong.data.database.AppDatabase;
import com.example.do_an_di_dong.data.database.ProductEntity;
import com.example.do_an_di_dong.model.*;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class DataRepository {
    private static final String TAG = "DataRepository";
    private final Context context;
    private final ApiService emarketService;
    private final ApiService aiService;
    private final AppDatabase database;
    private final ExecutorService executorService;

    public DataRepository(Context context) {
        this.context = context.getApplicationContext();
        this.emarketService = ApiClient.getEMarketService(this.context);
        this.aiService = ApiClient.getAiService(this.context);
        this.database = AppDatabase.getInstance(this.context);
        this.executorService = Executors.newFixedThreadPool(4);
    }

    public interface ApiCallback<T> {
        void onSuccess(T result);
        void onError(String message);
    }

    private boolean isNetworkAvailable() {
        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        if (cm == null) return false;
        NetworkInfo activeNetwork = cm.getActiveNetworkInfo();
        return activeNetwork != null && activeNetwork.isConnected();
    }

    // 1. LOGIN FLOW: Modern REST Web API with Legacy Fallback
    public void login(String username, String password, ApiCallback<LoginResponse> callback) {
        LoginRequest req = new LoginRequest(username, password);
        emarketService.loginRest(req).enqueue(new Callback<LoginResponse>() {
            @Override
            public void onResponse(Call<LoginResponse> call, Response<LoginResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    LoginResponse loginRes = response.body();
                    if (loginRes.isSuccess() && loginRes.getToken() != null) {
                        ApiClient.saveToken(context, loginRes.getToken());
                        callback.onSuccess(loginRes);
                        return;
                    }
                }
                // Fallback to legacy CSRF scraping if REST didn't succeed
                loginLegacy(username, password, callback);
            }

            @Override
            public void onFailure(Call<LoginResponse> call, Throwable t) {
                // Fallback to legacy CSRF scraping on failure
                loginLegacy(username, password, callback);
            }
        });
    }

    private void loginLegacy(String username, String password, ApiCallback<LoginResponse> callback) {
        emarketService.getLoginPage().enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    try {
                        String html = response.body().string();
                        String csrfToken = extractCsrfToken(html);
                        if (csrfToken == null) {
                            callback.onError("Không thể lấy mã CSRF bảo mật từ server.");
                            return;
                        }
                        executeLoginPost(username, password, csrfToken, callback);
                    } catch (IOException e) {
                        callback.onError("Lỗi đọc trang đăng nhập: " + e.getMessage());
                    }
                } else {
                    callback.onError("Lỗi kết nối trang đăng nhập: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    private void executeLoginPost(String username, String password, String csrfToken, ApiCallback<LoginResponse> callback) {
        emarketService.login(username, password, csrfToken).enqueue(new Callback<LoginResponse>() {
            @Override
            public void onResponse(Call<LoginResponse> call, Response<LoginResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    LoginResponse loginRes = response.body();
                    if (loginRes.isSuccess() && loginRes.getToken() != null) {
                        ApiClient.saveToken(context, loginRes.getToken());
                        callback.onSuccess(loginRes);
                    } else {
                        callback.onError(loginRes.getMessage());
                    }
                } else {
                    callback.onError("Đăng nhập thất bại. Mã lỗi: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<LoginResponse> call, Throwable t) {
                callback.onError("Lỗi mạng khi gửi đăng nhập: " + t.getMessage());
            }
        });
    }

    public void externalLogin(String provider, String email, String idToken, ApiCallback<LoginResponse> callback) {
        ExternalLoginRequest request = new ExternalLoginRequest(provider, email, idToken, null);
        emarketService.externalLogin(request).enqueue(new Callback<LoginResponse>() {
            @Override
            public void onResponse(Call<LoginResponse> call, Response<LoginResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    LoginResponse loginRes = response.body();
                    if (loginRes.isSuccess() && loginRes.getToken() != null) {
                        ApiClient.saveToken(context, loginRes.getToken());
                        callback.onSuccess(loginRes);
                    } else {
                        String msg = loginRes.getMessage();
                        callback.onError(msg != null && !msg.isEmpty() ? msg : "Đăng nhập thất bại");
                    }
                } else {
                    callback.onError("Đăng nhập thất bại. Mã lỗi: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<LoginResponse> call, Throwable t) {
                callback.onError("Lỗi mạng khi đăng nhập Google: " + t.getMessage());
            }
        });
    }

    private String extractCsrfToken(String html) {
        Pattern pattern = Pattern.compile("name=\"__RequestVerificationToken\"\\s+type=\"hidden\"\\s+value=\"([^\"]+)\"");
        Matcher matcher = pattern.matcher(html);
        if (matcher.find()) {
            return matcher.group(1);
        }
        return null;
    }

    // 2. DASHBOARD DATA
    public void getDashboardSummary(String fromDate, String toDate, String groupBy, ApiCallback<DashboardOverview> callback) {
        emarketService.getDashboardSummary(fromDate, toDate, groupBy).enqueue(new Callback<DashboardOverview>() {
            @Override
            public void onResponse(Call<DashboardOverview> call, Response<DashboardOverview> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Không thể tải báo cáo. Mã lỗi: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<DashboardOverview> call, Throwable t) {
                callback.onError("Lỗi kết nối báo cáo: " + t.getMessage());
            }
        });
    }

    public void searchProductsSimple(String keyword, ApiCallback<List<Product>> callback) {
        searchProducts(keyword, null, callback);
    }

    // 3. PRODUCT SEARCH (Online with offline fallback & caching)
    public void searchProducts(String keyword, Integer categoryId, ApiCallback<List<Product>> callback) {
        if (isNetworkAvailable()) {
            emarketService.searchProducts(keyword, categoryId, null, null, null).enqueue(new Callback<List<Product>>() {
                @Override
                public void onResponse(Call<List<Product>> call, Response<List<Product>> response) {
                    if (response.isSuccessful() && response.body() != null) {
                        List<Product> products = response.body();
                        callback.onSuccess(products);
                        
                        // Cache to Room in Background
                        executorService.execute(() -> {
                            List<ProductEntity> entities = new ArrayList<>();
                            for (Product p : products) {
                                entities.add(new ProductEntity(
                                        p.getProductId(),
                                        p.getName(),
                                        p.getSku(),
                                        p.getPrice(),
                                        p.getStockQuantity(),
                                        p.getImageUrl()
                                ));
                            }
                            database.productDao().insertProducts(entities);
                            Log.d(TAG, "Cached " + entities.size() + " products offline.");
                        });
                    } else {
                        callback.onError("Lỗi tải sản phẩm. Mã: " + response.code());
                    }
                }

                @Override
                public void onFailure(Call<List<Product>> call, Throwable t) {
                    // Fallback to offline search
                    searchProductsOffline(keyword, callback);
                }
            });
        } else {
            searchProductsOffline(keyword, callback);
        }
    }

    public void getProductCategories(ApiCallback<List<ProductCategoryDTO>> callback) {
        emarketService.getAllCategories().enqueue(new Callback<List<ProductCategoryDTO>>() {
            @Override
            public void onResponse(Call<List<ProductCategoryDTO>> call, Response<List<ProductCategoryDTO>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải danh mục. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<ProductCategoryDTO>> call, Throwable t) {
                callback.onError("Lỗi kết nối mạng tải danh mục: " + t.getMessage());
            }
        });
    }

    private void searchProductsOffline(String keyword, ApiCallback<List<Product>> callback) {
        executorService.execute(() -> {
            List<ProductEntity> entities = database.productDao().searchProductsOffline(keyword != null ? keyword : "");
            List<Product> products = new ArrayList<>();
            for (ProductEntity e : entities) {
                Product p = new Product();
                p.setProductId(e.getProductId());
                p.setName(e.getName());
                p.setSku(e.getSku());
                p.setPrice(e.getPrice());
                p.setStockQuantity(e.getStockQuantity());
                p.setImageUrl(e.getImageUrl());
                products.add(p);
            }
            // Return to main thread via callback logic in fragment/viewmodel
            callback.onSuccess(products);
        });
    }

    // 4. OFFLINE BARCODE SCAN RESOLVER
    public void getProductBySkuOffline(String sku, ApiCallback<Product> callback) {
        executorService.execute(() -> {
            ProductEntity entity = database.productDao().getProductBySku(sku);
            if (entity != null) {
                Product p = new Product();
                p.setProductId(entity.getProductId());
                p.setName(entity.getName());
                p.setSku(entity.getSku());
                p.setPrice(entity.getPrice());
                p.setStockQuantity(entity.getStockQuantity());
                p.setImageUrl(entity.getImageUrl());
                callback.onSuccess(p);
            } else {
                callback.onError("Không tìm thấy sản phẩm có mã này trong bộ nhớ tạm.");
            }
        });
    }

    // 5. QUOTATIONS LIST
    public void getQuotations(ApiCallback<List<Quotation>> callback) {
        getQuotations(null, callback);
    }

    public void getQuotations(String status, ApiCallback<List<Quotation>> callback) {
        emarketService.getQuotations(
                null,   // keyword
                null,   // branchId
                status, // status
                null,   // fromDate
                null    // toDate
        ).enqueue(new Callback<List<Quotation>>() {

            @Override
            public void onResponse(
                    Call<List<Quotation>> call,
                    Response<List<Quotation>> response
            ) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải báo giá. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<Quotation>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getQuotationDetail(int id, ApiCallback<Quotation> callback) {
        emarketService.getQuotationDetail(id).enqueue(new Callback<Quotation>() {
            @Override
            public void onResponse(Call<Quotation> call, Response<Quotation> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Không tìm thấy chi tiết báo giá. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<Quotation> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải chi tiết báo giá: " + t.getMessage());
            }
        });
    }

    public void getProductById(int productId, ApiCallback<Product> callback) {
        emarketService.getProductById(productId).enqueue(new Callback<Product>() {
            @Override
            public void onResponse(Call<Product> call, Response<Product> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Không tìm thấy sản phẩm. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<Product> call, Throwable t) {
                callback.onError("Lỗi kết nối khi tải sản phẩm: " + t.getMessage());
            }
        });
    }

    public void createProduct(ProductDTO dto, ApiCallback<Boolean> callback) {
        emarketService.createProductApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Lỗi tạo sản phẩm. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }

    public void updateProduct(ProductDTO dto, ApiCallback<Boolean> callback) {
        emarketService.updateProductApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Lỗi cập nhật sản phẩm. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }

    public void deleteProduct(int productId, ApiCallback<Boolean> callback) {
        emarketService.deleteProductApi(productId).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Lỗi xóa sản phẩm. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }

    public void getAllCategories(ApiCallback<List<ProductCategoryDTO>> callback) {
        emarketService.getAllCategories().enqueue(new Callback<List<ProductCategoryDTO>>() {
            @Override
            public void onResponse(Call<List<ProductCategoryDTO>> call, Response<List<ProductCategoryDTO>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải danh mục. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<ProductCategoryDTO>> call, Throwable t) {
                callback.onError("Lỗi kết nối danh mục: " + t.getMessage());
            }
        });
    }

    public void searchPurchasesPaged(int start, int length, String keyword, Integer supplierId, Integer branchId, Integer warehouseId, String status, String paymentStatus, String fromDate, String toDate, ApiCallback<PurchasePagedResponse> callback) {
        emarketService.searchPurchasesPaged(start, length, keyword, supplierId, branchId, warehouseId, status, paymentStatus, fromDate, toDate).enqueue(new Callback<PurchasePagedResponse>() {
            @Override
            public void onResponse(Call<PurchasePagedResponse> call, Response<PurchasePagedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải đơn nhập hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<PurchasePagedResponse> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải đơn nhập hàng: " + t.getMessage());
            }
        });
    }

    public void searchPurchases(String keyword, ApiCallback<List<PurchaseOrder>> callback) {
        emarketService.searchPurchases(keyword, null, null, null, null, null, null, null).enqueue(new Callback<List<PurchaseOrder>>() {
            @Override
            public void onResponse(Call<List<PurchaseOrder>> call, Response<List<PurchaseOrder>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải đơn nhập hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<PurchaseOrder>> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải đơn nhập hàng: " + t.getMessage());
            }
        });
    }

    public void getStockMovements(Integer warehouseId, String type, String keyword, ApiCallback<ResponseBody> callback) {
        emarketService.getStockMovements(0, 100, warehouseId, type, null, null, keyword).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải biến động kho. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải biến động kho: " + t.getMessage());
            }
        });
    }

    public void getAllSuppliers(ApiCallback<List<SupplierDTO>> callback) {
        emarketService.getAllSuppliers().enqueue(new Callback<List<SupplierDTO>>() {
            @Override
            public void onResponse(Call<List<SupplierDTO>> call, Response<List<SupplierDTO>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải nhà cung cấp. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<SupplierDTO>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    // 5.1 SALES & ORDERS
    public void getOrdersPaged(int start, int length, Integer userId, Integer branchId, String status, String fromDate, String toDate, String keyword, ApiCallback<OrderPagedResponse> callback) {
        emarketService.getOrdersPaged(start, length, userId, branchId, status, fromDate, toDate, keyword).enqueue(new Callback<OrderPagedResponse>() {
            @Override
            public void onResponse(Call<OrderPagedResponse> call, Response<OrderPagedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải danh sách đơn hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<OrderPagedResponse> call, Throwable t) {
                callback.onError("Lỗi kết nối đơn hàng: " + t.getMessage());
            }
        });
    }

    public void getAllOrders(ApiCallback<List<Order>> callback) {
        emarketService.getAllOrders().enqueue(new Callback<List<Order>>() {
            @Override
            public void onResponse(Call<List<Order>> call, Response<List<Order>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải danh sách đơn hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<Order>> call, Throwable t) {
                callback.onError("Lỗi kết nối đơn hàng: " + t.getMessage());
            }
        });
    }

    public void getOrderDetails(int orderId, ApiCallback<List<OrderDetail>> callback) {
        emarketService.getOrderDetails(orderId).enqueue(new Callback<List<OrderDetail>>() {
            @Override
            public void onResponse(Call<List<OrderDetail>> call, Response<List<OrderDetail>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải chi tiết đơn hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<OrderDetail>> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải chi tiết đơn hàng: " + t.getMessage());
            }
        });
    }

    public void updateOrderStatus(int orderId, String status, ApiCallback<Boolean> callback) {
        java.util.Map<String, String> body = new java.util.HashMap<>();
        body.put("status", status);
        body.put("connectionId", "");
        emarketService.updateOrderStatusApi(orderId, body).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Lỗi cập nhật trạng thái đơn hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }

    public void deleteOrder(int orderId, ApiCallback<Boolean> callback) {
        emarketService.deleteOrderApi(orderId).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Lỗi xóa đơn hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }

    public void checkoutOrder(CheckoutRequest request, ApiCallback<CheckoutResult> callback) {
        emarketService.checkoutOrder(request).enqueue(new Callback<CheckoutResult>() {
            @Override
            public void onResponse(Call<CheckoutResult> call, Response<CheckoutResult> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Thanh toán đơn hàng thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<CheckoutResult> call, Throwable t) {
                callback.onError("Lỗi kết nối khi thanh toán: " + t.getMessage());
            }
        });
    }

    public void getPaymentsByOrder(int orderId, ApiCallback<List<Payment>> callback) {
        emarketService.getPaymentsByOrder(orderId).enqueue(new Callback<List<Payment>>() {
            @Override
            public void onResponse(Call<List<Payment>> call, Response<List<Payment>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải lịch sử thanh toán. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<Payment>> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải thanh toán: " + t.getMessage());
            }
        });
    }

    public void getAllPromotions(ApiCallback<List<Promotion>> callback) {
        emarketService.getAllPromotions().enqueue(new Callback<List<Promotion>>() {
            @Override
            public void onResponse(Call<List<Promotion>> call, Response<List<Promotion>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải danh sách khuyến mãi. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<Promotion>> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải khuyến mãi: " + t.getMessage());
            }
        });
    }

    public void getActivePromotions(ApiCallback<List<Promotion>> callback) {
        emarketService.getActivePromotions().enqueue(new Callback<List<Promotion>>() {
            @Override
            public void onResponse(Call<List<Promotion>> call, Response<List<Promotion>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải khuyến mãi đang chạy. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<Promotion>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    // 6. APPROVE/REJECT QUOTATION (MVC style status update with csrf token validation)
    public void changeQuotationStatus(int quotationId, String status, ApiCallback<Boolean> callback) {
        // Fetch login page again (or any page) to get a fresh Anti-Forgery token
        emarketService.getLoginPage().enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    try {
                        String html = response.body().string();
                        String csrfToken = extractCsrfToken(html);
                        if (csrfToken == null) {
                            callback.onError("Không thể lấy mã bảo mật để duyệt.");
                            return;
                        }
                        
                        emarketService.changeQuotationStatus(quotationId, status, csrfToken).enqueue(new Callback<ResponseBody>() {
                            @Override
                            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                                if (response.isSuccessful()) {
                                    callback.onSuccess(true);
                                } else {
                                    callback.onError("Duyệt thất bại. Mã: " + response.code());
                                }
                            }

                            @Override
                            public void onFailure(Call<ResponseBody> call, Throwable t) {
                                callback.onError("Lỗi mạng khi thực hiện duyệt: " + t.getMessage());
                            }
                        });
                    } catch (IOException e) {
                        callback.onError("Lỗi lấy mã bảo mật: " + e.getMessage());
                    }
                } else {
                    callback.onError("Lỗi kết nối server lấy mã bảo mật: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    // 7. AI ASSISTANT CHAT
    public void askAi(String prompt, String username, ApiCallback<ChatResponse> callback) {
        ChatRequest request = new ChatRequest(prompt, username);
        aiService.askAi(request).enqueue(new Callback<ChatResponse>() {
            @Override
            public void onResponse(Call<ChatResponse> call, Response<ChatResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("AI không phản hồi. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ChatResponse> call, Throwable t) {
                callback.onError("Lỗi kết nối AI: " + t.getMessage());
            }
        });
    }

    // 7.1 AI ANALYSIS & PREDICTIONS
    public void getReplenishmentAdvice(int branchId, ApiCallback<List<ReplenishmentAdvice>> callback) {
        emarketService.getReplenishmentAdvice(branchId).enqueue(new Callback<List<ReplenishmentAdvice>>() {
            @Override
            public void onResponse(Call<List<ReplenishmentAdvice>> call, Response<List<ReplenishmentAdvice>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải lời khuyên nhập hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<ReplenishmentAdvice>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getAnomalies(int branchId, ApiCallback<List<Anomaly>> callback) {
        emarketService.getAnomalies(branchId).enqueue(new Callback<List<Anomaly>>() {
            @Override
            public void onResponse(Call<List<Anomaly>> call, Response<List<Anomaly>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải cảnh báo bất thường. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<Anomaly>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getProductInsights(int branchId, ApiCallback<List<Insight>> callback) {
        emarketService.getProductInsights(branchId).enqueue(new Callback<List<Insight>>() {
            @Override
            public void onResponse(Call<List<Insight>> call, Response<List<Insight>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải thông tin sản phẩm tiềm năng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<Insight>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getInventoryForecast(int branchId, ApiCallback<List<InventoryForecast>> callback) {
        emarketService.getInventoryForecast(branchId).enqueue(new Callback<List<InventoryForecast>>() {
            @Override
            public void onResponse(Call<List<InventoryForecast>> call, Response<List<InventoryForecast>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải dự báo tồn kho. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<InventoryForecast>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getLotFinancialRisk(int branchId, ApiCallback<List<LotFinancialRisk>> callback) {
        emarketService.getLotFinancialRisk(branchId).enqueue(new Callback<List<LotFinancialRisk>>() {
            @Override
            public void onResponse(Call<List<LotFinancialRisk>> call, Response<List<LotFinancialRisk>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải rủi ro tài chính lô hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<LotFinancialRisk>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void runProphet(ApiCallback<Boolean> callback) {
        emarketService.runProphet().enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Chạy thuật toán dự báo thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi kết nối thuật toán Prophet: " + t.getMessage());
            }
        });
    }

    // 7.2 PHASE 4: EXECUTIVE REPORTS, CRM & SYSTEM LOGS
    public void getFinanceDashboard(int daysBack, Integer branchId, ApiCallback<FinanceDashboard> callback) {
        emarketService.getFinanceDashboard(daysBack, branchId).enqueue(new Callback<FinanceDashboard>() {
            @Override
            public void onResponse(Call<FinanceDashboard> call, Response<FinanceDashboard> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải báo cáo tài chính. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<FinanceDashboard> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải báo cáo tài chính: " + t.getMessage());
            }
        });
    }

    public void getBranchPerformance(Integer branchId, ApiCallback<List<BranchPerformance>> callback) {
        emarketService.getBranchPerformance(branchId, null, null).enqueue(new Callback<List<BranchPerformance>>() {
            @Override
            public void onResponse(Call<List<BranchPerformance>> call, Response<List<BranchPerformance>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải hiệu suất chi nhánh. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<BranchPerformance>> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải hiệu suất chi nhánh: " + t.getMessage());
            }
        });
    }

    public void getWarehouseDashboard(int dayBacks, Integer branchId, Integer warehouseId, ApiCallback<WarehouseDashboard> callback) {
        emarketService.getWarehouseDashboard(dayBacks, branchId, warehouseId).enqueue(new Callback<WarehouseDashboard>() {
            @Override
            public void onResponse(Call<WarehouseDashboard> call, Response<WarehouseDashboard> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải báo cáo kho. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<WarehouseDashboard> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải báo cáo kho: " + t.getMessage());
            }
        });
    }

    public void getDebtsPaged(int start, int length, String keyword, Integer supplierId, String status, String fromDate, String toDate, ApiCallback<SupplierDebtPagedResponse> callback) {
        emarketService.getDebtsPaged(start, length, keyword, supplierId, status, fromDate, toDate).enqueue(new Callback<SupplierDebtPagedResponse>() {
            @Override
            public void onResponse(Call<SupplierDebtPagedResponse> call, Response<SupplierDebtPagedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải công nợ nhà cung cấp. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<SupplierDebtPagedResponse> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải công nợ: " + t.getMessage());
            }
        });
    }

    public void getFilteredDebts(ApiCallback<List<SupplierDebt>> callback) {
        emarketService.getFilteredDebts(null, null, null, null, null).enqueue(new Callback<List<SupplierDebt>>() {
            @Override
            public void onResponse(Call<List<SupplierDebt>> call, Response<List<SupplierDebt>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải công nợ nhà cung cấp. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<SupplierDebt>> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải công nợ: " + t.getMessage());
            }
        });
    }

    public void getCustomersPaged(int start, int length, String keyword, ApiCallback<CustomerPagedResponse> callback) {
        emarketService.getCustomersPaged(start, length, keyword).enqueue(new Callback<CustomerPagedResponse>() {
            @Override
            public void onResponse(Call<CustomerPagedResponse> call, Response<CustomerPagedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải danh sách khách hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<CustomerPagedResponse> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải khách hàng: " + t.getMessage());
            }
        });
    }

    public void searchCustomers(String keyword, ApiCallback<List<Customer>> callback) {
        getCustomersPaged(0, 20, keyword, new ApiCallback<CustomerPagedResponse>() {
            @Override
            public void onSuccess(CustomerPagedResponse result) {
                if (result != null && result.getData() != null) {
                    callback.onSuccess(result.getData());
                } else {
                    callback.onSuccess(new ArrayList<>());
                }
            }

            @Override
            public void onError(String message) {
                callback.onError(message);
            }
        });
    }

    public void getAllCustomers(ApiCallback<List<Customer>> callback) {
        emarketService.getAllCustomers().enqueue(new Callback<List<Customer>>() {
            @Override
            public void onResponse(Call<List<Customer>> call, Response<List<Customer>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải danh sách khách hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<Customer>> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải khách hàng: " + t.getMessage());
            }
        });
    }

    public void createCustomer(Customer customer, ApiCallback<Boolean> callback) {
        emarketService.createCustomerApi(customer).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Lỗi tạo khách hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }

    public void updateCustomer(Customer customer, ApiCallback<Boolean> callback) {
        emarketService.updateCustomerApi(customer).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Lỗi cập nhật khách hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }

    public void deleteCustomer(int customerId, ApiCallback<Boolean> callback) {
        emarketService.deleteCustomerApi(customerId).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Lỗi xóa khách hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }

    public void getUsersPaged(int start, int length, String keyword, ApiCallback<UserPagedResponse> callback) {
        emarketService.getUsersPaged(start, length, keyword).enqueue(new Callback<UserPagedResponse>() {
            @Override
            public void onResponse(Call<UserPagedResponse> call, Response<UserPagedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải danh sách người dùng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<UserPagedResponse> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải người dùng: " + t.getMessage());
            }
        });
    }

    public void getAllUsers(ApiCallback<List<User>> callback) {
        emarketService.getAllUsers().enqueue(new Callback<List<User>>() {
            @Override
            public void onResponse(Call<List<User>> call, Response<List<User>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải người dùng hệ thống. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<User>> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải người dùng: " + t.getMessage());
            }
        });
    }

    public void getLogStatistics(ApiCallback<LogStatistics> callback) {
        emarketService.getLogStatistics().enqueue(new Callback<LogStatistics>() {
            @Override
            public void onResponse(Call<LogStatistics> call, Response<LogStatistics> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải thống kê nhật ký. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<LogStatistics> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải thống kê logs: " + t.getMessage());
            }
        });
    }

    // =========================================================================
    // 8. NHÓM API GHI CRUDS (MVC CONTROLLERS)
    // =========================================================================

    private interface CsrfAction {
        void execute(String token);
    }

    private <T> void withCsrfToken(ApiCallback<T> callback, CsrfAction action) {
        emarketService.getLoginPage().enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    try {
                        String html = response.body().string();
                        String csrfToken = extractCsrfToken(html);
                        if (csrfToken == null) {
                            callback.onError("Không thể lấy mã bảo mật để thực hiện yêu cầu.");
                            return;
                        }
                        action.execute(csrfToken);
                    } catch (IOException e) {
                        callback.onError("Lỗi lấy mã bảo mật: " + e.getMessage());
                    }
                } else {
                    callback.onError("Lỗi server khi lấy mã bảo mật: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void activeProduct(int productId, int minStock, int maxStock, ApiCallback<Boolean> callback) {
        withCsrfToken(callback, token -> {
            emarketService.activeProduct(productId, minStock, maxStock, token).enqueue(new Callback<ResponseBody>() {
                @Override
                public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                    if (response.isSuccessful()) {
                        callback.onSuccess(true);
                    } else {
                        callback.onError("Kích hoạt sản phẩm thất bại. Mã: " + response.code());
                    }
                }

                @Override
                public void onFailure(Call<ResponseBody> call, Throwable t) {
                    callback.onError("Lỗi mạng: " + t.getMessage());
                }
            });
        });
    }

    public void createProductCategory(ProductCategoryDTO dto, ApiCallback<Boolean> callback) {
        emarketService.createProductCategoryApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    withCsrfToken(callback, token -> emarketService.createProductCategory(dto, token).enqueue(new Callback<ResponseBody>() {
                        @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                        @Override public void onFailure(Call<ResponseBody> c, Throwable t) { callback.onError("Lỗi mạng: " + t.getMessage()); }
                    }));
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                withCsrfToken(callback, token -> emarketService.createProductCategory(dto, token).enqueue(new Callback<ResponseBody>() {
                    @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                    @Override public void onFailure(Call<ResponseBody> c, Throwable t1) { callback.onError("Lỗi mạng: " + t1.getMessage()); }
                }));
            }
        });
    }

    public void updateProductCategory(ProductCategoryDTO dto, ApiCallback<Boolean> callback) {
        emarketService.updateProductCategoryApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    withCsrfToken(callback, token -> emarketService.updateProductCategory(dto, token).enqueue(new Callback<ResponseBody>() {
                        @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                        @Override public void onFailure(Call<ResponseBody> c, Throwable t) { callback.onError("Lỗi mạng: " + t.getMessage()); }
                    }));
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                withCsrfToken(callback, token -> emarketService.updateProductCategory(dto, token).enqueue(new Callback<ResponseBody>() {
                    @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                    @Override public void onFailure(Call<ResponseBody> c, Throwable t1) { callback.onError("Lỗi mạng: " + t1.getMessage()); }
                }));
            }
        });
    }

    public void deleteProductCategory(int categoryId, ApiCallback<Boolean> callback) {
        emarketService.deleteProductCategoryApi(categoryId).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    withCsrfToken(callback, token -> emarketService.deleteProductCategory(categoryId, token).enqueue(new Callback<ResponseBody>() {
                        @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                        @Override public void onFailure(Call<ResponseBody> c, Throwable t) { callback.onError("Lỗi mạng: " + t.getMessage()); }
                    }));
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                withCsrfToken(callback, token -> emarketService.deleteProductCategory(categoryId, token).enqueue(new Callback<ResponseBody>() {
                    @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                    @Override public void onFailure(Call<ResponseBody> c, Throwable t1) { callback.onError("Lỗi mạng: " + t1.getMessage()); }
                }));
            }
        });
    }

    public void createSupplier(SupplierDTO dto, ApiCallback<Boolean> callback) {
        emarketService.createSupplierApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    withCsrfToken(callback, token -> emarketService.createSupplier(dto, token).enqueue(new Callback<ResponseBody>() {
                        @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                        @Override public void onFailure(Call<ResponseBody> c, Throwable t) { callback.onError("Lỗi mạng: " + t.getMessage()); }
                    }));
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                withCsrfToken(callback, token -> emarketService.createSupplier(dto, token).enqueue(new Callback<ResponseBody>() {
                    @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                    @Override public void onFailure(Call<ResponseBody> c, Throwable t1) { callback.onError("Lỗi mạng: " + t1.getMessage()); }
                }));
            }
        });
    }

    public void updateSupplier(SupplierDTO dto, ApiCallback<Boolean> callback) {
        emarketService.updateSupplierApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    withCsrfToken(callback, token -> emarketService.updateSupplier(dto, token).enqueue(new Callback<ResponseBody>() {
                        @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                        @Override public void onFailure(Call<ResponseBody> c, Throwable t) { callback.onError("Lỗi mạng: " + t.getMessage()); }
                    }));
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                withCsrfToken(callback, token -> emarketService.updateSupplier(dto, token).enqueue(new Callback<ResponseBody>() {
                    @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                    @Override public void onFailure(Call<ResponseBody> c, Throwable t1) { callback.onError("Lỗi mạng: " + t1.getMessage()); }
                }));
            }
        });
    }

    public void deleteSupplier(int supplierId, ApiCallback<Boolean> callback) {
        emarketService.deleteSupplierApi(supplierId).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    withCsrfToken(callback, token -> emarketService.deleteSupplier(supplierId, token).enqueue(new Callback<ResponseBody>() {
                        @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                        @Override public void onFailure(Call<ResponseBody> c, Throwable t) { callback.onError("Lỗi mạng: " + t.getMessage()); }
                    }));
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                withCsrfToken(callback, token -> emarketService.deleteSupplier(supplierId, token).enqueue(new Callback<ResponseBody>() {
                    @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                    @Override public void onFailure(Call<ResponseBody> c, Throwable t1) { callback.onError("Lỗi mạng: " + t1.getMessage()); }
                }));
            }
        });
    }

    public void createWarehouse(WarehouseDTO dto, ApiCallback<Boolean> callback) {
        emarketService.createWarehouseApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    withCsrfToken(callback, token -> emarketService.createWarehouse(dto, token).enqueue(new Callback<ResponseBody>() {
                        @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                        @Override public void onFailure(Call<ResponseBody> c, Throwable t) { callback.onError("Lỗi mạng: " + t.getMessage()); }
                    }));
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                withCsrfToken(callback, token -> emarketService.createWarehouse(dto, token).enqueue(new Callback<ResponseBody>() {
                    @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                    @Override public void onFailure(Call<ResponseBody> c, Throwable t1) { callback.onError("Lỗi mạng: " + t1.getMessage()); }
                }));
            }
        });
    }

    public void updateWarehouse(WarehouseDTO dto, ApiCallback<Boolean> callback) {
        emarketService.updateWarehouseApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    withCsrfToken(callback, token -> emarketService.updateWarehouse(dto, token).enqueue(new Callback<ResponseBody>() {
                        @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                        @Override public void onFailure(Call<ResponseBody> c, Throwable t) { callback.onError("Lỗi mạng: " + t.getMessage()); }
                    }));
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                withCsrfToken(callback, token -> emarketService.updateWarehouse(dto, token).enqueue(new Callback<ResponseBody>() {
                    @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                    @Override public void onFailure(Call<ResponseBody> c, Throwable t1) { callback.onError("Lỗi mạng: " + t1.getMessage()); }
                }));
            }
        });
    }

    public void deleteWarehouse(int warehouseId, ApiCallback<Boolean> callback) {
        emarketService.deleteWarehouseApi(warehouseId).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    withCsrfToken(callback, token -> emarketService.deleteWarehouse(warehouseId, token).enqueue(new Callback<ResponseBody>() {
                        @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                        @Override public void onFailure(Call<ResponseBody> c, Throwable t) { callback.onError("Lỗi mạng: " + t.getMessage()); }
                    }));
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                withCsrfToken(callback, token -> emarketService.deleteWarehouse(warehouseId, token).enqueue(new Callback<ResponseBody>() {
                    @Override public void onResponse(Call<ResponseBody> c, Response<ResponseBody> r) { callback.onSuccess(r.isSuccessful()); }
                    @Override public void onFailure(Call<ResponseBody> c, Throwable t1) { callback.onError("Lỗi mạng: " + t1.getMessage()); }
                }));
            }
        });
    }

    public void adjustStock(StockAdjustmentRequest request, ApiCallback<Boolean> callback) {
        emarketService.adjustStockApi(request).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    adjustStockLegacy(request, callback);
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                adjustStockLegacy(request, callback);
            }
        });
    }

    private void adjustStockLegacy(StockAdjustmentRequest request, ApiCallback<Boolean> callback) {
        withCsrfToken(callback, token -> {
            emarketService.adjustStock(request, token).enqueue(new Callback<ResponseBody>() {
                @Override
                public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                    if (response.isSuccessful()) {
                        callback.onSuccess(true);
                    } else {
                        callback.onError("Điều chỉnh kho thất bại. Mã: " + response.code());
                    }
                }

                @Override
                public void onFailure(Call<ResponseBody> call, Throwable t) {
                    callback.onError("Lỗi mạng: " + t.getMessage());
                }
            });
        });
    }

    public void saveQuotation(QuotationSaveRequest request, ApiCallback<Boolean> callback) {
        withCsrfToken(callback, token -> {
            emarketService.saveQuotation(request, token).enqueue(new Callback<ResponseBody>() {
                @Override
                public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                    if (response.isSuccessful()) {
                        callback.onSuccess(true);
                    } else {
                        callback.onError("Lưu báo giá thất bại. Mã: " + response.code());
                    }
                }

                @Override
                public void onFailure(Call<ResponseBody> call, Throwable t) {
                    callback.onError("Lỗi mạng: " + t.getMessage());
                }
            });
        });
    }

    public void updateQuotation(QuotationSaveRequest request, ApiCallback<Boolean> callback) {
        withCsrfToken(callback, token -> {
            emarketService.updateQuotation(request, token).enqueue(new Callback<ResponseBody>() {
                @Override
                public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                    if (response.isSuccessful()) {
                        callback.onSuccess(true);
                    } else {
                        callback.onError("Cập nhật báo giá thất bại. Mã: " + response.code());
                    }
                }

                @Override
                public void onFailure(Call<ResponseBody> call, Throwable t) {
                    callback.onError("Lỗi mạng: " + t.getMessage());
                }
            });
        });
    }

    public void deleteQuotation(int quotationId, ApiCallback<Boolean> callback) {
        withCsrfToken(callback, token -> {
            emarketService.deleteQuotation(quotationId, token).enqueue(new Callback<ResponseBody>() {
                @Override
                public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                    if (response.isSuccessful()) {
                        callback.onSuccess(true);
                    } else {
                        callback.onError("Xóa báo giá thất bại. Mã: " + response.code());
                    }
                }

                @Override
                public void onFailure(Call<ResponseBody> call, Throwable t) {
                    callback.onError("Lỗi mạng: " + t.getMessage());
                }
            });
        });
    }



    public void getLatestSystemEvents(ApiCallback<List<SystemEvent>> callback) {
        emarketService.getLatestSystemEvents().enqueue(new Callback<List<SystemEvent>>() {
            @Override
            public void onResponse(Call<List<SystemEvent>> call, Response<List<SystemEvent>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải sự kiện hệ thống. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<SystemEvent>> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải sự kiện: " + t.getMessage());
            }
        });
    }

    public void getAuditLogs(int start, int length, String search, ApiCallback<AuditLogResponse> callback) {
        emarketService.getAuditLogs(start, length, null, null, search, null, null).enqueue(new Callback<AuditLogResponse>() {
            @Override
            public void onResponse(Call<AuditLogResponse> call, Response<AuditLogResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải nhật ký kiểm toán. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<AuditLogResponse> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải audit log: " + t.getMessage());
            }
        });
    }

    public void getAppLogs(int start, int length, String logLevel, ApiCallback<AppLogResponse> callback) {
        emarketService.getAppLogs(start, length, logLevel, null, null, null, null).enqueue(new Callback<AppLogResponse>() {
            @Override
            public void onResponse(Call<AppLogResponse> call, Response<AppLogResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải nhật ký ứng dụng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<AppLogResponse> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải app log: " + t.getMessage());
            }
        });
    }

    // =========================================================================
    // SYSTEM CONFIGURATION
    // =========================================================================

    public void getSystemConfig(ApiCallback<SystemConfig> callback) {
        emarketService.getSystemConfig().enqueue(new Callback<SystemConfig>() {
            @Override
            public void onResponse(Call<SystemConfig> call, Response<SystemConfig> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải cấu hình hệ thống. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<SystemConfig> call, Throwable t) {
                callback.onError("Lỗi mạng khi tải cấu hình: " + t.getMessage());
            }
        });
    }

    public void updateSystemConfig(List<ConfigUpdate> configs, ApiCallback<Boolean> callback) {
        emarketService.updateSystemConfig(configs).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Cập nhật cấu hình thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    // =========================================================================
    // LOYALTY PROGRAM (REST API)
    // =========================================================================

    public void getAllLoyaltyApi(ApiCallback<List<LoyaltyProgram>> callback) {
        emarketService.getAllLoyaltyApi().enqueue(new Callback<List<LoyaltyProgram>>() {
            @Override
            public void onResponse(Call<List<LoyaltyProgram>> call, Response<List<LoyaltyProgram>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải chương trình tích điểm. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<LoyaltyProgram>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getLoyaltyByIdApi(int id, ApiCallback<LoyaltyProgram> callback) {
        emarketService.getLoyaltyByIdApi(id).enqueue(new Callback<LoyaltyProgram>() {
            @Override
            public void onResponse(Call<LoyaltyProgram> call, Response<LoyaltyProgram> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải chi tiết loyalty. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<LoyaltyProgram> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void createLoyaltyApi(LoyaltyProgram dto, ApiCallback<Boolean> callback) {
        emarketService.createLoyaltyApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Tạo chương trình tích điểm thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void updateLoyaltyApi(LoyaltyProgram dto, ApiCallback<Boolean> callback) {
        emarketService.updateLoyaltyApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Cập nhật chương trình tích điểm thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void deleteLoyaltyApi(int id, ApiCallback<Boolean> callback) {
        emarketService.deleteLoyaltyApi(id).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Xóa loyalty thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    // =========================================================================
    // SUPPLIER DEBT & PAYMENTS (REST API)
    // =========================================================================

    public void getDebtByPurchaseOrder(int purchaseOrderId, ApiCallback<SupplierDebt> callback) {
        emarketService.getDebtByPurchaseOrder(purchaseOrderId).enqueue(new Callback<SupplierDebt>() {
            @Override
            public void onResponse(Call<SupplierDebt> call, Response<SupplierDebt> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải công nợ theo đơn nhập. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<SupplierDebt> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void createSupplierDebtApi(SupplierDebt dto, ApiCallback<Boolean> callback) {
        emarketService.createSupplierDebtApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Tạo công nợ thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void updateSupplierDebtApi(SupplierDebt dto, ApiCallback<Boolean> callback) {
        emarketService.updateSupplierDebtApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Cập nhật công nợ thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getPaymentsByDebtId(int debtId, ApiCallback<List<SupplierPayment>> callback) {
        emarketService.getPaymentsByDebtId(debtId).enqueue(new Callback<List<SupplierPayment>>() {
            @Override
            public void onResponse(Call<List<SupplierPayment>> call, Response<List<SupplierPayment>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải lịch sử thanh toán nợ. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<SupplierPayment>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void createSupplierPaymentApi(SupplierPayment paymentDto, ApiCallback<Boolean> callback) {
        emarketService.createSupplierPaymentApi(paymentDto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Tạo thanh toán công nợ thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void deleteSupplierPaymentApi(int id, ApiCallback<Boolean> callback) {
        emarketService.deleteSupplierPaymentApi(id).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Xóa thanh toán nợ thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    // =========================================================================
    // PRODUCT LOTS & IMAGES (REST API)
    // =========================================================================

    public void getProductLotsByProductId(int productId, ApiCallback<List<ProductLot>> callback) {
        emarketService.getProductLotsByProductId(productId).enqueue(new Callback<List<ProductLot>>() {
            @Override
            public void onResponse(Call<List<ProductLot>> call, Response<List<ProductLot>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải danh sách lô hàng. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<ProductLot>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getProductImages(int productId, ApiCallback<List<ProductImage>> callback) {
        emarketService.getProductImages(productId).enqueue(new Callback<List<ProductImage>>() {
            @Override
            public void onResponse(Call<List<ProductImage>> call, Response<List<ProductImage>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải danh sách ảnh sản phẩm. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<ProductImage>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void createProductImageApi(ProductImage dto, ApiCallback<Boolean> callback) {
        emarketService.createProductImageApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Thêm ảnh sản phẩm thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void createProductImage(ProductImage dto, ApiCallback<Boolean> callback) {
        createProductImageApi(dto, callback);
    }

    public void deleteProductImageApi(int id, ApiCallback<Boolean> callback) {
        emarketService.deleteProductImageApi(id).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Xóa ảnh sản phẩm thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void deleteProductImage(int id, ApiCallback<Boolean> callback) {
        deleteProductImageApi(id, callback);
    }

    // =========================================================================
    // PAYOS VIETQR & ORDER DETAILS (REST API)
    // =========================================================================

    public void createPayOSLink(CreateQrRequest qrRequest, ApiCallback<CreateQrResponse> callback) {
        emarketService.createPayOSLink(qrRequest).enqueue(new Callback<CreateQrResponse>() {
            @Override
            public void onResponse(Call<CreateQrResponse> call, Response<CreateQrResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Tạo mã VietQR PayOS thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<CreateQrResponse> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void checkPayOSStatus(long orderCode, ApiCallback<PayOSStatusResponse> callback) {
        emarketService.checkPayOSStatus(orderCode).enqueue(new Callback<PayOSStatusResponse>() {
            @Override
            public void onResponse(Call<PayOSStatusResponse> call, Response<PayOSStatusResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Kiểm tra trạng thái PayOS thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<PayOSStatusResponse> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void createOrderDetailApi(OrderDetail dto, ApiCallback<Boolean> callback) {
        emarketService.createOrderDetailApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Thêm chi tiết đơn hàng thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void updateOrderDetailApi(OrderDetail dto, ApiCallback<Boolean> callback) {
        emarketService.updateOrderDetailApi(dto).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Cập nhật chi tiết đơn hàng thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void deleteOrderDetailApi(int id, ApiCallback<Boolean> callback) {
        emarketService.deleteOrderDetailApi(id).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Xóa chi tiết đơn hàng thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    // =========================================================================
    // NEAREST BRANCH & AUTH REST
    // =========================================================================

    public void getNearestBranch(double lat, double lng, double maxDist, ApiCallback<Branch> callback) {
        emarketService.getNearestBranch(lat, lng, maxDist).enqueue(new Callback<Branch>() {
            @Override
            public void onResponse(Call<Branch> call, Response<Branch> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Không tìm thấy chi nhánh gần nhất. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<Branch> call, Throwable t) {
                callback.onError("Lỗi mạng khi tìm chi nhánh: " + t.getMessage());
            }
        });
    }

    public void loginRestApi(Map<String, String> credentials, ApiCallback<ResponseBody> callback) {
        emarketService.loginRestApi(credentials).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Đăng nhập REST thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void requestOtpRestApi(String email, ApiCallback<Boolean> callback) {
        java.util.Map<String, String> body = new java.util.HashMap<>();
        body.put("Email", email);
        emarketService.requestOtpRestApi(body).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Gửi OTP thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void resetPasswordRestApi(String email, String otp, String newPassword, ApiCallback<Boolean> callback) {
        java.util.Map<String, String> body = new java.util.HashMap<>();
        body.put("Email", email);
        body.put("Otp", otp);
        body.put("NewPassword", newPassword);
        emarketService.resetPasswordRestApi(body).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    callback.onSuccess(true);
                } else {
                    callback.onError("Đổi mật khẩu thất bại. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    // =========================================================================
    // PAGED & SUMMARY METHODS (PHASE 2 UPGRADE)
    // =========================================================================

    public void getProductsPaged(int start, int length, String keyword, Integer categoryId, Integer branchId, Integer supplierId, Integer warehouseId, ApiCallback<ProductPagedResponse> callback) {
        emarketService.getProductsPaged(start, length, keyword, categoryId, branchId, supplierId, warehouseId).enqueue(new Callback<ProductPagedResponse>() {
            @Override
            public void onResponse(Call<ProductPagedResponse> call, Response<ProductPagedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải danh sách sản phẩm phân trang. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ProductPagedResponse> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getStockMovementsPaged(int start, int length, Integer warehouseId, String type, String fromDate, String toDate, String keyword, ApiCallback<StockMovementPagedResponse> callback) {
        emarketService.getStockMovementsPaged(start, length, warehouseId, type, fromDate, toDate, keyword).enqueue(new Callback<StockMovementPagedResponse>() {
            @Override
            public void onResponse(Call<StockMovementPagedResponse> call, Response<StockMovementPagedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải biến động kho phân trang. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<StockMovementPagedResponse> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getInventoryItems(Integer warehouseId, Integer branchId, Integer productId, ApiCallback<List<InventoryItem>> callback) {
        emarketService.getInventoryItems(warehouseId, branchId, productId).enqueue(new Callback<List<InventoryItem>>() {
            @Override
            public void onResponse(Call<List<InventoryItem>> call, Response<List<InventoryItem>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải tồn kho thực tế. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<List<InventoryItem>> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getDebtSummary(Integer supplierId, String fromDate, String toDate, ApiCallback<DebtSummary> callback) {
        emarketService.getDebtSummary(supplierId, fromDate, toDate).enqueue(new Callback<DebtSummary>() {
            @Override
            public void onResponse(Call<DebtSummary> call, Response<DebtSummary> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải thống kê công nợ. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<DebtSummary> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getQuotationsPaged(int start, int length, String keyword, Integer branchId, String status, String fromDate, String toDate, ApiCallback<QuotationPagedResponse> callback) {
        emarketService.getQuotationsPaged(start, length, keyword, branchId, status, fromDate, toDate).enqueue(new Callback<QuotationPagedResponse>() {
            @Override
            public void onResponse(Call<QuotationPagedResponse> call, Response<QuotationPagedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải báo giá phân trang. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<QuotationPagedResponse> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getExpensesPaged(int start, int length, Integer branchId, Integer categoryId, String fromDate, String toDate, String status, ApiCallback<ExpensePagedResponse> callback) {
        emarketService.getExpensesPaged(start, length, branchId, categoryId, fromDate, toDate, status).enqueue(new Callback<ExpensePagedResponse>() {
            @Override
            public void onResponse(Call<ExpensePagedResponse> call, Response<ExpensePagedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải chi phí phân trang. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ExpensePagedResponse> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    public void getExpenseSummary(Integer branchId, String fromDate, String toDate, ApiCallback<ExpenseSummary> callback) {
        emarketService.getExpenseSummary(branchId, fromDate, toDate).enqueue(new Callback<ExpenseSummary>() {
            @Override
            public void onResponse(Call<ExpenseSummary> call, Response<ExpenseSummary> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    callback.onError("Lỗi tải tổng hợp chi phí. Mã: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ExpenseSummary> call, Throwable t) {
                callback.onError("Lỗi mạng: " + t.getMessage());
            }
        });
    }

    // =========================================================================
    // BRANCHES & WAREHOUSES DYNAMIC FETCHING & CACHING
    // =========================================================================

    public void getBranches(ApiCallback<List<Branch>> callback) {
        List<Branch> cached = com.example.do_an_di_dong.util.UserSessionManager.getCachedBranches(context);
        if (!cached.isEmpty()) {
            callback.onSuccess(cached);
        }

        // Primary: REST Web API
        emarketService.getAllBranches().enqueue(new Callback<List<Branch>>() {
            @Override
            public void onResponse(Call<List<Branch>> call, Response<List<Branch>> response) {
                if (response.isSuccessful() && response.body() != null && !response.body().isEmpty()) {
                    List<Branch> list = response.body();
                    com.example.do_an_di_dong.util.UserSessionManager.saveBranches(context, list);
                    callback.onSuccess(list);
                } else {
                    // Fallback to legacy MVC endpoint
                    fetchBranchesLegacy(cached, callback);
                }
            }

            @Override
            public void onFailure(Call<List<Branch>> call, Throwable t) {
                fetchBranchesLegacy(cached, callback);
            }
        });
    }

    private void fetchBranchesLegacy(List<Branch> cached, ApiCallback<List<Branch>> callback) {
        emarketService.getBranchList().enqueue(new Callback<BranchListResponse>() {
            @Override
            public void onResponse(Call<BranchListResponse> call, Response<BranchListResponse> response) {
                if (response.isSuccessful() && response.body() != null && response.body().getData() != null) {
                    List<Branch> list = response.body().getData();
                    com.example.do_an_di_dong.util.UserSessionManager.saveBranches(context, list);
                    callback.onSuccess(list);
                } else if (cached.isEmpty()) {
                    callback.onError("Không thể tải danh sách chi nhánh từ máy chủ.");
                }
            }

            @Override
            public void onFailure(Call<BranchListResponse> call, Throwable t) {
                if (cached.isEmpty()) {
                    callback.onError("Lỗi kết nối máy chủ: " + t.getMessage());
                }
            }
        });
    }

    public void getAllWarehouses(ApiCallback<List<Warehouse>> callback) {
        List<Warehouse> cached = com.example.do_an_di_dong.util.UserSessionManager.getCachedWarehouses(context);
        if (!cached.isEmpty()) {
            callback.onSuccess(cached);
        }

        // Primary: REST Web API
        emarketService.getAllWarehouses().enqueue(new Callback<List<Warehouse>>() {
            @Override
            public void onResponse(Call<List<Warehouse>> call, Response<List<Warehouse>> response) {
                if (response.isSuccessful() && response.body() != null && !response.body().isEmpty()) {
                    List<Warehouse> list = response.body();
                    com.example.do_an_di_dong.util.UserSessionManager.saveWarehouses(context, list);
                    callback.onSuccess(list);
                } else {
                    fetchWarehousesLegacy(cached, callback);
                }
            }

            @Override
            public void onFailure(Call<List<Warehouse>> call, Throwable t) {
                fetchWarehousesLegacy(cached, callback);
            }
        });
    }

    private void fetchWarehousesLegacy(List<Warehouse> cached, ApiCallback<List<Warehouse>> callback) {
        emarketService.getWarehouseList().enqueue(new Callback<WarehouseListResponse>() {
            @Override
            public void onResponse(Call<WarehouseListResponse> call, Response<WarehouseListResponse> response) {
                if (response.isSuccessful() && response.body() != null && response.body().getWarehouses() != null) {
                    List<Warehouse> list = response.body().getWarehouses();
                    com.example.do_an_di_dong.util.UserSessionManager.saveWarehouses(context, list);
                    callback.onSuccess(list);
                } else if (cached.isEmpty()) {
                    callback.onError("Không thể tải danh sách kho hàng từ máy chủ.");
                }
            }

            @Override
            public void onFailure(Call<WarehouseListResponse> call, Throwable t) {
                if (cached.isEmpty()) {
                    callback.onError("Lỗi kết nối máy chủ: " + t.getMessage());
                }
            }
        });
    }

    public void getFilteredWarehouses(String warehouseName, Integer branchId, ApiCallback<List<Warehouse>> callback) {
        // Primary: REST Web API
        emarketService.getWarehousesFiltered(warehouseName, branchId).enqueue(new Callback<List<Warehouse>>() {
            @Override
            public void onResponse(Call<List<Warehouse>> call, Response<List<Warehouse>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    getAllWarehouses(callback);
                }
            }

            @Override
            public void onFailure(Call<List<Warehouse>> call, Throwable t) {
                getAllWarehouses(callback);
            }
        });
    }
}


