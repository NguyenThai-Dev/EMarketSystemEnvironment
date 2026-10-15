package com.example.do_an_di_dong.ui.dashboard;

import android.app.AlertDialog;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.graphics.Color;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;

import com.example.do_an_di_dong.MainActivity;
import com.example.do_an_di_dong.databinding.FragmentDashboardBinding;
import com.example.do_an_di_dong.model.DashboardOverview;
import com.example.do_an_di_dong.model.DashboardTrend;
import com.example.do_an_di_dong.ui.admin.CustomersActivity;
import com.example.do_an_di_dong.ui.admin.ExpensesActivity;
import com.example.do_an_di_dong.ui.admin.SystemLogsActivity;
import com.example.do_an_di_dong.ui.ai.AIAdvisorHubActivity;
import com.example.do_an_di_dong.ui.inventory.PurchaseOrdersActivity;
import com.example.do_an_di_dong.ui.inventory.WarehouseInventoryActivity;
import com.example.do_an_di_dong.ui.quotation.QuotationsActivity;
import com.example.do_an_di_dong.ui.sales.PromotionsActivity;
import com.github.mikephil.charting.components.XAxis;
import com.github.mikephil.charting.data.Entry;
import com.github.mikephil.charting.data.LineData;
import com.github.mikephil.charting.data.LineDataSet;
import com.github.mikephil.charting.formatter.IndexAxisValueFormatter;

import java.text.NumberFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;

public class DashboardFragment extends Fragment {
    private FragmentDashboardBinding binding;
    private DashboardViewModel viewModel;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        binding = FragmentDashboardBinding.inflate(inflater, container, false);
        return binding.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        viewModel = new ViewModelProvider(this).get(DashboardViewModel.class);

        // Display user fullname and role badge
        SharedPreferences prefs = requireContext().getSharedPreferences("BiometricPrefs", Context.MODE_PRIVATE);
        String fullName = prefs.getString("user_fullname", "Sếp");
        String roleName = com.example.do_an_di_dong.util.UserSessionManager.getRoleName(requireContext());
        binding.tvUserWelcome.setText("Chào " + fullName + " (" + roleName + ")!");

        // Display today's date
        SimpleDateFormat sdf = new SimpleDateFormat("EEEE, dd MMMM", new Locale("vi", "VN"));
        binding.tvDate.setText(sdf.format(new Date()));

        // Logout button (header)
        binding.btnLogout.setOnClickListener(v -> {
            new AlertDialog.Builder(requireContext())
                    .setTitle("Đăng xuất")
                    .setMessage("Bạn có chắc chắn muốn đăng xuất không?")
                    .setNegativeButton("Hủy", null)
                    .setPositiveButton("Đăng xuất", (dialog, which) -> {
                        if (getActivity() instanceof MainActivity) {
                            ((MainActivity) getActivity()).signOut();
                        }
                    })
                    .show();
        });

        // Executive Shortcuts with RBAC Protection
        setupRbacShortcuts();

        // Observers
        viewModel.getDashboardData().observe(getViewLifecycleOwner(), this::updateUi);
        viewModel.getError().observe(getViewLifecycleOwner(), msg -> {
            if (msg != null) {
                showError(msg);
            }
        });

        binding.btnRetry.setOnClickListener(v -> loadData());

