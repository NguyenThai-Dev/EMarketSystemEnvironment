package com.example.do_an_di_dong.ui.product;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.ProductDTO;

public class ProductEditorViewModel extends AndroidViewModel {
    private final DataRepository repository;
    private final MutableLiveData<Boolean> saveSuccessLiveData = new MutableLiveData<>();
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>();

    public ProductEditorViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<Boolean> getSaveSuccess() {
        return saveSuccessLiveData;
    }

    public LiveData<String> getError() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoading() {
        return loadingLiveData;
    }

    public void saveProduct(ProductDTO dto, boolean isEdit) {
        loadingLiveData.postValue(true);
        if (isEdit) {
            repository.updateProduct(dto, new DataRepository.ApiCallback<Boolean>() {
                @Override
                public void onSuccess(Boolean result) {
                    loadingLiveData.postValue(false);
                    saveSuccessLiveData.postValue(true);
                }

                @Override
                public void onError(String message) {
                    loadingLiveData.postValue(false);
                    errorLiveData.postValue(message);
                }
            });
        } else {
            repository.createProduct(dto, new DataRepository.ApiCallback<Boolean>() {
                @Override
                public void onSuccess(Boolean result) {
                    loadingLiveData.postValue(false);
                    saveSuccessLiveData.postValue(true);
                }

                @Override
                public void onError(String message) {
                    loadingLiveData.postValue(false);
                    errorLiveData.postValue(message);
                }
            });
        }
    }
}
