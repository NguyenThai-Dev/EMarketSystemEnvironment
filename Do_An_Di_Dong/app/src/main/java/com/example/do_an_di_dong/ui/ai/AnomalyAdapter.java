package com.example.do_an_di_dong.ui.ai;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.ItemAnomalyBinding;
import com.example.do_an_di_dong.model.Anomaly;

import java.util.ArrayList;
import java.util.List;

public class AnomalyAdapter extends RecyclerView.Adapter<AnomalyAdapter.AnomalyViewHolder> {
    private List<Anomaly> list = new ArrayList<>();

    public void setList(List<Anomaly> list) {
        this.list = (list != null) ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public AnomalyViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemAnomalyBinding binding = ItemAnomalyBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new AnomalyViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull AnomalyViewHolder holder, int position) {
        holder.bind(list.get(position));
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    static class AnomalyViewHolder extends RecyclerView.ViewHolder {
        final ItemAnomalyBinding binding;

        public AnomalyViewHolder(ItemAnomalyBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(Anomaly item) {
            String cat = item.getCategoryName();
            binding.tvCategoryName.setText(cat != null && !cat.isEmpty() ? cat : "Nhóm danh mục");

            String type = item.getDetectionType();
            binding.tvDetectionType.setText(type != null && !type.isEmpty() ? type : "Bất thường");

            String details = item.getDetails();
            binding.tvDetails.setText(details != null && !details.isEmpty() ? details : "Phát hiện biến động doanh thu hoặc sản lượng bất thường.");
        }
    }
}
