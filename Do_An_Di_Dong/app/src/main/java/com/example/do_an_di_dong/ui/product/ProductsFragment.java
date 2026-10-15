package com.example.do_an_di_dong.ui.product;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.FragmentProductsBinding;
import com.example.do_an_di_dong.model.Product;
import com.example.do_an_di_dong.model.ProductCategoryDTO;
import com.google.android.material.chip.Chip;

import java.util.List;

public class ProductsFragment extends Fragment {
    private FragmentProductsBinding binding;
    private ProductsViewModel viewModel;
    private ProductAdapter adapter;
    private ActivityResultLauncher<Intent> barcodeLauncher;
    
    private Integer selectedCategoryId = null;
    private String currentKeyword = "";

    private final Handler searchHandler = new Handler(Looper.getMainLooper());
    private Runnable searchRunnable;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        binding = FragmentProductsBinding.inflate(inflater, container, false);
        return binding.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        viewModel = new ViewModelProvider(this).get(ProductsViewModel.class);
        adapter = new ProductAdapter();

        LinearLayoutManager layoutManager = new LinearLayoutManager(getContext());
        binding.rvProducts.setLayoutManager(layoutManager);
        binding.rvProducts.setAdapter(adapter);

        // Click product item -> Open ProductDetailActivity
        adapter.setOnProductClickListener(product -> {
            if (product != null && product.getProductId() > 0) {
                Intent intent = new Intent(getContext(), ProductDetailActivity.class);
                intent.putExtra(ProductDetailActivity.EXTRA_PRODUCT_ID, product.getProductId());
                intent.putExtra("product_id", product.getProductId());
                intent.putExtra("productId", product.getProductId());
                startActivity(intent);
            }
        });

        // Click FAB -> Open ProductCreateEditActivity / ProductEditorActivity
        binding.fabCreateProduct.setOnClickListener(v -> {
            Intent intent = new Intent(getContext(), ProductCreateEditActivity.class);
            startActivity(intent);
        });

        // Pull-to-refresh
        binding.swipeRefreshProducts.setOnRefreshListener(() -> {
            viewModel.refreshProducts();
        });

