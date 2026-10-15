package com.example.do_an_di_dong.ui.pos;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.ItemPosCartBinding;
import com.example.do_an_di_dong.model.CartItem;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class PosCartAdapter extends RecyclerView.Adapter<PosCartAdapter.CartViewHolder> {

    public interface OnCartItemChangeListener {
        void onQuantityChanged(CartItem item, int newQuantity);
        void onItemRemoved(CartItem item);
    }

    private final List<CartItem> items = new ArrayList<>();
    private OnCartItemChangeListener listener;
    private final NumberFormat vnFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));

    public void setListener(OnCartItemChangeListener listener) {
        this.listener = listener;
    }

    public void setItems(List<CartItem> newItems) {
        this.items.clear();
        if (newItems != null) {
            this.items.addAll(newItems);
        }
        notifyDataSetChanged();
    }

    public List<CartItem> getItems() {
        return items;
    }

    @NonNull
    @Override
    public CartViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemPosCartBinding binding = ItemPosCartBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new CartViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull CartViewHolder holder, int position) {
        CartItem item = items.get(position);
        holder.bind(item);
    }

    @Override
    public int getItemCount() {
        return items.size();
    }

    class CartViewHolder extends RecyclerView.ViewHolder {
        private final ItemPosCartBinding binding;

        public CartViewHolder(ItemPosCartBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(CartItem item) {
            binding.tvItemName.setText(item.getProductName());
            binding.tvItemPrice.setText(vnFormat.format(item.getPrice()));
            binding.tvQuantity.setText(String.valueOf(item.getQuantity()));
            binding.tvSubtotal.setText(vnFormat.format(item.getSubtotal()));

            binding.btnPlus.setOnClickListener(v -> {
                if (listener != null) {
                    listener.onQuantityChanged(item, item.getQuantity() + 1);
                }
            });

            binding.btnMinus.setOnClickListener(v -> {
                if (listener != null) {
                    if (item.getQuantity() > 1) {
                        listener.onQuantityChanged(item, item.getQuantity() - 1);
                    } else {
                        listener.onItemRemoved(item);
                    }
                }
            });

            binding.btnRemoveItem.setOnClickListener(v -> {
                if (listener != null) {
                    listener.onItemRemoved(item);
                }
            });
        }
    }
}
