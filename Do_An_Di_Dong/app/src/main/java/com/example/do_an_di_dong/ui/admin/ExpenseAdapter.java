package com.example.do_an_di_dong.ui.admin;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemExpenseBinding;
import com.example.do_an_di_dong.model.Expense;

import java.text.NumberFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;

public class ExpenseAdapter extends RecyclerView.Adapter<ExpenseAdapter.ViewHolder> {

    private final List<Expense> items = new ArrayList<>();
    private final NumberFormat currencyFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));
    private OnExpenseClickListener listener;

    public interface OnExpenseClickListener {
        void onExpenseClick(Expense expense);
    }

    public void setOnExpenseClickListener(OnExpenseClickListener listener) {
        this.listener = listener;
    }

    public void setExpenses(List<Expense> newItems) {
        this.items.clear();
        if (newItems != null) {
            this.items.addAll(newItems);
        }
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemExpenseBinding binding = ItemExpenseBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new ViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        holder.bind(items.get(position));
    }

    @Override
    public int getItemCount() {
        return items.size();
    }

    class ViewHolder extends RecyclerView.ViewHolder {
        private final ItemExpenseBinding binding;

        ViewHolder(ItemExpenseBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        void bind(Expense item) {
            Context context = binding.getRoot().getContext();

            // Category Name
            String catName = item.getCategoryName();
            binding.tvCategoryName.setText(catName != null && !catName.isEmpty() ? catName : "Khoản chi khác");

            // Note / Description
            String note = item.getNote();
            binding.tvNote.setText(note != null && !note.isEmpty() ? note : "Không có ghi chú");

            // Amount
            binding.tvAmount.setText("- " + currencyFormat.format(item.getAmount()));

            // User & Branch
            String user = item.getUserName() != null ? item.getUserName() : "Nhân viên";
            String branch = item.getBranchName() != null ? item.getBranchName() : "";
            if (!branch.isEmpty()) {
                binding.tvUserAndBranch.setText("Tạo bởi: " + user + " • " + branch);
            } else {
                binding.tvUserAndBranch.setText("Tạo bởi: " + user);
            }

            // Date
            String rawDate = item.getExpenseDate();
            binding.tvDate.setText(formatDate(rawDate));

            // Status Badge
            String status = item.getStatus() != null ? item.getStatus().toUpperCase() : "PENDING";
            if ("APPROVED".equals(status)) {
                binding.tvStatusBadge.setText("Đã duyệt");
                binding.tvStatusBadge.setBackgroundResource(R.drawable.bg_badge_success);
                binding.tvStatusBadge.setTextColor(ContextCompat.getColor(context, R.color.on_success_container));
            } else if ("REJECTED".equals(status)) {
                binding.tvStatusBadge.setText("Từ chối");
                binding.tvStatusBadge.setBackgroundResource(R.drawable.bg_badge_error);
                binding.tvStatusBadge.setTextColor(ContextCompat.getColor(context, R.color.on_error_container));
            } else {
                binding.tvStatusBadge.setText("Chờ duyệt");
                binding.tvStatusBadge.setBackgroundResource(R.drawable.bg_badge_warning);
                binding.tvStatusBadge.setTextColor(ContextCompat.getColor(context, R.color.on_secondary_container));
            }

            binding.getRoot().setOnClickListener(v -> {
                if (listener != null) {
                    listener.onExpenseClick(item);
                }
            });
        }

        private String formatDate(String rawDate) {
            if (rawDate == null || rawDate.isEmpty()) return "";
            try {
                SimpleDateFormat parser = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault());
                Date d = parser.parse(rawDate);
                if (d != null) {
                    SimpleDateFormat formatter = new SimpleDateFormat("dd/MM/yyyy", Locale.getDefault());
                    return formatter.format(d);
                }
            } catch (Exception ignored) {}
            return rawDate;
        }
    }
}
