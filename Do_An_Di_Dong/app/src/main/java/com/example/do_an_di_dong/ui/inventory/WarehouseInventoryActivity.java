package com.example.do_an_di_dong.ui.inventory;

import android.content.Intent;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.databinding.ActivityWarehouseInventoryBinding;
import com.example.do_an_di_dong.model.InventoryItem;
import com.example.do_an_di_dong.model.Warehouse;
import com.google.android.material.tabs.TabLayout;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class WarehouseInventoryActivity extends AppCompatActivity {

    private ActivityWarehouseInventoryBinding binding;
    private DataRepository repository;
    private InventoryItemAdapter adapter;

    private final List<Warehouse> warehouseList = new ArrayList<>();
    private Warehouse selectedWarehouse = null;
    private final NumberFormat currencyFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));
    private final NumberFormat numberFormat = NumberFormat.getInstance(new Locale("vi", "VN"));

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityWarehouseInventoryBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        repository = new DataRepository(this);

        setupViews();
        loadWarehouses();
    }

    private void setupViews() {
        // Back button
        binding.btnBack.setOnClickListener(v -> finish());

        // Purchase Orders button
        binding.btnPurchaseOrders.setOnClickListener(v -> {
            Intent intent = new Intent(this, PurchaseOrdersActivity.class);
            startActivity(intent);
        });

        // FAB Stock Adjustment
        binding.fabStockAdjustment.setOnClickListener(v -> {
            Intent intent = new Intent(this, StockAdjustmentActivity.class);
            if (selectedWarehouse != null) {
                intent.putExtra("warehouse_id", selectedWarehouse.getWarehouseId());
                intent.putExtra("warehouse_name", selectedWarehouse.getName());
            }
            startActivity(intent);
        });

        // RecyclerView & Adapter
        adapter = new InventoryItemAdapter();
        binding.rvInventory.setLayoutManager(new LinearLayoutManager(this));
        binding.rvInventory.setAdapter(adapter);

        // Swipe Refresh
        binding.swipeRefresh.setOnRefreshListener(this::loadInventory);

        // Search text watcher
        binding.etSearch.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                adapter.filter(s != null ? s.toString() : "");
            }

            @Override
            public void afterTextChanged(Editable s) {}
        });

        // Tab selection
        binding.tabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                if (tab.getPosition() == 1) {
                    // Switch to Stock Movements Activity
                    Intent intent = new Intent(WarehouseInventoryActivity.this, StockMovementsActivity.class);
                    if (selectedWarehouse != null) {
                        intent.putExtra("warehouse_id", selectedWarehouse.getWarehouseId());
                    }
                    startActivity(intent);
                    // Reset back to tab 0 on return
                    binding.tabLayout.post(() -> {
                        TabLayout.Tab tab0 = binding.tabLayout.getTabAt(0);
                        if (tab0 != null) tab0.select();
                    });
                }
            }

            @Override
            public void onTabUnselected(TabLayout.Tab tab) {}

            @Override
            public void onTabReselected(TabLayout.Tab tab) {}
        });
    }

    private void loadWarehouses() {
        binding.progressBar.setVisibility(View.VISIBLE);

        repository.getFilteredWarehouses(null, null, new DataRepository.ApiCallback<List<Warehouse>>() {
            @Override
            public void onSuccess(List<Warehouse> result) {
                binding.progressBar.setVisibility(View.GONE);
                if (result != null && !result.isEmpty()) {
                    warehouseList.clear();
                    warehouseList.addAll(result);

                    List<String> names = new ArrayList<>();
                    for (Warehouse w : warehouseList) {
                        names.add(w.getName());
                    }

                    ArrayAdapter<String> warehouseAdapter = new ArrayAdapter<>(
                            WarehouseInventoryActivity.this,
                            android.R.layout.simple_dropdown_item_1line,
                            names
                    );
                    binding.actvWarehouse.setAdapter(warehouseAdapter);

                    // Default select first warehouse
                    selectedWarehouse = warehouseList.get(0);
                    binding.actvWarehouse.setText(selectedWarehouse.getName(), false);
                    binding.tvSubtitle.setText("Kho: " + selectedWarehouse.getName());

                    binding.actvWarehouse.setOnItemClickListener((parent, view, position, id) -> {
                        selectedWarehouse = warehouseList.get(position);
                        binding.tvSubtitle.setText("Kho: " + selectedWarehouse.getName());
                        loadInventory();
                    });

                    loadInventory();
                } else {
                    binding.tvEmptyMessage.setText("Chưa có dữ liệu kho hàng nào.");
                    binding.layoutEmpty.setVisibility(View.VISIBLE);
                }
            }

            @Override
            public void onError(String message) {
                binding.progressBar.setVisibility(View.GONE);
                Toast.makeText(WarehouseInventoryActivity.this, "Lỗi tải kho: " + message, Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void loadInventory() {
        if (selectedWarehouse == null) {
            binding.swipeRefresh.setRefreshing(false);
            return;
        }

        binding.progressBar.setVisibility(View.VISIBLE);
        binding.layoutEmpty.setVisibility(View.GONE);

        repository.getInventoryItems(selectedWarehouse.getWarehouseId(), null, null, new DataRepository.ApiCallback<List<InventoryItem>>() {
            @Override
            public void onSuccess(List<InventoryItem> items) {
                binding.progressBar.setVisibility(View.GONE);
                binding.swipeRefresh.setRefreshing(false);

                if (items == null || items.isEmpty()) {
                    adapter.setItems(new ArrayList<>());
                    binding.tvTotalItems.setText("0");
                    binding.tvTotalQuantity.setText("0");
                    binding.tvTotalValuation.setText("0 ₫");
                    binding.layoutEmpty.setVisibility(View.VISIBLE);
                } else {
                    binding.layoutEmpty.setVisibility(View.GONE);
                    adapter.setItems(items);

                    // Compute KPI statistics
                    long totalQty = 0;
                    double totalValuation = 0.0;
                    for (InventoryItem item : items) {
                        totalQty += item.getQuantity();
                        totalValuation += item.getTotalValuation();
                    }

                    binding.tvTotalItems.setText(numberFormat.format(items.size()));
                    binding.tvTotalQuantity.setText(numberFormat.format(totalQty));
                    binding.tvTotalValuation.setText(currencyFormat.format(totalValuation));
                }
            }

            @Override
            public void onError(String message) {
                binding.progressBar.setVisibility(View.GONE);
                binding.swipeRefresh.setRefreshing(false);
                Toast.makeText(WarehouseInventoryActivity.this, "Lỗi tải tồn kho: " + message, Toast.LENGTH_LONG).show();
            }
        });
    }
}
