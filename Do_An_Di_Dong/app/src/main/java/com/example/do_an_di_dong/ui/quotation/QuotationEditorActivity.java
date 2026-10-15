package com.example.do_an_di_dong.ui.quotation;

import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.databinding.ActivityQuotationEditorBinding;
import com.example.do_an_di_dong.databinding.ItemQuotationEditorRowBinding;
import com.example.do_an_di_dong.model.Branch;
import com.example.do_an_di_dong.model.QuotationDetailRequest;
import com.example.do_an_di_dong.model.QuotationSaveRequest;
import com.example.do_an_di_dong.util.MoneyTextWatcher;
import com.example.do_an_di_dong.util.UserSessionManager;

import java.util.ArrayList;
import java.util.List;

public class QuotationEditorActivity extends AppCompatActivity {
    private ActivityQuotationEditorBinding binding;
    private QuotationEditorViewModel viewModel;
    private final List<ItemQuotationEditorRowBinding> rowBindings = new ArrayList<>();
    private int selectedBranchId = 1;
    private final List<Branch> availableBranches = new ArrayList<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityQuotationEditorBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(QuotationEditorViewModel.class);

        binding.btnBack.setOnClickListener(v -> finish());
        binding.btnAddItem.setOnClickListener(v -> addNewRow(0, 1, 0));

        // Load branches dynamically from API / Session cache
        setupBranchDropdown();

        // Customer name text watcher for Vietnamese input without layout destruction
        binding.etCustomerName.addTextChangedListener(new TextWatcher() {
            @Override public void beforeTextChanged(CharSequence s, int start, int count, int after) {}
            @Override public void onTextChanged(CharSequence s, int start, int before, int count) {}
            @Override public void afterTextChanged(Editable s) {
                if (binding.tilCustomerName.getError() != null) {
                    binding.tilCustomerName.setError(null);
                }
            }
        });

        // Add 1 default row
        addNewRow(0, 1, 0);

        binding.btnSave.setOnClickListener(v -> handleSaveQuotation());

