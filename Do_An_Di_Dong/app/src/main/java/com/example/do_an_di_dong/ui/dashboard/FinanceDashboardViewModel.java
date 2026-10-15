package com.example.do_an_di_dong.ui.dashboard;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.FinanceDashboard;

public class FinanceDashboardViewModel extends AndroidViewModel {
    private final DataRepository repository;
    private final MutableLiveData<FinanceDashboard> financeLiveData = new MutableLiveData<>();
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>();

    public FinanceDashboardViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<FinanceDashboard> getFinance() {
        return financeLiveData;
    }

    public LiveData<String> getError() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoading() {
        return loadingLiveData;
    }

    public void loadFinance(int daysBack, Integer branchId) {
        loadingLiveData.postValue(true);
        repository.getFinanceDashboard(daysBack, branchId, new DataRepository.ApiCallback<FinanceDashboard>() {
            @Override
            public void onSuccess(FinanceDashboard result) {
                financeLiveData.postValue(result);
                loadingLiveData.postValue(false);
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
            }
        });
    }
}
