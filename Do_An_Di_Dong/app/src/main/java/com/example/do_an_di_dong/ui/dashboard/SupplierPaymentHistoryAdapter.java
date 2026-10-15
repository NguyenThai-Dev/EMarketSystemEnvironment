package com.example.do_an_di_dong.ui.dashboard;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.ItemSupplierPaymentHistoryBinding;
import com.example.do_an_di_dong.model.SupplierPayment;
import com.example.do_an_di_dong.util.MoneyTextWatcher;

import java.util.ArrayList;
import java.util.List;

public class SupplierPaymentHistoryAdapter extends RecyclerView.Adapter<SupplierPaymentHistoryAdapter.HistoryViewHolder> {

    public interface OnDeletePaymentListener {
        void onDeletePayment(SupplierPayment payment);
    }

    private List<SupplierPayment> list = new ArrayList<>();
    private final OnDeletePaymentListener deleteListener;

    public SupplierPaymentHistoryAdapter(OnDeletePaymentListener deleteListener) {
        this.deleteListener = deleteListener;
    }

    public void setList(List<SupplierPayment> list) {
        this.list = list != null ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public HistoryViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemSupplierPaymentHistoryBinding binding = ItemSupplierPaymentHistoryBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new HistoryViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull HistoryViewHolder holder, int position) {
        holder.bind(list.get(position));
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    class HistoryViewHolder extends RecyclerView.ViewHolder {
        final ItemSupplierPaymentHistoryBinding binding;

        HistoryViewHolder(ItemSupplierPaymentHistoryBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        void bind(SupplierPayment item) {
            binding.tvPaymentAmount.setText("+ " + MoneyTextWatcher.formatCurrency(item.getAmount()));
            String method = "Cash".equalsIgnoreCase(item.getPaymentMethod()) ? "Tiền mặt" : "Chuyển khoản";
            binding.tvPaymentMethod.setText(method);
            binding.tvPaymentDate.setText(item.getPaymentDate() != null ? item.getPaymentDate() : "");

            binding.btnDeletePayment.setOnClickListener(v -> {
                if (deleteListener != null) {
                    deleteListener.onDeletePayment(item);
                }
            });
        }
    }
}
