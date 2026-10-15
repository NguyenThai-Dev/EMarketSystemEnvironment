package com.example.do_an_di_dong.ui.admin;

import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.ItemCustomerBinding;
import com.example.do_an_di_dong.model.Customer;

import java.util.ArrayList;
import java.util.List;

public class CustomerAdapter extends RecyclerView.Adapter<CustomerAdapter.CustomerViewHolder> {
    private List<Customer> list = new ArrayList<>();

    public void setList(List<Customer> list) {
        this.list = (list != null) ? list : new ArrayList<>();
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public CustomerViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        ItemCustomerBinding binding = ItemCustomerBinding.inflate(
                LayoutInflater.from(parent.getContext()), parent, false);
        return new CustomerViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(@NonNull CustomerViewHolder holder, int position) {
        holder.bind(list.get(position));
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    static class CustomerViewHolder extends RecyclerView.ViewHolder {
        final ItemCustomerBinding binding;

        public CustomerViewHolder(ItemCustomerBinding binding) {
            super(binding.getRoot());
            this.binding = binding;
        }

        public void bind(Customer item) {
            String name = item.getCustomerName();
            binding.tvCustomerName.setText(name != null ? name : "Khách hàng #" + item.getCustomerId());

            String phone = item.getPhone();
            binding.tvPhone.setText("SĐT: " + (phone != null ? phone : "—"));

            String email = item.getEmail();
            binding.tvEmail.setText(email != null ? email : "—");

            String type = item.getCustomerType();
            if ("VIP".equalsIgnoreCase(type)) {
                binding.tvCustomerType.setText("Thành viên VIP");
                binding.tvCustomerType.setBackgroundResource(R.drawable.bg_badge_success);
                binding.tvCustomerType.setTextColor(binding.tvCustomerType.getContext().getColor(R.color.on_success_container));
            } else {
                binding.tvCustomerType.setText(type != null ? type : "Thành viên");
                binding.tvCustomerType.setBackgroundResource(R.drawable.bg_badge_pending);
                binding.tvCustomerType.setTextColor(binding.tvCustomerType.getContext().getColor(R.color.on_warning_container));
            }
        }
    }
}