        loadData();
    }

    private void loadData() {
        binding.scrollView.setVisibility(View.GONE);
        binding.layoutError.setVisibility(View.GONE);
        binding.layoutLoading.setVisibility(View.VISIBLE);
        viewModel.fetchDashboardSummary();
    }

    private void showError(String message) {
        binding.layoutLoading.setVisibility(View.GONE);
        binding.scrollView.setVisibility(View.GONE);
        binding.layoutError.setVisibility(View.VISIBLE);

        String friendlyMessage = "Đã xảy ra lỗi không xác định.";
        if (message.contains("ConnectException") || message.contains("timeout") || message.contains("Unable to resolve host")) {
            friendlyMessage = "Không thể kết nối mạng tới server. Vui lòng kiểm tra lại Wifi/Mạng hoặc máy chủ.";
        } else if (message.contains("401") || message.contains("Unauthorized")) {
            friendlyMessage = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
        } else if (message.contains("403")) {
            friendlyMessage = "Bạn không có quyền truy cập dữ liệu này.";
        } else if (message.contains("500")) {
            friendlyMessage = "Lỗi hệ thống từ server backend (500).";
        }
        binding.tvErrorMessage.setText(friendlyMessage);
    }

    private void updateUi(DashboardOverview data) {
        if (data == null) {
            showError("Dữ liệu trả về trống.");
            return;
        }

        binding.layoutLoading.setVisibility(View.GONE);
        binding.layoutError.setVisibility(View.GONE);
        binding.scrollView.setVisibility(View.VISIBLE);

        NumberFormat vnFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));

        double totalSales = 0;
        int totalOrders = 0;
        int lowStock = 0;
        int totalProducts = 0;

        if (data.getSummary() != null) {
            totalSales = data.getSummary().getTotalSales();
            totalOrders = data.getSummary().getTotalOrders();
            lowStock = data.getSummary().getLowStockProducts();
            totalProducts = data.getSummary().getTotalProducts();
        }

        binding.tvRevenue.setText(vnFormat.format(totalSales));
        binding.tvOrders.setText(totalOrders + " đơn");
        binding.tvLowStock.setText(lowStock + " mã");
        binding.tvNewCustomers.setText(totalProducts + " sản phẩm");

        if (data.getTrend() != null) {
            setupChart(data.getTrend());
        }
    }

    private void setupChart(DashboardTrend trend) {
        List<Entry> entries = new ArrayList<>();
        List<Double> sales = trend.getSales();

        if (sales == null || sales.isEmpty()) {
            binding.lineChart.clear();
            return;
        }

        for (int i = 0; i < sales.size(); i++) {
            Double val = sales.get(i);
            entries.add(new Entry(i, val == null ? 0f : val.floatValue()));
        }

        LineDataSet dataSet = new LineDataSet(entries, "Doanh thu");
        dataSet.setColor(Color.parseColor("#4F46E5")); // Indigo primary
        dataSet.setLineWidth(2.5f);
        dataSet.setMode(LineDataSet.Mode.CUBIC_BEZIER);
        dataSet.setCircleColor(Color.parseColor("#4F46E5"));
        dataSet.setCircleRadius(3.5f);
        dataSet.setCircleHoleColor(Color.WHITE);
        dataSet.setCircleHoleRadius(2f);
        dataSet.setDrawValues(false);
        dataSet.setDrawFilled(true);
        dataSet.setFillColor(Color.parseColor("#818CF8")); // Indigo light
        dataSet.setFillAlpha(30);

        LineData lineData = new LineData(dataSet);
        binding.lineChart.setData(lineData);

        binding.lineChart.getDescription().setEnabled(false);
        binding.lineChart.getLegend().setEnabled(false);
        binding.lineChart.setScaleEnabled(false);
        binding.lineChart.setDragEnabled(true);

        XAxis xAxis = binding.lineChart.getXAxis();
        xAxis.setPosition(XAxis.XAxisPosition.BOTTOM);
        xAxis.setGranularity(1f);
        xAxis.setDrawGridLines(false);
        xAxis.setLabelRotationAngle(-25);
        xAxis.setTextColor(Color.parseColor("#6B7280"));
        xAxis.setTextSize(10f);

        List<String> labels = trend.getLabels();
        if (labels != null && !labels.isEmpty()) {
            xAxis.setValueFormatter(new IndexAxisValueFormatter(labels));
        }

        binding.lineChart.getAxisLeft().setDrawGridLines(true);
        binding.lineChart.getAxisLeft().setGridColor(Color.parseColor("#F3F4F6"));
        binding.lineChart.getAxisLeft().setTextColor(Color.parseColor("#6B7280"));
        binding.lineChart.getAxisLeft().setTextSize(10f);
        binding.lineChart.getAxisRight().setEnabled(false);

        binding.lineChart.animateY(600);
        binding.lineChart.invalidate();
    }

    private void setupRbacShortcuts() {
        Context context = getContext();
        if (context == null) return;

        boolean canWarehouse = com.example.do_an_di_dong.util.UserSessionManager.hasPermission(context, "InventoryModule");
        boolean canPO = com.example.do_an_di_dong.util.UserSessionManager.hasPermission(context, "InventoryModule");
        boolean canExpenses = com.example.do_an_di_dong.util.UserSessionManager.hasPermission(context, "ExpenseModule");
        boolean canDebt = com.example.do_an_di_dong.util.UserSessionManager.hasAnyPermission(context, "InventoryModule", "FinanceModule");
        boolean canFinance = com.example.do_an_di_dong.util.UserSessionManager.hasAnyPermission(context, "FinanceModule", "ReportModule")
                || com.example.do_an_di_dong.util.UserSessionManager.isAdmin(context);
        boolean canQuotations = com.example.do_an_di_dong.util.UserSessionManager.hasPermission(context, "QuotationModule");
        boolean canPromotions = com.example.do_an_di_dong.util.UserSessionManager.hasPermission(context, "SalesModule");
        boolean canAI = com.example.do_an_di_dong.util.UserSessionManager.hasAnyPermission(context, "AIModule", "ReportModule");
        boolean canCustomers = com.example.do_an_di_dong.util.UserSessionManager.hasPermission(context, "CustomerModule");
        boolean canLogs = com.example.do_an_di_dong.util.UserSessionManager.isAdmin(context)
                || com.example.do_an_di_dong.util.UserSessionManager.hasPermission(context, "LogModule");

        configureShortcutButton(binding.btnWarehouseReport, canWarehouse, "Kho & Tồn kho", WarehouseInventoryActivity.class);
        configureShortcutButton(binding.btnPurchaseOrdersReport, canPO, "Nhập hàng PO", PurchaseOrdersActivity.class);
        configureShortcutButton(binding.btnExpensesReport, canExpenses, "Chi phí vận hành", ExpensesActivity.class);
        configureShortcutButton(binding.btnDebtReport, canDebt, "Công nợ NCC", SupplierDebtActivity.class);
        configureShortcutButton(binding.btnFinanceReport, canFinance, "Tài chính", FinanceDashboardActivity.class);
        configureShortcutButton(binding.btnQuotationsReport, canQuotations, "Báo giá KH", QuotationsActivity.class);
        configureShortcutButton(binding.btnPromotionsReport, canPromotions, "Khuyến mãi", PromotionsActivity.class);
        configureShortcutButton(binding.btnAIAdvisorReport, canAI, "Cố vấn & AI", AIAdvisorHubActivity.class);
        configureShortcutButton(binding.btnCustomersReport, canCustomers, "Khách hàng CRM", CustomersActivity.class);
        configureShortcutButton(binding.btnSystemLogsReport, canLogs, "Nhật ký hệ thống", SystemLogsActivity.class);
    }

    private void configureShortcutButton(com.google.android.material.button.MaterialButton btn, boolean hasPermission, String title, Class<?> targetActivity) {
        if (btn == null) return;
        if (!hasPermission) {
            btn.setAlpha(0.45f);
            btn.setOnClickListener(v ->
                android.widget.Toast.makeText(getContext(), "Bạn không có quyền truy cập: " + title, android.widget.Toast.LENGTH_SHORT).show()
            );
        } else {
            btn.setAlpha(1.0f);
            btn.setOnClickListener(v -> {
                Intent intent = new Intent(getContext(), targetActivity);
                startActivity(intent);
            });
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}
