package com.example.do_an_di_dong.ui.sales;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemOrderBinding;
import com.example.do_an_di_dong.model.Order;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class OrderAdapter extends RecyclerView.Adapter<OrderAdapter.OrderViewHolder> {

    public interface OnOrderClickListener {
        void onOrderClick(Order order);
    }

    private List<Order> orders = new ArrayList<>();
    private OnOrderClickListener listener;

    public void setOrders(List<Order> orders) {
        this.orders = (orders != null) ? orders : new ArrayList<>();
        notifyDataSetChanged();
    }

    public void setOnOrderClickListener(OnOrderClickListener listener) {
        this.listener = listener;
    }

    @NonNull
    @Override
    public OrderViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemOrderBinding binding = ItemOrderBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new OrderViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull OrderViewHolder holder, int position) {
        Order order = orders.get(position);
        holder.bind(order);
        holder.itemView.setOnClickListener(v -> {
            if (listener != null) {
                listener.onOrderClick(order);
            }
        });
    }

    @Override
    public int getItemCount() {
        return orders.size();
    }

    static class OrderViewHolder extends RecyclerView.ViewHolder {
        final ItemOrderBinding binding;

        public OrderViewHolder(ItemOrderBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(Order order) {
            Context ctx = binding.getRoot().getContext();

            // Order Code
            String code = order.getOrderCode();
            binding.tvOrderCode.setText(code != null && !code.isEmpty() ? code : "#ORD-" + order.getOrderId());

            // Customer Name
            String customer = order.getCustomerName();
            binding.tvCustomerName.setText(customer != null && !customer.isEmpty() ? customer : "Khách vãng lai");

            // Date
            String date = order.getOrderDate();
            if (date != null && date.contains("T")) {
                date = date.replace("T", " ");
                if (date.length() > 16) date = date.substring(0, 16);
            }
            binding.tvOrderDate.setText(date != null ? date : "—");

            // Total Amount
            NumberFormat vnFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));
            binding.tvTotalAmount.setText(vnFormat.format(order.getTotalAmount()));

            // Status Badge
            String status = order.getStatus();
            if (status == null) status = "Pending";

            if ("Delivered".equalsIgnoreCase(status) || "Completed".equalsIgnoreCase(status) || "Paid".equalsIgnoreCase(status)) {
                binding.tvOrderStatus.setText("Hoàn thành");
                binding.tvOrderStatus.setBackgroundResource(R.drawable.bg_badge_success);
                binding.tvOrderStatus.setTextColor(ctx.getColor(R.color.on_success_container));
            } else if ("Shipped".equalsIgnoreCase(status) || "Processing".equalsIgnoreCase(status)) {
                binding.tvOrderStatus.setText("Đang xử lý");
                binding.tvOrderStatus.setBackgroundResource(R.drawable.bg_badge_pending);
                binding.tvOrderStatus.setTextColor(ctx.getColor(R.color.on_warning_container));
            } else if ("Cancelled".equalsIgnoreCase(status)) {
                binding.tvOrderStatus.setText("Đã hủy");
                binding.tvOrderStatus.setBackgroundResource(R.drawable.bg_badge_error);
                binding.tvOrderStatus.setTextColor(ctx.getColor(R.color.on_error_container));
            } else {
                binding.tvOrderStatus.setText("Chờ duyệt");
                binding.tvOrderStatus.setBackgroundResource(R.drawable.bg_badge_pending);
                binding.tvOrderStatus.setTextColor(ctx.getColor(R.color.on_warning_container));
            }
        }
    }
}