        // Infinite Scroll / Load-More on RecyclerView
        binding.rvProducts.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                if (dy > 0) { // Scrolling down
                    int visibleItemCount = layoutManager.getChildCount();
                    int totalItemCount = layoutManager.getItemCount();
                    int firstVisibleItemPosition = layoutManager.findFirstVisibleItemPosition();

                    if ((visibleItemCount + firstVisibleItemPosition) >= totalItemCount - 4
                            && firstVisibleItemPosition >= 0
                            && totalItemCount >= 20) {
                        viewModel.loadNextPage();
                    }
                }
            }
        });

        // Observers
        viewModel.getProducts().observe(getViewLifecycleOwner(), list -> {
            binding.layoutError.setVisibility(View.GONE);
            binding.layoutContent.setVisibility(View.VISIBLE);

            if (list == null || list.isEmpty()) {
                binding.rvProducts.setVisibility(View.GONE);
                binding.layoutEmpty.setVisibility(View.VISIBLE);
            } else {
                binding.layoutEmpty.setVisibility(View.GONE);
                binding.rvProducts.setVisibility(View.VISIBLE);
                adapter.setProducts(list);
            }
        });

        viewModel.getInitialLoading().observe(getViewLifecycleOwner(), isLoading -> {
            binding.swipeRefreshProducts.setRefreshing(false);
            if (Boolean.TRUE.equals(isLoading)) {
                if (adapter.getItemCount() == 0) {
                    binding.layoutLoading.setVisibility(View.VISIBLE);
                    binding.layoutContent.setVisibility(View.GONE);
                    binding.layoutEmpty.setVisibility(View.GONE);
                    binding.layoutError.setVisibility(View.GONE);
                }
            } else {
                binding.layoutLoading.setVisibility(View.GONE);
            }
        });

        viewModel.getLoadMoreLoading().observe(getViewLifecycleOwner(), isLoadMore -> {
            binding.pbLoadMore.setVisibility(Boolean.TRUE.equals(isLoadMore) ? View.VISIBLE : View.GONE);
        });

        viewModel.getCategories().observe(getViewLifecycleOwner(), this::populateCategories);

        viewModel.getScannedProduct().observe(getViewLifecycleOwner(), product -> {
            if (product != null) {
                binding.etSearch.setText(product.getSku());
                Toast.makeText(getContext(), "Tìm thấy: " + product.getName(), Toast.LENGTH_LONG).show();
            }
        });

        viewModel.getError().observe(getViewLifecycleOwner(), msg -> {
            binding.swipeRefreshProducts.setRefreshing(false);
            if (msg != null && adapter.getItemCount() == 0) {
                showError(msg);
            } else if (msg != null) {
                Toast.makeText(getContext(), "Lỗi tải thêm: " + msg, Toast.LENGTH_SHORT).show();
            }
        });

        // Search text watcher with 350ms debounce for smooth Vietnamese typing
        binding.etSearch.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                currentKeyword = s.toString().trim();
                if (searchRunnable != null) {
                    searchHandler.removeCallbacks(searchRunnable);
                }
                searchRunnable = () -> triggerSearch();
                searchHandler.postDelayed(searchRunnable, 350);
            }

            @Override
            public void afterTextChanged(Editable s) {}
        });

        // Register Barcode Scan launcher
        barcodeLauncher = registerForActivityResult(
                new ActivityResultContracts.StartActivityForResult(),
                result -> {
                    if (result.getResultCode() == Activity.RESULT_OK && result.getData() != null) {
                        String barcode = result.getData().getStringExtra("scanned_barcode");
                        if (barcode != null) {
                            viewModel.lookupBarcode(barcode);
                        }
                    }
                }
        );

        binding.btnBarcode.setOnClickListener(v -> {
            Intent intent = new Intent(getContext(), BarcodeScanActivity.class);
            barcodeLauncher.launch(intent);
        });

        binding.btnRetry.setOnClickListener(v -> loadData());

        loadData();
    }

    private void loadData() {
        binding.layoutContent.setVisibility(View.GONE);
        binding.layoutEmpty.setVisibility(View.GONE);
        binding.layoutError.setVisibility(View.GONE);
        binding.layoutLoading.setVisibility(View.VISIBLE);

        viewModel.fetchCategories();
        triggerSearch();
    }

    private void triggerSearch() {
        viewModel.searchProducts(currentKeyword, selectedCategoryId);
    }

    private void showError(String message) {
        binding.layoutLoading.setVisibility(View.GONE);
        binding.layoutContent.setVisibility(View.GONE);
        binding.layoutEmpty.setVisibility(View.GONE);
        binding.layoutError.setVisibility(View.VISIBLE);

        String friendlyMessage = "Lỗi tải danh sách sản phẩm.";
        if (message.contains("ConnectException") || message.contains("timeout") || message.contains("Unable to resolve host")) {
            friendlyMessage = "Không thể kết nối mạng. Vui lòng kiểm tra Wifi/Mạng.";
        } else if (message.contains("401")) {
            friendlyMessage = "Hết hạn phiên đăng nhập. Vui lòng đăng nhập lại.";
        }
        binding.tvErrorMessage.setText(friendlyMessage);
    }

    private void populateCategories(List<ProductCategoryDTO> categoryList) {
        binding.chipGroupCategories.removeAllViews();

        // 1. Add "Tất cả" chip
        Chip allChip = new Chip(requireContext());
        allChip.setText("Tất cả");
        allChip.setCheckable(true);
        allChip.setChecked(selectedCategoryId == null);
        allChip.setOnCheckedChangeListener((buttonView, isChecked) -> {
            if (isChecked) {
                selectedCategoryId = null;
                triggerSearch();
            }
        });
        binding.chipGroupCategories.addView(allChip);

        // 2. Add dynamic chips
        if (categoryList != null) {
            for (ProductCategoryDTO cat : categoryList) {
                Chip chip = new Chip(requireContext());
                chip.setText(cat.getName());
                chip.setCheckable(true);
                chip.setOnCheckedChangeListener((buttonView, isChecked) -> {
                    if (isChecked) {
                        selectedCategoryId = cat.getCategoryId();
                        triggerSearch();
                    }
                });
                binding.chipGroupCategories.addView(chip);
            }
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        if (searchRunnable != null) {
            searchHandler.removeCallbacks(searchRunnable);
        }
        binding = null;
    }
}
