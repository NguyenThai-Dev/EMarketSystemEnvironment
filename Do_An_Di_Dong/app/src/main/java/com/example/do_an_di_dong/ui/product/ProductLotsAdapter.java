package com.example.do_an_di_dong.ui.product;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.ItemProductLotBinding;
import com.example.do_an_di_dong.model.ProductLot;
import com.example.do_an_di_dong.util.MoneyTextWatcher;

import java.util.ArrayList;
import java.util.List;

public class ProductLotsAdapter extends RecyclerView.Adapter<ProductLotsAdapter.LotViewHolder> {

    private List<ProductLot> list = new ArrayList<>();

    public void setList(List<ProductLot> list) {
        this.list = list != null ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public LotViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemProductLotBinding binding = ItemProductLotBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new LotViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull LotViewHolder holder, int position) {
        holder.bind(list.get(position));
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    static class LotViewHolder extends RecyclerView.ViewHolder {
        final ItemProductLotBinding binding;

        LotViewHolder(ItemProductLotBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        void bind(ProductLot lot) {
            binding.tvLotBatchCode.setText("Mã lô: " + lot.getBatchCode());
            binding.tvLotCostPrice.setText("Giá nhập: " + MoneyTextWatcher.formatCurrency(lot.getCostPrice()));
            binding.tvLotMfgDate.setText("NSX: " + (lot.getManufacturingDate() != null ? lot.getManufacturingDate() : "—"));
            binding.tvLotExpDate.setText("HSD: " + (lot.getExpiryDate() != null ? lot.getExpiryDate() : "—"));
        }
    }
}
