package com.example.do_an_di_dong.ui.sales;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.ItemPromotionBinding;
import com.example.do_an_di_dong.model.Promotion;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class PromotionAdapter extends RecyclerView.Adapter<PromotionAdapter.PromotionViewHolder> {
    private List<Promotion> promotions = new ArrayList<>();

    public void setPromotions(List<Promotion> promotions) {
        this.promotions = (promotions != null) ? promotions : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public PromotionViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemPromotionBinding binding = ItemPromotionBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new PromotionViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull PromotionViewHolder holder, int position) {
        holder.bind(promotions.get(position));
    }

    @Override
    public int getItemCount() {
        return promotions.size();
    }

    static class PromotionViewHolder extends RecyclerView.ViewHolder {
        final ItemPromotionBinding binding;

        public PromotionViewHolder(ItemPromotionBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(Promotion promo) {
            binding.tvPromotionName.setText(promo.getName() != null ? promo.getName() : "Khuyến mãi");

            NumberFormat vnFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));
            if ("Percentage".equalsIgnoreCase(promo.getDiscountType())) {
                binding.tvDiscountValue.setText("Giảm " + (int) promo.getDiscountValue() + "% trên giá trị đơn hàng");
            } else {
                binding.tvDiscountValue.setText("Giảm trực tiếp " + vnFormat.format(promo.getDiscountValue()));
            }

            String start = promo.getStartDate();
            String end = promo.getEndDate();
            if (start != null && start.contains("T")) start = start.split("T")[0];
            if (end != null && end.contains("T")) end = end.split("T")[0];

            binding.tvDateRange.setText((start != null ? start : "—") + " đến " + (end != null ? end : "—"));
        }
    }
}
