package com.example.do_an_di_dong.ui.inventory;

import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.ActivityPurchaseOrdersBinding;

public class PurchaseOrdersActivity extends AppCompatActivity {
    private ActivityPurchaseOrdersBinding binding;
    private PurchaseOrdersViewModel viewModel;
    private PurchaseOrderAdapter adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityPurchaseOrdersBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(PurchaseOrdersViewModel.class);
        adapter = new PurchaseOrderAdapter();

        binding.btnBack.setOnClickListener(v -> finish());

        LinearLayoutManager layoutManager = new LinearLayoutManager(this);
        binding.rvPurchaseOrders.setLayoutManager(layoutManager);
        binding.rvPurchaseOrders.setAdapter(adapter);

        binding.rvPurchaseOrders.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                if (dy > 0) {
                    int visibleItemCount = layoutManager.getChildCount();
                    int totalItemCount = layoutManager.getItemCount();
                    int firstVisibleItemPosition = layoutManager.findFirstVisibleItemPosition();

                    if ((visibleItemCount + firstVisibleItemPosition) >= totalItemCount - 3 && firstVisibleItemPosition >= 0) {
                        viewModel.loadNextPage();
                    }
                }
            }
        });

        // Observers
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                boolean showGlobalLoading = Boolean.TRUE.equals(isLoading) && adapter.getItemCount() == 0;
                binding.layoutLoading.setVisibility(showGlobalLoading ? View.VISIBLE : View.GONE);
                binding.layoutContent.setVisibility(showGlobalLoading ? View.GONE : View.VISIBLE);
            }
        });

        viewModel.getPurchaseOrders().observe(this, list -> {
            if (list != null) {
                adapter.setPurchaseOrders(list);
            }
        });

        viewModel.getError().observe(this, msg -> {
            if (msg != null && !msg.trim().isEmpty()) {
                Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
            }
        });

        viewModel.fetchPurchaseOrders();
    }
}
