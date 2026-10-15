package com.example.do_an_di_dong.data.realtime;

import android.os.Handler;
import android.os.Looper;
import android.util.Log;

import org.json.JSONObject;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.TimeUnit;

import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import okhttp3.WebSocket;
import okhttp3.WebSocketListener;

public class SignalRRealtimeClient {
    private static final String TAG = "SignalRRealtimeClient";
    private static SignalRRealtimeClient instance;

    public interface OnOrderEventListener {
        void onOrderChanged(JSONObject payload);
    }

    private final List<OnOrderEventListener> listeners = new ArrayList<>();
    private final Handler mainHandler = new Handler(Looper.getMainLooper());
    private WebSocket webSocket;
    private boolean isConnected = false;
    private final OkHttpClient client;

    private SignalRRealtimeClient() {
        client = new OkHttpClient.Builder()
                .readTimeout(0, TimeUnit.MILLISECONDS)
                .build();
    }

    public static synchronized SignalRRealtimeClient getInstance() {
        if (instance == null) {
            instance = new SignalRRealtimeClient();
        }
        return instance;
    }

    public void addListener(OnOrderEventListener listener) {
        if (listener != null && !listeners.contains(listener)) {
            listeners.add(listener);
        }
    }

    public void removeListener(OnOrderEventListener listener) {
        listeners.remove(listener);
    }

    public void connect(String baseUrl, int branchId, boolean isAdmin) {
        if (isConnected) return;

        String wsUrl = baseUrl;
        if (wsUrl.endsWith("/")) wsUrl = wsUrl.substring(0, wsUrl.length() - 1);
        wsUrl = wsUrl.replace("http://", "ws://").replace("https://", "wss://");
        wsUrl += "/signalr/connect?transport=webSockets&clientProtocol=1.5&connectionData=%5B%7B%22name%22%3A%22orderhub%22%7D%5D"
                + "&branchId=" + branchId + "&isAdmin=" + isAdmin;

        Request request = new Request.Builder()
                .url(wsUrl)
                .build();

        webSocket = client.newWebSocket(request, new WebSocketListener() {
            @Override
            public void onOpen(WebSocket ws, Response response) {
                isConnected = true;
                Log.d(TAG, "SignalR WebSocket Connected to OrderHub");
            }

            @Override
            public void onMessage(WebSocket ws, String text) {
                try {
                    JSONObject json = new JSONObject(text);
                    if (json.has("M")) { // SignalR message payload
                        mainHandler.post(() -> notifyListeners(json));
                    }
                } catch (Exception ignored) {}
            }

            @Override
            public void onClosing(WebSocket ws, int code, String reason) {
                isConnected = false;
            }

            @Override
            public void onFailure(WebSocket ws, Throwable t, Response response) {
                isConnected = false;
                Log.e(TAG, "SignalR WebSocket Error: " + t.getMessage());
            }
        });
    }

    public void disconnect() {
        if (webSocket != null) {
            webSocket.close(1000, "App closed");
            webSocket = null;
        }
        isConnected = false;
    }

    private void notifyListeners(JSONObject payload) {
        for (OnOrderEventListener listener : listeners) {
            try {
                listener.onOrderChanged(payload);
            } catch (Exception e) {
                Log.e(TAG, "Error in listener callback", e);
            }
        }
    }
}
