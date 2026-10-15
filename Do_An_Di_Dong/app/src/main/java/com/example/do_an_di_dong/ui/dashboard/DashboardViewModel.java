package com.example.do_an_di_dong.ui.dashboard;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.DashboardOverview;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Locale;

public class DashboardViewModel extends AndroidViewModel {
    private final DataRepository repository;
    private final MutableLiveData<DashboardOverview> dashboardData = new MutableLiveData<>();
    private final MutableLiveData<String> error = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loading = new MutableLiveData<>();

    public DashboardViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<DashboardOverview> getDashboardData() {
        return dashboardData;
    }

    public LiveData<String> getError() {
        return error;
    }

    public LiveData<Boolean> getLoading() {
        return loading;
    }

    public void fetchDashboardSummary() {
        loading.setValue(true);

        // Get past 30 days range
        SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault());
        Calendar cal = Calendar.getInstance();
        String toDate = sdf.format(cal.getTime());
        cal.add(Calendar.DAY_OF_YEAR, -30);
        String fromDate = sdf.format(cal.getTime());

        repository.getDashboardSummary(fromDate, toDate, "day", new DataRepository.ApiCallback<DashboardOverview>() {
            @Override
            public void onSuccess(DashboardOverview result) {
                loading.postValue(false);
                dashboardData.postValue(result);
            }

            @Override
            public void onError(String message) {
                loading.postValue(false);
                error.postValue(message);
            }
        });
    }
}
