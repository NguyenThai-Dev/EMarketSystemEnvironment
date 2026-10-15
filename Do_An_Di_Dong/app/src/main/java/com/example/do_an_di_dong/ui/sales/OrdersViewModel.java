package com.example.do_an_di_dong.ui.sales;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.Order;
import com.example.do_an_di_dong.model.OrderPagedResponse;

import java.util.ArrayList;
import java.util.List;

public class OrdersViewModel extends AndroidViewModel {
    private static final int PAGE_SIZE = 20;

    private final DataRepository repository;
    private final MutableLiveData<List<Order>> ordersLiveData = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> emptyLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> lastPageLiveData = new MutableLiveData<>(false);

    private final List<Order> accumulatedOrders = new ArrayList<>();
    private int currentStart = 0;
    private String currentStatusFilter = null; // null or "ALL"
    private String currentKeyword = "";

    public OrdersViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<Order>> getOrders() {
        return ordersLiveData;
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

    public void fetchOrders() {
        refreshOrders();
    }

    public void refreshOrders() {
        currentStart = 0;
        accumulatedOrders.clear();
        lastPageLiveData.setValue(false);
        loadNextPage();
    }

    public void loadNextPage() {
        if (Boolean.TRUE.equals(loadingLiveData.getValue()) || Boolean.TRUE.equals(lastPageLiveData.getValue())) {
            return;
        }

        loadingLiveData.setValue(true);

        String statusParam = ("ALL".equalsIgnoreCase(currentStatusFilter) || currentStatusFilter == null || currentStatusFilter.isEmpty()) 
                ? null : currentStatusFilter;
        String keywordParam = currentKeyword.isEmpty() ? null : currentKeyword;
        String toDateToday = new java.text.SimpleDateFormat("yyyy-MM-dd", java.util.Locale.getDefault()).format(new java.util.Date());

        repository.getOrdersPaged(currentStart, PAGE_SIZE, null, null, statusParam, null, toDateToday, keywordParam, new DataRepository.ApiCallback<OrderPagedResponse>() {
            @Override
            public void onSuccess(OrderPagedResponse result) {
                loadingLiveData.postValue(false);
                List<Order> newItems = (result != null && result.getData() != null) ? result.getData() : new ArrayList<>();

                accumulatedOrders.addAll(newItems);
                ordersLiveData.postValue(new ArrayList<>(accumulatedOrders));

                currentStart += newItems.size();
                int total = (result != null) ? result.getTotal() : 0;

                boolean isLast = newItems.isEmpty() || accumulatedOrders.size() >= total || newItems.size() < PAGE_SIZE;
                lastPageLiveData.postValue(isLast);

                emptyLiveData.postValue(accumulatedOrders.isEmpty());
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
                emptyLiveData.postValue(accumulatedOrders.isEmpty());
            }
        });
    }

    public void setStatusFilter(String status) {
        if (!isEqual(this.currentStatusFilter, status)) {
            this.currentStatusFilter = status;
            refreshOrders();
        }
    }

    public void setSearchKeyword(String keyword) {
        String cleanKey = (keyword != null) ? keyword.trim() : "";
        if (!cleanKey.equalsIgnoreCase(currentKeyword)) {
            this.currentKeyword = cleanKey;
            refreshOrders();
        }
    }

    private boolean isEqual(String a, String b) {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return a.equalsIgnoreCase(b);
    }
}
