package com.example.do_an_di_dong.ui.approval;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemApprovalBinding;
import com.example.do_an_di_dong.model.Quotation;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class QuotationAdapter extends RecyclerView.Adapter<QuotationAdapter.QuotationViewHolder> {
    private List<Quotation> quotations = new ArrayList<>();

    public void setQuotations(List<Quotation> quotations) {
        this.quotations = (quotations != null) ? quotations : new ArrayList<>();
        notifyDataSetChanged();
    }

    public List<Quotation> getQuotations() {
        return quotations;
    }

    @NonNull
    @Override
    public QuotationViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemApprovalBinding binding = ItemApprovalBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new QuotationViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull QuotationViewHolder holder, int position) {
        holder.bind(quotations.get(position));
        // Reset foreground position on rebind (after swipe cancel)
        holder.binding.cardForeground.setTranslationX(0);
        holder.binding.layoutRevealApprove.setVisibility(View.VISIBLE);
        holder.binding.layoutRevealReject.setVisibility(View.GONE);
    }

    @Override
    public int getItemCount() {
        return quotations.size();
    }

    static class QuotationViewHolder extends RecyclerView.ViewHolder {
        final ItemApprovalBinding binding;

        public QuotationViewHolder(ItemApprovalBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(Quotation quotation) {
            // Code
            binding.tvQuotationCode.setText(
                    quotation.getQuotationCode() != null ? quotation.getQuotationCode() : "—"
            );

            // Customer
            binding.tvCustomerName.setText(
                    quotation.getCustomerName() != null ? quotation.getCustomerName() : "N/A"
            );

            // Date: trim time part if present
            String dateStr = quotation.getIssueDate();
            if (dateStr != null && dateStr.contains("T")) {
                dateStr = dateStr.split("T")[0];
            }
            binding.tvIssueDate.setText(dateStr != null ? dateStr : "—");

            // Total amount
            NumberFormat vnFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));
            binding.tvTotalAmount.setText(vnFormat.format(quotation.getTotalAmount()));

            // Status badge
            String status = quotation.getStatus();
            if ("Sent".equalsIgnoreCase(status)) {
                binding.tvStatus.setText("Chờ duyệt");
                binding.tvStatus.setBackgroundResource(R.drawable.bg_badge_pending);
                binding.tvStatus.setTextColor(
                        binding.tvStatus.getContext().getColor(R.color.on_warning_container));
            } else if ("Accepted".equalsIgnoreCase(status) || "Converted".equalsIgnoreCase(status)) {
                binding.tvStatus.setText("Đã duyệt");
                binding.tvStatus.setBackgroundResource(R.drawable.bg_badge_success);
                binding.tvStatus.setTextColor(
                        binding.tvStatus.getContext().getColor(R.color.on_success_container));
            } else {
                binding.tvStatus.setText("Đã hủy");
                binding.tvStatus.setBackgroundResource(R.drawable.bg_badge_error);
                binding.tvStatus.setTextColor(
                        binding.tvStatus.getContext().getColor(R.color.on_error_container));
            }
        }
    }
}
