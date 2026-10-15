package com.example.do_an_di_dong.ui.quotation;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.Quotation;
import com.example.do_an_di_dong.model.QuotationPagedResponse;

import java.util.ArrayList;
import java.util.List;

public class QuotationsViewModel extends AndroidViewModel {
    private static final int PAGE_SIZE = 20;

    private final DataRepository repository;
    private final MutableLiveData<List<Quotation>> quotationsLiveData = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> emptyLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> lastPageLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();

    private final List<Quotation> accumulatedQuotations = new ArrayList<>();
    private int currentStart = 0;
    private String currentKeyword = "";
    private String currentStatus = null;

    public QuotationsViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<Quotation>> getQuotations() { return quotationsLiveData; }
    public LiveData<Boolean> getLoading() { return loadingLiveData; }
    public LiveData<Boolean> getEmpty() { return emptyLiveData; }
    public LiveData<Boolean> getLastPage() { return lastPageLiveData; }
    public LiveData<String> getError() { return errorLiveData; }

    public void loadQuotations() {
        refreshQuotations();
    }

    public void refreshQuotations() {
        currentStart = 0;
        accumulatedQuotations.clear();
        lastPageLiveData.setValue(false);
        loadNextPage();
    }

    public void loadNextPage() {
        if (Boolean.TRUE.equals(loadingLiveData.getValue()) || Boolean.TRUE.equals(lastPageLiveData.getValue())) {
            return;
        }

        loadingLiveData.setValue(true);
        String keywordParam = currentKeyword.isEmpty() ? null : currentKeyword;

        repository.getQuotationsPaged(currentStart, PAGE_SIZE, keywordParam, null, currentStatus, null, null, new DataRepository.ApiCallback<QuotationPagedResponse>() {
            @Override
            public void onSuccess(QuotationPagedResponse result) {
                loadingLiveData.postValue(false);
                List<Quotation> newItems = (result != null && result.getData() != null) ? result.getData() : new ArrayList<>();

                accumulatedQuotations.addAll(newItems);
                quotationsLiveData.postValue(new ArrayList<>(accumulatedQuotations));

                currentStart += newItems.size();
                int total = (result != null) ? result.getTotal() : 0;

                boolean isLast = newItems.isEmpty() || accumulatedQuotations.size() >= total || newItems.size() < PAGE_SIZE;
                lastPageLiveData.postValue(isLast);

                emptyLiveData.postValue(accumulatedQuotations.isEmpty());
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
                emptyLiveData.postValue(accumulatedQuotations.isEmpty());
            }
        });
    }

    public void setSearchKeyword(String keyword) {
        String cleanKey = (keyword != null) ? keyword.trim() : "";
        if (!cleanKey.equalsIgnoreCase(currentKeyword)) {
            this.currentKeyword = cleanKey;
            refreshQuotations();
        }
    }

    public void setStatusFilter(String status) {
        this.currentStatus = status;
        refreshQuotations();
    }
}
