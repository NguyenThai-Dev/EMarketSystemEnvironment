package com.example.do_an_di_dong.ui.inventory;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemStockMovementBinding;
import com.example.do_an_di_dong.model.StockMovement;

import java.util.ArrayList;
import java.util.List;

public class StockMovementAdapter extends RecyclerView.Adapter<StockMovementAdapter.MovementViewHolder> {
    private List<StockMovement> movements = new ArrayList<>();

    public void setMovements(List<StockMovement> movements) {
        this.movements = (movements != null) ? movements : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public MovementViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemStockMovementBinding binding = ItemStockMovementBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new MovementViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull MovementViewHolder holder, int position) {
        holder.bind(movements.get(position));
    }

    @Override
    public int getItemCount() {
        return movements.size();
    }

    static class MovementViewHolder extends RecyclerView.ViewHolder {
        final ItemStockMovementBinding binding;

        public MovementViewHolder(ItemStockMovementBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(StockMovement item) {
            Context ctx = binding.getRoot().getContext();

            String name = item.getProductName();
            binding.tvProductName.setText(name != null && !name.isEmpty() ? name : "Sản phẩm #" + item.getProductId());

            int qty = item.getQuantity();
            String type = item.getType();
            if (type == null) type = "Movement";

            if ("In".equalsIgnoreCase(type) || qty > 0) {
                binding.tvQuantity.setText("+" + qty);
                binding.tvQuantity.setTextColor(ctx.getColor(R.color.success));
                binding.tvMovementType.setText("Loại: Nhập kho (In)");
            } else if ("Out".equalsIgnoreCase(type) || qty < 0) {
                binding.tvQuantity.setText(String.valueOf(qty));
                binding.tvQuantity.setTextColor(ctx.getColor(R.color.error));
                binding.tvMovementType.setText("Loại: Xuất kho (Out)");
            } else {
                binding.tvQuantity.setText(String.valueOf(qty));
                binding.tvQuantity.setTextColor(ctx.getColor(R.color.primary));
                binding.tvMovementType.setText("Loại: " + type);
            }

            String date = item.getCreatedDate();
            if (date != null && date.contains("T")) {
                date = date.replace("T", " ");
                if (date.length() > 16) date = date.substring(0, 16);
            }
            binding.tvCreatedDate.setText(date != null ? date : "—");
        }
    }
}
