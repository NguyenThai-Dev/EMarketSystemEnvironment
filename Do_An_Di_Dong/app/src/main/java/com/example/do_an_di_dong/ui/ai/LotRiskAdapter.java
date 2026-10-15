package com.example.do_an_di_dong.ui.ai;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemLotRiskBinding;
import com.example.do_an_di_dong.model.LotFinancialRisk;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class LotRiskAdapter extends RecyclerView.Adapter<LotRiskAdapter.LotRiskViewHolder> {
    private List<LotFinancialRisk> list = new ArrayList<>();

    public void setList(List<LotFinancialRisk> list) {
        this.list = (list != null) ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public LotRiskViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemLotRiskBinding binding = ItemLotRiskBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new LotRiskViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull LotRiskViewHolder holder, int position) {
        holder.bind(list.get(position));
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    static class LotRiskViewHolder extends RecyclerView.ViewHolder {
        final ItemLotRiskBinding binding;

        public LotRiskViewHolder(ItemLotRiskBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(LotFinancialRisk item) {
            String lot = item.getLotCode();
            binding.tvLotCode.setText(lot != null ? "Lô #" + lot : "Lô hàng");

            String name = item.getProductName();
            binding.tvProductName.setText(name != null ? name : "N/A");

            NumberFormat vnFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));
            binding.tvRiskValue.setText(vnFormat.format(item.getRiskValue()));

            String exp = item.getExpiryDate();
            if (exp != null && exp.contains("T")) exp = exp.split("T")[0];
            binding.tvExpiryDate.setText("Hạn sử dụng: " + (exp != null ? exp : "—"));

            String status = item.getStatus();
            if ("Critical".equalsIgnoreCase(status) || "High".equalsIgnoreCase(status)) {
                binding.tvRiskStatus.setText("Rủi ro cao");
                binding.tvRiskStatus.setBackgroundResource(R.drawable.bg_badge_error);
                binding.tvRiskStatus.setTextColor(binding.tvRiskStatus.getContext().getColor(R.color.on_error_container));
            } else {
                binding.tvRiskStatus.setText("Cảnh báo");
                binding.tvRiskStatus.setBackgroundResource(R.drawable.bg_badge_pending);
                binding.tvRiskStatus.setTextColor(binding.tvRiskStatus.getContext().getColor(R.color.on_warning_container));
            }
        }
    }
}
