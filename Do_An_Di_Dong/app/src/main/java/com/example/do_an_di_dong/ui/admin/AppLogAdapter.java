package com.example.do_an_di_dong.ui.admin;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemAppLogBinding;
import com.example.do_an_di_dong.model.AppLog;

import java.util.ArrayList;
import java.util.List;

public class AppLogAdapter extends RecyclerView.Adapter<AppLogAdapter.AppLogViewHolder> {

    public interface OnItemClickListener {
        void onItemClick(AppLog item);
    }

    private List<AppLog> list = new ArrayList<>();
    private OnItemClickListener listener;

    public void setOnItemClickListener(OnItemClickListener listener) {
        this.listener = listener;
    }

    public void setList(List<AppLog> list) {
        this.list = (list != null) ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public AppLogViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemAppLogBinding binding = ItemAppLogBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new AppLogViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull AppLogViewHolder holder, int position) {
        holder.bind(list.get(position), listener);
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    static class AppLogViewHolder extends RecyclerView.ViewHolder {
        final ItemAppLogBinding binding;

        public AppLogViewHolder(ItemAppLogBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(AppLog item, OnItemClickListener listener) {
            Context ctx = binding.getRoot().getContext();

            String level = item.getLogLevel() != null ? item.getLogLevel().trim().toUpperCase() : "INFO";
            binding.tvLogLevel.setText(level);

            if ("ERROR".equals(level) || "FATAL".equals(level)) {
                binding.tvLogLevel.setBackgroundResource(R.drawable.bg_badge_rejected);
                binding.tvLogLevel.setTextColor(ContextCompat.getColor(ctx, R.color.on_error_container));
            } else if ("WARN".equals(level) || "WARNING".equals(level)) {
                binding.tvLogLevel.setBackgroundResource(R.drawable.bg_badge_pending);
                binding.tvLogLevel.setTextColor(ContextCompat.getColor(ctx, R.color.on_warning_container));
            } else {
                binding.tvLogLevel.setBackgroundResource(R.drawable.bg_badge_success);
                binding.tvLogLevel.setTextColor(ContextCompat.getColor(ctx, R.color.on_success_container));
            }

            String logger = item.getLogger();
            if (logger != null && logger.contains(".")) {
                String[] parts = logger.split("\\.");
                logger = parts[parts.length - 1];
            }
            binding.tvLogger.setText(logger != null ? logger : "Hệ thống");

            String msg = item.getMessage();
            binding.tvMessage.setText(msg != null ? msg : "—");

            String thread = item.getThread();
            binding.tvThread.setText("Luồng: " + (thread != null ? thread : "Main"));

            String time = item.getCreatedAt();
            binding.tvCreatedAt.setText(time != null ? time : "");

            boolean hasEx = item.getException() != null && !item.getException().trim().isEmpty();
            binding.tvHasException.setVisibility(hasEx ? View.VISIBLE : View.GONE);

            binding.getRoot().setOnClickListener(v -> {
                if (listener != null) {
                    listener.onItemClick(item);
                }
            });
        }
    }
}
