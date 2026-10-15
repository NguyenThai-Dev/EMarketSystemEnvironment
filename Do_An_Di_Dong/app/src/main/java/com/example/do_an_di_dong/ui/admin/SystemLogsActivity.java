package com.example.do_an_di_dong.ui.admin;

import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.databinding.ActivitySystemLogsBinding;
import com.example.do_an_di_dong.model.AppLog;
import com.example.do_an_di_dong.model.AuditLog;
import com.google.android.material.dialog.MaterialAlertDialogBuilder;
import com.google.android.material.tabs.TabLayout;

import java.util.List;

public class SystemLogsActivity extends AppCompatActivity {
    private ActivitySystemLogsBinding binding;
    private SystemLogsViewModel viewModel;

    private AuditLogAdapter auditLogAdapter;
    private AppLogAdapter appLogAdapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivitySystemLogsBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(SystemLogsViewModel.class);

        setupUI();
        setupObservers();

        viewModel.loadAllData();
    }

    private void setupUI() {
        binding.btnBack.setOnClickListener(v -> finish());
        binding.btnRefresh.setOnClickListener(v -> viewModel.loadAllData());

        // Adapters
        auditLogAdapter = new AuditLogAdapter();
        auditLogAdapter.setOnItemClickListener(this::showAuditLogDetail);

        appLogAdapter = new AppLogAdapter();
        appLogAdapter.setOnItemClickListener(this::showAppLogDetail);

        binding.rvLogs.setLayoutManager(new LinearLayoutManager(this));
        binding.rvLogs.setAdapter(auditLogAdapter); // Default Tab 0

        // Setup TabLayout for 2 Log Streams
        TabLayout.Tab tabAudit = binding.tabLayout.newTab().setText("Kiểm toán CRUD");
        TabLayout.Tab tabApp = binding.tabLayout.newTab().setText("Lỗi & Sự cố");

        binding.tabLayout.addTab(tabAudit);
        binding.tabLayout.addTab(tabApp);

        binding.tabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                viewModel.setSelectedTab(tab.getPosition());
            }

            @Override
            public void onTabUnselected(TabLayout.Tab tab) {}

            @Override
            public void onTabReselected(TabLayout.Tab tab) {}
        });
    }

    private void setupObservers() {
        // Loading State
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                binding.layoutLoading.setVisibility(isLoading ? View.VISIBLE : View.GONE);
                binding.layoutContent.setVisibility(isLoading ? View.GONE : View.VISIBLE);
            }
        });

        // Statistics
        viewModel.getStats().observe(this, stats -> {
            if (stats != null) {
                binding.tvTodayAudit.setText(String.valueOf(stats.getTodayAudit()));
                binding.tvTodayErrors.setText(String.valueOf(stats.getTodayErrors()));
                binding.tvTotalAudit.setText(String.valueOf(stats.getTotalAudit()));
            }
        });

        // Tab Selection Switcher
        viewModel.getSelectedTab().observe(this, tabIndex -> {
            if (tabIndex == 0) {
                binding.rvLogs.setAdapter(auditLogAdapter);
                updateEmptyState(viewModel.getAuditLogs().getValue() == null || viewModel.getAuditLogs().getValue().isEmpty(),
                        "Không tìm thấy nhật ký kiểm toán CRUD nào.");
            } else {
                binding.rvLogs.setAdapter(appLogAdapter);
                updateEmptyState(viewModel.getAppLogs().getValue() == null || viewModel.getAppLogs().getValue().isEmpty(),
                        "Không tìm thấy nhật ký lỗi hệ thống nào.");
            }
        });

        // Audit Logs (CRUD)
        viewModel.getAuditLogs().observe(this, logs -> {
            if (logs != null) {
                auditLogAdapter.setList(logs);
                if (Boolean.TRUE.equals(viewModel.getSelectedTab().getValue() == 0)) {
                    updateEmptyState(logs.isEmpty(), "Không tìm thấy nhật ký kiểm toán CRUD nào.");
                }
            }
        });

        // App Logs (Errors)
        viewModel.getAppLogs().observe(this, logs -> {
            if (logs != null) {
                appLogAdapter.setList(logs);
                if (Boolean.TRUE.equals(viewModel.getSelectedTab().getValue() == 1)) {
                    updateEmptyState(logs.isEmpty(), "Không tìm thấy nhật ký lỗi hệ thống nào.");
                }
            }
        });

        // Errors
        viewModel.getError().observe(this, msg -> {
            if (msg != null && !msg.trim().isEmpty()) {
                Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void updateEmptyState(boolean isEmpty, String subtitleText) {
        if (isEmpty) {
            binding.layoutEmpty.setVisibility(View.VISIBLE);
            binding.rvLogs.setVisibility(View.GONE);
            binding.tvEmptySubtitle.setText(subtitleText);
        } else {
            binding.layoutEmpty.setVisibility(View.GONE);
            binding.rvLogs.setVisibility(View.VISIBLE);
        }
    }

    private void showAuditLogDetail(AuditLog item) {
        StringBuilder sb = new StringBuilder();
        sb.append("Bảng dữ liệu: ").append(item.getTableName()).append("\n");
        sb.append("Mã bản ghi (ID): ").append(item.getPrimaryKeyId() != null ? item.getPrimaryKeyId() : "N/A").append("\n");
        sb.append("Hành động: ").append(item.getFormattedAction()).append("\n");
        sb.append("Thực hiện bởi: ").append(item.getUsername() != null ? item.getUsername() : "Hệ thống").append("\n");
        sb.append("Địa chỉ IP: ").append(item.getIpAddress() != null ? item.getIpAddress() : "N/A").append("\n");
        sb.append("Thời gian: ").append(item.getCreatedAt() != null ? item.getCreatedAt() : "N/A").append("\n\n");

        if (item.getOldValues() != null && !item.getOldValues().trim().isEmpty()) {
            sb.append("--- Giá trị cũ (Old Values) ---\n").append(item.getOldValues()).append("\n\n");
        }
        if (item.getNewValues() != null && !item.getNewValues().trim().isEmpty()) {
            sb.append("--- Giá trị mới (New Values) ---\n").append(item.getNewValues());
        }

        new MaterialAlertDialogBuilder(this)
                .setTitle("Chi tiết Nhật ký Kiểm toán #" + item.getAuditId())
                .setMessage(sb.toString())
                .setPositiveButton("Đóng", (dialog, which) -> dialog.dismiss())
                .show();
    }

    private void showAppLogDetail(AppLog item) {
        StringBuilder sb = new StringBuilder();
        sb.append("Cấp độ: ").append(item.getLogLevel()).append("\n");
        sb.append("Module/Logger: ").append(item.getLogger() != null ? item.getLogger() : "N/A").append("\n");
        sb.append("Luồng xử lý: ").append(item.getThread() != null ? item.getThread() : "Main").append("\n");
        sb.append("Thời gian: ").append(item.getCreatedAt() != null ? item.getCreatedAt() : "N/A").append("\n\n");
        sb.append("--- Thông điệp ---\n").append(item.getMessage() != null ? item.getMessage() : "—").append("\n\n");

        if (item.getException() != null && !item.getException().trim().isEmpty()) {
            sb.append("--- Stack Trace / Exception ---\n").append(item.getException());
        }

        new MaterialAlertDialogBuilder(this)
                .setTitle("Chi tiết Nhật ký Sự cố #" + item.getLogId())
                .setMessage(sb.toString())
                .setPositiveButton("Đóng", (dialog, which) -> dialog.dismiss())
                .show();
    }
}
