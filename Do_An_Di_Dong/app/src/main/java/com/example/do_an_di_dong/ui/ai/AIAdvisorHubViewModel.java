package com.example.do_an_di_dong.ui.ai;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.Anomaly;
import com.example.do_an_di_dong.model.Insight;
import com.example.do_an_di_dong.model.ReplenishmentAdvice;

import java.util.List;

public class AIAdvisorHubViewModel extends AndroidViewModel {
    private final DataRepository repository;
    private final MutableLiveData<List<ReplenishmentAdvice>> adviceLiveData = new MutableLiveData<>();
    private final MutableLiveData<List<Anomaly>> anomaliesLiveData = new MutableLiveData<>();
    private final MutableLiveData<List<Insight>> insightsLiveData = new MutableLiveData<>();
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>();

    public AIAdvisorHubViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<ReplenishmentAdvice>> getAdvice() {
        return adviceLiveData;
    }

    public LiveData<List<Anomaly>> getAnomalies() {
        return anomaliesLiveData;
    }

    public LiveData<List<Insight>> getInsights() {
        return insightsLiveData;
    }

    public LiveData<String> getError() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoading() {
        return loadingLiveData;
    }

    public void loadAdvisorData(int branchId) {
        loadingLiveData.postValue(true);

        // 1. Replenishment Advice
        repository.getReplenishmentAdvice(branchId, new DataRepository.ApiCallback<List<ReplenishmentAdvice>>() {
            @Override
            public void onSuccess(List<ReplenishmentAdvice> result) {
                adviceLiveData.postValue(result);
                loadingLiveData.postValue(false);
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
            }
        });

        // 2. Anomalies
        repository.getAnomalies(branchId, new DataRepository.ApiCallback<List<Anomaly>>() {
            @Override
            public void onSuccess(List<Anomaly> result) {
                anomaliesLiveData.postValue(result);
            }

            @Override
            public void onError(String message) {}
        });

        // 3. Insights
        repository.getProductInsights(branchId, new DataRepository.ApiCallback<List<Insight>>() {
            @Override
            public void onSuccess(List<Insight> result) {
                insightsLiveData.postValue(result);
            }

            @Override
            public void onError(String message) {}
        });
    }
}
