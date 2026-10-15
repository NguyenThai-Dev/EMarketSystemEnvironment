package com.example.do_an_di_dong.ui.pos;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.ItemPosProductSuggestionBinding;
import com.example.do_an_di_dong.model.Product;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class PosSuggestionAdapter extends RecyclerView.Adapter<PosSuggestionAdapter.SuggestionViewHolder> {

    public interface OnProductSelectedListener {
        void onProductSelected(Product product);
    }

    private final List<Product> products = new ArrayList<>();
    private OnProductSelectedListener listener;
    private final NumberFormat vnFormat = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));

    public PosSuggestionAdapter() {}

    public PosSuggestionAdapter(OnProductSelectedListener listener) {
        this.listener = listener;
    }

    public void setListener(OnProductSelectedListener listener) {
        this.listener = listener;
    }

    public void setList(List<Product> newProducts) {
        setProducts(newProducts);
    }

    public void setProducts(List<Product> newProducts) {
        this.products.clear();
        if (newProducts != null) {
            this.products.addAll(newProducts);
        }
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public SuggestionViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemPosProductSuggestionBinding binding = ItemPosProductSuggestionBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new SuggestionViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull SuggestionViewHolder holder, int position) {
        Product product = products.get(position);
        holder.bind(product);
    }

    @Override
    public int getItemCount() {
        return products.size();
    }

    class SuggestionViewHolder extends RecyclerView.ViewHolder {
        private final ItemPosProductSuggestionBinding binding;

        public SuggestionViewHolder(ItemPosProductSuggestionBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(Product product) {
            binding.tvSuggestName.setText(product.getName());
            binding.tvSuggestSku.setText("SKU: " + (product.getSku() != null ? product.getSku() : "—"));
            binding.tvSuggestPrice.setText(vnFormat.format(product.getPrice()));

            itemView.setOnClickListener(v -> {
                if (listener != null) listener.onProductSelected(product);
            });

            binding.btnQuickAdd.setOnClickListener(v -> {
                if (listener != null) listener.onProductSelected(product);
            });
        }
    }
}
