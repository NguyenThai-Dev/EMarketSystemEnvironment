package com.example.do_an_di_dong.ui.pos;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AlertDialog;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.bumptech.glide.Glide;
import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.databinding.DialogInvoiceReceiptBinding;
import com.example.do_an_di_dong.databinding.DialogPosPaymentBinding;
import com.example.do_an_di_dong.databinding.FragmentPosBinding;
import com.example.do_an_di_dong.model.CartItem;
import com.example.do_an_di_dong.model.CheckoutResult;
import com.example.do_an_di_dong.model.CreateQrRequest;
import com.example.do_an_di_dong.model.CreateQrResponse;
import com.example.do_an_di_dong.model.Customer;
import com.example.do_an_di_dong.model.PayOSStatusResponse;
import com.example.do_an_di_dong.model.Product;
import com.example.do_an_di_dong.ui.product.BarcodeScanActivity;
import com.example.do_an_di_dong.util.MoneyTextWatcher;
import com.google.android.material.dialog.MaterialAlertDialogBuilder;
import com.google.android.material.tabs.TabLayout;

import java.net.URLEncoder;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;

public class PosFragment extends Fragment {

    private FragmentPosBinding binding;
    private PosViewModel viewModel;
    private PosCartAdapter cartAdapter;
    private PosSuggestionAdapter suggestionAdapter;

    private final Handler searchHandler = new Handler(Looper.getMainLooper());
    private Runnable searchRunnable;

    private final Handler payosPollHandler = new Handler(Looper.getMainLooper());
    private Runnable payosPollRunnable;

    private final ActivityResultLauncher<Intent> barcodeLauncher =
            registerForActivityResult(new ActivityResultContracts.StartActivityForResult(), result -> {
                if (result.getResultCode() == Activity.RESULT_OK && result.getData() != null) {
                    String barcode = result.getData().getStringExtra("scanned_barcode");
                    if (barcode != null && !barcode.isEmpty()) {
                        viewModel.addByBarcode(barcode);
                    }
                }
            });

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        binding = FragmentPosBinding.inflate(inflater, container, false);
        return binding.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        viewModel = new ViewModelProvider(requireActivity()).get(PosViewModel.class);

