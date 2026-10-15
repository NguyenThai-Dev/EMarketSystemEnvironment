package com.example.do_an_di_dong.ui.chat;

import android.Manifest;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.speech.RecognitionListener;
import android.speech.RecognizerIntent;
import android.speech.SpeechRecognizer;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.example.do_an_di_dong.databinding.FragmentChatBinding;
import com.example.do_an_di_dong.model.ChatMessage;
import com.example.do_an_di_dong.ui.ai.AIAdvisorHubActivity;

import java.util.ArrayList;

public class ChatFragment extends Fragment {
    private static final int PERMISSION_RECORD_AUDIO_REQUEST = 2001;

    private FragmentChatBinding binding;
    private ChatViewModel viewModel;
    private ChatAdapter adapter;
    private SpeechRecognizer speechRecognizer;
    private Intent speechRecognizerIntent;
    private String userName = "Manager";

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        binding = FragmentChatBinding.inflate(inflater, container, false);
        return binding.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        viewModel = new ViewModelProvider(this).get(ChatViewModel.class);
        adapter = new ChatAdapter();

        // Use new ID: rvMessages — LinearLayoutManager with stackFromEnd for chat UX
        LinearLayoutManager layoutManager = new LinearLayoutManager(getContext());
        layoutManager.setStackFromEnd(true);
        binding.rvMessages.setLayoutManager(layoutManager);
        binding.rvMessages.setAdapter(adapter);

        // Scroll to bottom when keyboard pops up
        androidx.core.view.ViewCompat.setOnApplyWindowInsetsListener(binding.getRoot(), (v, insets) -> {
            int imeInset = insets.getInsets(androidx.core.view.WindowInsetsCompat.Type.ime()).bottom;
            if (imeInset > 0 && adapter.getItemCount() > 0) {
                binding.rvMessages.post(() ->
                        binding.rvMessages.smoothScrollToPosition(adapter.getItemCount() - 1));
            }
            return insets;
        });

        // Fetch Username
        SharedPreferences prefs = requireContext().getSharedPreferences("BiometricPrefs", Context.MODE_PRIVATE);
        userName = prefs.getString("user_fullname", "Manager");

        // Welcome message from bot
        adapter.addMessage(new ChatMessage("Báo cáo sếp, tôi là trợ lý AI phân tích kho EMarket. Tôi có thể giúp gì cho sếp hôm nay?", false));

        // Observers
        viewModel.getResponse().observe(getViewLifecycleOwner(), chatResponse -> {
            // Use new ID: layoutThinking instead of layoutLoading
            binding.layoutThinking.setVisibility(View.GONE);
            if (chatResponse != null && chatResponse.getAnswer() != null) {
                adapter.addMessage(new ChatMessage(chatResponse.getAnswer(), false));
                binding.rvMessages.smoothScrollToPosition(adapter.getItemCount() - 1);
            }
        });

        viewModel.getError().observe(getViewLifecycleOwner(), msg -> {
            if (msg != null) {
                binding.layoutThinking.setVisibility(View.GONE);
                String friendlyMessage = "Lỗi kết nối AI.";
                if (msg.contains("ConnectException") || msg.contains("timeout") || msg.contains("Unable to resolve host")) {
                    friendlyMessage = "Không thể kết nối mạng tới máy chủ AI.";
                }
                Toast.makeText(getContext(), friendlyMessage, Toast.LENGTH_SHORT).show();
            }
        });

        // Send button
        binding.btnSend.setOnClickListener(v -> sendMessage());

        // Open AI Advisor Hub
        binding.btnOpenAdvisorHub.setOnClickListener(v -> {
            Intent intent = new Intent(getContext(), AIAdvisorHubActivity.class);
            startActivity(intent);
        });

        // Quick Prompt Chips
        binding.chipPromptStock.setOnClickListener(v -> {
            binding.etMessage.setText("Báo cáo danh sách hàng sắp hết kho và đề xuất số lượng nhập?");
            sendMessage();
        });

