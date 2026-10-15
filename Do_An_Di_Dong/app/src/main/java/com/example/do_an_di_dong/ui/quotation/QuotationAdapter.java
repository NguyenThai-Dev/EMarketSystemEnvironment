package com.example.do_an_di_dong.ui.quotation;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemQuotationBinding;
import com.example.do_an_di_dong.model.Quotation;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class QuotationAdapter extends RecyclerView.Adapter<QuotationAdapter.QuotationViewHolder> {

    public interface OnQuotationClickListener {
        void onQuotationClick(Quotation quotation);
    }

    private final List<Quotation> quotations = new ArrayList<>();
    private OnQuotationClickListener listener;
    private final NumberFormat vnFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));

    public void setListener(OnQuotationClickListener listener) {
        this.listener = listener;
    }

    public void setQuotations(List<Quotation> newQuotations) {
        this.quotations.clear();
        if (newQuotations != null) {
            this.quotations.addAll(newQuotations);
        }
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public QuotationViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemQuotationBinding binding = ItemQuotationBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new QuotationViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull QuotationViewHolder holder, int position) {
        holder.bind(quotations.get(position));
    }

    @Override
    public int getItemCount() {
        return quotations.size();
    }

    class QuotationViewHolder extends RecyclerView.ViewHolder {
        private final ItemQuotationBinding binding;

        public QuotationViewHolder(ItemQuotationBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(Quotation q) {
            binding.tvQuotationCode.setText(q.getQuotationCode() != null ? q.getQuotationCode() : "#QUO-" + q.getQuotationId());
            binding.tvCustomerName.setText(q.getCustomerName() != null ? ("Khách hàng: " + q.getCustomerName()) : "Khách hàng: Khách vãng lai");
            binding.tvDate.setText(q.getExpiryDate() != null ? ("Hạn: " + q.getExpiryDate().split("T")[0]) : "Không có hạn");
            binding.tvFinalAmount.setText(vnFormat.format(q.getFinalAmount() > 0 ? q.getFinalAmount() : q.getTotalAmount()));

            String status = q.getStatus();
            if ("Approved".equalsIgnoreCase(status) || "Accepted".equalsIgnoreCase(status)) {
                binding.tvStatus.setText("Đã duyệt");
                binding.tvStatus.setBackgroundResource(R.drawable.bg_badge_success);
                binding.tvStatus.setTextColor(itemView.getContext().getColor(R.color.on_success_container));
            } else if ("Rejected".equalsIgnoreCase(status) || "Cancelled".equalsIgnoreCase(status)) {
                binding.tvStatus.setText("Từ chối");
                binding.tvStatus.setBackgroundResource(R.drawable.bg_badge_error);
                binding.tvStatus.setTextColor(itemView.getContext().getColor(R.color.on_error_container));
            } else {
                binding.tvStatus.setText("Chờ duyệt");
                binding.tvStatus.setBackgroundResource(R.drawable.bg_badge_pending);
                binding.tvStatus.setTextColor(itemView.getContext().getColor(R.color.on_warning_container));
            }

            itemView.setOnClickListener(v -> {
                if (listener != null) listener.onQuotationClick(q);
            });
        }
    }
}
