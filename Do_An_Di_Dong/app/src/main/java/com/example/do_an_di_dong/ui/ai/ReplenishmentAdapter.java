package com.example.do_an_di_dong.ui.ai;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.ItemReplenishmentBinding;
import com.example.do_an_di_dong.model.ReplenishmentAdvice;

import java.util.ArrayList;
import java.util.List;

public class ReplenishmentAdapter extends RecyclerView.Adapter<ReplenishmentAdapter.ReplenishmentViewHolder> {
    private List<ReplenishmentAdvice> list = new ArrayList<>();

    public void setList(List<ReplenishmentAdvice> list) {
        this.list = (list != null) ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public ReplenishmentViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemReplenishmentBinding binding = ItemReplenishmentBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new ReplenishmentViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull ReplenishmentViewHolder holder, int position) {
        holder.bind(list.get(position));
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    static class ReplenishmentViewHolder extends RecyclerView.ViewHolder {
        final ItemReplenishmentBinding binding;

        public ReplenishmentViewHolder(ItemReplenishmentBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(ReplenishmentAdvice item) {
            String name = item.getProductName();
            binding.tvProductName.setText(name != null && !name.isEmpty() ? name : "Sản phẩm #" + item.getProductId());

            binding.tvSuggestedQuantity.setText("Cần nhập: " + item.getSuggestedQuantity());
            binding.tvCurrentStock.setText(item.getCurrentStock() + " cái");

            String reason = item.getReason();
            binding.tvReason.setText("Lý do: " + (reason != null && !reason.isEmpty() ? reason : "Tốc độ tiêu thụ cao hơn mức tồn an toàn."));
        }
    }
}
