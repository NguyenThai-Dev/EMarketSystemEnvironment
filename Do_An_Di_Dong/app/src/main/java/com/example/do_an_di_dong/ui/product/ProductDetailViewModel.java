package com.example.do_an_di_dong.ui.product;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.Product;
import com.example.do_an_di_dong.model.ProductImage;
import com.example.do_an_di_dong.model.ProductLot;

import java.util.ArrayList;
import java.util.List;

public class ProductDetailViewModel extends AndroidViewModel {
    private final DataRepository repository;
    private final MutableLiveData<Product> productLiveData = new MutableLiveData<>();
    private final MutableLiveData<List<ProductLot>> lotsLiveData = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<List<ProductImage>> imagesLiveData = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> deleteSuccessLiveData = new MutableLiveData<>();

    public ProductDetailViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<Product> getProduct() { return productLiveData; }
    public LiveData<List<ProductLot>> getLots() { return lotsLiveData; }
    public LiveData<List<ProductImage>> getImages() { return imagesLiveData; }
    public LiveData<String> getError() { return errorLiveData; }
    public LiveData<Boolean> getLoading() { return loadingLiveData; }
    public LiveData<Boolean> getDeleteSuccess() { return deleteSuccessLiveData; }

    public void loadProduct(int productId) {
        loadingLiveData.postValue(true);

        repository.getProductById(productId, new DataRepository.ApiCallback<Product>() {
            @Override
            public void onSuccess(Product result) {
                productLiveData.postValue(result);
                loadingLiveData.postValue(false);
            }

            @Override
            public void onError(String message) {
                errorLiveData.postValue(message);
                loadingLiveData.postValue(false);
            }
        });

        loadLots(productId);
        loadImages(productId);
    }

    public void loadLots(int productId) {
        repository.getProductLotsByProductId(productId, new DataRepository.ApiCallback<List<ProductLot>>() {
            @Override
            public void onSuccess(List<ProductLot> result) {
                lotsLiveData.postValue(result != null ? result : new ArrayList<>());
            }

            @Override
            public void onError(String message) {
                // Non-fatal
            }
        });
    }

    public void loadImages(int productId) {
        repository.getProductImages(productId, new DataRepository.ApiCallback<List<ProductImage>>() {
            @Override
            public void onSuccess(List<ProductImage> result) {
                imagesLiveData.postValue(result != null ? result : new ArrayList<>());
            }

            @Override
            public void onError(String message) {
                // Non-fatal
            }
        });
    }

    public void addProductImage(int productId, String imageUrl, DataRepository.ApiCallback<Boolean> callback) {
        ProductImage img = new ProductImage(productId, imageUrl);
        repository.createProductImage(img, new DataRepository.ApiCallback<Boolean>() {
            @Override
            public void onSuccess(Boolean result) {
                loadImages(productId);
                if (callback != null) callback.onSuccess(true);
            }

            @Override
            public void onError(String message) {
                if (callback != null) callback.onError(message);
            }
        });
    }

    public void deleteProductImage(int imageId, int productId, DataRepository.ApiCallback<Boolean> callback) {
        repository.deleteProductImage(imageId, new DataRepository.ApiCallback<Boolean>() {
            @Override
            public void onSuccess(Boolean result) {
                loadImages(productId);
                if (callback != null) callback.onSuccess(true);
            }

            @Override
            public void onError(String message) {
                if (callback != null) callback.onError(message);
            }
        });
    }

    public void deleteProduct(int productId) {
        loadingLiveData.postValue(true);
        repository.deleteProduct(productId, new DataRepository.ApiCallback<Boolean>() {
            @Override
            public void onSuccess(Boolean result) {
                loadingLiveData.postValue(false);
                deleteSuccessLiveData.postValue(true);
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
            }
        });
    }
}
