package com.example.do_an_di_dong.ui.admin;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.Expense;
import com.example.do_an_di_dong.model.ExpensePagedResponse;
import com.example.do_an_di_dong.model.ExpenseSummary;

import java.util.ArrayList;
import java.util.List;

public class ExpensesViewModel extends AndroidViewModel {
    private static final int PAGE_SIZE = 20;

    private final DataRepository repository;
    private final MutableLiveData<List<Expense>> expensesLiveData = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<ExpenseSummary> summaryLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> emptyLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<Boolean> lastPageLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();

    private final List<Expense> accumulatedExpenses = new ArrayList<>();
    private int currentStart = 0;
    private Integer currentBranchId = null;
    private Integer currentCategoryId = null;
    private String currentStatus = null;

    public ExpensesViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<List<Expense>> getExpenses() { return expensesLiveData; }
    public LiveData<ExpenseSummary> getSummary() { return summaryLiveData; }
    public LiveData<Boolean> getLoading() { return loadingLiveData; }
    public LiveData<Boolean> getEmpty() { return emptyLiveData; }
    public LiveData<Boolean> getLastPage() { return lastPageLiveData; }
    public LiveData<String> getError() { return errorLiveData; }

    public void fetchExpenses() {
        refreshExpenses();
        fetchSummary();
    }

    public void fetchSummary() {
        repository.getExpenseSummary(currentBranchId, null, null, new DataRepository.ApiCallback<ExpenseSummary>() {
            @Override
            public void onSuccess(ExpenseSummary result) {
                summaryLiveData.postValue(result);
            }

            @Override
            public void onError(String message) {
                // Non-fatal
            }
        });
    }

    public void refreshExpenses() {
        currentStart = 0;
        accumulatedExpenses.clear();
        lastPageLiveData.setValue(false);
        loadNextPage();
    }

    public void loadNextPage() {
        if (Boolean.TRUE.equals(loadingLiveData.getValue()) || Boolean.TRUE.equals(lastPageLiveData.getValue())) {
            return;
        }

        loadingLiveData.setValue(true);

        repository.getExpensesPaged(currentStart, PAGE_SIZE, currentBranchId, currentCategoryId, null, null, currentStatus, new DataRepository.ApiCallback<ExpensePagedResponse>() {
            @Override
            public void onSuccess(ExpensePagedResponse result) {
                loadingLiveData.postValue(false);
                List<Expense> newItems = (result != null && result.getData() != null) ? result.getData() : new ArrayList<>();

                accumulatedExpenses.addAll(newItems);
                expensesLiveData.postValue(new ArrayList<>(accumulatedExpenses));

                currentStart += newItems.size();
                int total = (result != null) ? result.getTotal() : 0;

                boolean isLast = newItems.isEmpty() || accumulatedExpenses.size() >= total || newItems.size() < PAGE_SIZE;
                lastPageLiveData.postValue(isLast);

                emptyLiveData.postValue(accumulatedExpenses.isEmpty());
            }

            @Override
            public void onError(String message) {
                loadingLiveData.postValue(false);
                errorLiveData.postValue(message);
                emptyLiveData.postValue(accumulatedExpenses.isEmpty());
            }
        });
    }

    public void setFilter(Integer branchId, Integer categoryId, String status) {
        this.currentBranchId = branchId;
        this.currentCategoryId = categoryId;
        this.currentStatus = status;
        refreshExpenses();
        fetchSummary();
    }
}
