package com.example.do_an_di_dong.ui.product;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemProductImageBinding;
import com.example.do_an_di_dong.model.ProductImage;

import java.util.ArrayList;
import java.util.List;

public class ProductImagesAdapter extends RecyclerView.Adapter<ProductImagesAdapter.ImageViewHolder> {

    public interface OnImageDeleteListener {
        void onImageDelete(ProductImage image);
    }

    private List<ProductImage> list = new ArrayList<>();
    private final OnImageDeleteListener deleteListener;

    public ProductImagesAdapter(OnImageDeleteListener deleteListener) {
        this.deleteListener = deleteListener;
    }

    public void setList(List<ProductImage> list) {
        this.list = list != null ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public ImageViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemProductImageBinding binding = ItemProductImageBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new ImageViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull ImageViewHolder holder, int position) {
        holder.bind(list.get(position));
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    class ImageViewHolder extends RecyclerView.ViewHolder {
        final ItemProductImageBinding binding;

        ImageViewHolder(ItemProductImageBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        void bind(ProductImage item) {
            Glide.with(binding.getRoot().getContext())
                    .load(item.getFullImageUrl())
                    .placeholder(R.drawable.bg_card)
                    .into(binding.ivProductGalleryThumb);

            binding.btnDeleteGalleryImg.setOnClickListener(v -> {
                if (deleteListener != null) {
                    deleteListener.onImageDelete(item);
                }
            });
        }
    }
}
