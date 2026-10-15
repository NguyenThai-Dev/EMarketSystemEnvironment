package com.example.do_an_di_dong.ui.product;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.appcompat.app.AppCompatActivity;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.databinding.ActivityProductCreateEditBinding;
import com.example.do_an_di_dong.model.Product;
import com.example.do_an_di_dong.model.ProductCategoryDTO;
import com.example.do_an_di_dong.model.ProductDTO;
import com.example.do_an_di_dong.model.SupplierDTO;
import com.google.android.material.dialog.MaterialAlertDialogBuilder;

import java.util.ArrayList;
import java.util.List;

public class ProductCreateEditActivity extends AppCompatActivity {
    public static final String EXTRA_PRODUCT_ID = "extra_product_id";

    private ActivityProductCreateEditBinding binding;
    private DataRepository repository;
    private int productId = 0;

    private List<ProductCategoryDTO> categoryList = new ArrayList<>();
    private List<SupplierDTO> supplierList = new ArrayList<>();

    private final ActivityResultLauncher<Intent> barcodeLauncher =
            registerForActivityResult(new ActivityResultContracts.StartActivityForResult(), result -> {
                if (result.getResultCode() == Activity.RESULT_OK && result.getData() != null) {
                    String barcode = result.getData().getStringExtra("scanned_barcode");
                    if (barcode != null && !barcode.isEmpty()) {
                        binding.etBarcode.setText(barcode);
                    }
                }
            });

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityProductCreateEditBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        repository = new DataRepository(this);
        productId = getIntent().getIntExtra(EXTRA_PRODUCT_ID, 0);

        setupToolbar();
        setupListeners();
        loadCategoriesAndSuppliers();

