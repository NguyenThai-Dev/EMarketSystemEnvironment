package com.example.do_an_di_dong.ui.sales;

import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.databinding.ActivityPromotionsBinding;

public class PromotionsActivity extends AppCompatActivity {
    private ActivityPromotionsBinding binding;
    private PromotionsViewModel viewModel;
    private PromotionAdapter adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityPromotionsBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(PromotionsViewModel.class);
        adapter = new PromotionAdapter();

        binding.btnBack.setOnClickListener(v -> finish());

        binding.rvPromotions.setLayoutManager(new LinearLayoutManager(this));
        binding.rvPromotions.setAdapter(adapter);

        // Observers
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                binding.layoutLoading.setVisibility(isLoading ? View.VISIBLE : View.GONE);
                if (isLoading) {
                    binding.layoutContent.setVisibility(View.GONE);
                    binding.layoutEmpty.setVisibility(View.GONE);
                }
            }
        });

        viewModel.getPromotions().observe(this, list -> {
            if (list == null || list.isEmpty()) {
                binding.layoutContent.setVisibility(View.GONE);
                binding.layoutEmpty.setVisibility(View.VISIBLE);
            } else {
                binding.layoutEmpty.setVisibility(View.GONE);
                binding.layoutContent.setVisibility(View.VISIBLE);
                adapter.setPromotions(list);
            }
        });

        viewModel.getError().observe(this, msg -> {
            if (msg != null) {
                Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
            }
        });

        viewModel.fetchPromotions();
    }
}
