package com.example.do_an_di_dong.ui.inventory;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.PurchaseOrder;
import com.example.do_an_di_dong.model.PurchasePagedResponse;

import java.util.ArrayList;
import java.util.List;

public class PurchaseOrdersViewModel extends AndroidViewModel {
    private static final int PAGE_SIZE = 20;

    private final DataRepository repository;
    private final MutableLiveData<List<PurchaseOrder>> purchaseOrdersLiveData = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> emptyLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> lastPageLiveData = new MutableLiveData<>(false);

    private final List<PurchaseOrder> accumulatedPurchases = new ArrayList<>();
    private int currentStart = 0;
    private String currentKeyword = "";

    public PurchaseOrdersViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<PurchaseOrder>> getPurchaseOrders() {
        return purchaseOrdersLiveData;
    }

    public LiveData<String> getError() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoading() {
        return loadingLiveData;
    }

    public LiveData<Boolean> getEmpty() {
        return emptyLiveData;
    }

    public LiveData<Boolean> getLastPage() {
        return lastPageLiveData;
    }

    public void fetchPurchaseOrders() {
        refreshPurchaseOrders();
    }

    public void refreshPurchaseOrders() {
        currentStart = 0;
        accumulatedPurchases.clear();
        lastPageLiveData.setValue(false);
        loadNextPage();
    }

    public void loadNextPage() {
        if (Boolean.TRUE.equals(loadingLiveData.getValue()) || Boolean.TRUE.equals(lastPageLiveData.getValue())) {
            return;
        }

        loadingLiveData.setValue(true);
        String keywordParam = currentKeyword.isEmpty() ? null : currentKeyword;

        repository.searchPurchasesPaged(currentStart, PAGE_SIZE, keywordParam, null, null, null, null, null, null, null, new DataRepository.ApiCallback<PurchasePagedResponse>() {
            @Override
            public void onSuccess(PurchasePagedResponse result) {
                loadingLiveData.postValue(false);
                List<PurchaseOrder> newItems = (result != null && result.getData() != null) ? result.getData() : new ArrayList<>();

                accumulatedPurchases.addAll(newItems);
                purchaseOrdersLiveData.postValue(new ArrayList<>(accumulatedPurchases));

                currentStart += newItems.size();
                int total = (result != null) ? result.getTotal() : 0;

                boolean isLast = newItems.isEmpty() || accumulatedPurchases.size() >= total || newItems.size() < PAGE_SIZE;
                lastPageLiveData.postValue(isLast);

                emptyLiveData.postValue(accumulatedPurchases.isEmpty());
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
                emptyLiveData.postValue(accumulatedPurchases.isEmpty());
            }
        });
    }

    public void setSearchKeyword(String keyword) {
        String cleanKey = (keyword != null) ? keyword.trim() : "";
        if (!cleanKey.equalsIgnoreCase(currentKeyword)) {
            this.currentKeyword = cleanKey;
            refreshPurchaseOrders();
        }
    }
}
