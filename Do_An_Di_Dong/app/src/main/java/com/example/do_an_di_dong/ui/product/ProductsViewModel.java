package com.example.do_an_di_dong.ui.product;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.Product;
import com.example.do_an_di_dong.model.ProductCategoryDTO;
import com.example.do_an_di_dong.model.ProductPagedResponse;

import java.util.ArrayList;
import java.util.List;

public class ProductsViewModel extends AndroidViewModel {
    private static final int PAGE_SIZE = 20;

    private final DataRepository repository;
    private final MutableLiveData<List<Product>> productsLiveData = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<Product> scannedProduct = new MutableLiveData<>();
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> initialLoadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> loadMoreLoadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> emptyLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> lastPageLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<List<ProductCategoryDTO>> categoriesLiveData = new MutableLiveData<>();

    private final List<Product> accumulatedProducts = new ArrayList<>();
    private int currentStart = 0;
    private String currentKeyword = "";
    private Integer currentCategoryId = null;
    private boolean isFetching = false;

    public ProductsViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<Product>> getProducts() { return productsLiveData; }
    public LiveData<Product> getScannedProduct() { return scannedProduct; }
    public LiveData<String> getError() { return errorLiveData; }
    public LiveData<Boolean> getInitialLoading() { return initialLoadingLiveData; }
    public LiveData<Boolean> getLoadMoreLoading() { return loadMoreLoadingLiveData; }
    public LiveData<Boolean> getEmpty() { return emptyLiveData; }
    public LiveData<Boolean> getLastPage() { return lastPageLiveData; }
    public LiveData<List<ProductCategoryDTO>> getCategories() { return categoriesLiveData; }

    public void fetchProducts() {
        refreshProducts();
    }

    public void refreshProducts() {
        currentStart = 0;
        accumulatedProducts.clear();
        lastPageLiveData.setValue(false);
        emptyLiveData.setValue(false);
        loadNextPage();
    }

    public void loadNextPage() {
        if (isFetching || Boolean.TRUE.equals(lastPageLiveData.getValue())) {
            return;
        }

        isFetching = true;
        if (currentStart == 0) {
            initialLoadingLiveData.setValue(true);
        } else {
            loadMoreLoadingLiveData.setValue(true);
        }

        String keywordParam = currentKeyword.isEmpty() ? null : currentKeyword;

        repository.getProductsPaged(currentStart, PAGE_SIZE, keywordParam, currentCategoryId, null, null, null, new DataRepository.ApiCallback<ProductPagedResponse>() {
            @Override
            public void onSuccess(ProductPagedResponse result) {
                isFetching = false;
                initialLoadingLiveData.postValue(false);
                loadMoreLoadingLiveData.postValue(false);

                List<Product> newItems = (result != null && result.getData() != null) ? result.getData() : new ArrayList<>();

                // De-duplicate items
                for (Product item : newItems) {
                    if (!hasProduct(accumulatedProducts, item.getProductId())) {
                        accumulatedProducts.add(item);
                    }
                }

                productsLiveData.postValue(new ArrayList<>(accumulatedProducts));

                currentStart += newItems.size();
                int total = (result != null) ? result.getTotal() : 0;

                boolean isLast = newItems.isEmpty() || accumulatedProducts.size() >= total || newItems.size() < PAGE_SIZE;
                lastPageLiveData.postValue(isLast);

                emptyLiveData.postValue(accumulatedProducts.isEmpty());
            }

            @Override
            public void onError(String message) {
                isFetching = false;
                initialLoadingLiveData.postValue(false);
                loadMoreLoadingLiveData.postValue(false);
                errorLiveData.postValue(message);
                emptyLiveData.postValue(accumulatedProducts.isEmpty());
            }
        });
    }

    private boolean hasProduct(List<Product> list, int productId) {
        for (Product p : list) {
            if (p.getProductId() == productId) return true;
        }
        return false;
    }

    public void searchProducts(String keyword, Integer categoryId) {
        this.currentKeyword = (keyword != null) ? keyword.trim() : "";
        this.currentCategoryId = categoryId;
        refreshProducts();
    }

    public void fetchCategories() {
        repository.getProductCategories(new DataRepository.ApiCallback<List<ProductCategoryDTO>>() {
            @Override
            public void onSuccess(List<ProductCategoryDTO> result) {
                categoriesLiveData.postValue(result);
            }

            @Override
            public void onError(String message) {
                errorLiveData.postValue(message);
            }
        });
    }

    public void lookupBarcode(String barcode) {
        initialLoadingLiveData.setValue(true);
        repository.getProductBySkuOffline(barcode, new DataRepository.ApiCallback<Product>() {
            @Override
            public void onSuccess(Product result) {
                initialLoadingLiveData.postValue(false);
                scannedProduct.postValue(result);
            }

            @Override
            public void onError(String message) {
                initialLoadingLiveData.postValue(false);
                errorLiveData.postValue(message);
            }
        });
    }
}
