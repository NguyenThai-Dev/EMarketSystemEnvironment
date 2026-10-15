package com.example.do_an_di_dong.ui.admin;

import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.ActivityCustomersBinding;

public class CustomersActivity extends AppCompatActivity {
    private ActivityCustomersBinding binding;
    private CustomersViewModel viewModel;
    private CustomerAdapter adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityCustomersBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(CustomersViewModel.class);
        adapter = new CustomerAdapter();

        binding.btnBack.setOnClickListener(v -> finish());

        LinearLayoutManager layoutManager = new LinearLayoutManager(this);
        binding.rvCustomers.setLayoutManager(layoutManager);
        binding.rvCustomers.setAdapter(adapter);

        binding.rvCustomers.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                if (dy > 0) {
                    int visibleItemCount = layoutManager.getChildCount();
                    int totalItemCount = layoutManager.getItemCount();
                    int firstVisibleItemPosition = layoutManager.findFirstVisibleItemPosition();

                    if ((visibleItemCount + firstVisibleItemPosition) >= totalItemCount - 3 && firstVisibleItemPosition >= 0) {
                        viewModel.loadNextPage();
                    }
                }
            }
        });

        // Observers
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                boolean showGlobalLoading = Boolean.TRUE.equals(isLoading) && adapter.getItemCount() == 0;
                binding.layoutLoading.setVisibility(showGlobalLoading ? View.VISIBLE : View.GONE);
                binding.layoutContent.setVisibility(showGlobalLoading ? View.GONE : View.VISIBLE);
            }
        });

        viewModel.getCustomers().observe(this, list -> {
            if (list != null) {
                adapter.setList(list);
            }
        });

        viewModel.getError().observe(this, msg -> {
            if (msg != null && !msg.trim().isEmpty()) {
                Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
            }
        });

        binding.fabAddCustomer.setOnClickListener(v -> showAddCustomerDialog());

        viewModel.fetchCustomers();
    }

    private void showAddCustomerDialog() {
        android.widget.LinearLayout layout = new android.widget.LinearLayout(this);
        layout.setOrientation(android.widget.LinearLayout.VERTICAL);
        layout.setPadding(48, 24, 48, 24);

        final android.widget.EditText etName = new android.widget.EditText(this);
        etName.setHint("Họ và tên khách hàng *");
        layout.addView(etName);

        final android.widget.EditText etPhone = new android.widget.EditText(this);
        etPhone.setHint("Số điện thoại *");
        etPhone.setInputType(android.text.InputType.TYPE_CLASS_PHONE);
        layout.addView(etPhone);

        final android.widget.EditText etEmail = new android.widget.EditText(this);
        etEmail.setHint("Email");
        etEmail.setInputType(android.text.InputType.TYPE_TEXT_VARIATION_EMAIL_ADDRESS);
        layout.addView(etEmail);

        new com.google.android.material.dialog.MaterialAlertDialogBuilder(this)
                .setTitle("Thêm khách hàng mới")
                .setView(layout)
                .setPositiveButton("Lưu", (dialog, which) -> {
                    String name = etName.getText().toString().trim();
                    String phone = etPhone.getText().toString().trim();
                    String email = etEmail.getText().toString().trim();

                    if (name.isEmpty()) {
                        Toast.makeText(this, "Vui lòng nhập tên khách hàng!", Toast.LENGTH_SHORT).show();
                        return;
                    }

                    com.example.do_an_di_dong.model.Customer customer = new com.example.do_an_di_dong.model.Customer();
                    customer.setCustomerName(name);
                    customer.setPhone(phone);
                    customer.setEmail(email);

                    new com.example.do_an_di_dong.data.repository.DataRepository(this)
                            .createCustomer(customer, new com.example.do_an_di_dong.data.repository.DataRepository.ApiCallback<Boolean>() {
                                @Override
                                public void onSuccess(Boolean result) {
                                    Toast.makeText(CustomersActivity.this, "Thêm khách hàng thành công!", Toast.LENGTH_SHORT).show();
                                    viewModel.fetchCustomers();
                                }

                                @Override
                                public void onError(String message) {
                                    Toast.makeText(CustomersActivity.this, message, Toast.LENGTH_SHORT).show();
                                }
                            });
                })
                .setNegativeButton("Hủy", null)
                .show();
    }
}
