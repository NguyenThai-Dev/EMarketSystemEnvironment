package com.example.do_an_di_dong.ui.auth;

import android.accounts.Account;
import android.accounts.AccountManager;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.do_an_di_dong.MainActivity;
import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.data.api.ApiClient;
import com.example.do_an_di_dong.data.repository.DataRepository;
import com.example.do_an_di_dong.databinding.ActivityLoginBinding;
import com.example.do_an_di_dong.model.LoginResponse;
import com.google.android.material.bottomsheet.BottomSheetDialog;
import com.google.android.material.textfield.TextInputEditText;
import com.google.android.material.textfield.TextInputLayout;

public class LoginActivity extends AppCompatActivity {
    private ActivityLoginBinding binding;
    private DataRepository repository;
    private SharedPreferences prefs;

    private static final String PREFS_BIOMETRICS = "BiometricPrefs";
    private static final String KEY_LAST_GOOGLE_EMAIL = "last_google_email";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityLoginBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        repository = new DataRepository(this);
        prefs = getSharedPreferences(PREFS_BIOMETRICS, MODE_PRIVATE);

        // Auto login if token exists (credentials already validated and session is persistent)
        String token = ApiClient.getToken(this);
        if (token != null) {
            navigateToMain();
            return;
        }

        setupTextWatchers();

