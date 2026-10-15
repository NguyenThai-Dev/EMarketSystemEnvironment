package com.example.do_an_di_dong.ui.approval;

import android.graphics.Canvas;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AlertDialog;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.ItemTouchHelper;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.do_an_di_dong.databinding.FragmentApprovalBinding;
import com.example.do_an_di_dong.model.Quotation;
import com.example.do_an_di_dong.ui.quotation.QuotationEditorActivity;

public class ApprovalFragment extends Fragment {
    private FragmentApprovalBinding binding;
    private ApprovalViewModel viewModel;
    private QuotationAdapter adapter;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        binding = FragmentApprovalBinding.inflate(inflater, container, false);
        return binding.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        viewModel = new ViewModelProvider(this).get(ApprovalViewModel.class);
        adapter = new QuotationAdapter();

        binding.rvApproval.setLayoutManager(new LinearLayoutManager(getContext()));
        binding.rvApproval.setAdapter(adapter);

        // Observers
        viewModel.getQuotations().observe(getViewLifecycleOwner(), list -> {
            binding.layoutLoading.setVisibility(View.GONE);
            binding.layoutError.setVisibility(View.GONE);

            if (list == null || list.isEmpty()) {
                binding.layoutContent.setVisibility(View.GONE);
                binding.layoutEmpty.setVisibility(View.VISIBLE);
            } else {
                binding.layoutEmpty.setVisibility(View.GONE);
                binding.layoutContent.setVisibility(View.VISIBLE);
                adapter.setQuotations(list);
            }
        });

        viewModel.getStatusUpdated().observe(getViewLifecycleOwner(), msg -> {
            if (msg != null) {
                Toast.makeText(getContext(), msg, Toast.LENGTH_SHORT).show();
                loadData();
            }
        });

        viewModel.getError().observe(getViewLifecycleOwner(), msg -> {
            if (msg != null) {
                showError(msg);
            }
        });

        binding.btnRetry.setOnClickListener(v -> loadData());

        binding.fabCreateQuotation.setOnClickListener(v -> {
            android.content.Intent intent = new android.content.Intent(getContext(), QuotationEditorActivity.class);
            startActivity(intent);
        });

        setupSwipeHelper();
        loadData();
    }

    private void loadData() {
        binding.layoutContent.setVisibility(View.GONE);
        binding.layoutEmpty.setVisibility(View.GONE);
        binding.layoutError.setVisibility(View.GONE);
        binding.layoutLoading.setVisibility(View.VISIBLE);
        viewModel.fetchQuotations("Sent");
    }

    private void showError(String message) {
        binding.layoutLoading.setVisibility(View.GONE);
        binding.layoutContent.setVisibility(View.GONE);
        binding.layoutEmpty.setVisibility(View.GONE);
        binding.layoutError.setVisibility(View.VISIBLE);

        String friendlyMessage = "Lỗi tải báo giá.";
        if (message.contains("ConnectException") || message.contains("timeout") || message.contains("Unable to resolve host")) {
            friendlyMessage = "Không thể kết nối mạng. Vui lòng kiểm tra Wifi/Mạng.";
        } else if (message.contains("401")) {
            friendlyMessage = "Hết hạn phiên đăng nhập. Vui lòng đăng nhập lại.";
        }
        binding.tvErrorMessage.setText(friendlyMessage);
    }

    /**
     * Swipe → Reveal → Confirm → API flow.
     *
     * The ItemTouchHelper translates the foreground card (cardForeground)
     * in onChildDraw, revealing the colored action background behind it.
     * On swipe completion, a dialog confirms before calling the API.
     */
    private void setupSwipeHelper() {
        ItemTouchHelper.SimpleCallback callback = new ItemTouchHelper.SimpleCallback(
                0, ItemTouchHelper.LEFT | ItemTouchHelper.RIGHT) {

            @Override
            public boolean onMove(@NonNull RecyclerView rv,
                                  @NonNull RecyclerView.ViewHolder vh,
                                  @NonNull RecyclerView.ViewHolder target) {
                return false;
            }

            @Override
            public void onChildDraw(@NonNull Canvas c,
                                    @NonNull RecyclerView recyclerView,
                                    @NonNull RecyclerView.ViewHolder viewHolder,
                                    float dX, float dY,
                                    int actionState,
                                    boolean isCurrentlyActive) {

                if (viewHolder instanceof QuotationAdapter.QuotationViewHolder) {
                    QuotationAdapter.QuotationViewHolder holder =
                            (QuotationAdapter.QuotationViewHolder) viewHolder;
                    View cardFg = holder.binding.cardForeground;
                    View revealApprove = holder.binding.layoutRevealApprove;
                    View revealReject = holder.binding.layoutRevealReject;

                    // Translate only the foreground card, revealing background
                    cardFg.setTranslationX(dX);

                    // Show the correct action background based on direction
                    if (dX > 0) {
                        // Swiping right → show Approve action
                        revealApprove.setVisibility(View.VISIBLE);
                        revealReject.setVisibility(View.GONE);
                    } else if (dX < 0) {
                        // Swiping left → show Reject action
                        revealApprove.setVisibility(View.GONE);
                        revealReject.setVisibility(View.VISIBLE);
                    }
                    // Do NOT call super to avoid default background drawing
                } else {
                    super.onChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
                }
            }

            @Override
            public void onSwiped(@NonNull RecyclerView.ViewHolder viewHolder, int direction) {
                int position = viewHolder.getAdapterPosition();
                if (position == RecyclerView.NO_ID) return;

                Quotation quotation = adapter.getQuotations().get(position);
                boolean isApprove = (direction == ItemTouchHelper.RIGHT);

                String title = isApprove ? "Phê duyệt báo giá" : "Từ chối báo giá";
                String message = isApprove
                        ? "Bạn có chắc muốn phê duyệt báo giá " + quotation.getQuotationCode() + "?"
                        : "Bạn có chắc muốn từ chối báo giá " + quotation.getQuotationCode() + "?";
                String confirmText = isApprove ? "Phê duyệt" : "Từ chối";
                String nextStatus = isApprove ? "Accepted" : "Cancelled";

                // Reset foreground card back BEFORE showing dialog
                // so the card snaps back to original position during confirmation
                adapter.notifyItemChanged(position);

                new AlertDialog.Builder(requireContext())
                        .setTitle(title)
                        .setMessage(message)
                        .setPositiveButton(confirmText, (dialog, which) -> {
                            binding.layoutLoading.setVisibility(View.VISIBLE);
                            viewModel.updateQuotationStatus(quotation.getQuotationId(), nextStatus);
                        })
                        .setNegativeButton("Hủy bỏ", (dialog, which) -> {
                            // Item already reset by notifyItemChanged above
                        })
                        .setCancelable(false)
                        .show();
            }

            @Override
            public float getSwipeThreshold(@NonNull RecyclerView.ViewHolder viewHolder) {
                // 40% of item width to trigger the onSwiped callback
                return 0.4f;
            }
        };

        new ItemTouchHelper(callback).attachToRecyclerView(binding.rvApproval);
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}
