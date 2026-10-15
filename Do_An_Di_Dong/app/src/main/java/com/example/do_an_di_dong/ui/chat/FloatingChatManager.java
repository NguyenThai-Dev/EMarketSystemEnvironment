package com.example.do_an_di_dong.ui.chat;

import android.animation.Animator;
import android.animation.AnimatorSet;
import android.animation.ObjectAnimator;
import android.animation.ValueAnimator;
import android.content.Context;
import android.content.SharedPreferences;
import android.graphics.PixelFormat;
import android.os.Handler;
import android.os.Looper;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewConfiguration;
import android.view.WindowManager;
import android.view.animation.DecelerateInterpolator;
import android.view.animation.OvershootInterpolator;
import android.widget.Toast;

import androidx.lifecycle.LifecycleOwner;
import androidx.lifecycle.ViewModelProvider;
import androidx.lifecycle.ViewModelStoreOwner;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.R;
import com.example.do_an_di_dong.databinding.LayoutFloatingBubbleBinding;
import com.example.do_an_di_dong.databinding.LayoutFloatingChatPanelBinding;
import com.example.do_an_di_dong.model.ChatMessage;
import com.example.do_an_di_dong.ui.ai.AIAdvisorHubActivity;

/**
 * FloatingChatManager v2 — Draggable AI Bubble & Chat Panel
 *
 * Architecture:
 * - Bubble View: Added to WindowManager at TYPE_APPLICATION, always on top.
 * - Chat Panel: Added to WindowManager as a separate view (above normal Dialogs & Drawers).
 * - Both support drag via MotionEvent on the WindowManager params.
 * - Bubble snaps to nearest edge when dragged.
 * - Glow/pulse animation on bubble when AI is thinking.
 *
 * No SYSTEM_ALERT_WINDOW permission required — uses TYPE_APPLICATION which
 * works within the same app process and stays above Fragments/Dialogs/Drawers.
 */
public class FloatingChatManager {

    // ─── Configuration ───────────────────────────────────────────
    private static final int SNAP_THRESHOLD_DP = 40;    // dp from edge to trigger snap
    private static final int BUBBLE_SIZE_DP = 72;       // total touch area for the bubble
    private static final long SNAP_DURATION_MS = 280;
    private static final long OPEN_DURATION_MS = 320;
    private static final long CLOSE_DURATION_MS = 220;
    private static final long GLOW_PERIOD_MS = 900;

    // ─── References ─────────────────────────────────────────────
    private final Context context;
    private final LifecycleOwner lifecycleOwner;
    private final ViewModelStoreOwner vmStoreOwner;
    private final WindowManager windowManager;

    // ─── Bubble ─────────────────────────────────────────────────
    private LayoutFloatingBubbleBinding bubbleBinding;
    private View bubbleView;
    private WindowManager.LayoutParams bubbleParams;
    private boolean isBubbleAttached = false;

    // ─── Chat Panel ─────────────────────────────────────────────
    private LayoutFloatingChatPanelBinding panelBinding;
    private View panelView;
    private WindowManager.LayoutParams panelParams;
    private boolean isPanelAttached = false;
    private boolean isPanelOpen = false;

    // ─── ViewModel / Adapter ────────────────────────────────────
    private ChatViewModel viewModel;
    private ChatAdapter adapter;
    private boolean isWelcomeAdded = false;

    // ─── Drag state ─────────────────────────────────────────────
    private int initialTouchX, initialTouchY;
    private int initialParamX, initialParamY;
    private boolean isDragging = false;
    private long touchDownTime;
    private static final long CLICK_DURATION_MS = 200;
    private int touchSlop;

    // ─── Pulse / Glow animation ──────────────────────────────────
    private ValueAnimator pulseAnimator;
    private ValueAnimator glowAnimator;
    private final Handler glowHandler = new Handler(Looper.getMainLooper());

    public FloatingChatManager(Context context, LifecycleOwner lifecycleOwner, ViewModelStoreOwner vmStoreOwner) {
        this.context = context;
        this.lifecycleOwner = lifecycleOwner;
        this.vmStoreOwner = vmStoreOwner;
        this.windowManager = (WindowManager) context.getSystemService(Context.WINDOW_SERVICE);
        this.touchSlop = ViewConfiguration.get(context).getScaledTouchSlop();
    }

    // ═══════════════════════════════════════════════════════════════
    //  PUBLIC API
    // ═══════════════════════════════════════════════════════════════

    /** Show the floating bubble. Call once on app start. */
    public void show() {
        if (isBubbleAttached) return;
        attachBubble();
        startIdlePulse();
    }

