package com.example.do_an_di_dong.ui.admin;

import android.app.Application;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.model.AppLog;
import com.example.do_an_di_dong.model.AppLogResponse;
import com.example.do_an_di_dong.model.AuditLog;
import com.example.do_an_di_dong.model.AuditLogResponse;
import com.example.do_an_di_dong.model.LogStatistics;

import java.util.List;

public class SystemLogsViewModel extends AndroidViewModel {
    private final DataRepository repository;
    private final MutableLiveData<LogStatistics> statsLiveData = new MutableLiveData<>();
    private final MutableLiveData<List<AuditLog>> auditLogsLiveData = new MutableLiveData<>();
    private final MutableLiveData<List<AppLog>> appLogsLiveData = new MutableLiveData<>();
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>();
    private final MutableLiveData<Integer> selectedTabLiveData = new MutableLiveData<>(0); // 0 = Audit Logs, 1 = App Logs

    public SystemLogsViewModel(@NonNull Application application) {
        super(application);
        this.repository = new DataRepository(application);
    }

    public LiveData<LogStatistics> getStats() {
        return statsLiveData;
    }

    public LiveData<List<AuditLog>> getAuditLogs() {
        return auditLogsLiveData;
    }

    public LiveData<List<AppLog>> getAppLogs() {
        return appLogsLiveData;
    }

    public LiveData<String> getError() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoading() {
        return loadingLiveData;
    }

    public LiveData<Integer> getSelectedTab() {
        return selectedTabLiveData;
    }

    public void setSelectedTab(int tabIndex) {
        selectedTabLiveData.setValue(tabIndex);
    }

    public void loadAllData() {
        loadingLiveData.postValue(true);
        loadStatistics();
        loadAuditLogs(0, 50, null);
        loadAppLogs(0, 50, null);
    }

    public void loadStatistics() {
        repository.getLogStatistics(new DataRepository.ApiCallback<LogStatistics>() {
            @Override
            public void onSuccess(LogStatistics result) {
                statsLiveData.postValue(result);
                checkLoadingComplete();
            }

            @Override
            public void onError(String message) {
                errorLiveData.postValue("Lỗi lấy thống kê: " + message);
                checkLoadingComplete();
            }
        });
    }

    public void loadAuditLogs(int start, int length, String search) {
        repository.getAuditLogs(start, length, search, new DataRepository.ApiCallback<AuditLogResponse>() {
            @Override
            public void onSuccess(AuditLogResponse result) {
                if (result != null && result.getData() != null) {
                    auditLogsLiveData.postValue(result.getData());
                }
                checkLoadingComplete();
            }

            @Override
            public void onError(String message) {
                errorLiveData.postValue("Lỗi lấy nhật ký kiểm toán: " + message);
                checkLoadingComplete();
            }
        });
    }

    public void loadAppLogs(int start, int length, String logLevel) {
        repository.getAppLogs(start, length, logLevel, new DataRepository.ApiCallback<AppLogResponse>() {
            @Override
            public void onSuccess(AppLogResponse result) {
                if (result != null && result.getData() != null) {
                    appLogsLiveData.postValue(result.getData());
                }
                checkLoadingComplete();
            }

            @Override
            public void onError(String message) {
                errorLiveData.postValue("Lỗi lấy nhật ký hệ thống lỗi: " + message);
                checkLoadingComplete();
            }
        });
    }

    private void checkLoadingComplete() {
        loadingLiveData.postValue(false);
    }
}
