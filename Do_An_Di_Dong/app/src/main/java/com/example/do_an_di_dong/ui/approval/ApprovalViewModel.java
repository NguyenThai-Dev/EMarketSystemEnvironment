package com.example.do_an_di_dong.ui.approval;

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

public class ApprovalViewModel extends AndroidViewModel {
    private static final int PAGE_SIZE = 20;

    private final DataRepository repository;
    private final MutableLiveData<List<Quotation>> quotations = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<String> statusUpdated = new MutableLiveData<>();
    private final MutableLiveData<String> error = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loading = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> empty = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> lastPage = new MutableLiveData<>(false);

    private final List<Quotation> accumulatedQuotations = new ArrayList<>();
    private int currentStart = 0;
    private String currentStatus = "Sent";

    public ApprovalViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<Quotation>> getQuotations() {
        return quotations;
    }

    public LiveData<String> getStatusUpdated() {
        return statusUpdated;
    }

    public LiveData<String> getError() {
        return error;
    }

    public LiveData<Boolean> getLoading() {
        return loading;
    }

    public LiveData<Boolean> getEmpty() {
        return empty;
    }

    public LiveData<Boolean> getLastPage() {
        return lastPage;
    }

    public void fetchQuotations(String status) {
        this.currentStatus = (status != null && !status.isEmpty()) ? status : "Sent";
        refreshQuotations();
    }

    public void refreshQuotations() {
        currentStart = 0;
        accumulatedQuotations.clear();
        lastPage.setValue(false);
        loadNextPage();
    }

    public void loadNextPage() {
        if (Boolean.TRUE.equals(loading.getValue()) || Boolean.TRUE.equals(lastPage.getValue())) {
            return;
        }

        loading.setValue(true);
        repository.getQuotationsPaged(currentStart, PAGE_SIZE, null, null, currentStatus, null, null, new DataRepository.ApiCallback<QuotationPagedResponse>() {
            @Override
            public void onSuccess(QuotationPagedResponse result) {
                loading.postValue(false);
                List<Quotation> newItems = (result != null && result.getData() != null) ? result.getData() : new ArrayList<>();

                accumulatedQuotations.addAll(newItems);
                quotations.postValue(new ArrayList<>(accumulatedQuotations));

                currentStart += newItems.size();
                int total = (result != null) ? result.getTotal() : 0;

                boolean isLast = newItems.isEmpty() || accumulatedQuotations.size() >= total || newItems.size() < PAGE_SIZE;
                lastPage.postValue(isLast);

                empty.postValue(accumulatedQuotations.isEmpty());
            }

            @Override
            public void onError(String message) {
                loading.postValue(false);
                error.postValue(message);
                empty.postValue(accumulatedQuotations.isEmpty());
            }
        });
    }

    public void updateQuotationStatus(int id, String status) {
        loading.setValue(true);
        repository.changeQuotationStatus(id, status, new DataRepository.ApiCallback<Boolean>() {
            @Override
            public void onSuccess(Boolean result) {
                loading.postValue(false);
                statusUpdated.postValue("Cập nhật trạng thái thành công!");
                refreshQuotations();
            }

            @Override
            public void onError(String message) {
                loading.postValue(false);
                error.postValue(message);
                refreshQuotations();
            }
        });
    }
}
