package com.example.do_an_di_dong.ui.admin;

import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ActivityExpensesBinding;
import com.example.do_an_di_dong.model.ExpenseSummary;

import java.text.NumberFormat;
import java.util.Locale;

public class ExpensesActivity extends AppCompatActivity {

    private ActivityExpensesBinding binding;
    private ExpensesViewModel viewModel;
    private ExpenseAdapter adapter;
    private final NumberFormat currencyFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityExpensesBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(ExpensesViewModel.class);

        setupViews();
        setupObservers();

        viewModel.fetchExpenses();
    }

    private void setupViews() {
        binding.btnBack.setOnClickListener(v -> finish());

        // Setup RecyclerView
        adapter = new ExpenseAdapter();
        LinearLayoutManager layoutManager = new LinearLayoutManager(this);
        binding.rvExpenses.setLayoutManager(layoutManager);
        binding.rvExpenses.setAdapter(adapter);

        // Infinite scroll pagination
        binding.rvExpenses.addOnScrollListener(new RecyclerView.OnScrollListener() {
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

        // Swipe refresh
        binding.swipeRefresh.setOnRefreshListener(() -> viewModel.refreshExpenses());

        // Chip group filter
        binding.chipGroupStatus.setOnCheckedChangeListener((group, checkedId) -> {
            String status = null;
            if (checkedId == R.id.chipApproved) {
                status = "APPROVED";
            } else if (checkedId == R.id.chipPending) {
                status = "PENDING";
            } else if (checkedId == R.id.chipRejected) {
                status = "REJECTED";
            }
            viewModel.setFilter(null, null, status);
        });
    }

    private void setupObservers() {
        viewModel.getExpenses().observe(this, list -> {
            adapter.setExpenses(list);
        });

        viewModel.getSummary().observe(this, this::updateSummaryUI);

        viewModel.getLoading().observe(this, loading -> {
            binding.swipeRefresh.setRefreshing(loading != null && loading);
        });

        viewModel.getEmpty().observe(this, empty -> {
            binding.layoutEmpty.setVisibility(Boolean.TRUE.equals(empty) ? View.VISIBLE : View.GONE);
            binding.rvExpenses.setVisibility(Boolean.TRUE.equals(empty) ? View.GONE : View.VISIBLE);
        });

        viewModel.getError().observe(this, err -> {
            if (err != null && !err.isEmpty()) {
                Toast.makeText(this, err, Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void updateSummaryUI(ExpenseSummary summary) {
        if (summary == null) return;
        binding.tvApprovedAmount.setText(currencyFormat.format(summary.getTotalApprovedAmount()));
        binding.tvPendingAmount.setText(currencyFormat.format(summary.getTotalPendingAmount()));
        binding.tvRejectedAmount.setText(currencyFormat.format(summary.getTotalRejectedAmount()));
    }
}
