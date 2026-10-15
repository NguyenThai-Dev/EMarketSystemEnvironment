package com.example.do_an_di_dong.ui.dashboard;

import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.example.do_an_di_dong.databinding.ActivityFinanceDashboardBinding;

import java.text.NumberFormat;
import java.util.Locale;

public class FinanceDashboardActivity extends AppCompatActivity {
    private ActivityFinanceDashboardBinding binding;
    private FinanceDashboardViewModel viewModel;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityFinanceDashboardBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(FinanceDashboardViewModel.class);

        binding.btnBack.setOnClickListener(v -> finish());

        // Observers
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                binding.layoutLoading.setVisibility(isLoading ? View.VISIBLE : View.GONE);
                binding.layoutContent.setVisibility(isLoading ? View.GONE : View.VISIBLE);
            }
        });

        viewModel.getFinance().observe(this, finance -> {
            if (finance != null) {
                NumberFormat vnFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));
                binding.tvRevenue.setText(vnFormat.format(finance.getRevenue()));
                binding.tvProfit.setText(vnFormat.format(finance.getProfit()));
                binding.tvCashFlow.setText(vnFormat.format(finance.getCashFlow()));
            }
        });

        viewModel.getError().observe(this, msg -> {
            if (msg != null) {
                Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
            }
        });

        viewModel.loadFinance(30, null);
    }
}
