package com.example.do_an_di_dong.ui.inventory;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemPurchaseOrderBinding;
import com.example.do_an_di_dong.model.PurchaseOrder;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class PurchaseOrderAdapter extends RecyclerView.Adapter<PurchaseOrderAdapter.POViewHolder> {
    private List<PurchaseOrder> purchaseOrders = new ArrayList<>();

    public void setPurchaseOrders(List<PurchaseOrder> list) {
        this.purchaseOrders = (list != null) ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public POViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemPurchaseOrderBinding binding = ItemPurchaseOrderBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new POViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull POViewHolder holder, int position) {
        holder.bind(purchaseOrders.get(position));
    }

    @Override
    public int getItemCount() {
        return purchaseOrders.size();
    }

    static class POViewHolder extends RecyclerView.ViewHolder {
        final ItemPurchaseOrderBinding binding;

        public POViewHolder(ItemPurchaseOrderBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(PurchaseOrder po) {
            Context ctx = binding.getRoot().getContext();

            binding.tvPurchaseOrderCode.setText("#PO-" + po.getPurchaseOrderId());

            String supplier = po.getSupplierName();
            binding.tvSupplierName.setText(supplier != null && !supplier.isEmpty() ? supplier : "NCC #" + po.getSupplierId());

            NumberFormat vnFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));
            binding.tvTotalAmount.setText(vnFormat.format(po.getTotalAmount()));

            String status = po.getStatus();
            if ("Received".equalsIgnoreCase(status) || "Approved".equalsIgnoreCase(status)) {
                binding.tvStatus.setText("Đã nhập kho");
                binding.tvStatus.setBackgroundResource(R.drawable.bg_badge_success);
                binding.tvStatus.setTextColor(ctx.getColor(R.color.on_success_container));
            } else if ("Cancelled".equalsIgnoreCase(status)) {
                binding.tvStatus.setText("Đã hủy");
                binding.tvStatus.setBackgroundResource(R.drawable.bg_badge_error);
                binding.tvStatus.setTextColor(ctx.getColor(R.color.on_error_container));
            } else {
                binding.tvStatus.setText("Chờ duyệt");
                binding.tvStatus.setBackgroundResource(R.drawable.bg_badge_pending);
                binding.tvStatus.setTextColor(ctx.getColor(R.color.on_warning_container));
            }
        }
    }
}
