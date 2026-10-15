package com.example.do_an_di_dong.ui.inventory;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.StockMovement;
import com.example.do_an_di_dong.model.StockMovementPagedResponse;

import java.util.ArrayList;
import java.util.List;

public class StockMovementsViewModel extends AndroidViewModel {
    private static final int PAGE_SIZE = 20;

    private final DataRepository repository;
    private final MutableLiveData<List<StockMovement>> movementsLiveData = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> emptyLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> lastPageLiveData = new MutableLiveData<>(false);

    private final List<StockMovement> accumulatedMovements = new ArrayList<>();
    private int currentStart = 0;
    private Integer currentWarehouseId = null;
    private String currentType = null;
    private String currentKeyword = "";

    public StockMovementsViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<StockMovement>> getMovements() {
        return movementsLiveData;
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

    public void fetchMovements() {
        refreshMovements();
    }

    public void refreshMovements() {
        currentStart = 0;
        accumulatedMovements.clear();
        lastPageLiveData.setValue(false);
        loadNextPage();
    }

    public void loadNextPage() {
        if (Boolean.TRUE.equals(loadingLiveData.getValue()) || Boolean.TRUE.equals(lastPageLiveData.getValue())) {
            return;
        }

        loadingLiveData.setValue(true);
        String keywordParam = currentKeyword.isEmpty() ? null : currentKeyword;
        String toDateToday = new java.text.SimpleDateFormat("yyyy-MM-dd", java.util.Locale.getDefault()).format(new java.util.Date());

        repository.getStockMovementsPaged(currentStart, PAGE_SIZE, currentWarehouseId, currentType, null, toDateToday, keywordParam, new DataRepository.ApiCallback<StockMovementPagedResponse>() {
            @Override
            public void onSuccess(StockMovementPagedResponse result) {
                loadingLiveData.postValue(false);
                List<StockMovement> newItems = (result != null && result.getData() != null) ? result.getData() : new ArrayList<>();

                accumulatedMovements.addAll(newItems);
                movementsLiveData.postValue(new ArrayList<>(accumulatedMovements));

                currentStart += newItems.size();
                int total = (result != null) ? result.getTotal() : 0;

                boolean isLast = newItems.isEmpty() || accumulatedMovements.size() >= total || newItems.size() < PAGE_SIZE;
                lastPageLiveData.postValue(isLast);

                emptyLiveData.postValue(accumulatedMovements.isEmpty());
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
                emptyLiveData.postValue(accumulatedMovements.isEmpty());
            }
        });
    }

    public void setFilters(Integer warehouseId, String type, String keyword) {
        this.currentWarehouseId = warehouseId;
        this.currentType = type;
        this.currentKeyword = (keyword != null) ? keyword.trim() : "";
        refreshMovements();
    }
}
