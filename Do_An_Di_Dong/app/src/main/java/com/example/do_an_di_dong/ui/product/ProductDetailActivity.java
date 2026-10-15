package com.example.do_an_di_dong.ui.product;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.bumptech.glide.Glide;
import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.databinding.ActivityProductDetailBinding;
import com.example.do_an_di_dong.model.Product;
import com.example.do_an_di_dong.model.ProductImage;
import com.example.do_an_di_dong.ui.inventory.StockAdjustmentActivity;
import com.example.do_an_di_dong.ui.inventory.StockMovementsActivity;
import com.example.do_an_di_dong.util.MoneyTextWatcher;
import com.google.android.material.snackbar.Snackbar;

public class ProductDetailActivity extends AppCompatActivity {

    public static final String EXTRA_PRODUCT_ID = "PRODUCT_ID";

    private ActivityProductDetailBinding binding;
    private ProductDetailViewModel viewModel;
    private int productId;
    private Product currentProduct;

    private ProductLotsAdapter lotsAdapter;
    private ProductImagesAdapter imagesAdapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityProductDetailBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        productId = getIntent().getIntExtra(EXTRA_PRODUCT_ID, -1);
        if (productId <= 0) {
            productId = getIntent().getIntExtra("product_id", -1);
        }
        if (productId <= 0) {
            productId = getIntent().getIntExtra("productId", -1);
        }
        if (productId <= 0) {
            productId = getIntent().getIntExtra("id", -1);
        }
        if (productId <= 0) {
            Toast.makeText(this, "Không tìm thấy thông tin sản phẩm!", Toast.LENGTH_SHORT).show();
            finish();
            return;
        }

        viewModel = new ViewModelProvider(this).get(ProductDetailViewModel.class);

        setupViews();
        setupObservers();

