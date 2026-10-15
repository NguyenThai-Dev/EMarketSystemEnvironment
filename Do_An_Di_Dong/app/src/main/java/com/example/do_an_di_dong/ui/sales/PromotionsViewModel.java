package com.example.do_an_di_dong.ui.sales;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.Promotion;

import java.util.List;

public class PromotionsViewModel extends AndroidViewModel {
    private final DataRepository repository;
    private final MutableLiveData<List<Promotion>> promotionsLiveData = new MutableLiveData<>();
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>();

    public PromotionsViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<Promotion>> getPromotions() {
        return promotionsLiveData;
    }

    public LiveData<String> getError() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoading() {
        return loadingLiveData;
    }

    public void fetchPromotions() {
        loadingLiveData.postValue(true);
        repository.getActivePromotions(new DataRepository.ApiCallback<List<Promotion>>() {
            @Override
            public void onSuccess(List<Promotion> result) {
                promotionsLiveData.postValue(result);
                loadingLiveData.postValue(false);
            }

            @Override
            public void onError(String message) {
                errorLiveData.postValue(message);
                loadingLiveData.postValue(false);
            }
        });
    }
}
