package com.example.do_an_di_dong.ui.ai;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.InventoryForecast;
import com.example.do_an_di_dong.model.LotFinancialRisk;

import java.util.List;

public class AIPredictionDashboardViewModel extends AndroidViewModel {
    private final DataRepository repository;
    private final MutableLiveData<List<LotFinancialRisk>> lotRiskLiveData = new MutableLiveData<>();
    private final MutableLiveData<List<InventoryForecast>> forecastLiveData = new MutableLiveData<>();
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> prophetSuccessLiveData = new MutableLiveData<>();

    public AIPredictionDashboardViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<LotFinancialRisk>> getLotRisk() {
        return lotRiskLiveData;
    }

    public LiveData<List<InventoryForecast>> getForecast() {
        return forecastLiveData;
    }

    public LiveData<String> getError() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoading() {
        return loadingLiveData;
    }

    public LiveData<Boolean> getProphetSuccess() {
        return prophetSuccessLiveData;
    }

    public void loadPredictions(int branchId) {
        loadingLiveData.postValue(true);

        // 1. Lot Financial Risk
        repository.getLotFinancialRisk(branchId, new DataRepository.ApiCallback<List<LotFinancialRisk>>() {
            @Override
            public void onSuccess(List<LotFinancialRisk> result) {
                lotRiskLiveData.postValue(result);
                loadingLiveData.postValue(false);
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
            }
        });

        // 2. Inventory Forecast
        repository.getInventoryForecast(branchId, new DataRepository.ApiCallback<List<InventoryForecast>>() {
            @Override
            public void onSuccess(List<InventoryForecast> result) {
                forecastLiveData.postValue(result);
            }

            @Override
            public void onError(String message) {}
        });
    }

    public void triggerProphetTraining() {
        loadingLiveData.postValue(true);
        repository.runProphet(new DataRepository.ApiCallback<Boolean>() {
            @Override
            public void onSuccess(Boolean result) {
                loadingLiveData.postValue(false);
                prophetSuccessLiveData.postValue(true);
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
            }
        });
    }
}
