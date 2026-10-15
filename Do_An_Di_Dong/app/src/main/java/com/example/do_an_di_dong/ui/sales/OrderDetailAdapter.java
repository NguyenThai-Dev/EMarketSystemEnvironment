package com.example.do_an_di_dong.ui.sales;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.ItemOrderDetailBinding;
import com.example.do_an_di_dong.model.OrderDetail;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class OrderDetailAdapter extends RecyclerView.Adapter<OrderDetailAdapter.DetailViewHolder> {
    private List<OrderDetail> details = new ArrayList<>();

    public void setDetails(List<OrderDetail> details) {
        this.details = (details != null) ? details : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public DetailViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemOrderDetailBinding binding = ItemOrderDetailBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new DetailViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull DetailViewHolder holder, int position) {
        holder.bind(details.get(position));
    }

    @Override
    public int getItemCount() {
        return details.size();
    }

    static class DetailViewHolder extends RecyclerView.ViewHolder {
        final ItemOrderDetailBinding binding;

        public DetailViewHolder(ItemOrderDetailBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(OrderDetail item) {
            NumberFormat vnFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));

            String name = item.getProductName();
            binding.tvProductName.setText(name != null && !name.isEmpty() ? name : "Sản phẩm #" + item.getProductId());

            binding.tvUnitPriceQuantity.setText(vnFormat.format(item.getUnitPrice()) + " x " + item.getQuantity());

            double total = item.getTotalPrice() > 0 ? item.getTotalPrice() : (item.getUnitPrice() * item.getQuantity());
            binding.tvLineTotal.setText(vnFormat.format(total));
        }
    }
}