        // Observers
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                binding.layoutProgress.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            }
        });

        viewModel.getSaveSuccess().observe(this, success -> {
            if (Boolean.TRUE.equals(success)) {
                Toast.makeText(this, "Tạo báo giá thành công!", Toast.LENGTH_LONG).show();
                setResult(RESULT_OK);
                finish();
            }
        });

        viewModel.getError().observe(this, msg -> {
            if (msg != null) {
                Toast.makeText(this, "Lỗi: " + msg, Toast.LENGTH_LONG).show();
            }
        });
    }

    private void setupBranchDropdown() {
        DataRepository repository = new DataRepository(this);
        selectedBranchId = UserSessionManager.getCurrentBranchId(this);

        repository.getBranches(new DataRepository.ApiCallback<List<Branch>>() {
            @Override
            public void onSuccess(List<Branch> result) {
                if (result == null || result.isEmpty()) return;
                availableBranches.clear();
                availableBranches.addAll(result);

                String[] branchNames = new String[result.size()];
                int defaultSelectionIndex = 0;

                for (int i = 0; i < result.size(); i++) {
                    Branch b = result.get(i);
                    branchNames[i] = b.getName();
                    if (b.getBranchId() == selectedBranchId) {
                        defaultSelectionIndex = i;
                    }
                }

                ArrayAdapter<String> branchAdapter = new ArrayAdapter<>(
                        QuotationEditorActivity.this, android.R.layout.simple_dropdown_item_1line, branchNames);
                binding.actvBranch.setAdapter(branchAdapter);
                binding.actvBranch.setText(branchNames[defaultSelectionIndex], false);
                selectedBranchId = result.get(defaultSelectionIndex).getBranchId();

                binding.actvBranch.setOnItemClickListener((parent, v, position, id) -> {
                    if (position >= 0 && position < availableBranches.size()) {
                        selectedBranchId = availableBranches.get(position).getBranchId();
                    }
                });
            }

            @Override
            public void onError(String message) {
                // Fallback handled by repository cache
            }
        });
    }

    private void addNewRow(int defaultProductId, int defaultQuantity, double defaultPrice) {
        ItemQuotationEditorRowBinding rowBinding = ItemQuotationEditorRowBinding.inflate(
                LayoutInflater.from(this), binding.layoutItemsContainer, false);

        if (defaultProductId > 0) rowBinding.etProductId.setText(String.valueOf(defaultProductId));
        rowBinding.etQuantity.setText(String.valueOf(defaultQuantity));
        if (defaultPrice > 0) rowBinding.etUnitPrice.setText(String.valueOf((long) defaultPrice));

        TextWatcher calculationWatcher = new TextWatcher() {
            @Override public void beforeTextChanged(CharSequence s, int start, int count, int after) {}
            @Override public void onTextChanged(CharSequence s, int start, int before, int count) {
                recalculateTotal();
            }
            @Override public void afterTextChanged(Editable s) {}
        };

        rowBinding.etQuantity.addTextChangedListener(calculationWatcher);
        rowBinding.etUnitPrice.addTextChangedListener(calculationWatcher);

        rowBinding.btnRemoveRow.setOnClickListener(v -> {
            if (rowBindings.size() > 1) {
                binding.layoutItemsContainer.removeView(rowBinding.getRoot());
                rowBindings.remove(rowBinding);
                recalculateTotal();
            } else {
                Toast.makeText(this, "Báo giá phải có ít nhất một mặt hàng.", Toast.LENGTH_SHORT).show();
            }
        });

        rowBindings.add(rowBinding);
        binding.layoutItemsContainer.addView(rowBinding.getRoot());
        recalculateTotal();
    }

    private double recalculateTotal() {
        double grandTotal = 0;
        for (ItemQuotationEditorRowBinding row : rowBindings) {
            int qty = parseSafeInt(row.etQuantity);
            double price = parseSafeDouble(row.etUnitPrice);
            grandTotal += (qty * price);
        }
        binding.tvEstimatedTotal.setText(MoneyTextWatcher.formatCurrency(grandTotal));
        return grandTotal;
    }

    private int parseSafeInt(EditText et) {
        try {
            return Integer.parseInt(et.getText().toString().trim());
        } catch (Exception e) {
            return 0;
        }
    }

    private double parseSafeDouble(EditText et) {
        try {
            return Double.parseDouble(et.getText().toString().trim());
        } catch (Exception e) {
            return 0.0;
        }
    }

    private void handleSaveQuotation() {
        String customer = binding.etCustomerName.getText().toString().trim();
        int branchId = selectedBranchId > 0 ? selectedBranchId : UserSessionManager.getCurrentBranchId(this);

        if (customer.isEmpty()) {
            binding.tilCustomerName.setError("Vui lòng nhập tên khách hàng");
            return;
        } else if (binding.tilCustomerName.getError() != null) {
            binding.tilCustomerName.setError(null);
        }

        List<QuotationDetailRequest> details = new ArrayList<>();
        for (ItemQuotationEditorRowBinding row : rowBindings) {
            int productId = parseSafeInt(row.etProductId);
            int qty = parseSafeInt(row.etQuantity);
            double price = parseSafeDouble(row.etUnitPrice);

            if (productId <= 0) {
                Toast.makeText(this, "Vui lòng nhập ID sản phẩm hợp lệ cho mọi dòng.", Toast.LENGTH_SHORT).show();
                return;
            }
            if (qty <= 0) {
                Toast.makeText(this, "Số lượng phải lớn hơn 0.", Toast.LENGTH_SHORT).show();
                return;
            }
            details.add(new QuotationDetailRequest(productId, qty, price));
        }

        if (details.isEmpty()) {
            Toast.makeText(this, "Vui lòng thêm ít nhất một mặt hàng.", Toast.LENGTH_SHORT).show();
            return;
        }

        double total = recalculateTotal();
        final QuotationSaveRequest request = new QuotationSaveRequest(0, customer, branchId, total, details);

        // Safe Confirmation Dialog before sending mutation request
        new AlertDialog.Builder(this)
                .setTitle("Xác nhận tạo báo giá")
                .setMessage("Tạo báo giá cho khách hàng \"" + customer + "\" với tổng giá trị " + binding.tvEstimatedTotal.getText() + "?")
                .setPositiveButton("Xác nhận gửi", (dialog, which) -> {
                    viewModel.saveQuotation(request);
                })
                .setNegativeButton("Xem lại", null)
                .show();
    }
}
