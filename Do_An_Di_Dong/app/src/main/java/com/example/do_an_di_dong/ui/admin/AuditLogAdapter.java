package com.example.do_an_di_dong.ui.admin;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemAuditLogBinding;
import com.example.do_an_di_dong.model.AuditLog;

import java.util.ArrayList;
import java.util.List;

public class AuditLogAdapter extends RecyclerView.Adapter<AuditLogAdapter.AuditLogViewHolder> {

    public interface OnItemClickListener {
        void onItemClick(AuditLog item);
    }

    private List<AuditLog> list = new ArrayList<>();
    private OnItemClickListener listener;

    public void setOnItemClickListener(OnItemClickListener listener) {
        this.listener = listener;
    }

    public void setList(List<AuditLog> list) {
        this.list = (list != null) ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public AuditLogViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemAuditLogBinding binding = ItemAuditLogBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new AuditLogViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull AuditLogViewHolder holder, int position) {
        holder.bind(list.get(position), listener);
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    static class AuditLogViewHolder extends RecyclerView.ViewHolder {
        final ItemAuditLogBinding binding;

        public AuditLogViewHolder(ItemAuditLogBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(AuditLog item, OnItemClickListener listener) {
            Context ctx = binding.getRoot().getContext();

            String actionText = item.getFormattedAction();
            binding.tvActionType.setText(actionText);

            String actionType = item.getActionType() != null ? item.getActionType().trim() : "";
            if ("Added".equalsIgnoreCase(actionType) || "Create".equalsIgnoreCase(actionType)) {
                binding.tvActionType.setBackgroundResource(R.drawable.bg_badge_success);
                binding.tvActionType.setTextColor(ContextCompat.getColor(ctx, R.color.on_success_container));
            } else if ("Deleted".equalsIgnoreCase(actionType) || "Delete".equalsIgnoreCase(actionType)) {
                binding.tvActionType.setBackgroundResource(R.drawable.bg_badge_rejected);
                binding.tvActionType.setTextColor(ContextCompat.getColor(ctx, R.color.on_error_container));
            } else {
                binding.tvActionType.setBackgroundResource(R.drawable.bg_badge_pending);
                binding.tvActionType.setTextColor(ContextCompat.getColor(ctx, R.color.on_warning_container));
            }

            String tableFormatted = item.getFormattedTable();
            String keyStr = item.getPrimaryKeyId() != null ? " (#" + item.getPrimaryKeyId() + ")" : "";
            binding.tvTableName.setText(tableFormatted + keyStr);

            String user = item.getUsername();
            binding.tvUsername.setText("Bởi: " + (user != null ? user : "Hệ thống"));

            String ip = item.getIpAddress();
            binding.tvIpAddress.setText("IP: " + (ip != null ? ip : "—"));

            String time = item.getCreatedAt();
            binding.tvCreatedAt.setText(time != null ? time : "");

            binding.getRoot().setOnClickListener(v -> {
                if (listener != null) {
                    listener.onItemClick(item);
                }
            });
        }
    }
}