        binding.chipPromptTopSales.setOnClickListener(v -> {
            binding.etMessage.setText("Phân tích top 5 sản phẩm có doanh thu và biên lợi nhuận cao nhất tuần này?");
            sendMessage();
        });

        binding.chipPromptExpiry.setOnClickListener(v -> {
            binding.etMessage.setText("Cảnh báo các lô hàng cận hạn sử dụng có nguy cơ thiệt hại vốn?");
            sendMessage();
        });

        // Setup Speech To Text for long-press on send button as voice input hint
        setupSpeechToText();
        binding.btnSend.setOnLongClickListener(v -> {
            startVoiceRecognition();
            return true;
        });
    }

    private void sendMessage() {
        String text = binding.etMessage.getText().toString().trim();
        if (text.isEmpty()) return;

        // Add user message to UI
        adapter.addMessage(new ChatMessage(text, true));
        binding.etMessage.setText("");
        binding.rvMessages.smoothScrollToPosition(adapter.getItemCount() - 1);

        // Show thinking indicator
        binding.layoutThinking.setVisibility(View.VISIBLE);

        // Call API
        viewModel.sendMessage(text, userName);
    }

    private void setupSpeechToText() {
        if (!SpeechRecognizer.isRecognitionAvailable(getContext())) return;

        speechRecognizer = SpeechRecognizer.createSpeechRecognizer(getContext());
        speechRecognizerIntent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
        speechRecognizerIntent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
        speechRecognizerIntent.putExtra(RecognizerIntent.EXTRA_LANGUAGE, "vi-VN");

        speechRecognizer.setRecognitionListener(new RecognitionListener() {
            @Override public void onReadyForSpeech(Bundle params) {
                Toast.makeText(getContext(), "Hãy nói...", Toast.LENGTH_SHORT).show();
            }
            @Override public void onBeginningOfSpeech() {}
            @Override public void onRmsChanged(float rmsdB) {}
            @Override public void onBufferReceived(byte[] buffer) {}
            @Override public void onEndOfSpeech() {}
            @Override public void onError(int error) {
                String msg;
                switch (error) {
                    case SpeechRecognizer.ERROR_AUDIO: msg = "Lỗi âm thanh"; break;
                    case SpeechRecognizer.ERROR_NO_MATCH: msg = "Không hiểu câu nói"; break;
                    case SpeechRecognizer.ERROR_SPEECH_TIMEOUT: msg = "Không nghe thấy câu nói"; break;
                    default: msg = "Lỗi ghi âm"; break;
                }
                Toast.makeText(getContext(), msg, Toast.LENGTH_SHORT).show();
            }
            @Override public void onResults(Bundle results) {
                ArrayList<String> matches = results.getStringArrayList(SpeechRecognizer.RESULTS_RECOGNITION);
                if (matches != null && !matches.isEmpty() && binding != null) {
                    binding.etMessage.setText(matches.get(0));
                }
            }
            @Override public void onPartialResults(Bundle partialResults) {}
            @Override public void onEvent(int eventType, Bundle params) {}
        });
    }

    private void startVoiceRecognition() {
        if (speechRecognizer == null) {
            Toast.makeText(getContext(), "Thiết bị không hỗ trợ nhận dạng giọng nói.", Toast.LENGTH_SHORT).show();
            return;
        }
        if (ContextCompat.checkSelfPermission(requireContext(), Manifest.permission.RECORD_AUDIO)
                == PackageManager.PERMISSION_GRANTED) {
            speechRecognizer.startListening(speechRecognizerIntent);
        } else {
            requestPermissions(new String[]{Manifest.permission.RECORD_AUDIO}, PERMISSION_RECORD_AUDIO_REQUEST);
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        if (requestCode == PERMISSION_RECORD_AUDIO_REQUEST
                && grantResults.length > 0
                && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            speechRecognizer.startListening(speechRecognizerIntent);
        } else {
            Toast.makeText(getContext(), "Quyền ghi âm là bắt buộc để sử dụng giọng nói.", Toast.LENGTH_SHORT).show();
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        if (speechRecognizer != null) {
            speechRecognizer.destroy();
        }
        binding = null;
    }
}