        binding.btnLogin.setOnClickListener(v -> handlePasswordLogin());
        binding.btnGoogleLogin.setOnClickListener(v -> handleGoogleLogin());
        binding.btnServerSettings.setOnClickListener(v -> showServerSettingsDialog());
    }

    private void showServerSettingsDialog() {
        String currentUrl = ApiClient.getBaseUrl(this);
        String[] options = {
                "Localhost (54446): http://localhost:54446/",
                "Android Emulator: http://10.0.2.2:54446/",
                "Tự nhập địa chỉ IP khác..."
        };

        new com.google.android.material.dialog.MaterialAlertDialogBuilder(this)
                .setTitle("Cấu hình máy chủ Backend")
                .setMessage("Địa chỉ hiện tại: " + currentUrl)
                .setItems(options, (dialog, which) -> {
                    if (which == 0) {
                        ApiClient.setBaseUrl(this, ApiClient.DEFAULT_EMARKET_BASE_URL);
                        Toast.makeText(this, "Đã chuyển sang Localhost: 54446", Toast.LENGTH_SHORT).show();
                    } else if (which == 1) {
                        ApiClient.setBaseUrl(this, ApiClient.EMULATOR_EMARKET_BASE_URL);
                        Toast.makeText(this, "Đã chuyển sang Máy ảo (10.0.2.2:54446)", Toast.LENGTH_SHORT).show();
                    } else {
                        showCustomIpInputDialog();
                    }
                })
                .setNegativeButton("Đóng", null)
                .show();
    }

    private void showCustomIpInputDialog() {
        android.widget.EditText etUrl = new android.widget.EditText(this);
        etUrl.setHint("http://192.168.1.xxx:54446/");
        etUrl.setText(ApiClient.getBaseUrl(this));

        new androidx.appcompat.app.AlertDialog.Builder(this)
                .setTitle("Nhập URL Backend tùy chỉnh")
                .setView(etUrl)
                .setPositiveButton("Lưu", (dialog, which) -> {
                    String url = etUrl.getText().toString().trim();
                    if (!url.isEmpty()) {
                        ApiClient.setBaseUrl(this, url);
                        Toast.makeText(this, "Đã lưu URL: " + url, Toast.LENGTH_SHORT).show();
                    }
                })
                .setNegativeButton("Hủy", null)
                .show();
    }

    private void setupTextWatchers() {
        binding.etEmail.addTextChangedListener(new TextWatcher() {
            @Override public void beforeTextChanged(CharSequence s, int start, int count, int after) {}
            @Override public void onTextChanged(CharSequence s, int start, int before, int count) {}
            @Override public void afterTextChanged(Editable s) {
                if (binding.tilEmail.getError() != null) {
                    binding.tilEmail.setError(null);
                }
            }
        });

        binding.etPassword.addTextChangedListener(new TextWatcher() {
            @Override public void beforeTextChanged(CharSequence s, int start, int count, int after) {}
            @Override public void onTextChanged(CharSequence s, int start, int before, int count) {}
            @Override public void afterTextChanged(Editable s) {
                if (binding.tilPassword.getError() != null) {
                    binding.tilPassword.setError(null);
                }
            }
        });
    }

    private boolean validateEmail(String email) {
        if (email.isEmpty()) {
            binding.tilEmail.setError("Tài khoản hoặc email không được để trống");
            return false;
        } else {
            if (binding.tilEmail.getError() != null) binding.tilEmail.setError(null);
            return true;
        }
    }

    private boolean validatePassword(String password) {
        if (password.isEmpty()) {
            binding.tilPassword.setError("Mật khẩu không được để trống");
            return false;
        } else if (password.length() < 6) {
            binding.tilPassword.setError("Mật khẩu phải chứa ít nhất 6 ký tự");
            return false;
        } else {
            if (binding.tilPassword.getError() != null) binding.tilPassword.setError(null);
            return true;
        }
    }

    private void handlePasswordLogin() {
        String username = binding.etEmail.getText().toString().trim();
        String password = binding.etPassword.getText().toString().trim();

        boolean isEmailValid = validateEmail(username);
        boolean isPasswordValid = validatePassword(password);

        if (!isEmailValid || !isPasswordValid) {
            Toast.makeText(this, "Vui lòng sửa các thông tin chưa hợp lệ.", Toast.LENGTH_SHORT).show();
            return;
        }

        // Show full screen loading
        binding.layoutProgress.setVisibility(View.VISIBLE);

        repository.login(username, password, new DataRepository.ApiCallback<LoginResponse>() {
            @Override
            public void onSuccess(LoginResponse result) {
                binding.layoutProgress.setVisibility(View.GONE);
                
                String fullName = "Manager";
                if (result != null && result.getUser() != null) {
                    com.example.do_an_di_dong.util.UserSessionManager.saveUserSession(LoginActivity.this, result.getUser());
                    if (result.getUser().getFullName() != null) {
                        fullName = result.getUser().getFullName();
                    }
                }

                // Save user full name
                SharedPreferences.Editor editor = prefs.edit();
                editor.putString("user_fullname", fullName);
                editor.apply();

                Toast.makeText(LoginActivity.this, "Đăng nhập thành công!", Toast.LENGTH_SHORT).show();
                navigateToMain();
            }

            @Override
            public void onError(String message) {
                binding.layoutProgress.setVisibility(View.GONE);
                
                // Map connection/auth issues to human readable messages
                String userFriendlyMessage = message;
                if (message.contains("401") || message.contains("Unauthorized")) {
                    userFriendlyMessage = "Tài khoản hoặc mật khẩu không chính xác.";
                } else if (message.contains("ConnectException") || message.contains("timeout") || message.contains("Unable to resolve host")) {
                    userFriendlyMessage = "Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại mạng hoặc IP backend.";
                } else if (message.contains("CSRF") || message.contains("verification token")) {
                    userFriendlyMessage = "Lỗi xác thực bảo mật CSRF. Vui lòng thử lại.";
                }
                
                Toast.makeText(LoginActivity.this, "Lỗi đăng nhập: " + userFriendlyMessage, Toast.LENGTH_LONG).show();
            }
        });
    }

    private String detectAvailableGoogleAccount() {
        // 1. Ưu tiên tài khoản đã đăng nhập thành công trước đó
        String lastEmail = prefs.getString(KEY_LAST_GOOGLE_EMAIL, null);
        if (lastEmail != null && !lastEmail.trim().isEmpty()) {
            return lastEmail.trim();
        }

        // 2. Thử đọc tài khoản Google trên hệ điều hành máy
        try {
            Account[] accounts = AccountManager.get(this).getAccountsByType("com.google");
            if (accounts != null && accounts.length > 0 && accounts[0].name != null) {
                return accounts[0].name.trim();
            }
        } catch (Exception ignored) {
        }
        return null;
    }

    private void handleGoogleLogin() {
        // Hiển thị trực tiếp Google Account Picker BottomSheet cao cấp
        showGoogleSignInDialog();
    }

    private void showGoogleSignInDialog() {
        BottomSheetDialog bottomSheetDialog = new BottomSheetDialog(this, com.google.android.material.R.style.Theme_Design_BottomSheetDialog);
        View dialogView = getLayoutInflater().inflate(R.layout.dialog_google_signin, null);
        bottomSheetDialog.setContentView(dialogView);

        View cardQuickAccount = dialogView.findViewById(R.id.cardQuickAccount);
        TextView tvAccountEmail = dialogView.findViewById(R.id.tvAccountEmail);
        TextView tvAccountInitial = dialogView.findViewById(R.id.tvAccountInitial);
        View btnSwitchAccount = dialogView.findViewById(R.id.btnSwitchAccount);
        TextView tvSwitchAccountLabel = dialogView.findViewById(R.id.tvSwitchAccountLabel);
        View layoutOtherAccountInput = dialogView.findViewById(R.id.layoutOtherAccountInput);
        TextInputLayout tilCustomEmail = dialogView.findViewById(R.id.tilCustomEmail);
        TextInputEditText etCustomEmail = dialogView.findViewById(R.id.etCustomEmail);
        View btnSubmitCustomEmail = dialogView.findViewById(R.id.btnSubmitCustomEmail);
        View btnCloseDialog = dialogView.findViewById(R.id.btnCloseDialog);

        if (btnCloseDialog != null) {
            btnCloseDialog.setOnClickListener(v -> bottomSheetDialog.dismiss());
        }

        String detectedAccount = detectAvailableGoogleAccount();

        if (detectedAccount != null && !detectedAccount.isEmpty()) {
            if (cardQuickAccount != null) cardQuickAccount.setVisibility(View.VISIBLE);
            if (tvAccountEmail != null) tvAccountEmail.setText(detectedAccount);
            if (tvAccountInitial != null) {
                String initial = detectedAccount.substring(0, 1).toUpperCase();
                tvAccountInitial.setText(initial);
            }
            if (cardQuickAccount != null) {
                cardQuickAccount.setOnClickListener(v -> {
                    bottomSheetDialog.dismiss();
                    performGoogleLogin(detectedAccount);
                });
            }
            if (layoutOtherAccountInput != null) layoutOtherAccountInput.setVisibility(View.GONE);
            if (btnSwitchAccount != null) {
                btnSwitchAccount.setOnClickListener(v -> {
                    if (layoutOtherAccountInput != null) {
                        boolean isVisible = layoutOtherAccountInput.getVisibility() == View.VISIBLE;
                        layoutOtherAccountInput.setVisibility(isVisible ? View.GONE : View.VISIBLE);
                        if (tvSwitchAccountLabel != null) {
                            tvSwitchAccountLabel.setText(isVisible ? "Sử dụng một tài khoản khác" : "Đóng nhập tài khoản khác");
                        }
                    }
                });
            }
        } else {
            // Chưa có tài khoản nào được phát hiện -> mở trực tiếp khung nhập
            if (cardQuickAccount != null) cardQuickAccount.setVisibility(View.GONE);
            if (btnSwitchAccount != null) btnSwitchAccount.setVisibility(View.GONE);
            if (layoutOtherAccountInput != null) layoutOtherAccountInput.setVisibility(View.VISIBLE);
        }

        if (btnSubmitCustomEmail != null) {
            btnSubmitCustomEmail.setOnClickListener(v -> {
                String email = etCustomEmail != null && etCustomEmail.getText() != null
                        ? etCustomEmail.getText().toString().trim()
                        : "";

                if (email.isEmpty()) {
                    if (tilCustomEmail != null) tilCustomEmail.setError("Vui lòng nhập địa chỉ Gmail");
                    return;
                }
                if (!android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
                    if (tilCustomEmail != null) tilCustomEmail.setError("Địa chỉ email không đúng định dạng");
                    return;
                }
                if (tilCustomEmail != null) {
                    tilCustomEmail.setError(null);
                }
                bottomSheetDialog.dismiss();
                performGoogleLogin(email);
            });
        }

        bottomSheetDialog.show();
    }

    private void performGoogleLogin(String email) {
        binding.layoutProgress.setVisibility(View.VISIBLE);

        repository.externalLogin("Google", email, null, new DataRepository.ApiCallback<LoginResponse>() {
            @Override
            public void onSuccess(LoginResponse result) {
                binding.layoutProgress.setVisibility(View.GONE);

                String fullName = "Google User";
                if (result != null && result.getUser() != null) {
                    com.example.do_an_di_dong.util.UserSessionManager.saveUserSession(LoginActivity.this, result.getUser());
                    if (result.getUser().getFullName() != null && !result.getUser().getFullName().isEmpty()) {
                        fullName = result.getUser().getFullName();
                    }
                }

                // Ghi nhớ tài khoản Google vừa đăng nhập thành công cho lần sau
                SharedPreferences.Editor editor = prefs.edit();
                editor.putString("user_fullname", fullName);
                editor.putString(KEY_LAST_GOOGLE_EMAIL, email);
                editor.apply();

                Toast.makeText(LoginActivity.this, "Đăng nhập Google thành công! Xin chào " + fullName, Toast.LENGTH_SHORT).show();
                navigateToMain();
            }

            @Override
            public void onError(String message) {
                binding.layoutProgress.setVisibility(View.GONE);

                String userFriendlyMessage = message;
                if (message.contains("ConnectException") || message.contains("timeout") || message.contains("Unable to resolve host")) {
                    userFriendlyMessage = "Không thể kết nối đến máy chủ EMarket. Vui lòng kiểm tra lại mạng hoặc IP backend.";
                }

                Toast.makeText(LoginActivity.this, "Đăng nhập Google thất bại: " + userFriendlyMessage, Toast.LENGTH_LONG).show();
            }
        });
    }

    private void navigateToMain() {
        Intent intent = new Intent(LoginActivity.this, MainActivity.class);
        startActivity(intent);
        finish();
    }
}
