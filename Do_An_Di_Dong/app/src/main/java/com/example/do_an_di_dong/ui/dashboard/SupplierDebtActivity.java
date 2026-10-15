package com.example.do_an_di_dong.ui.dashboard;

import android.app.DatePickerDialog;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.databinding.ActivitySupplierDebtBinding;
import com.example.do_an_di_dong.databinding.DialogSupplierDebtEditBinding;
import com.example.do_an_di_dong.databinding.DialogSupplierPaymentBinding;
import com.example.do_an_di_dong.model.DebtSummary;
import com.example.do_an_di_dong.model.SupplierDebt;
import com.example.do_an_di_dong.model.SupplierPayment;
import com.example.do_an_di_dong.util.MoneyTextWatcher;
import com.google.android.material.snackbar.Snackbar;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.List;
import java.util.Locale;

public class SupplierDebtActivity extends AppCompatActivity implements SupplierDebtAdapter.OnDebtActionListener {

    private ActivitySupplierDebtBinding binding;
    private SupplierDebtViewModel viewModel;
    private SupplierDebtAdapter adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivitySupplierDebtBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(SupplierDebtViewModel.class);

        setupViews();
        setupObservers();

        viewModel.fetchDebts();
    }

    private void setupViews() {
        binding.btnBack.setOnClickListener(v -> finish());
        binding.btnRefresh.setOnClickListener(v -> viewModel.fetchDebts());

        binding.rvDebts.setLayoutManager(new LinearLayoutManager(this));
        adapter = new SupplierDebtAdapter(this);
        binding.rvDebts.setAdapter(adapter);

        binding.swipeRefreshDebts.setOnRefreshListener(() -> viewModel.fetchDebts());

        // Search text watcher with debounce for Vietnamese typing
        final android.os.Handler searchHandler = new android.os.Handler(android.os.Looper.getMainLooper());
        final Runnable[] searchRunnable = new Runnable[1];

        binding.etSearchDebt.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                if (searchRunnable[0] != null) {
                    searchHandler.removeCallbacks(searchRunnable[0]);
                }
                searchRunnable[0] = () -> viewModel.setSearchKeyword(s.toString().trim());
                searchHandler.postDelayed(searchRunnable[0], 350);
            }

            @Override
            public void afterTextChanged(Editable s) {}
        });

        // Chip group filter
        binding.chipGroupStatus.setOnCheckedStateChangeListener((group, checkedIds) -> {
            if (checkedIds.isEmpty()) return;
            int id = checkedIds.get(0);
            if (id == R.id.chipUnpaid) {
                viewModel.setFilters(null, "Unpaid");
            } else if (id == R.id.chipPartial) {
                viewModel.setFilters(null, "Partial");
            } else if (id == R.id.chipPaid) {
                viewModel.setFilters(null, "Paid");
            } else {
                viewModel.setFilters(null, null);
            }
        });
    }

    private void setupObservers() {
        viewModel.getDebts().observe(this, debts -> {
            binding.swipeRefreshDebts.setRefreshing(false);
            binding.pbLoading.setVisibility(View.GONE);
            adapter.setList(debts);
        });

        viewModel.getSummary().observe(this, this::updateSummaryWidgets);

        viewModel.getLoading().observe(this, loading -> {
            if (Boolean.TRUE.equals(loading)) {
                if (!binding.swipeRefreshDebts.isRefreshing()) {
                    binding.pbLoading.setVisibility(View.VISIBLE);
                }
            } else {
                binding.pbLoading.setVisibility(View.GONE);
                binding.swipeRefreshDebts.setRefreshing(false);
            }
        });

        viewModel.getEmpty().observe(this, empty -> {
            binding.layoutEmptyState.setVisibility(Boolean.TRUE.equals(empty) ? View.VISIBLE : View.GONE);
        });

        viewModel.getError().observe(this, error -> {
            if (error != null && !error.isEmpty()) {
                Snackbar.make(binding.getRoot(), error, Snackbar.LENGTH_LONG)
                        .setAction("Thử lại", v -> viewModel.fetchDebts())
                        .show();
            }
        });
    }

    private void updateSummaryWidgets(DebtSummary summary) {
        if (summary == null) return;
        binding.tvWidgetTotalDebt.setText(MoneyTextWatcher.formatCurrency(summary.getTotalDebt()));
        binding.tvWidgetTotalPaid.setText(MoneyTextWatcher.formatCurrency(summary.getTotalPaid()));
        binding.tvWidgetRemaining.setText(MoneyTextWatcher.formatCurrency(summary.getTotalRemaining()));
        binding.tvWidgetPayRate.setText(summary.getPayRate() + "%");
    }

    // =========================================================================
    // SUPPLIER DEBT ACTIONS (PAY, EDIT, DELETE, HISTORY)
    // =========================================================================

    @Override
    public void onPayDebt(SupplierDebt debt) {
        if (debt == null) return;

        DialogSupplierPaymentBinding dialogBinding = DialogSupplierPaymentBinding.inflate(LayoutInflater.from(this));
        AlertDialog dialog = new AlertDialog.Builder(this)
                .setView(dialogBinding.getRoot())
                .setCancelable(false)
                .create();

        dialogBinding.tvDialogSupplierName.setText("NCC: " + (debt.getSupplierName() != null ? debt.getSupplierName() : ""));
        double remaining = (debt.getUnpaidAmount() > 0) ? debt.getUnpaidAmount() : Math.max(0, debt.getTotalDebt() - debt.getPaidAmount());
        dialogBinding.tvDialogRemainingDebt.setText(MoneyTextWatcher.formatCurrency(remaining));

        dialogBinding.etPaymentAmount.addTextChangedListener(new MoneyTextWatcher(dialogBinding.etPaymentAmount));

        // Full Pay Hint Button
        dialogBinding.btnFullPay.setOnClickListener(v -> {
            dialogBinding.etPaymentAmount.setText(String.valueOf((long) remaining));
        });

        dialogBinding.btnCancelPayment.setOnClickListener(v -> dialog.dismiss());

        dialogBinding.btnConfirmPayment.setOnClickListener(v -> {
            double amount = MoneyTextWatcher.parseAmount(dialogBinding.etPaymentAmount);
            if (amount <= 0) {
                dialogBinding.tilPaymentAmount.setError("Vui lòng nhập số tiền thanh toán hợp lệ (> 0)");
                return;
            }
            dialogBinding.tilPaymentAmount.setError(null);

            String method = dialogBinding.rbCash.isChecked() ? "Cash" : "BankTransfer";
            String note = dialogBinding.etPaymentNote.getText() != null ? dialogBinding.etPaymentNote.getText().toString().trim() : "";

            SupplierPayment payment = new SupplierPayment();
            payment.setDebtId(debt.getDebtId());
            payment.setAmount(amount);
            payment.setPaymentMethod(method);
            payment.setPaymentProof(note);

            dialogBinding.btnConfirmPayment.setEnabled(false);
            dialogBinding.btnConfirmPayment.setText("Đang xử lý...");

            viewModel.createSupplierPayment(payment, new DataRepository.ApiCallback<Boolean>() {
                @Override
                public void onSuccess(Boolean result) {
                    dialog.dismiss();
                    Toast.makeText(SupplierDebtActivity.this, "Đã ghi nhận thanh toán thành công!", Toast.LENGTH_SHORT).show();
                }

                @Override
                public void onError(String message) {
                    dialogBinding.btnConfirmPayment.setEnabled(true);
                    dialogBinding.btnConfirmPayment.setText("Xác nhận trả nợ");
                    dialogBinding.tilPaymentAmount.setError("Thất bại: " + message);
                }
            });
        });

        dialog.show();
    }

    @Override
    public void onEditDueDate(SupplierDebt debt) {
        if (debt == null) return;

        DialogSupplierDebtEditBinding editBinding = DialogSupplierDebtEditBinding.inflate(LayoutInflater.from(this));
        AlertDialog dialog = new AlertDialog.Builder(this)
                .setView(editBinding.getRoot())
                .create();

        editBinding.tvEditDialogSupplier.setText("NCC: " + (debt.getSupplierName() != null ? debt.getSupplierName() : ""));
        editBinding.etEditDueDate.setText(debt.getDueDate() != null ? debt.getDueDate() : "");

        Calendar calendar = Calendar.getInstance();
        editBinding.etEditDueDate.setOnClickListener(v -> {
            DatePickerDialog dpd = new DatePickerDialog(this, (view, year, month, dayOfMonth) -> {
                calendar.set(year, month, dayOfMonth);
                SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault());
                editBinding.etEditDueDate.setText(sdf.format(calendar.getTime()));
            }, calendar.get(Calendar.YEAR), calendar.get(Calendar.MONTH), calendar.get(Calendar.DAY_OF_MONTH));
            dpd.show();
        });

        editBinding.btnCancelEdit.setOnClickListener(v -> dialog.dismiss());

        editBinding.btnConfirmEdit.setOnClickListener(v -> {
            String newDueDate = editBinding.etEditDueDate.getText() != null ? editBinding.etEditDueDate.getText().toString().trim() : "";
            if (newDueDate.isEmpty()) {
                editBinding.tilEditDueDate.setError("Vui lòng chọn ngày đáo hạn!");
                return;
            }

            debt.setDueDate(newDueDate);
            editBinding.btnConfirmEdit.setEnabled(false);
            editBinding.btnConfirmEdit.setText("Đang lưu...");

            viewModel.updateSupplierDebt(debt, new DataRepository.ApiCallback<Boolean>() {
                @Override
                public void onSuccess(Boolean result) {
                    dialog.dismiss();
                    Toast.makeText(SupplierDebtActivity.this, "Cập nhật hạn nợ thành công!", Toast.LENGTH_SHORT).show();
                }

                @Override
                public void onError(String message) {
                    editBinding.btnConfirmEdit.setEnabled(true);
                    editBinding.btnConfirmEdit.setText("Lưu thay đổi");
                    editBinding.tilEditDueDate.setError("Lỗi: " + message);
                }
            });
        });

        dialog.show();
    }

    @Override
    public void onLoadHistory(SupplierDebt debt, SupplierDebtAdapter.DebtHistoryCallback callback) {
        if (debt == null) return;
        viewModel.getPaymentsByDebtId(debt.getDebtId(), new DataRepository.ApiCallback<List<SupplierPayment>>() {
            @Override
            public void onSuccess(List<SupplierPayment> result) {
                callback.onHistoryLoaded(result);
            }

            @Override
            public void onError(String message) {
                callback.onHistoryError(message);
            }
        });
    }

    @Override
    public void onDeletePayment(SupplierPayment payment, SupplierDebt parentDebt) {
        if (payment == null) return;

        new AlertDialog.Builder(this)
                .setTitle("Xác nhận xóa phiếu thanh toán")
                .setMessage("Bạn có chắc chắn muốn xóa phiếu thanh toán số tiền "
                        + MoneyTextWatcher.formatCurrency(payment.getAmount())
                        + "? Số dư nợ sẽ được tính toán lại tự động.")
                .setPositiveButton("Xóa", (d, which) -> {
                    viewModel.deleteSupplierPayment(payment.getPaymentId(), new DataRepository.ApiCallback<Boolean>() {
                        @Override
                        public void onSuccess(Boolean result) {
                            Toast.makeText(SupplierDebtActivity.this, "Đã xóa phiếu thanh toán và cập nhật lại công nợ!", Toast.LENGTH_SHORT).show();
                        }

                        @Override
                        public void onError(String message) {
                            Toast.makeText(SupplierDebtActivity.this, "Xóa thất bại: " + message, Toast.LENGTH_LONG).show();
                        }
                    });
                })
                .setNegativeButton("Hủy", null)
                .show();
    }
}