    /** Toggle the chat panel open/close. */
    public void toggleChat() {
        if (isPanelOpen) {
            closePanel();
        } else {
            openPanel();
        }
    }

    public void openPanel() {
        if (isPanelOpen) return;
        if (!isPanelAttached) attachPanel();
        isPanelOpen = true;
        animatePanelOpen();
        stopIdlePulse();
        hideBubbleUnread();
    }

    public void closePanel() {
        if (!isPanelOpen) return;
        animatePanelClose(() -> {
            isPanelOpen = false;
            if (isPanelAttached) {
                try { windowManager.removeView(panelView); } catch (Exception ignored) {}
                isPanelAttached = false;
            }
            startIdlePulse();
        });
    }

    public boolean isPanelOpen() { return isPanelOpen; }

    /** Clean up all views — call in onDestroy. */
    public void destroy() {
        stopIdlePulse();
        stopGlow();
        glowHandler.removeCallbacksAndMessages(null);
        if (isBubbleAttached) {
            try { windowManager.removeView(bubbleView); } catch (Exception ignored) {}
            isBubbleAttached = false;
        }
        if (isPanelAttached) {
            try { windowManager.removeView(panelView); } catch (Exception ignored) {}
            isPanelAttached = false;
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  BUBBLE
    // ═══════════════════════════════════════════════════════════════

    private void attachBubble() {
        bubbleBinding = LayoutFloatingBubbleBinding.inflate(LayoutInflater.from(context));
        bubbleView = bubbleBinding.getRoot();

        int screenWidth = getScreenWidth();
        int screenHeight = getScreenHeight();
        int bubblePx = dpToPx(BUBBLE_SIZE_DP);

        bubbleParams = new WindowManager.LayoutParams(
                WindowManager.LayoutParams.WRAP_CONTENT,
                WindowManager.LayoutParams.WRAP_CONTENT,
                WindowManager.LayoutParams.TYPE_APPLICATION,
                WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE
                        | WindowManager.LayoutParams.FLAG_LAYOUT_IN_SCREEN,
                PixelFormat.TRANSLUCENT
        );
        bubbleParams.gravity = Gravity.TOP | Gravity.START;
        // Default position: right side, vertically centered
        bubbleParams.x = screenWidth - bubblePx - dpToPx(16);
        bubbleParams.y = screenHeight / 2 - bubblePx / 2;

        windowManager.addView(bubbleView, bubbleParams);
        isBubbleAttached = true;

        setupBubbleDragAndClick();

        // Entrance animation
        bubbleView.setScaleX(0f);
        bubbleView.setScaleY(0f);
        bubbleView.setAlpha(0f);
        bubbleView.animate()
                .scaleX(1f).scaleY(1f).alpha(1f)
                .setDuration(350)
                .setInterpolator(new OvershootInterpolator(1.5f))
                .start();
    }

    @SuppressWarnings("ClickableViewAccessibility")
    private void setupBubbleDragAndClick() {
        bubbleBinding.fabAiBubble.setOnTouchListener((view, event) -> {
            switch (event.getAction()) {
                case MotionEvent.ACTION_DOWN:
                    initialTouchX = (int) event.getRawX();
                    initialTouchY = (int) event.getRawY();
                    initialParamX = bubbleParams.x;
                    initialParamY = bubbleParams.y;
                    isDragging = false;
                    touchDownTime = System.currentTimeMillis();

                    // Press feedback
                    bubbleBinding.fabAiBubble.animate().scaleX(0.9f).scaleY(0.9f)
                            .setDuration(100).start();
                    return true;

                case MotionEvent.ACTION_MOVE:
                    int deltaX = (int) event.getRawX() - initialTouchX;
                    int deltaY = (int) event.getRawY() - initialTouchY;

                    if (!isDragging && (Math.abs(deltaX) > touchSlop || Math.abs(deltaY) > touchSlop)) {
                        isDragging = true;
                    }

                    if (isDragging) {
                        bubbleParams.x = clamp(initialParamX + deltaX, 0, getScreenWidth() - dpToPx(BUBBLE_SIZE_DP));
                        bubbleParams.y = clamp(initialParamY + deltaY, 0, getScreenHeight() - dpToPx(BUBBLE_SIZE_DP));
                        if (isBubbleAttached) windowManager.updateViewLayout(bubbleView, bubbleParams);
                    }
                    return true;

                case MotionEvent.ACTION_UP:
                case MotionEvent.ACTION_CANCEL:
                    bubbleBinding.fabAiBubble.animate().scaleX(1f).scaleY(1f)
                            .setDuration(120).setInterpolator(new OvershootInterpolator(2f)).start();

                    if (isDragging) {
                        snapBubbleToEdge();
                    } else {
                        // Click
                        long elapsed = System.currentTimeMillis() - touchDownTime;
                        if (elapsed < CLICK_DURATION_MS * 2) {
                            toggleChat();
                        }
                    }
                    isDragging = false;
                    return true;
            }
            return false;
        });
    }

    private void snapBubbleToEdge() {
        int screenWidth = getScreenWidth();
        int screenHeight = getScreenHeight();
        int bubblePx = dpToPx(BUBBLE_SIZE_DP);
        int snapThresholdPx = dpToPx(SNAP_THRESHOLD_DP);

        int targetX = bubbleParams.x;
        int targetY = bubbleParams.y;

        // Snap to left or right edge
        if (bubbleParams.x < screenWidth / 2) {
            targetX = dpToPx(12);    // left edge
        } else {
            targetX = screenWidth - bubblePx - dpToPx(12); // right edge
        }

        // Clamp Y
        targetY = clamp(targetY, dpToPx(24), screenHeight - bubblePx - dpToPx(24));

        final int finalX = targetX;
        final int finalY = targetY;
        final int startX = bubbleParams.x;
        final int startY = bubbleParams.y;

        ValueAnimator snapAnim = ValueAnimator.ofFloat(0f, 1f);
        snapAnim.setDuration(SNAP_DURATION_MS);
        snapAnim.setInterpolator(new DecelerateInterpolator(2f));
        snapAnim.addUpdateListener(animator -> {
            float frac = (float) animator.getAnimatedValue();
            bubbleParams.x = (int) (startX + (finalX - startX) * frac);
            bubbleParams.y = (int) (startY + (finalY - startY) * frac);
            if (isBubbleAttached) windowManager.updateViewLayout(bubbleView, bubbleParams);
        });
        snapAnim.start();
    }

    // ═══════════════════════════════════════════════════════════════
    //  CHAT PANEL
    // ═══════════════════════════════════════════════════════════════

    private void attachPanel() {
        panelBinding = LayoutFloatingChatPanelBinding.inflate(LayoutInflater.from(context));
        panelView = panelBinding.getRoot();

        int screenWidth = getScreenWidth();
        int screenHeight = getScreenHeight();
        int panelWidth = Math.min(screenWidth - dpToPx(24), dpToPx(400));
        int panelHeight = (int) (screenHeight * 0.72f);

        panelParams = new WindowManager.LayoutParams(
                panelWidth,
                panelHeight,
                WindowManager.LayoutParams.TYPE_APPLICATION,
                WindowManager.LayoutParams.FLAG_LAYOUT_IN_SCREEN,
                PixelFormat.TRANSLUCENT
        );
        panelParams.gravity = Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL;
        panelParams.y = dpToPx(16); // margin from bottom

        panelView.setAlpha(0f);
        panelView.setTranslationY(dpToPx(80));
        panelView.setScaleX(0.95f);

        windowManager.addView(panelView, panelParams);
        isPanelAttached = true;

        setupPanelContent();
    }

    private void setupPanelContent() {
        // ViewModel / Adapter
        if (viewModel == null) {
            viewModel = new ViewModelProvider(vmStoreOwner).get(ChatViewModel.class);
        }
        adapter = new ChatAdapter();

        LinearLayoutManager layoutManager = new LinearLayoutManager(context);
        layoutManager.setStackFromEnd(true);
        panelBinding.rvMessages.setLayoutManager(layoutManager);
        panelBinding.rvMessages.setAdapter(adapter);

        if (!isWelcomeAdded) {
            adapter.addMessage(new ChatMessage(
                    "Báo cáo sếp, tôi là trợ lý AI phân tích kho EMarket. Tôi có thể giúp gì cho sếp hôm nay?", false));
            isWelcomeAdded = true;
        }

        // Observers
        viewModel.getResponse().observe(lifecycleOwner, chatResponse -> {
            stopGlow();
            setAiThinkingState(false);
            if (chatResponse != null && chatResponse.getAnswer() != null) {
                adapter.addMessage(new ChatMessage(chatResponse.getAnswer(), false));
                scrollToBottom();
                showBubbleUnread();
            }
        });

        viewModel.getError().observe(lifecycleOwner, msg -> {
            if (msg != null) {
                stopGlow();
                setAiThinkingState(false);
                String friendly = "Lỗi kết nối AI.";
                if (msg.contains("ConnectException") || msg.contains("timeout")) {
                    friendly = "Không thể kết nối tới máy chủ AI.";
                }
                Toast.makeText(context, friendly, Toast.LENGTH_SHORT).show();
            }
        });

        // Buttons
        panelBinding.btnSend.setOnClickListener(v -> sendMessage());
        panelBinding.btnCloseChat.setOnClickListener(v -> closePanel());

        panelBinding.btnOpenAdvisorHub.setOnClickListener(v -> {
            android.content.Intent intent = new android.content.Intent(context, AIAdvisorHubActivity.class);
            intent.addFlags(android.content.Intent.FLAG_ACTIVITY_NEW_TASK);
            context.startActivity(intent);
        });

        // Quick chips
        panelBinding.chipPromptStock.setOnClickListener(v -> {
            panelBinding.etMessage.setText("Báo cáo danh sách hàng sắp hết kho và đề xuất số lượng nhập?");
            sendMessage();
        });
        panelBinding.chipPromptTopSales.setOnClickListener(v -> {
            panelBinding.etMessage.setText("Phân tích top 5 sản phẩm có doanh thu cao nhất tuần này?");
            sendMessage();
        });
        panelBinding.chipPromptExpiry.setOnClickListener(v -> {
            panelBinding.etMessage.setText("Cảnh báo các lô hàng cận hạn sử dụng có nguy cơ thiệt hại vốn?");
            sendMessage();
        });

        // Panel drag via header
        panelBinding.layoutChatHeader.setOnTouchListener(createPanelDragListener());
    }

    @SuppressWarnings("ClickableViewAccessibility")
    private View.OnTouchListener createPanelDragListener() {
        final int[] initialPanelTouchX = {0};
        final int[] initialPanelTouchY = {0};
        final int[] initialPanelParamX = {0};
        final int[] initialPanelParamY = {0};
        final boolean[] isPanelDragging = {false};

        return (view, event) -> {
            switch (event.getAction()) {
                case MotionEvent.ACTION_DOWN:
                    initialPanelTouchX[0] = (int) event.getRawX();
                    initialPanelTouchY[0] = (int) event.getRawY();
                    initialPanelParamX[0] = panelParams.x;
                    initialPanelParamY[0] = panelParams.y;
                    isPanelDragging[0] = false;
                    return true;

                case MotionEvent.ACTION_MOVE:
                    int dx = (int) event.getRawX() - initialPanelTouchX[0];
                    int dy = (int) event.getRawY() - initialPanelTouchY[0];

                    if (!isPanelDragging[0] && (Math.abs(dx) > touchSlop || Math.abs(dy) > touchSlop)) {
                        isPanelDragging[0] = true;
                    }
                    if (isPanelDragging[0] && isPanelAttached) {
                        panelParams.x = initialPanelParamX[0] + dx;
                        panelParams.y = initialPanelParamY[0] - dy; // invert Y since gravity is BOTTOM
                        windowManager.updateViewLayout(panelView, panelParams);
                    }
                    return true;

                case MotionEvent.ACTION_UP:
                    isPanelDragging[0] = false;
                    return true;
            }
            return false;
        };
    }

    // ═══════════════════════════════════════════════════════════════
    //  PANEL ANIMATIONS
    // ═══════════════════════════════════════════════════════════════

    private void animatePanelOpen() {
        panelView.animate().cancel();
        panelView.setAlpha(0f);
        panelView.setTranslationY(dpToPx(60));
        panelView.setScaleX(0.96f);
        panelView.setScaleY(0.96f);

        panelView.animate()
                .alpha(1f)
                .translationY(0f)
                .scaleX(1f).scaleY(1f)
                .setDuration(OPEN_DURATION_MS)
                .setInterpolator(new DecelerateInterpolator(2f))
                .start();
    }

    private void animatePanelClose(Runnable onEnd) {
        panelView.animate().cancel();
        panelView.animate()
                .alpha(0f)
                .translationY(dpToPx(60))
                .scaleX(0.96f).scaleY(0.96f)
                .setDuration(CLOSE_DURATION_MS)
                .setInterpolator(new DecelerateInterpolator(2f))
                .setListener(new android.animation.AnimatorListenerAdapter() {
                    @Override
                    public void onAnimationEnd(Animator animation) {
                        if (onEnd != null) onEnd.run();
                    }
                })
                .start();
    }

    // ═══════════════════════════════════════════════════════════════
    //  GLOW / PULSE ANIMATIONS
    // ═══════════════════════════════════════════════════════════════

    private void startIdlePulse() {
        if (!isBubbleAttached) return;
        stopIdlePulse();
        pulseAnimator = ValueAnimator.ofFloat(0f, 0.35f, 0f);
        pulseAnimator.setDuration(2400);
        pulseAnimator.setRepeatCount(ValueAnimator.INFINITE);
        pulseAnimator.setInterpolator(new DecelerateInterpolator());
        pulseAnimator.addUpdateListener(anim -> {
            if (bubbleBinding != null) {
                bubbleBinding.viewPulseRing.setAlpha((float) anim.getAnimatedValue());
                float scale = 1f + (float) anim.getAnimatedValue() * 0.6f;
                bubbleBinding.viewPulseRing.setScaleX(scale);
                bubbleBinding.viewPulseRing.setScaleY(scale);
            }
        });
        pulseAnimator.start();
    }

    private void stopIdlePulse() {
        if (pulseAnimator != null) {
            pulseAnimator.cancel();
            pulseAnimator = null;
        }
        if (bubbleBinding != null) {
            bubbleBinding.viewPulseRing.setAlpha(0f);
        }
    }

    private void startGlow() {
        if (!isBubbleAttached) return;
        stopGlow();
        glowAnimator = ValueAnimator.ofFloat(0f, 0.8f, 0f);
        glowAnimator.setDuration(GLOW_PERIOD_MS);
        glowAnimator.setRepeatCount(ValueAnimator.INFINITE);
        glowAnimator.addUpdateListener(anim -> {
            if (bubbleBinding != null) {
                bubbleBinding.viewGlowRing.setAlpha((float) anim.getAnimatedValue());
                float scale = 1f + (float) anim.getAnimatedValue() * 0.25f;
                bubbleBinding.viewGlowRing.setScaleX(scale);
                bubbleBinding.viewGlowRing.setScaleY(scale);
            }
        });
        glowAnimator.start();
    }

    private void stopGlow() {
        if (glowAnimator != null) {
            glowAnimator.cancel();
            glowAnimator = null;
        }
        if (bubbleBinding != null) {
            bubbleBinding.viewGlowRing.setAlpha(0f);
        }
    }

    private void setAiThinkingState(boolean thinking) {
        if (panelBinding == null) return;
        panelBinding.layoutThinking.setVisibility(thinking ? View.VISIBLE : View.GONE);
        if (thinking) {
            panelBinding.tvStatusLabel.setText("Đang xử lý...");
            panelBinding.dotStatus.setBackgroundTintList(
                    android.content.res.ColorStateList.valueOf(0xFFFF9800));
            startGlow();
        } else {
            panelBinding.tvStatusLabel.setText("Đang hoạt động");
            panelBinding.dotStatus.setBackgroundTintList(null); // reset to XML tint
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  UNREAD BADGE ON BUBBLE
    // ═══════════════════════════════════════════════════════════════

    private void showBubbleUnread() {
        if (isPanelOpen || bubbleBinding == null) return;
        bubbleBinding.dotUnread.setVisibility(View.VISIBLE);
        bubbleBinding.dotUnread.setAlpha(0f);
        bubbleBinding.dotUnread.animate().alpha(1f).setDuration(200).start();
    }

    private void hideBubbleUnread() {
        if (bubbleBinding == null) return;
        bubbleBinding.dotUnread.setVisibility(View.GONE);
    }

    // ═══════════════════════════════════════════════════════════════
    //  CHAT LOGIC
    // ═══════════════════════════════════════════════════════════════

    private void sendMessage() {
        if (panelBinding == null) return;
        String text = panelBinding.etMessage.getText().toString().trim();
        if (text.isEmpty()) return;

        SharedPreferences prefs = context.getSharedPreferences("BiometricPrefs", Context.MODE_PRIVATE);
        String userName = prefs.getString("user_fullname", "Manager");

        adapter.addMessage(new ChatMessage(text, true));
        panelBinding.etMessage.setText("");
        scrollToBottom();
        setAiThinkingState(true);
        viewModel.sendMessage(text, userName);
    }

    private void scrollToBottom() {
        if (adapter != null && adapter.getItemCount() > 0) {
            panelBinding.rvMessages.smoothScrollToPosition(adapter.getItemCount() - 1);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  HELPERS
    // ═══════════════════════════════════════════════════════════════

    private int getScreenWidth() {
        return context.getResources().getDisplayMetrics().widthPixels;
    }

    private int getScreenHeight() {
        return context.getResources().getDisplayMetrics().heightPixels;
    }

    private int dpToPx(int dp) {
        float density = context.getResources().getDisplayMetrics().density;
        return (int) (dp * density + 0.5f);
    }

    private static int clamp(int value, int min, int max) {
        return Math.max(min, Math.min(max, value));
    }
}
