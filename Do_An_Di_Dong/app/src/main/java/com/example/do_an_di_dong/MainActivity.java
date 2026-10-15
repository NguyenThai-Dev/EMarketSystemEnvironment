package com.example.do_an_di_dong;

import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.view.WindowInsets;
import android.view.WindowManager;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentTransaction;

import com.example.do_an_di_dong.data.api.ApiClient;
import com.example.do_an_di_dong.databinding.ActivityMainBinding;
import com.example.do_an_di_dong.ui.approval.ApprovalFragment;
import com.example.do_an_di_dong.ui.auth.LoginActivity;
import com.example.do_an_di_dong.ui.chat.FloatingChatManager;
import com.example.do_an_di_dong.ui.dashboard.DashboardFragment;
import com.example.do_an_di_dong.ui.pos.PosFragment;
import com.example.do_an_di_dong.ui.product.ProductsFragment;
import com.example.do_an_di_dong.ui.sales.OrdersFragment;

public class MainActivity extends AppCompatActivity {
    private ActivityMainBinding binding;
    private int currentNavId = R.id.nav_dashboard;

    /** Quản lý floating AI chat bubble + panel via WindowManager */
    private FloatingChatManager floatingChatManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityMainBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        // Load default fragment on first create
        if (savedInstanceState == null) {
            loadFragment(new DashboardFragment(), false);
        }

        // Ẩn thanh trạng thái (Status Bar)
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
            getWindow().getInsetsController().hide(WindowInsets.Type.statusBars());
        } else {
            getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                    WindowManager.LayoutParams.FLAG_FULLSCREEN);
        }

        // ─── Floating AI Chat (WindowManager overlay) ───
        floatingChatManager = new FloatingChatManager(this, this, this);
        floatingChatManager.show(); // Hiển thị bubble ngay khi app mở

        // ─── Bottom Navigation ───
        binding.bottomNavigation.setOnItemSelectedListener(item -> {
            int newId = item.getItemId();
            if (newId == currentNavId) return false;

            Fragment fragment = null;
            if (newId == R.id.nav_dashboard) {
                fragment = new DashboardFragment();
            } else if (newId == R.id.nav_products) {
                fragment = new ProductsFragment();
            } else if (newId == R.id.nav_orders) {
                fragment = new OrdersFragment();
            } else if (newId == R.id.nav_approval) {
                fragment = new ApprovalFragment();
            } else if (newId == R.id.nav_pos) {
                fragment = new PosFragment();
            }

            if (fragment != null) {
                boolean isForward = getNavOrder(newId) > getNavOrder(currentNavId);
                currentNavId = newId;
                loadFragment(fragment, isForward);
                return true;
            }
            return false;
        });

        // ─── Real-time SignalR Integration ───
        setupSignalRRealtime();

        // ─── RBAC Bottom Navigation Filtering ───
        applyRbacToBottomNavigation();
    }

    @Override
    protected void onResume() {
        super.onResume();
        applyRbacToBottomNavigation();
    }

    private void applyRbacToBottomNavigation() {
        if (binding == null || binding.bottomNavigation == null) return;
        android.view.Menu menu = binding.bottomNavigation.getMenu();

        boolean hasProduct = com.example.do_an_di_dong.util.UserSessionManager.hasPermission(this, "ProductModule");
        boolean hasSales = com.example.do_an_di_dong.util.UserSessionManager.hasPermission(this, "SalesModule");
        boolean hasApproval = com.example.do_an_di_dong.util.UserSessionManager.hasAnyPermission(this, "ExpenseModule", "QuotationModule", "ApprovalModule");
        boolean hasPos = com.example.do_an_di_dong.util.UserSessionManager.hasPermission(this, "SalesModule");

        if (menu.findItem(R.id.nav_products) != null) {
            menu.findItem(R.id.nav_products).setVisible(hasProduct);
        }
        if (menu.findItem(R.id.nav_orders) != null) {
            menu.findItem(R.id.nav_orders).setVisible(hasSales);
        }
        if (menu.findItem(R.id.nav_approval) != null) {
            menu.findItem(R.id.nav_approval).setVisible(hasApproval);
        }
        if (menu.findItem(R.id.nav_pos) != null) {
            menu.findItem(R.id.nav_pos).setVisible(hasPos);
        }
    }

    private void setupSignalRRealtime() {
        com.example.do_an_di_dong.data.realtime.SignalRRealtimeClient.getInstance()
                .connect("http://localhost:54446/", 1, true);

        com.example.do_an_di_dong.data.realtime.SignalRRealtimeClient.getInstance()
                .addListener(payload -> {
                    // Update Orders tab badge
                    com.google.android.material.badge.BadgeDrawable badge =
                            binding.bottomNavigation.getOrCreateBadge(R.id.nav_orders);
                    badge.setVisible(true);
                    badge.setNumber(badge.getNumber() + 1);

                    Toast.makeText(MainActivity.this, "🔔 Có cập nhật đơn hàng mới từ hệ thống!", Toast.LENGTH_SHORT).show();
                });
    }

    @Override
    public void onBackPressed() {
        // Close chat panel on back press if open
        if (floatingChatManager != null && floatingChatManager.isPanelOpen()) {
            floatingChatManager.closePanel();
        } else {
            super.onBackPressed();
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (floatingChatManager != null) floatingChatManager.destroy();
        com.example.do_an_di_dong.data.realtime.SignalRRealtimeClient.getInstance().disconnect();
    }

    private int getNavOrder(int navId) {
        if (navId == R.id.nav_dashboard) return 0;
        if (navId == R.id.nav_products) return 1;
        if (navId == R.id.nav_orders) return 2;
        if (navId == R.id.nav_approval) return 3;
        if (navId == R.id.nav_pos) return 4;
        return 0;
    }

    private void loadFragment(Fragment fragment, boolean slideForward) {
        FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
        transaction.setCustomAnimations(
                android.R.anim.fade_in,
                android.R.anim.fade_out
        );
        transaction.replace(R.id.fragment_container, fragment);
        transaction.commit();
    }

    public void signOut() {
        if (floatingChatManager != null) floatingChatManager.destroy();
        ApiClient.clearToken(this);
        com.example.do_an_di_dong.util.UserSessionManager.clearSession(this);
        Toast.makeText(this, "Đã đăng xuất.", Toast.LENGTH_SHORT).show();
        Intent intent = new Intent(this, LoginActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }
}
