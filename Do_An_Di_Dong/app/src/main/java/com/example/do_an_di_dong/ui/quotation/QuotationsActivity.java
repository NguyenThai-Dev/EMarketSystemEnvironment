package com.example.do_an_di_dong.ui.quotation;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.databinding.ActivityQuotationsBinding;

public class QuotationsActivity extends AppCompatActivity {

    private ActivityQuotationsBinding binding;
    private QuotationsViewModel viewModel;
    private QuotationAdapter adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityQuotationsBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(QuotationsViewModel.class);

        binding.toolbar.setNavigationOnClickListener(v -> finish());

        adapter = new QuotationAdapter();
        adapter.setListener(q -> {
            Intent intent = new Intent(this, QuotationEditorActivity.class);
            intent.putExtra("quotation_id", q.getQuotationId());
            startActivity(intent);
        });

        binding.rvQuotations.setLayoutManager(new LinearLayoutManager(this));
        binding.rvQuotations.setAdapter(adapter);

        binding.swipeRefresh.setOnRefreshListener(() -> viewModel.loadQuotations());

        binding.fabCreateQuotation.setOnClickListener(v -> {
            Intent intent = new Intent(this, QuotationEditorActivity.class);
            startActivity(intent);
        });

        viewModel.getQuotations().observe(this, list -> {
            binding.swipeRefresh.setRefreshing(false);
            adapter.setQuotations(list);
            binding.layoutEmpty.setVisibility((list == null || list.isEmpty()) ? View.VISIBLE : View.GONE);
            binding.rvQuotations.setVisibility((list == null || list.isEmpty()) ? View.GONE : View.VISIBLE);
        });

        viewModel.getLoading().observe(this, isLoading -> {
            if (Boolean.TRUE.equals(isLoading)) {
                binding.swipeRefresh.setRefreshing(true);
            }
        });

        viewModel.getError().observe(this, msg -> {
            binding.swipeRefresh.setRefreshing(false);
            if (msg != null) {
                Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
            }
        });

        viewModel.loadQuotations();
    }

    @Override
    protected void onResume() {
        super.onResume();
        viewModel.loadQuotations();
    }
}
