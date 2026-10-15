package com.example.do_an_di_dong.ui.sales;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.OrderDetail;
import com.example.do_an_di_dong.model.Payment;

import java.util.List;

public class OrderDetailViewModel extends AndroidViewModel {
    private final DataRepository repository;
    private final MutableLiveData<List<OrderDetail>> detailsLiveData = new MutableLiveData<>();
    private final MutableLiveData<List<Payment>> paymentsLiveData = new MutableLiveData<>();
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>();

    public OrderDetailViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<OrderDetail>> getDetails() {
        return detailsLiveData;
    }

    public LiveData<List<Payment>> getPayments() {
        return paymentsLiveData;
    }

    public LiveData<String> getError() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoading() {
        return loadingLiveData;
    }

    public void loadOrderInfo(int orderId) {
        loadingLiveData.postValue(true);

        // 1. Fetch Order Details (Line items)
        repository.getOrderDetails(orderId, new DataRepository.ApiCallback<List<OrderDetail>>() {
            @Override
            public void onSuccess(List<OrderDetail> result) {
                detailsLiveData.postValue(result);
                loadingLiveData.postValue(false);
            }

            @Override
            public void onError(String message) {
                errorLiveData.postValue(message);
                loadingLiveData.postValue(false);
            }
        });

        // 2. Fetch Payments History
        repository.getPaymentsByOrder(orderId, new DataRepository.ApiCallback<List<Payment>>() {
            @Override
            public void onSuccess(List<Payment> result) {
                paymentsLiveData.postValue(result);
            }

            @Override
            public void onError(String message) {
                // Non-fatal error for payments
            }
        });
    }
}
