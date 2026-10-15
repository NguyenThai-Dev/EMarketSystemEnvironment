package com.example.do_an_di_dong.ui.sales;

import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.databinding.ActivityOrderDetailBinding;
import com.example.do_an_di_dong.databinding.DialogInvoiceReceiptBinding;
import com.example.do_an_di_dong.model.OrderDetail;
import com.example.do_an_di_dong.util.MoneyTextWatcher;
import com.google.android.material.dialog.MaterialAlertDialogBuilder;

import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class OrderDetailActivity extends AppCompatActivity {
    private ActivityOrderDetailBinding binding;
    private OrderDetailViewModel viewModel;
    private OrderDetailAdapter detailsAdapter;
    private OrderPaymentAdapter paymentsAdapter;

    private int orderId;
    private String orderCode;
    private String customerName;
    private String orderDate;
    private String currentStatus;
    private double totalAmount;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityOrderDetailBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        viewModel = new ViewModelProvider(this).get(OrderDetailViewModel.class);

        orderId = getIntent().getIntExtra("order_id", 0);
        orderCode = getIntent().getStringExtra("order_code");
        customerName = getIntent().getStringExtra("customer_name");
        orderDate = getIntent().getStringExtra("order_date");
        currentStatus = getIntent().getStringExtra("order_status");
        totalAmount = getIntent().getDoubleExtra("total_amount", 0);

        if (orderCode == null || orderCode.isEmpty()) {
            orderCode = "#ORD-" + orderId;
        }

        // Header and basic info setup
        binding.btnBack.setOnClickListener(v -> finish());
        binding.tvHeaderTitle.setText(orderCode);
        binding.tvOrderCode.setText(orderCode);
        binding.tvCustomerName.setText(customerName != null ? customerName : "Khách vãng lai");

        if (orderDate != null && orderDate.contains("T")) {
            orderDate = orderDate.replace("T", " ");
            if (orderDate.length() > 16) orderDate = orderDate.substring(0, 16);
        }
        binding.tvOrderDate.setText(orderDate != null ? orderDate : "—");

        binding.tvTotalAmount.setText(MoneyTextWatcher.formatCurrency(totalAmount));

        updateStatusUI(currentStatus);

        binding.btnChangeStatus.setOnClickListener(v -> showChangeStatusDialog(orderId));
        binding.btnViewReceipt.setOnClickListener(v -> showThermalReceipt());

        // Setup Details RecyclerView
        detailsAdapter = new OrderDetailAdapter();
        binding.rvOrderDetails.setLayoutManager(new LinearLayoutManager(this));
        binding.rvOrderDetails.setAdapter(detailsAdapter);

        // Setup Payments RecyclerView
        paymentsAdapter = new OrderPaymentAdapter();
        binding.rvPayments.setLayoutManager(new LinearLayoutManager(this));
        binding.rvPayments.setAdapter(paymentsAdapter);

        // Observers
        viewModel.getLoading().observe(this, isLoading -> {
            if (isLoading != null) {
                binding.layoutLoading.setVisibility(isLoading ? View.VISIBLE : View.GONE);
                binding.layoutContent.setVisibility(isLoading ? View.GONE : View.VISIBLE);
            }
        });

        viewModel.getDetails().observe(this, details -> {
            if (details != null) {
                detailsAdapter.setDetails(details);
            }
        });

        viewModel.getPayments().observe(this, payments -> {
            if (payments != null && !payments.isEmpty()) {
                binding.tvEmptyPayments.setVisibility(View.GONE);
                binding.rvPayments.setVisibility(View.VISIBLE);
                paymentsAdapter.setPayments(payments);
            } else {
                binding.rvPayments.setVisibility(View.GONE);
                binding.tvEmptyPayments.setVisibility(View.VISIBLE);
            }
        });

        viewModel.getError().observe(this, msg -> {
            if (msg != null) {
                Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
            }
        });

        if (orderId > 0) {
            viewModel.loadOrderInfo(orderId);
        }
    }

    private void updateStatusUI(String orderStatus) {
        currentStatus = orderStatus;
        if ("Delivered".equalsIgnoreCase(orderStatus) || "Completed".equalsIgnoreCase(orderStatus) || "Paid".equalsIgnoreCase(orderStatus)) {
            binding.tvOrderStatus.setText("Hoàn thành");
            binding.tvOrderStatus.setBackgroundResource(R.drawable.bg_badge_success);
            binding.tvOrderStatus.setTextColor(getColor(R.color.on_primary_container));
            binding.btnChangeStatus.setVisibility(View.GONE); // Final state
        } else if ("Cancelled".equalsIgnoreCase(orderStatus)) {
            binding.tvOrderStatus.setText("Đã hủy");
            binding.tvOrderStatus.setBackgroundResource(R.drawable.bg_badge_error);
            binding.tvOrderStatus.setTextColor(getColor(R.color.on_error_container));
            binding.btnChangeStatus.setVisibility(View.GONE); // Final state
        } else if ("Shipping".equalsIgnoreCase(orderStatus) || "Processing".equalsIgnoreCase(orderStatus)) {
            binding.tvOrderStatus.setText("Đang giao hàng");
            binding.tvOrderStatus.setBackgroundResource(R.drawable.bg_badge_warning);
            binding.tvOrderStatus.setTextColor(getColor(R.color.on_secondary_container));
            binding.btnChangeStatus.setVisibility(View.VISIBLE);
        } else {
            binding.tvOrderStatus.setText("Chờ xử lý");
            binding.tvOrderStatus.setBackgroundResource(R.drawable.bg_badge_pending);
            binding.tvOrderStatus.setTextColor(getColor(R.color.on_secondary_container));
            binding.btnChangeStatus.setVisibility(View.VISIBLE);
        }
    }

    private void showChangeStatusDialog(int orderId) {
        String[] statuses = {"Processing (Đang giao/xử lý)", "Delivered (Đã hoàn thành giao hàng)", "Cancelled (Hủy đơn hàng)"};
        String[] statusKeys = {"Processing", "Delivered", "Cancelled"};

        new MaterialAlertDialogBuilder(this)
                .setTitle("Cập nhật trạng thái đơn hàng")
                .setItems(statuses, (dialog, which) -> {
                    String selectedStatus = statusKeys[which];
                    if ("Cancelled".equalsIgnoreCase(selectedStatus)) {
                        promptCancellationReason(orderId);
                    } else {
                        executeUpdateStatus(orderId, selectedStatus, null);
                    }
                })
                .setNegativeButton("Hủy", null)
                .show();
    }

    private void promptCancellationReason(int orderId) {
        EditText etReason = new EditText(this);
        etReason.setHint("Nhập lý do hủy đơn bắt buộc...");

        new AlertDialog.Builder(this)
                .setTitle("Lý do hủy đơn hàng")
                .setMessage("Vui lòng ghi rõ lý do hủy đơn để lưu vào nhật ký hệ thống:")
                .setView(etReason)
                .setPositiveButton("Xác nhận hủy", (dialog, which) -> {
                    String reason = etReason.getText().toString().trim();
                    if (reason.isEmpty()) {
                        Toast.makeText(OrderDetailActivity.this, "Lý do hủy đơn không được để trống!", Toast.LENGTH_SHORT).show();
                        return;
                    }
                    executeUpdateStatus(orderId, "Cancelled", reason);
                })
                .setNegativeButton("Đóng", null)
                .show();
    }

    private void executeUpdateStatus(int orderId, String selectedStatus, String reason) {
        new DataRepository(this).updateOrderStatus(orderId, selectedStatus, new DataRepository.ApiCallback<Boolean>() {
            @Override
            public void onSuccess(Boolean result) {
                Toast.makeText(OrderDetailActivity.this, "Cập nhật trạng thái thành công!", Toast.LENGTH_SHORT).show();
                updateStatusUI(selectedStatus);
            }

            @Override
            public void onError(String message) {
                Toast.makeText(OrderDetailActivity.this, "Lỗi: " + message, Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void showThermalReceipt() {
        List<OrderDetail> details = viewModel.getDetails().getValue();
        if (details == null) details = new ArrayList<>();

        DialogInvoiceReceiptBinding receiptBinding = DialogInvoiceReceiptBinding.inflate(LayoutInflater.from(this));
        AlertDialog receiptDialog = new AlertDialog.Builder(this)
                .setView(receiptBinding.getRoot())
                .create();

        receiptBinding.tvReceiptOrderCode.setText("Mã đơn: " + orderCode);
        receiptBinding.tvReceiptDateTime.setText(orderDate != null ? orderDate : "");
        receiptBinding.tvReceiptCustomer.setText("Khách hàng: " + (customerName != null ? customerName : "Khách vãng lai"));
        receiptBinding.tvReceiptCashier.setText("TN: Thu Ngân Mobile");

        // Dynamic item rows
        receiptBinding.layoutReceiptItemsList.removeAllViews();
        double subtotal = 0;
        for (OrderDetail item : details) {
            subtotal += item.getSubtotal();
            View row = LayoutInflater.from(this).inflate(R.layout.item_receipt_row, receiptBinding.layoutReceiptItemsList, false);
            TextView tvName = row.findViewById(R.id.tvReceiptItemName);
            TextView tvQty = row.findViewById(R.id.tvReceiptItemQty);
            TextView tvPrice = row.findViewById(R.id.tvReceiptItemPrice);

            tvName.setText(item.getProductName() != null ? item.getProductName() : "Sản phẩm #" + item.getProductId());
            tvQty.setText(String.valueOf(item.getQuantity()));
            tvPrice.setText(MoneyTextWatcher.formatCurrency(item.getSubtotal()));

            receiptBinding.layoutReceiptItemsList.addView(row);
        }

        receiptBinding.tvReceiptSubtotal.setText(MoneyTextWatcher.formatCurrency(subtotal > 0 ? subtotal : totalAmount));
        receiptBinding.tvReceiptTotal.setText(MoneyTextWatcher.formatCurrency(totalAmount));
        receiptBinding.rowReceiptDiscount.setVisibility(View.GONE);

        receiptBinding.tvReceiptPaymentMethodLabel.setText("Phương thức thanh toán:");
        receiptBinding.tvReceiptCustomerCash.setText("Tiền mặt / Chuyển khoản");
        receiptBinding.tvReceiptChange.setText("0 ₫");

        receiptBinding.btnFinishReceipt.setText("Đóng");
        receiptBinding.btnFinishReceipt.setOnClickListener(v -> receiptDialog.dismiss());

        receiptBinding.btnShareReceipt.setOnClickListener(v -> {
            Intent share = new Intent(Intent.ACTION_SEND);
            share.setType("text/plain");
            share.putExtra(Intent.EXTRA_SUBJECT, "Hóa đơn EMarket " + orderCode);
            share.putExtra(Intent.EXTRA_TEXT, "Hóa đơn siêu thị EMarket " + orderCode + " - Tổng tiền: " + MoneyTextWatcher.formatCurrency(totalAmount));
            startActivity(Intent.createChooser(share, "Chia sẻ hóa đơn"));
        });

        receiptDialog.show();
    }
}
