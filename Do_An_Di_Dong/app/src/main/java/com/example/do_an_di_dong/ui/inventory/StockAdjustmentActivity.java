package com.example.do_an_di_dong.ui.inventory;

import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.databinding.ActivityStockAdjustmentBinding;
import com.example.do_an_di_dong.model.StockAdjustmentRequest;
import com.example.do_an_di_dong.model.Warehouse;
import com.example.do_an_di_dong.util.UserSessionManager;

import java.util.ArrayList;
import java.util.List;

public class StockAdjustmentActivity extends AppCompatActivity {
    private ActivityStockAdjustmentBinding binding;
    private StockAdjustmentViewModel viewModel;
    private int selectedWarehouseId = 1;
    private final List<Warehouse> availableWarehouses = new ArrayList<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityStockAdjustmentBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(StockAdjustmentViewModel.class);

        int initialProductId = getIntent().getIntExtra("product_id", 0);
        if (initialProductId > 0) {
            binding.etProductId.setText(String.valueOf(initialProductId));
        }

        // Warehouse Dropdown Setup from API / Cache
        setupWarehouseDropdown();

        // Vietnamese-friendly text watcher on reason
        binding.etReason.addTextChangedListener(new TextWatcher() {
            @Override public void beforeTextChanged(CharSequence s, int start, int count, int after) {}
            @Override public void onTextChanged(CharSequence s, int start, int before, int count) {}
            @Override public void afterTextChanged(Editable s) {
                if (binding.tilReason.getError() != null) {
                    binding.tilReason.setError(null);
                }
            }
        });

        binding.btnBack.setOnClickListener(v -> finish());
        binding.btnSubmitAdjustment.setOnClickListener(v -> handleSubmit());

        // Observers
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                binding.layoutProgress.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            }
        });

        viewModel.getSuccess().observe(this, success -> {
            if (Boolean.TRUE.equals(success)) {
                Toast.makeText(this, "Điều chỉnh tồn kho thành công!", Toast.LENGTH_SHORT).show();
                finish();
            }
        });

        viewModel.getError().observe(this, msg -> {
            if (msg != null) {
                Toast.makeText(this, "Lỗi: " + msg, Toast.LENGTH_LONG).show();
            }
        });
    }

    private void setupWarehouseDropdown() {
        DataRepository repository = new DataRepository(this);
        int currentBranchId = UserSessionManager.getCurrentBranchId(this);

        repository.getFilteredWarehouses(null, currentBranchId, new DataRepository.ApiCallback<List<Warehouse>>() {
            @Override
            public void onSuccess(List<Warehouse> result) {
                if (result == null || result.isEmpty()) return;
                availableWarehouses.clear();
                availableWarehouses.addAll(result);

                String[] warehouseNames = new String[result.size()];
                int defaultSelectionIndex = 0;

                for (int i = 0; i < result.size(); i++) {
                    Warehouse w = result.get(i);
                    warehouseNames[i] = w.getName();
                    if (w.getWarehouseId() == selectedWarehouseId) {
                        defaultSelectionIndex = i;
                    }
                }

                ArrayAdapter<String> warehouseAdapter = new ArrayAdapter<>(
                        StockAdjustmentActivity.this, android.R.layout.simple_dropdown_item_1line, warehouseNames);
                binding.actvWarehouse.setAdapter(warehouseAdapter);
                binding.actvWarehouse.setText(warehouseNames[defaultSelectionIndex], false);
                selectedWarehouseId = result.get(defaultSelectionIndex).getWarehouseId();

                binding.actvWarehouse.setOnItemClickListener((parent, v, position, id) -> {
                    if (position >= 0 && position < availableWarehouses.size()) {
                        selectedWarehouseId = availableWarehouses.get(position).getWarehouseId();
                    }
                });
            }

            @Override
            public void onError(String message) {
                // Fallback handled by repository cache
            }
        });
    }

    private void handleSubmit() {
        String productIdStr = binding.etProductId.getText().toString().trim();
        String qtyStr = binding.etAdjustedQuantity.getText().toString().trim();
        String reason = binding.etReason.getText().toString().trim();

        int productId;
        try {
            productId = Integer.parseInt(productIdStr);
            if (productId <= 0) throw new NumberFormatException();
            if (binding.tilProductId.getError() != null) binding.tilProductId.setError(null);
        } catch (Exception e) {
            binding.tilProductId.setError("Mã sản phẩm không hợp lệ");
            return;
        }

        int warehouseId = selectedWarehouseId > 0 ? selectedWarehouseId : 1;

        int adjustedQty;
        try {
            adjustedQty = Integer.parseInt(qtyStr);
            if (binding.tilAdjustedQuantity.getError() != null) binding.tilAdjustedQuantity.setError(null);
        } catch (Exception e) {
            binding.tilAdjustedQuantity.setError("Vui lòng nhập số lượng hợp lệ");
            return;
        }

        if (reason.isEmpty()) {
            binding.tilReason.setError("Vui lòng nhập lý do điều chỉnh");
            return;
        } else if (binding.tilReason.getError() != null) {
            binding.tilReason.setError(null);
        }

        final StockAdjustmentRequest request = new StockAdjustmentRequest(productId, warehouseId, adjustedQty, reason);

        new AlertDialog.Builder(this)
                .setTitle("Xác nhận điều chỉnh kho")
                .setMessage("Điều chỉnh sản phẩm #" + productId + " tại kho #" + warehouseId + " với số lượng " + (adjustedQty > 0 ? "+" + adjustedQty : String.valueOf(adjustedQty)) + "?")
                .setPositiveButton("Xác nhận", (dialog, which) -> {
                    viewModel.adjustStock(request);
                })
                .setNegativeButton("Hủy", null)
                .show();
    }
}
