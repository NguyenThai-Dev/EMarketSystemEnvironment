package com.example.do_an_di_dong.ui.ai;

import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ActivityAiPredictionDashboardBinding;
import com.example.do_an_di_dong.model.InventoryForecast;
import com.example.do_an_di_dong.model.LotFinancialRisk;

import java.util.ArrayList;
import java.util.List;

public class AIPredictionDashboardActivity extends AppCompatActivity {
    private ActivityAiPredictionDashboardBinding binding;
    private AIPredictionDashboardViewModel viewModel;

    private LotRiskAdapter lotRiskAdapter;
    private InventoryForecastAdapter forecastAdapter;

    private List<LotFinancialRisk> cachedLotRisks = new ArrayList<>();
    private List<InventoryForecast> cachedForecasts = new ArrayList<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityAiPredictionDashboardBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(AIPredictionDashboardViewModel.class);

        lotRiskAdapter = new LotRiskAdapter();
        forecastAdapter = new InventoryForecastAdapter();

        binding.btnBack.setOnClickListener(v -> finish());

        binding.rvPredictions.setLayoutManager(new LinearLayoutManager(this));
        binding.rvPredictions.setAdapter(lotRiskAdapter);

        // Chip group listener
        binding.chipGroupPrediction.setOnCheckedChangeListener((group, checkedId) -> {
            if (checkedId == R.id.chipForecast) {
                binding.rvPredictions.setAdapter(forecastAdapter);
                forecastAdapter.setList(cachedForecasts);
            } else {
                binding.rvPredictions.setAdapter(lotRiskAdapter);
                lotRiskAdapter.setList(cachedLotRisks);
            }
        });

        // Run Prophet Button
        binding.btnRunProphet.setOnClickListener(v -> {
            new AlertDialog.Builder(this)
                    .setTitle("Kích hoạt huấn luyện AI")
                    .setMessage("Chạy lại mô hình chuỗi thời gian Prophet trên toàn bộ dữ liệu giao dịch? Quá trình có thể mất vài giây.")
                    .setPositiveButton("Bắt đầu chạy", (dialog, which) -> {
                        viewModel.triggerProphetTraining();
                    })
                    .setNegativeButton("Hủy", null)
                    .show();
        });

        // Observers
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                binding.layoutLoading.setVisibility(isLoading ? View.VISIBLE : View.GONE);
                binding.layoutContent.setVisibility(isLoading ? View.GONE : View.VISIBLE);
            }
        });

        viewModel.getLotRisk().observe(this, list -> {
            cachedLotRisks = (list != null) ? list : new ArrayList<>();
            if (binding.chipLotRisk.isChecked()) {
                lotRiskAdapter.setList(cachedLotRisks);
            }
        });

        viewModel.getForecast().observe(this, list -> {
            cachedForecasts = (list != null) ? list : new ArrayList<>();
            if (binding.chipForecast.isChecked()) {
                forecastAdapter.setList(cachedForecasts);
            }
        });

        viewModel.getProphetSuccess().observe(this, success -> {
            if (Boolean.TRUE.equals(success)) {
                Toast.makeText(this, "Huấn luyện mô hình AI Prophet hoàn tất!", Toast.LENGTH_LONG).show();
                viewModel.loadPredictions(1);
            }
        });

        viewModel.getError().observe(this, msg -> {
            if (msg != null) {
                Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
            }
        });

        viewModel.loadPredictions(1);
    }
}
