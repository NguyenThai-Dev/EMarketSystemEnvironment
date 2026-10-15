package com.example.do_an_di_dong.ui.inventory;

import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.databinding.ActivityStockMovementsBinding;

public class StockMovementsActivity extends AppCompatActivity {
    private ActivityStockMovementsBinding binding;
    private StockMovementsViewModel viewModel;
    private StockMovementAdapter adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityStockMovementsBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(StockMovementsViewModel.class);
        adapter = new StockMovementAdapter();

        binding.btnBack.setOnClickListener(v -> finish());

        binding.rvMovements.setLayoutManager(new LinearLayoutManager(this));
        binding.rvMovements.setAdapter(adapter);

        // Observers
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                binding.layoutLoading.setVisibility(isLoading ? View.VISIBLE : View.GONE);
                binding.layoutContent.setVisibility(isLoading ? View.GONE : View.VISIBLE);
            }
        });

        viewModel.getMovements().observe(this, list -> {
            if (list != null) {
                adapter.setMovements(list);
            }
        });

        viewModel.getError().observe(this, msg -> {
            if (msg != null) {
                Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
            }
        });

        viewModel.fetchMovements();
    }
}
