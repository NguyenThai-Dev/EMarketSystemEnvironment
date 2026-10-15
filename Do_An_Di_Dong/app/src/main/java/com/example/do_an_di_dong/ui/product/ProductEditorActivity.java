package com.example.do_an_di_dong.ui.product;

import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.example.do_an_di_dong.databinding.ActivityProductEditorBinding;
import com.example.do_an_di_dong.model.ProductDTO;

public class ProductEditorActivity extends AppCompatActivity {
    private ActivityProductEditorBinding binding;
    private ProductEditorViewModel viewModel;
    private int productId = 0;
    private boolean isEditMode = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityProductEditorBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(ProductEditorViewModel.class);

        productId = getIntent().getIntExtra("product_id", 0);
        isEditMode = (productId > 0);

        if (isEditMode) {
            binding.tvHeaderTitle.setText("Chỉnh sửa sản phẩm #" + productId);
        } else {
            binding.tvHeaderTitle.setText("Thêm sản phẩm mới");
        }

        binding.btnBack.setOnClickListener(v -> finish());
        binding.btnSaveProduct.setOnClickListener(v -> handleSave());

        // Observers
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                binding.layoutProgress.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            }
        });

        viewModel.getSaveSuccess().observe(this, success -> {
            if (Boolean.TRUE.equals(success)) {
                Toast.makeText(this, isEditMode ? "Cập nhật sản phẩm thành công!" : "Tạo sản phẩm thành công!", Toast.LENGTH_SHORT).show();
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

    private void handleSave() {
        String name = binding.etName.getText().toString().trim();
        String barcode = binding.etBarcode.getText().toString().trim();
        String unit = binding.etUnit.getText().toString().trim();
        String description = binding.etDescription.getText().toString().trim();
        String priceStr = binding.etPrice.getText().toString().trim();
        String minStockStr = binding.etMinStock.getText().toString().trim();
        String maxStockStr = binding.etMaxStock.getText().toString().trim();

        if (name.isEmpty()) {
            binding.tilName.setError("Vui lòng nhập tên sản phẩm");
            return;
        } else if (binding.tilName.getError() != null) {
            binding.tilName.setError(null);
        }

        if (barcode.isEmpty()) {
            binding.tilBarcode.setError("Vui lòng nhập mã vạch / SKU");
            return;
        } else if (binding.tilBarcode.getError() != null) {
            binding.tilBarcode.setError(null);
        }

        double price;
        try {
            price = Double.parseDouble(priceStr);
            if (price <= 0) throw new NumberFormatException();
            if (binding.tilPrice.getError() != null) binding.tilPrice.setError(null);
        } catch (Exception e) {
            binding.tilPrice.setError("Giá bán phải lớn hơn 0");
            return;
        }

        int minStock = 5;
        try { minStock = Integer.parseInt(minStockStr); } catch (Exception ignored) {}

        int maxStock = 1000;
        try { maxStock = Integer.parseInt(maxStockStr); } catch (Exception ignored) {}

        final ProductDTO dto = new ProductDTO(
                productId, name, barcode, unit, description,
                1, 1, price, minStock, maxStock
        );

        new AlertDialog.Builder(this)
                .setTitle(isEditMode ? "Xác nhận cập nhật" : "Xác nhận thêm sản phẩm")
                .setMessage("Lưu thông tin sản phẩm \"" + name + "\" vào hệ thống?")
                .setPositiveButton("Lưu ngay", (dialog, which) -> {
                    viewModel.saveProduct(dto, isEditMode);
                })
                .setNegativeButton("Xem lại", null)
                .show();
    }
}