        viewModel.loadProduct(productId);
    }

    private void setupViews() {
        binding.btnBack.setOnClickListener(v -> finish());

        // Lots RecyclerView
        binding.rvProductLots.setLayoutManager(new LinearLayoutManager(this));
        lotsAdapter = new ProductLotsAdapter();
        binding.rvProductLots.setAdapter(lotsAdapter);

        // Images RecyclerView (Horizontal)
        binding.rvProductImages.setLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.HORIZONTAL, false));
        imagesAdapter = new ProductImagesAdapter(this::confirmDeleteImage);
        binding.rvProductImages.setAdapter(imagesAdapter);

        binding.btnAddImage.setOnClickListener(v -> showAddImageDialog());

        binding.btnEditProduct.setOnClickListener(v -> {
            if (currentProduct != null) {
                Intent intent = new Intent(this, ProductCreateEditActivity.class);
                intent.putExtra("PRODUCT_ID", currentProduct.getProductId());
                startActivity(intent);
            }
        });

        binding.btnDeleteProduct.setOnClickListener(v -> {
            new AlertDialog.Builder(this)
                    .setTitle("Xác nhận xóa")
                    .setMessage("Bạn có chắc chắn muốn xóa sản phẩm này khỏi hệ thống?")
                    .setPositiveButton("Xóa", (dialog, which) -> viewModel.deleteProduct(productId))
                    .setNegativeButton("Hủy", null)
                    .show();
        });

        binding.btnStockAdjust.setOnClickListener(v -> {
            Intent intent = new Intent(this, StockAdjustmentActivity.class);
            if (currentProduct != null) {
                intent.putExtra("PRODUCT_ID", currentProduct.getProductId());
            }
            startActivity(intent);
        });

        binding.btnStockMovements.setOnClickListener(v -> {
            Intent intent = new Intent(this, StockMovementsActivity.class);
            if (currentProduct != null) {
                intent.putExtra("PRODUCT_ID", currentProduct.getProductId());
            }
            startActivity(intent);
        });
    }

    private void setupObservers() {
        viewModel.getProduct().observe(this, product -> {
            if (product != null) {
                currentProduct = product;
                renderProduct(product);
            }
        });

        viewModel.getLots().observe(this, lots -> {
            lotsAdapter.setList(lots);
            binding.tvEmptyLots.setVisibility((lots == null || lots.isEmpty()) ? View.VISIBLE : View.GONE);
            binding.rvProductLots.setVisibility((lots != null && !lots.isEmpty()) ? View.VISIBLE : View.GONE);
        });

        viewModel.getImages().observe(this, images -> {
            imagesAdapter.setList(images);
            binding.tvEmptyImages.setVisibility((images == null || images.isEmpty()) ? View.VISIBLE : View.GONE);
            binding.rvProductImages.setVisibility((images != null && !images.isEmpty()) ? View.VISIBLE : View.GONE);
        });

        viewModel.getLoading().observe(this, loading -> {
            binding.layoutLoading.setVisibility(Boolean.TRUE.equals(loading) ? View.VISIBLE : View.GONE);
            binding.layoutContent.setVisibility(Boolean.TRUE.equals(loading) ? View.GONE : View.VISIBLE);
        });

        viewModel.getError().observe(this, error -> {
            if (error != null && !error.isEmpty()) {
                Snackbar.make(binding.getRoot(), error, Snackbar.LENGTH_LONG).show();
            }
        });

        viewModel.getDeleteSuccess().observe(this, success -> {
            if (Boolean.TRUE.equals(success)) {
                Toast.makeText(this, "Đã xóa sản phẩm thành công!", Toast.LENGTH_SHORT).show();
                finish();
            }
        });
    }

    private void renderProduct(Product p) {
        binding.tvProductName.setText(p.getName());
        binding.tvProductPrice.setText(MoneyTextWatcher.formatCurrency(p.getPrice()));
        binding.tvProductSku.setText(p.getSku() != null ? p.getSku() : "—");
        binding.tvProductId.setText(p.getBarcode() != null && !p.getBarcode().isEmpty() ? p.getBarcode() : ("ID #" + p.getProductId()));

        int stock = p.getActualStock();
        binding.tvStockBadge.setText("Tồn kho: " + stock);
        if (stock <= 0) {
            binding.tvStockBadge.setBackgroundResource(R.drawable.bg_badge_error);
            binding.tvStockBadge.setTextColor(getResources().getColor(R.color.on_error_container, getTheme()));
        } else if (stock < 10) {
            binding.tvStockBadge.setBackgroundResource(R.drawable.bg_badge_warning);
            binding.tvStockBadge.setTextColor(getResources().getColor(R.color.on_secondary_container, getTheme()));
        } else {
            binding.tvStockBadge.setBackgroundResource(R.drawable.bg_badge_success);
            binding.tvStockBadge.setTextColor(getResources().getColor(R.color.on_primary_container, getTheme()));
        }

        Glide.with(this)
                .load(p.getFullImageUrl())
                .placeholder(R.drawable.bg_card)
                .error(R.drawable.bg_card)
                .into(binding.imgProduct);
    }

    private void showAddImageDialog() {
        EditText etUrl = new EditText(this);
        etUrl.setHint("Nhập URL hình ảnh...");

        new AlertDialog.Builder(this)
                .setTitle("Thêm ảnh phụ cho sản phẩm")
                .setView(etUrl)
                .setPositiveButton("Thêm", (dialog, which) -> {
                    String url = etUrl.getText().toString().trim();
                    if (!url.isEmpty()) {
                        viewModel.addProductImage(productId, url, new DataRepository.ApiCallback<Boolean>() {
                            @Override
                            public void onSuccess(Boolean result) {
                                Toast.makeText(ProductDetailActivity.this, "Đã thêm ảnh!", Toast.LENGTH_SHORT).show();
                            }

                            @Override
                            public void onError(String message) {
                                Toast.makeText(ProductDetailActivity.this, "Lỗi thêm ảnh: " + message, Toast.LENGTH_SHORT).show();
                            }
                        });
                    }
                })
                .setNegativeButton("Hủy", null)
                .show();
    }

    private void confirmDeleteImage(ProductImage image) {
        new AlertDialog.Builder(this)
                .setTitle("Xác nhận xóa ảnh")
                .setMessage("Bạn có chắc muốn xóa ảnh này khỏi bộ sưu tập?")
                .setPositiveButton("Xóa", (dialog, which) -> {
                    viewModel.deleteProductImage(image.getImageId(), productId, new DataRepository.ApiCallback<Boolean>() {
                        @Override
                        public void onSuccess(Boolean result) {
                            Toast.makeText(ProductDetailActivity.this, "Đã xóa ảnh!", Toast.LENGTH_SHORT).show();
                        }

                        @Override
                        public void onError(String message) {
                            Toast.makeText(ProductDetailActivity.this, "Lỗi xóa: " + message, Toast.LENGTH_SHORT).show();
                        }
                    });
                })
                .setNegativeButton("Hủy", null)
                .show();
    }
}
