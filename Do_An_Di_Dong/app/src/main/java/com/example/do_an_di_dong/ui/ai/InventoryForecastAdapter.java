package com.example.do_an_di_dong.ui.ai;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemInventoryForecastBinding;
import com.example.do_an_di_dong.model.InventoryForecast;

import java.util.ArrayList;
import java.util.List;

public class InventoryForecastAdapter extends RecyclerView.Adapter<InventoryForecastAdapter.ForecastViewHolder> {
    private List<InventoryForecast> list = new ArrayList<>();

    public void setList(List<InventoryForecast> list) {
        this.list = (list != null) ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public ForecastViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemInventoryForecastBinding binding = ItemInventoryForecastBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new ForecastViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull ForecastViewHolder holder, int position) {
        holder.bind(list.get(position));
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    static class ForecastViewHolder extends RecyclerView.ViewHolder {
        final ItemInventoryForecastBinding binding;

        public ForecastViewHolder(ItemInventoryForecastBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(InventoryForecast item) {
            String name = item.getProductName();
            binding.tvProductName.setText(name != null ? name : "Sản phẩm #" + item.getProductId());

            binding.tvCurrentStock.setText("Tồn hiện tại: " + item.getCurrentStock() + " cái");
            binding.tvPredictedStock.setText("Dự báo sau 30 ngày: " + item.getPredictedStockIn30Days() + " cái");

            if (item.isOutOfStockRisk()) {
                binding.tvRiskStatus.setText("Nguy cơ đứt hàng");
                binding.tvRiskStatus.setBackgroundResource(R.drawable.bg_badge_error);
                binding.tvRiskStatus.setTextColor(binding.tvRiskStatus.getContext().getColor(R.color.on_error_container));
            } else {
                binding.tvRiskStatus.setText("Tồn kho an toàn");
                binding.tvRiskStatus.setBackgroundResource(R.drawable.bg_badge_success);
                binding.tvRiskStatus.setTextColor(binding.tvRiskStatus.getContext().getColor(R.color.on_success_container));
            }
        }
    }
}
