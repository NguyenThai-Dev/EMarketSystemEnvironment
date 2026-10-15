package com.example.do_an_di_dong.ui.ai;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ActivityAiAdvisorHubBinding;
import com.example.do_an_di_dong.model.Anomaly;
import com.example.do_an_di_dong.model.Insight;
import com.example.do_an_di_dong.model.ReplenishmentAdvice;

import java.util.ArrayList;
import java.util.List;

public class AIAdvisorHubActivity extends AppCompatActivity {
    private ActivityAiAdvisorHubBinding binding;
    private AIAdvisorHubViewModel viewModel;

    private ReplenishmentAdapter replenishmentAdapter;
    private AnomalyAdapter anomalyAdapter;
    private InsightAdapter insightAdapter;

    private List<ReplenishmentAdvice> cachedAdvice = new ArrayList<>();
    private List<Anomaly> cachedAnomalies = new ArrayList<>();
    private List<Insight> cachedInsights = new ArrayList<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityAiAdvisorHubBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(AIAdvisorHubViewModel.class);

        replenishmentAdapter = new ReplenishmentAdapter();
        anomalyAdapter = new AnomalyAdapter();
        insightAdapter = new InsightAdapter();

        binding.btnBack.setOnClickListener(v -> finish());
        binding.btnOpenPrediction.setOnClickListener(v -> {
            Intent intent = new Intent(this, AIPredictionDashboardActivity.class);
            startActivity(intent);
        });

        binding.rvAdvisorList.setLayoutManager(new LinearLayoutManager(this));
        binding.rvAdvisorList.setAdapter(replenishmentAdapter);

        // Chip group listener to switch adapters
        binding.chipGroupTab.setOnCheckedChangeListener((group, checkedId) -> {
            if (checkedId == R.id.chipAnomalies) {
                binding.rvAdvisorList.setAdapter(anomalyAdapter);
                anomalyAdapter.setList(cachedAnomalies);
            } else if (checkedId == R.id.chipInsights) {
                binding.rvAdvisorList.setAdapter(insightAdapter);
                insightAdapter.setList(cachedInsights);
            } else {
                binding.rvAdvisorList.setAdapter(replenishmentAdapter);
                replenishmentAdapter.setList(cachedAdvice);
            }
        });

        // Observers
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                binding.layoutLoading.setVisibility(isLoading ? View.VISIBLE : View.GONE);
                binding.layoutContent.setVisibility(isLoading ? View.GONE : View.VISIBLE);
            }
        });

        viewModel.getAdvice().observe(this, list -> {
            cachedAdvice = (list != null) ? list : new ArrayList<>();
            if (binding.chipReplenishment.isChecked()) {
                replenishmentAdapter.setList(cachedAdvice);
            }
        });

        viewModel.getAnomalies().observe(this, list -> {
            cachedAnomalies = (list != null) ? list : new ArrayList<>();
            if (binding.chipAnomalies.isChecked()) {
                anomalyAdapter.setList(cachedAnomalies);
            }
        });

        viewModel.getInsights().observe(this, list -> {
            cachedInsights = (list != null) ? list : new ArrayList<>();
            if (binding.chipInsights.isChecked()) {
                insightAdapter.setList(cachedInsights);
            }
        });

        viewModel.getError().observe(this, msg -> {
            if (msg != null) {
                Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
            }
        });

        viewModel.loadAdvisorData(1);
    }
}