        setupRecyclerViews();
        setupSearchAndScanner();
        setupCustomerAndLoyalty();
        setupActions();
        observeViewModel();
    }

    private void setupRecyclerViews() {
        cartAdapter = new PosCartAdapter();
        cartAdapter.setListener(new PosCartAdapter.OnCartItemChangeListener() {
            @Override
            public void onQuantityChanged(CartItem item, int newQuantity) {
                viewModel.updateItemQuantity(item, newQuantity);
            }

            @Override
            public void onItemRemoved(CartItem item) {
                viewModel.removeItem(item);
            }
        });

        binding.rvPosCart.setLayoutManager(new LinearLayoutManager(requireContext()));
        binding.rvPosCart.setAdapter(cartAdapter);

        suggestionAdapter = new PosSuggestionAdapter(product -> {
            viewModel.addProductToCart(product);
            binding.etPosSearch.setText("");
            binding.rvPosSuggestions.setVisibility(View.GONE);
        });

        binding.rvPosSuggestions.setLayoutManager(new LinearLayoutManager(requireContext()));
        binding.rvPosSuggestions.setAdapter(suggestionAdapter);
    }

    private void setupSearchAndScanner() {
        binding.btnPosBarcode.setOnClickListener(v -> {
            Intent intent = new Intent(requireContext(), BarcodeScanActivity.class);
            barcodeLauncher.launch(intent);
        });

        binding.btnPosClearSearch.setOnClickListener(v -> {
            binding.etPosSearch.setText("");
            binding.rvPosSuggestions.setVisibility(View.GONE);
            binding.btnPosClearSearch.setVisibility(View.GONE);
        });

        binding.etPosSearch.addTextChangedListener(new TextWatcher() {
            @Override public void beforeTextChanged(CharSequence s, int start, int count, int after) {}
            @Override public void onTextChanged(CharSequence s, int start, int before, int count) {
                binding.btnPosClearSearch.setVisibility(s.length() > 0 ? View.VISIBLE : View.GONE);
                if (searchRunnable != null) searchHandler.removeCallbacks(searchRunnable);
                searchRunnable = () -> {
                    String query = s.toString().trim();
                    if (query.length() >= 1) {
                        viewModel.searchProducts(query);
                    } else {
                        binding.rvPosSuggestions.setVisibility(View.GONE);
                    }
                };
                searchHandler.postDelayed(searchRunnable, 300);
            }
            @Override public void afterTextChanged(Editable s) {}
        });
    }

    private void setupCustomerAndLoyalty() {
        binding.btnPosSelectCustomer.setOnClickListener(v -> showSelectCustomerDialog());
        binding.chipPosCustomer.setOnCloseIconClickListener(v -> viewModel.clearCustomer());

        binding.switchPosUsePoints.setOnCheckedChangeListener((buttonView, isChecked) -> {
            viewModel.setUsePoints(isChecked);
        });
    }

    private void setupActions() {
        binding.btnPosClearCart.setOnClickListener(v -> {
            new MaterialAlertDialogBuilder(requireContext())
                    .setTitle("Xóa giỏ hàng")
                    .setMessage("Bạn có chắc chắn muốn dọn sạch toàn bộ món hàng trong giỏ?")
                    .setPositiveButton("Xóa hết", (dialog, which) -> viewModel.clearCart())
                    .setNegativeButton("Hủy", null)
                    .show();
        });

        binding.btnPosCheckout.setOnClickListener(v -> {
            List<CartItem> items = viewModel.getCart().getValue();
            if (items == null || items.isEmpty()) {
                Toast.makeText(requireContext(), "Giỏ hàng đang trống! Vui lòng chọn sản phẩm.", Toast.LENGTH_SHORT).show();
                return;
            }
            showPaymentDialog();
        });
    }

    private void observeViewModel() {
        viewModel.getCart().observe(getViewLifecycleOwner(), items -> {
            cartAdapter.setItems(items);
            boolean isEmpty = items == null || items.isEmpty();
            binding.layoutEmptyCart.setVisibility(isEmpty ? View.VISIBLE : View.GONE);
            binding.rvPosCart.setVisibility(isEmpty ? View.GONE : View.VISIBLE);
            binding.btnPosCheckout.setEnabled(!isEmpty);
        });

        viewModel.getSuggestions().observe(getViewLifecycleOwner(), suggestions -> {
            if (suggestions != null && !suggestions.isEmpty() && binding.etPosSearch.getText().length() > 0) {
                suggestionAdapter.setProducts(suggestions);
                binding.rvPosSuggestions.setVisibility(View.VISIBLE);
            } else {
                binding.rvPosSuggestions.setVisibility(View.GONE);
            }
        });

        viewModel.getFinalPayableAmount().observe(getViewLifecycleOwner(), payable -> {
            binding.tvPosTotalAmount.setText(MoneyTextWatcher.formatCurrency(payable));
        });

        viewModel.getTotalQuantity().observe(getViewLifecycleOwner(), qty -> {
            binding.tvPosItemCount.setText("Số lượng món: " + (qty != null ? qty : 0));
        });

        viewModel.getSelectedCustomer().observe(getViewLifecycleOwner(), customer -> {
            if (customer != null) {
                binding.btnPosSelectCustomer.setVisibility(View.GONE);
                binding.chipPosCustomer.setVisibility(View.VISIBLE);
                binding.chipPosCustomer.setText(customer.getFullName() + " (" + customer.getLoyaltyPoints() + " pts)");
                binding.switchPosUsePoints.setVisibility(View.VISIBLE);
            } else {
                binding.btnPosSelectCustomer.setVisibility(View.VISIBLE);
                binding.chipPosCustomer.setVisibility(View.GONE);
                binding.switchPosUsePoints.setVisibility(View.GONE);
                binding.switchPosUsePoints.setChecked(false);
            }
        });

        viewModel.getLoading().observe(getViewLifecycleOwner(), isLoading -> {
            binding.progressPosLoading.setVisibility(isLoading ? View.VISIBLE : View.GONE);
        });

        viewModel.getError().observe(getViewLifecycleOwner(), error -> {
            if (error != null && !error.isEmpty()) {
                Toast.makeText(requireContext(), error, Toast.LENGTH_LONG).show();
            }
        });
    }

    private void showSelectCustomerDialog() {
        EditText etSearch = new EditText(requireContext());
        etSearch.setHint("Nhập tên hoặc số điện thoại...");

        new MaterialAlertDialogBuilder(requireContext())
                .setTitle("Tìm kiếm khách hàng CRM")
                .setView(etSearch)
                .setPositiveButton("Tìm", (dialog, which) -> {
                    String query = etSearch.getText().toString().trim();
                    if (!query.isEmpty()) {
                        searchCustomerApi(query);
                    }
                })
                .setNegativeButton("Đóng", null)
                .show();
    }

    private void searchCustomerApi(String query) {
        new DataRepository(requireContext()).searchCustomers(query, new DataRepository.ApiCallback<List<Customer>>() {
            @Override
            public void onSuccess(List<Customer> result) {
                if (result != null && !result.isEmpty()) {
                    String[] names = new String[result.size()];
                    for (int i = 0; i < result.size(); i++) {
                        Customer c = result.get(i);
                        names[i] = c.getFullName() + " - " + c.getPhone() + " (" + c.getLoyaltyPoints() + " pts)";
                    }

                    new MaterialAlertDialogBuilder(requireContext())
                            .setTitle("Chọn khách hàng")
                            .setItems(names, (dialog, which) -> {
                                viewModel.selectCustomer(result.get(which));
                            })
                            .show();
                } else {
                    Toast.makeText(requireContext(), "Không tìm thấy khách hàng nào", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onError(String message) {
                Toast.makeText(requireContext(), "Lỗi: " + message, Toast.LENGTH_SHORT).show();
            }
        });
    }

    // =========================================================================
    // MULTI-TENDER POS PAYMENT & REAL-TIME PAYOS / VIETQR WORKFLOW
    // =========================================================================

    private void showPaymentDialog() {
        double payableAmount = viewModel.getFinalPayableAmount().getValue() != null ? viewModel.getFinalPayableAmount().getValue() : 0.0;

        DialogPosPaymentBinding payBinding = DialogPosPaymentBinding.inflate(LayoutInflater.from(requireContext()));
        AlertDialog dialog = new MaterialAlertDialogBuilder(requireContext())
                .setView(payBinding.getRoot())
                .setCancelable(false)
                .create();

        payBinding.tvPosPayableAmount.setText(MoneyTextWatcher.formatCurrency(payableAmount));

        final String[] paymentMethod = {"Cash"};
        final long[] currentPayosOrderCode = {0};

        // Tab selection
        payBinding.tabPosPaymentMethods.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                if (tab.getPosition() == 0) {
                    paymentMethod[0] = "Cash";
                    stopPayosPolling();
                    payBinding.layoutCashPayment.setVisibility(View.VISIBLE);
                    payBinding.layoutQrPayment.setVisibility(View.GONE);
                } else {
                    paymentMethod[0] = "BankTransfer";
                    payBinding.layoutCashPayment.setVisibility(View.GONE);
                    payBinding.layoutQrPayment.setVisibility(View.VISIBLE);
                    loadVietQrPayOS(payBinding, (long) payableAmount, currentPayosOrderCode, dialog);
                }
            }
            @Override public void onTabUnselected(TabLayout.Tab tab) {}
            @Override public void onTabReselected(TabLayout.Tab tab) {}
        });

        // Quick cash denomination buttons
        payBinding.btnDenom50k.setOnClickListener(v -> setCashAmount(payBinding, 50000, payableAmount));
        payBinding.btnDenom100k.setOnClickListener(v -> setCashAmount(payBinding, 100000, payableAmount));
        payBinding.btnDenom200k.setOnClickListener(v -> setCashAmount(payBinding, 200000, payableAmount));
        payBinding.btnDenom500k.setOnClickListener(v -> setCashAmount(payBinding, 500000, payableAmount));

        // Real-time money format & change calculation
        payBinding.etCustomerCash.addTextChangedListener(new MoneyTextWatcher(payBinding.etCustomerCash, cleanVal -> {
            double givenCash = cleanVal.isEmpty() ? 0 : Double.parseDouble(cleanVal);
            double change = givenCash - payableAmount;
            if (change >= 0) {
                payBinding.tvPosChangeAmount.setText(MoneyTextWatcher.formatCurrency(change));
                payBinding.tvPosChangeAmount.setTextColor(requireContext().getColor(R.color.on_primary_container));
            } else {
                payBinding.tvPosChangeAmount.setText("Chưa đủ tiền mặt");
                payBinding.tvPosChangeAmount.setTextColor(requireContext().getColor(R.color.error));
            }
        }));

        // Check PayOS status manually
        payBinding.btnCheckPayosStatus.setOnClickListener(v -> {
            if (currentPayosOrderCode[0] > 0) {
                checkPayosManualStatus(payBinding, currentPayosOrderCode[0], payableAmount, dialog);
            } else {
                Toast.makeText(requireContext(), "Chưa khởi tạo mã đơn PayOS", Toast.LENGTH_SHORT).show();
            }
        });

        payBinding.btnCancelPosPayment.setOnClickListener(v -> {
            stopPayosPolling();
            dialog.dismiss();
        });

        payBinding.btnConfirmPosPayment.setOnClickListener(v -> {
            if ("Cash".equals(paymentMethod[0])) {
                String cashStr = payBinding.etCustomerCash.getText() != null ? payBinding.etCustomerCash.getText().toString().replaceAll("[^0-9]", "") : "";
                double givenCash = cashStr.isEmpty() ? 0 : Double.parseDouble(cashStr);
                if (givenCash < payableAmount) {
                    Toast.makeText(requireContext(), "Số tiền khách đưa chưa đủ để thanh toán!", Toast.LENGTH_SHORT).show();
                    return;
                }
                executeCheckout("Cash", givenCash, givenCash - payableAmount, dialog);
            } else {
                // BankTransfer (PayOS) manual confirmation
                executeCheckout("BankTransfer", payableAmount, 0, dialog);
            }
        });

        dialog.show();
    }

    private void setCashAmount(DialogPosPaymentBinding payBinding, long amount, double payableAmount) {
        payBinding.etCustomerCash.setText(String.valueOf(amount));
        double change = amount - payableAmount;
        if (change >= 0) {
            payBinding.tvPosChangeAmount.setText(MoneyTextWatcher.formatCurrency(change));
            payBinding.tvPosChangeAmount.setTextColor(requireContext().getColor(R.color.on_primary_container));
        } else {
            payBinding.tvPosChangeAmount.setText("Chưa đủ tiền mặt");
            payBinding.tvPosChangeAmount.setTextColor(requireContext().getColor(R.color.error));
        }
    }

    private void loadVietQrPayOS(DialogPosPaymentBinding payBinding, long amount, long[] orderCodeHolder, AlertDialog parentDialog) {
        stopPayosPolling();
        payBinding.pbQrLoading.setVisibility(View.VISIBLE);
        payBinding.ivPayosQrCode.setVisibility(View.GONE);
        payBinding.tvQrError.setVisibility(View.GONE);
        payBinding.tvQrStatusMessage.setText("Đang khởi tạo mã giao dịch VietQR PayOS...");

        CreateQrRequest request = new CreateQrRequest((int) amount, "POS " + System.currentTimeMillis());

        viewModel.createPayOSLink(request, new DataRepository.ApiCallback<CreateQrResponse>() {
            @Override
            public void onSuccess(CreateQrResponse response) {
                if (!isAdded()) return;
                payBinding.pbQrLoading.setVisibility(View.GONE);

                if (response != null && response.isSuccess() && response.getQrCode() != null) {
                    payBinding.ivPayosQrCode.setVisibility(View.VISIBLE);
                    orderCodeHolder[0] = response.getOrderCode();
                    payBinding.tvQrOrderCode.setText("Mã GD PayOS: #" + response.getOrderCode());
                    payBinding.tvQrStatusMessage.setText("Đang chờ khách chuyển khoản... Hệ thống sẽ tự động tạo đơn khi nhận tiền.");

                    String qrString = response.getQrCode();
                    String renderUrl;
                    try {
                        if (qrString.startsWith("http")) {
                            renderUrl = qrString;
                        } else {
                            renderUrl = "https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=" + URLEncoder.encode(qrString, "UTF-8");
                        }
                        Glide.with(requireContext()).load(renderUrl).into(payBinding.ivPayosQrCode);
                    } catch (Exception e) {
                        Glide.with(requireContext()).load(qrString).into(payBinding.ivPayosQrCode);
                    }

                    // Start automatic Real-Time Polling loop
                    startPayosPolling(payBinding, response.getOrderCode(), amount, parentDialog);

                } else {
                    String msg = response != null && response.getMessage() != null ? response.getMessage() : "Lỗi khởi tạo cổng PayOS";
                    payBinding.tvQrError.setText(msg);
                    payBinding.tvQrError.setVisibility(View.VISIBLE);
                    payBinding.tvQrStatusMessage.setText("Không thể tải mã VietQR");
                }
            }

            @Override
            public void onError(String message) {
                if (!isAdded()) return;
                payBinding.pbQrLoading.setVisibility(View.GONE);
                payBinding.tvQrError.setText("Lỗi kết nối PayOS: " + message);
                payBinding.tvQrError.setVisibility(View.VISIBLE);
                payBinding.tvQrStatusMessage.setText("Lỗi mạng khi gọi PayOS");
            }
        });
    }

    private void startPayosPolling(DialogPosPaymentBinding payBinding, long orderCode, double payableAmount, AlertDialog parentDialog) {
        stopPayosPolling();
        payosPollRunnable = new Runnable() {
            @Override
            public void run() {
                if (!isAdded() || parentDialog == null || !parentDialog.isShowing()) {
                    stopPayosPolling();
                    return;
                }

                viewModel.checkPayOSStatus(orderCode, new DataRepository.ApiCallback<PayOSStatusResponse>() {
                    @Override
                    public void onSuccess(PayOSStatusResponse response) {
                        if (!isAdded() || parentDialog == null || !parentDialog.isShowing()) return;

                        if (response != null && response.isPaid()) {
                            stopPayosPolling();
                            payBinding.tvQrStatusMessage.setText("✓ ĐÃ NHẬN TIỀN QUA VIETQR PAYOS! Đang tạo đơn hàng...");
                            payBinding.tvQrStatusMessage.setTextColor(requireContext().getColor(R.color.on_primary_container));
                            payBinding.btnConfirmPosPayment.setEnabled(false);
                            payBinding.btnCancelPosPayment.setEnabled(false);

                            Toast.makeText(requireContext(), "Xác nhận đã nhận tiền PayOS! Tự động tạo đơn hàng...", Toast.LENGTH_SHORT).show();

                            // Short delay as requested before auto-checkout
                            new Handler(Looper.getMainLooper()).postDelayed(() -> {
                                if (!isAdded() || !parentDialog.isShowing()) return;
                                executeCheckout("BankTransfer", payableAmount, 0, parentDialog);
                            }, 800);
                        } else {
                            // Poll again in 2.5 seconds
                            payosPollHandler.postDelayed(payosPollRunnable, 2500);
                        }
                    }

                    @Override
                    public void onError(String message) {
                        if (!isAdded() || parentDialog == null || !parentDialog.isShowing()) return;
                        // Retry polling in 3 seconds
                        payosPollHandler.postDelayed(payosPollRunnable, 3000);
                    }
                });
            }
        };

        // First poll check after 2.5 seconds
        payosPollHandler.postDelayed(payosPollRunnable, 2500);
    }

    private void stopPayosPolling() {
        if (payosPollRunnable != null) {
            payosPollHandler.removeCallbacks(payosPollRunnable);
            payosPollRunnable = null;
        }
    }

    private void checkPayosManualStatus(DialogPosPaymentBinding payBinding, long orderCode, double payableAmount, AlertDialog parentDialog) {
        payBinding.tvQrStatusMessage.setText("Đang kiểm tra với cổng PayOS...");
        viewModel.checkPayOSStatus(orderCode, new DataRepository.ApiCallback<PayOSStatusResponse>() {
            @Override
            public void onSuccess(PayOSStatusResponse response) {
                if (!isAdded()) return;
                if (response != null && response.isPaid()) {
                    stopPayosPolling();
                    payBinding.tvQrStatusMessage.setText("✓ ĐÃ NHẬN TIỀN QUA PAYOS! Đang tạo đơn hàng...");
                    payBinding.tvQrStatusMessage.setTextColor(requireContext().getColor(R.color.on_primary_container));
                    Toast.makeText(requireContext(), "Xác nhận đã thanh toán PayOS thành công! Tự động tạo đơn...", Toast.LENGTH_SHORT).show();

                    new Handler(Looper.getMainLooper()).postDelayed(() -> {
                        if (!isAdded() || !parentDialog.isShowing()) return;
                        executeCheckout("BankTransfer", payableAmount, 0, parentDialog);
                    }, 600);
                } else {
                    payBinding.tvQrStatusMessage.setText("Chưa nhận được tiền (Trạng thái: PENDING)");
                    Toast.makeText(requireContext(), "Chưa phát hiện giao dịch thành công cho đơn #" + orderCode, Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onError(String message) {
                if (!isAdded()) return;
                payBinding.tvQrStatusMessage.setText("Lỗi kiểm tra: " + message);
            }
        });
    }

    private void executeCheckout(String paymentMethod, double givenCash, double change, AlertDialog parentDialog) {
        stopPayosPolling();

        // Capture cart and pricing snapshots before clearing
        List<CartItem> cartSnapshot = new ArrayList<>();
        if (viewModel.getCart().getValue() != null) {
            cartSnapshot.addAll(viewModel.getCart().getValue());
        }
        double rawTotal = viewModel.getRawTotalAmount().getValue() != null ? viewModel.getRawTotalAmount().getValue() : 0.0;
        double discount = viewModel.getTotalDiscount().getValue() != null ? viewModel.getTotalDiscount().getValue() : 0.0;
        double payable = viewModel.getFinalPayableAmount().getValue() != null ? viewModel.getFinalPayableAmount().getValue() : 0.0;
        Customer customerSnapshot = viewModel.getSelectedCustomer().getValue();

        viewModel.checkout(paymentMethod, new DataRepository.ApiCallback<CheckoutResult>() {
            @Override
            public void onSuccess(CheckoutResult result) {
                if (!isAdded()) return;
                if (parentDialog != null && parentDialog.isShowing()) {
                    parentDialog.dismiss();
                }

                // 1. Tự động xóa giỏ hàng
                viewModel.clearCart();

                // 2. Tự động in bill / hiển thị hóa đơn ra màn hình
                showThermalReceipt(result, paymentMethod, givenCash, change, cartSnapshot, rawTotal, discount, payable, customerSnapshot);
            }

            @Override
            public void onError(String message) {
                if (!isAdded()) return;
                Toast.makeText(requireContext(), "Lỗi thanh toán: " + message, Toast.LENGTH_LONG).show();
            }
        });
    }

    private void showThermalReceipt(CheckoutResult result, String paymentMethod, double givenCash, double change,
                                    List<CartItem> items, double rawTotal, double discount, double payable, Customer customer) {
        DialogInvoiceReceiptBinding receiptBinding = DialogInvoiceReceiptBinding.inflate(LayoutInflater.from(requireContext()));
        AlertDialog receiptDialog = new AlertDialog.Builder(requireContext())
                .setView(receiptBinding.getRoot())
                .setCancelable(false)
                .create();

        int realOrderId = result != null ? result.getOrderId() : 0;
        receiptBinding.tvReceiptOrderCode.setText("Mã đơn: #ORD-" + realOrderId);

        SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss", Locale.getDefault());
        receiptBinding.tvReceiptDateTime.setText("Ngày: " + sdf.format(new Date()));

        receiptBinding.tvReceiptCustomer.setText("Khách hàng: " + (customer != null ? customer.getFullName() : "Khách vãng lai"));
        String cashierName = com.example.do_an_di_dong.util.UserSessionManager.getCurrentUserFullName(requireContext());
        receiptBinding.tvReceiptCashier.setText("Thu ngân: " + cashierName);

        // Items list
        receiptBinding.layoutReceiptItemsList.removeAllViews();
        if (items != null) {
            for (CartItem item : items) {
                View row = LayoutInflater.from(requireContext()).inflate(R.layout.item_receipt_row, receiptBinding.layoutReceiptItemsList, false);
                TextView tvName = row.findViewById(R.id.tvReceiptItemName);
                TextView tvQty = row.findViewById(R.id.tvReceiptItemQty);
                TextView tvPrice = row.findViewById(R.id.tvReceiptItemPrice);

                tvName.setText(item.getProductName());
                tvQty.setText(String.valueOf(item.getQuantity()));
                tvPrice.setText(MoneyTextWatcher.formatCurrency(item.getSubtotal()));

                receiptBinding.layoutReceiptItemsList.addView(row);
            }
        }

        receiptBinding.tvReceiptSubtotal.setText(MoneyTextWatcher.formatCurrency(rawTotal));
        if (discount > 0) {
            receiptBinding.rowReceiptDiscount.setVisibility(View.VISIBLE);
            receiptBinding.tvReceiptDiscount.setText("-" + MoneyTextWatcher.formatCurrency(discount));
        } else {
            receiptBinding.rowReceiptDiscount.setVisibility(View.GONE);
        }
        receiptBinding.tvReceiptTotal.setText(MoneyTextWatcher.formatCurrency(payable));

        receiptBinding.tvReceiptPaymentMethodLabel.setText("Phương thức: " + ("Cash".equals(paymentMethod) ? "Tiền mặt" : "Chuyển khoản VietQR PayOS"));
        receiptBinding.tvReceiptCustomerCash.setText(MoneyTextWatcher.formatCurrency(givenCash));
        receiptBinding.tvReceiptChange.setText(MoneyTextWatcher.formatCurrency(change));

        receiptBinding.btnFinishReceipt.setOnClickListener(v -> {
            receiptDialog.dismiss();
            Toast.makeText(requireContext(), "Đã hoàn tất đơn hàng #" + realOrderId, Toast.LENGTH_SHORT).show();
        });

        receiptBinding.btnShareReceipt.setOnClickListener(v -> {
            Intent share = new Intent(Intent.ACTION_SEND);
            share.setType("text/plain");
            share.putExtra(Intent.EXTRA_SUBJECT, "Hóa đơn EMarket #ORD-" + realOrderId);
            share.putExtra(Intent.EXTRA_TEXT, "Hóa đơn siêu thị EMarket #ORD-" + realOrderId + " - Tổng thanh toán: " + MoneyTextWatcher.formatCurrency(payable));
            startActivity(Intent.createChooser(share, "Chia sẻ hóa đơn"));
        });

        receiptDialog.show();
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        stopPayosPolling();
        binding = null;
    }
}
