package com.example.do_an_di_dong.ui.dashboard;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemSupplierDebtBinding;
import com.example.do_an_di_dong.model.SupplierDebt;
import com.example.do_an_di_dong.model.SupplierPayment;
import com.example.do_an_di_dong.util.MoneyTextWatcher;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

public class SupplierDebtAdapter extends RecyclerView.Adapter<SupplierDebtAdapter.DebtViewHolder> {

    public interface OnDebtActionListener {
        void onPayDebt(SupplierDebt debt);
        void onEditDueDate(SupplierDebt debt);
        void onLoadHistory(SupplierDebt debt, DebtHistoryCallback callback);
        void onDeletePayment(SupplierPayment payment, SupplierDebt parentDebt);
    }

    public interface DebtHistoryCallback {
        void onHistoryLoaded(List<SupplierPayment> payments);
        void onHistoryError(String error);
    }

    private List<SupplierDebt> list = new ArrayList<>();
    private final Set<Integer> expandedDebtIds = new HashSet<>();
    private final OnDebtActionListener listener;

    public SupplierDebtAdapter(OnDebtActionListener listener) {
        this.listener = listener;
    }

    public void setList(List<SupplierDebt> list) {
        this.list = list != null ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public DebtViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemSupplierDebtBinding binding = ItemSupplierDebtBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new DebtViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull DebtViewHolder holder, int position) {
        holder.bind(list.get(position));
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    class DebtViewHolder extends RecyclerView.ViewHolder {
        final ItemSupplierDebtBinding binding;
        private final SupplierPaymentHistoryAdapter historyAdapter;

        DebtViewHolder(ItemSupplierDebtBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
            Context context = binding.getRoot().getContext();
            binding.rvPaymentHistory.setLayoutManager(new LinearLayoutManager(context));
            this.historyAdapter = new SupplierPaymentHistoryAdapter(payment -> {
                int pos = getAdapterPosition();
                if (pos != RecyclerView.NO_POSITION && listener != null) {
                    listener.onDeletePayment(payment, list.get(pos));
                }
            });
            binding.rvPaymentHistory.setAdapter(historyAdapter);
        }

        void bind(SupplierDebt item) {
            Context context = binding.getRoot().getContext();
            binding.tvSupplierName.setText(item.getSupplierName() != null ? item.getSupplierName() : "Nhà cung cấp #" + item.getSupplierId());
            binding.tvPurchaseOrderCode.setText("Đơn nhập: #PO-" + item.getPurchaseOrderId());

            double total = item.getTotalDebt();
            double paid = item.getPaidAmount();
            double remaining = (item.getUnpaidAmount() > 0) ? item.getUnpaidAmount() : Math.max(0, total - paid);

            binding.tvTotalDebt.setText(MoneyTextWatcher.formatCurrency(total));
            binding.tvPaidAmount.setText(MoneyTextWatcher.formatCurrency(paid));
            binding.tvRemainingDebt.setText(MoneyTextWatcher.formatCurrency(remaining));

            // Status badge styling
            String status = item.getStatus() != null ? item.getStatus() : "Unpaid";
            if ("Paid".equalsIgnoreCase(status) || remaining <= 0) {
                binding.tvDebtStatus.setText("Đã thanh toán");
                binding.tvDebtStatus.setBackgroundResource(R.drawable.bg_badge_success);
                binding.tvDebtStatus.setTextColor(ContextCompat.getColor(context, R.color.on_primary_container));
                binding.btnPayDebt.setVisibility(View.GONE);
            } else if ("Partial".equalsIgnoreCase(status) || "PartiallyPaid".equalsIgnoreCase(status) || paid > 0) {
                binding.tvDebtStatus.setText("Trả 1 phần");
                binding.tvDebtStatus.setBackgroundResource(R.drawable.bg_badge_warning);
                binding.tvDebtStatus.setTextColor(ContextCompat.getColor(context, R.color.on_secondary_container));
                binding.btnPayDebt.setVisibility(View.VISIBLE);
            } else {
                binding.tvDebtStatus.setText("Chưa trả");
                binding.tvDebtStatus.setBackgroundResource(R.drawable.bg_badge_error);
                binding.tvDebtStatus.setTextColor(ContextCompat.getColor(context, R.color.on_error_container));
                binding.btnPayDebt.setVisibility(View.VISIBLE);
            }

            if (item.getDueDate() != null && !item.getDueDate().isEmpty()) {
                binding.tvDueDate.setText("📅 Hạn thanh toán: " + item.getDueDate());
                binding.tvDueDate.setVisibility(View.VISIBLE);
            } else {
                binding.tvDueDate.setVisibility(View.GONE);
            }

            // Expandable History Logic
            boolean isExpanded = expandedDebtIds.contains(item.getDebtId());
            binding.layoutHistoryContainer.setVisibility(isExpanded ? View.VISIBLE : View.GONE);
            binding.btnHistory.setText(isExpanded ? "Ẩn lịch sử" : "Lịch sử trả");

            binding.btnHistory.setOnClickListener(v -> {
                if (expandedDebtIds.contains(item.getDebtId())) {
                    expandedDebtIds.remove(item.getDebtId());
                    binding.layoutHistoryContainer.setVisibility(View.GONE);
                    binding.btnHistory.setText("Lịch sử trả");
                } else {
                    expandedDebtIds.add(item.getDebtId());
                    binding.layoutHistoryContainer.setVisibility(View.VISIBLE);
                    binding.btnHistory.setText("Ẩn lịch sử");
                    loadHistory(item);
                }
            });

            binding.btnPayDebt.setOnClickListener(v -> {
                if (listener != null) listener.onPayDebt(item);
            });

            binding.btnEditDueDate.setOnClickListener(v -> {
                if (listener != null) listener.onEditDueDate(item);
            });
        }

        private void loadHistory(SupplierDebt item) {
            binding.pbHistoryLoading.setVisibility(View.VISIBLE);
            binding.tvEmptyHistory.setVisibility(View.GONE);
            binding.rvPaymentHistory.setVisibility(View.GONE);

            if (listener != null) {
                listener.onLoadHistory(item, new DebtHistoryCallback() {
                    @Override
                    public void onHistoryLoaded(List<SupplierPayment> payments) {
                        binding.pbHistoryLoading.setVisibility(View.GONE);
                        if (payments == null || payments.isEmpty()) {
                            binding.tvEmptyHistory.setVisibility(View.VISIBLE);
                            binding.rvPaymentHistory.setVisibility(View.GONE);
                        } else {
                            binding.tvEmptyHistory.setVisibility(View.GONE);
                            binding.rvPaymentHistory.setVisibility(View.VISIBLE);
                            historyAdapter.setList(payments);
                        }
                    }

                    @Override
                    public void onHistoryError(String error) {
                        binding.pbHistoryLoading.setVisibility(View.GONE);
                        binding.tvEmptyHistory.setText("Không thể tải lịch sử: " + error);
                        binding.tvEmptyHistory.setVisibility(View.VISIBLE);
                    }
                });
            }
        }
    }
}
