package com.example.do_an_di_dong.ui.admin;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.Customer;
import com.example.do_an_di_dong.model.CustomerPagedResponse;

import java.util.ArrayList;
import java.util.List;

public class CustomersViewModel extends AndroidViewModel {
    private static final int PAGE_SIZE = 20;

    private final DataRepository repository;
    private final MutableLiveData<List<Customer>> customersLiveData = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> emptyLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> lastPageLiveData = new MutableLiveData<>(false);

    private final List<Customer> accumulatedCustomers = new ArrayList<>();
    private int currentStart = 0;
    private String currentKeyword = "";

    public CustomersViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<Customer>> getCustomers() {
        return customersLiveData;
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

    public void fetchCustomers() {
        refreshCustomers();
    }

    public void refreshCustomers() {
        currentStart = 0;
        accumulatedCustomers.clear();
        lastPageLiveData.setValue(false);
        loadNextPage();
    }

    public void loadNextPage() {
        if (Boolean.TRUE.equals(loadingLiveData.getValue()) || Boolean.TRUE.equals(lastPageLiveData.getValue())) {
            return;
        }

        loadingLiveData.setValue(true);
        String keywordParam = currentKeyword.isEmpty() ? null : currentKeyword;

        repository.getCustomersPaged(currentStart, PAGE_SIZE, keywordParam, new DataRepository.ApiCallback<CustomerPagedResponse>() {
            @Override
            public void onSuccess(CustomerPagedResponse result) {
                loadingLiveData.postValue(false);
                List<Customer> newItems = (result != null && result.getData() != null) ? result.getData() : new ArrayList<>();

                accumulatedCustomers.addAll(newItems);
                customersLiveData.postValue(new ArrayList<>(accumulatedCustomers));

                currentStart += newItems.size();
                int total = (result != null) ? result.getTotal() : 0;

                boolean isLast = newItems.isEmpty() || accumulatedCustomers.size() >= total || newItems.size() < PAGE_SIZE;
                lastPageLiveData.postValue(isLast);

                emptyLiveData.postValue(accumulatedCustomers.isEmpty());
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
                emptyLiveData.postValue(accumulatedCustomers.isEmpty());
            }
        });
    }

    public void setSearchKeyword(String keyword) {
        String cleanKey = (keyword != null) ? keyword.trim() : "";
        if (!cleanKey.equalsIgnoreCase(currentKeyword)) {
            this.currentKeyword = cleanKey;
            refreshCustomers();
        }
    }
}
