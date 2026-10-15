package com.example.do_an_di_dong.ui.sales;

import android.content.Intent;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.FragmentOrdersBinding;

public class OrdersFragment extends Fragment {
    private FragmentOrdersBinding binding;
    private OrdersViewModel viewModel;
    private OrderAdapter adapter;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        binding = FragmentOrdersBinding.inflate(inflater, container, false);
        return binding.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        viewModel = new ViewModelProvider(this).get(OrdersViewModel.class);
        adapter = new OrderAdapter();

        LinearLayoutManager layoutManager = new LinearLayoutManager(getContext());
        binding.rvOrders.setLayoutManager(layoutManager);
        binding.rvOrders.setAdapter(adapter);

        binding.rvOrders.addOnScrollListener(new androidx.recyclerview.widget.RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull androidx.recyclerview.widget.RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                if (dy > 0) { // Scrolling down
                    int visibleItemCount = layoutManager.getChildCount();
                    int totalItemCount = layoutManager.getItemCount();
                    int firstVisibleItemPosition = layoutManager.findFirstVisibleItemPosition();

                    if ((visibleItemCount + firstVisibleItemPosition) >= totalItemCount - 3 && firstVisibleItemPosition >= 0) {
                        viewModel.loadNextPage();
                    }
                }
            }
        });

        // Click on order -> Open OrderDetailActivity
        adapter.setOnOrderClickListener(order -> {
            Intent intent = new Intent(getContext(), OrderDetailActivity.class);
            intent.putExtra("order_id", order.getOrderId());
            intent.putExtra("order_code", order.getOrderCode());
            intent.putExtra("customer_name", order.getCustomerName());
            intent.putExtra("order_date", order.getOrderDate());
            intent.putExtra("order_status", order.getStatus());
            intent.putExtra("total_amount", order.getTotalAmount());
            startActivity(intent);
        });

        // Observers
        viewModel.getOrders().observe(getViewLifecycleOwner(), list -> {
            binding.layoutLoading.setVisibility(View.GONE);
            binding.layoutError.setVisibility(View.GONE);
            binding.layoutContent.setVisibility(View.VISIBLE);

            if (list == null || list.isEmpty()) {
                binding.rvOrders.setVisibility(View.GONE);
                binding.layoutEmpty.setVisibility(View.VISIBLE);
            } else {
                binding.layoutEmpty.setVisibility(View.GONE);
                binding.rvOrders.setVisibility(View.VISIBLE);
                adapter.setOrders(list);
            }
        });

        viewModel.getError().observe(getViewLifecycleOwner(), msg -> {
            if (msg != null) {
                showError(msg);
            }
        });

        // Search text watcher with debounce for Vietnamese typing
        final android.os.Handler searchHandler = new android.os.Handler(android.os.Looper.getMainLooper());
        final Runnable[] searchRunnable = new Runnable[1];

        binding.etSearch.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                if (searchRunnable[0] != null) {
                    searchHandler.removeCallbacks(searchRunnable[0]);
                }
                searchRunnable[0] = () -> viewModel.setSearchKeyword(s.toString().trim());
                searchHandler.postDelayed(searchRunnable[0], 350);
            }

            @Override
            public void afterTextChanged(Editable s) {}
        });

        // Status chip listener
        binding.chipGroupStatus.setOnCheckedChangeListener((group, checkedId) -> {
            if (checkedId == R.id.chipPending) {
                viewModel.setStatusFilter("Pending");
            } else if (checkedId == R.id.chipProcessing) {
                viewModel.setStatusFilter("Processing");
            } else if (checkedId == R.id.chipCompleted) {
                viewModel.setStatusFilter("Delivered");
            } else if (checkedId == R.id.chipCancelled) {
                viewModel.setStatusFilter("Cancelled");
            } else {
                viewModel.setStatusFilter("ALL");
            }
        });

        // Promotions shortcut button
        binding.btnPromotions.setOnClickListener(v -> {
            Intent intent = new Intent(getContext(), PromotionsActivity.class);
            startActivity(intent);
        });

        binding.btnRetry.setOnClickListener(v -> loadData());

        loadData();
    }

    private void loadData() {
        binding.layoutContent.setVisibility(View.GONE);
        binding.layoutEmpty.setVisibility(View.GONE);
        binding.layoutError.setVisibility(View.GONE);
        binding.layoutLoading.setVisibility(View.VISIBLE);
        viewModel.fetchOrders();
    }

    private void showError(String message) {
        binding.layoutLoading.setVisibility(View.GONE);
        binding.layoutContent.setVisibility(View.GONE);
        binding.layoutEmpty.setVisibility(View.GONE);
        binding.layoutError.setVisibility(View.VISIBLE);

        String friendlyMessage = "Lỗi tải danh sách đơn hàng.";
        if (message.contains("ConnectException") || message.contains("timeout") || message.contains("Unable to resolve host")) {
            friendlyMessage = "Không thể kết nối mạng. Vui lòng kiểm tra Wifi/Mạng.";
        } else if (message.contains("401")) {
            friendlyMessage = "Hết hạn phiên đăng nhập. Vui lòng đăng nhập lại.";
        }
        binding.tvErrorMessage.setText(friendlyMessage);
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}