        if (productId > 0) {
            binding.toolbar.setTitle("Chỉnh sửa sản phẩm");
            binding.btnDeleteProduct.setVisibility(View.VISIBLE);
            loadProductDetail();
        } else {
            binding.toolbar.setTitle("Thêm sản phẩm mới");
            binding.btnDeleteProduct.setVisibility(View.GONE);
        }
    }

    private void setupToolbar() {
        binding.toolbar.setNavigationOnClickListener(v -> finish());
    }

    private void setupListeners() {
        binding.btnCancel.setOnClickListener(v -> finish());

        binding.btnScanBarcode.setOnClickListener(v -> {
            Intent intent = new Intent(this, BarcodeScanActivity.class);
            barcodeLauncher.launch(intent);
        });

        binding.btnSaveProduct.setOnClickListener(v -> saveProduct());

        binding.btnDeleteProduct.setOnClickListener(v -> confirmDelete());
    }

    private void loadCategoriesAndSuppliers() {
        // Load categories
        repository.getAllCategories(new DataRepository.ApiCallback<List<ProductCategoryDTO>>() {
            @Override
            public void onSuccess(List<ProductCategoryDTO> result) {
                categoryList = result != null ? result : new ArrayList<>();
                List<String> categoryNames = new ArrayList<>();
                for (ProductCategoryDTO c : categoryList) {
                    categoryNames.add(c.getName() != null ? c.getName() : ("DM #" + c.getCategoryId()));
                }
                ArrayAdapter<String> adapter = new ArrayAdapter<>(ProductCreateEditActivity.this,
                        android.R.layout.simple_spinner_dropdown_item, categoryNames);
                binding.spinnerCategory.setAdapter(adapter);
            }

            @Override
            public void onError(String message) {}
        });

        // Load suppliers
        repository.getAllSuppliers(new DataRepository.ApiCallback<List<SupplierDTO>>() {
            @Override
            public void onSuccess(List<SupplierDTO> result) {
                supplierList = result != null ? result : new ArrayList<>();
                List<String> supplierNames = new ArrayList<>();
                for (SupplierDTO s : supplierList) {
                    supplierNames.add(s.getName() != null ? s.getName() : ("NCC #" + s.getSupplierId()));
                }
                ArrayAdapter<String> adapter = new ArrayAdapter<>(ProductCreateEditActivity.this,
                        android.R.layout.simple_spinner_dropdown_item, supplierNames);
                binding.spinnerSupplier.setAdapter(adapter);
            }

            @Override
            public void onError(String message) {}
        });
    }

    private void loadProductDetail() {
        repository.getProductById(productId, new DataRepository.ApiCallback<Product>() {
            @Override
            public void onSuccess(Product product) {
                if (product == null) return;
                binding.etProductName.setText(product.getName());
                binding.etBarcode.setText(product.getSku());
                binding.etPrice.setText(String.valueOf((long) product.getPrice()));
                if (product.getUnit() != null) binding.etUnit.setText(product.getUnit());
                if (product.getDescription() != null) binding.etDescription.setText(product.getDescription());
            }

            @Override
            public void onError(String message) {
                Toast.makeText(ProductCreateEditActivity.this, "Lỗi tải thông tin sản phẩm: " + message, Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void saveProduct() {
        String name = binding.etProductName.getText() != null ? binding.etProductName.getText().toString().trim() : "";
        String barcode = binding.etBarcode.getText() != null ? binding.etBarcode.getText().toString().trim() : "";
        String priceStr = binding.etPrice.getText() != null ? binding.etPrice.getText().toString().trim() : "";
        String unit = binding.etUnit.getText() != null ? binding.etUnit.getText().toString().trim() : "Cái";
        String minStockStr = binding.etMinStock.getText() != null ? binding.etMinStock.getText().toString().trim() : "5";
        String maxStockStr = binding.etMaxStock.getText() != null ? binding.etMaxStock.getText().toString().trim() : "500";
        String description = binding.etDescription.getText() != null ? binding.etDescription.getText().toString().trim() : "";

        if (name.isEmpty()) {
            binding.tilProductName.setError("Vui lòng nhập tên sản phẩm");
            return;
        } else if (binding.tilProductName.getError() != null) {
            binding.tilProductName.setError(null);
        }

        double price = 0;
        try {
            price = Double.parseDouble(priceStr);
        } catch (Exception e) {
            binding.tilPrice.setError("Giá bán không hợp lệ");
            return;
        }

        if (price <= 0) {
            binding.tilPrice.setError("Giá bán phải lớn hơn 0");
            return;
        } else if (binding.tilPrice.getError() != null) {
            binding.tilPrice.setError(null);
        }

        int minStock = 5;
        try { minStock = Integer.parseInt(minStockStr); } catch (Exception ignored) {}

        int maxStock = 500;
        try { maxStock = Integer.parseInt(maxStockStr); } catch (Exception ignored) {}

        int categoryId = 1;
        int catPos = binding.spinnerCategory.getSelectedItemPosition();
        if (catPos >= 0 && catPos < categoryList.size()) {
            categoryId = categoryList.get(catPos).getCategoryId();
        }

        int supplierId = 1;
        int supPos = binding.spinnerSupplier.getSelectedItemPosition();
        if (supPos >= 0 && supPos < supplierList.size()) {
            supplierId = supplierList.get(supPos).getSupplierId();
        }

        ProductDTO dto = new ProductDTO(productId, name, barcode, unit, description, categoryId, supplierId, price, minStock, maxStock);

        binding.btnSaveProduct.setEnabled(false);

        if (productId > 0) {
            // Update mode
            repository.updateProduct(dto, new DataRepository.ApiCallback<Boolean>() {
                @Override
                public void onSuccess(Boolean result) {
                    Toast.makeText(ProductCreateEditActivity.this, "Cập nhật sản phẩm thành công!", Toast.LENGTH_SHORT).show();
                    finish();
                }

                @Override
                public void onError(String message) {
                    binding.btnSaveProduct.setEnabled(true);
                    Toast.makeText(ProductCreateEditActivity.this, message, Toast.LENGTH_SHORT).show();
                }
            });
        } else {
            // Create mode
            repository.createProduct(dto, new DataRepository.ApiCallback<Boolean>() {
                @Override
                public void onSuccess(Boolean result) {
                    Toast.makeText(ProductCreateEditActivity.this, "Thêm sản phẩm mới thành công!", Toast.LENGTH_SHORT).show();
                    finish();
                }

                @Override
                public void onError(String message) {
                    binding.btnSaveProduct.setEnabled(true);
                    Toast.makeText(ProductCreateEditActivity.this, message, Toast.LENGTH_SHORT).show();
                }
            });
        }
    }

    private void confirmDelete() {
        new MaterialAlertDialogBuilder(this)
                .setTitle("Xác nhận xóa")
                .setMessage("Bạn có chắc chắn muốn ngưng kinh doanh sản phẩm này?")
                .setPositiveButton("Xóa", (dialog, which) -> {
                    repository.deleteProduct(productId, new DataRepository.ApiCallback<Boolean>() {
                        @Override
                        public void onSuccess(Boolean result) {
                            Toast.makeText(ProductCreateEditActivity.this, "Đã xóa sản phẩm thành công!", Toast.LENGTH_SHORT).show();
                            finish();
                        }

                        @Override
                        public void onError(String message) {
                            Toast.makeText(ProductCreateEditActivity.this, message, Toast.LENGTH_SHORT).show();
                        }
                    });
                })
                .setNegativeButton("Hủy", null)
                .show();
    }
}
