package com.example.do_an_di_dong.data.api;

import android.content.Context;
import android.content.SharedPreferences;
import com.example.do_an_di_dong.model.ExternalLoginRequest;
import com.example.do_an_di_dong.model.LoginResponse;
import okhttp3.Cookie;
import okhttp3.CookieJar;
import okhttp3.HttpUrl;
import okhttp3.Interceptor;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import okhttp3.logging.HttpLoggingInterceptor;
import retrofit2.Call;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

import java.io.IOException;
import java.security.cert.CertificateException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLSocketFactory;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

public class ApiClient {
    private static final String PREF_NAME = "EMarketPrefs";
    private static final String KEY_TOKEN = "jwt_token";
    
    public static final String DEFAULT_EMARKET_BASE_URL = "http://localhost:54446/";
    public static final String EMULATOR_EMARKET_BASE_URL = "http://10.0.2.2:54446/";
    public static final String KEY_CUSTOM_BASE_URL = "custom_base_url";
    private static final String AI_BASE_URL = "http://localhost:5000/";
    
    private static Retrofit emarketRetrofit = null;
    private static String currentBaseUrl = null;
    private static Retrofit aiRetrofit = null;

    // In-memory Cookie store for ASP.NET Session and Anti-Forgery Cookies
    private static final HashMap<String, List<Cookie>> cookieStore = new HashMap<>();

    private static OkHttpClient.Builder getUnsafeOkHttpClientBuilder() {
        try {
            // Create a trust manager that does not validate certificate chains
            final TrustManager[] trustAllCerts = new TrustManager[] {
                new X509TrustManager() {
                    @Override
                    public void checkClientTrusted(java.security.cert.X509Certificate[] chain, String authType) throws CertificateException {}

                    @Override
                    public void checkServerTrusted(java.security.cert.X509Certificate[] chain, String authType) throws CertificateException {}

                    @Override
                    public java.security.cert.X509Certificate[] getAcceptedIssuers() {
                        return new java.security.cert.X509Certificate[]{};
                    }
                }
            };

            // Install the all-trusting trust manager
            final SSLContext sslContext = SSLContext.getInstance("SSL");
            sslContext.init(null, trustAllCerts, new java.security.SecureRandom());
            
            // Create an ssl socket factory with our all-trusting manager
            final SSLSocketFactory sslSocketFactory = sslContext.getSocketFactory();

            OkHttpClient.Builder builder = new OkHttpClient.Builder();
            builder.sslSocketFactory(sslSocketFactory, (X509TrustManager)trustAllCerts[0]);
            builder.hostnameVerifier((hostname, session) -> true);
            return builder;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    public static ApiService getEMarketService(Context context) {
        String baseUrl = getBaseUrl(context);
        if (emarketRetrofit == null || !baseUrl.equals(currentBaseUrl)) {
            currentBaseUrl = baseUrl;
            HttpLoggingInterceptor logging = new HttpLoggingInterceptor();
            logging.setLevel(HttpLoggingInterceptor.Level.BODY);

            OkHttpClient client = getUnsafeOkHttpClientBuilder()
                    .cookieJar(new CookieJar() {
                        @Override
                        public void saveFromResponse(HttpUrl url, List<Cookie> cookies) {
                            cookieStore.put(url.host(), cookies);
                        }

                        @Override
                        public List<Cookie> loadForRequest(HttpUrl url) {
                            List<Cookie> cookies = cookieStore.get(url.host());
                            return cookies != null ? cookies : new ArrayList<>();
                        }
                    })
                    .addInterceptor(new Interceptor() {
                        @Override
                        public Response intercept(Chain chain) throws IOException {
                            SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
                            String token = prefs.getString(KEY_TOKEN, null);
                            
                            Request.Builder builder = chain.request().newBuilder();
                            if (token != null) {
                                builder.addHeader("Authorization", "Bearer " + token);
                            }
                            Response response = chain.proceed(builder.build());
                            if (response.code() == 401 && token != null) {
                                clearToken(context);
                                com.example.do_an_di_dong.util.UserSessionManager.clearSession(context);
                            }
                            return response;
                        }
                    })
                    .addInterceptor(logging)
                    .followRedirects(false)
                    .followSslRedirects(false)
                    .build();

            emarketRetrofit = new Retrofit.Builder()
                    .baseUrl(baseUrl)
                    .client(client)
                    .addConverterFactory(GsonConverterFactory.create())
                    .build();
        }
        return emarketRetrofit.create(ApiService.class);
    }

    public static ApiService getAiService(Context context) {
        if (aiRetrofit == null) {
            HttpLoggingInterceptor logging = new HttpLoggingInterceptor();
            logging.setLevel(HttpLoggingInterceptor.Level.BODY);

            OkHttpClient client = getUnsafeOkHttpClientBuilder()
                    .addInterceptor(new Interceptor() {
                        @Override
                        public Response intercept(Chain chain) throws IOException {
                            SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
                            String token = prefs.getString(KEY_TOKEN, null);
                            
                            Request.Builder builder = chain.request().newBuilder();
                            if (token != null) {
                                builder.addHeader("Authorization", "Bearer " + token);
                            }
                            builder.addHeader("Accept", "application/json");
                            return chain.proceed(builder.build());
                        }
                    })
                    .addInterceptor(logging)
                    .build();

            aiRetrofit = new Retrofit.Builder()
                    .baseUrl(AI_BASE_URL)
                    .client(client)
                    .addConverterFactory(GsonConverterFactory.create())
                    .build();
        }
        return aiRetrofit.create(ApiService.class);
    }

    public static void saveToken(Context context, String token) {
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        prefs.edit().putString(KEY_TOKEN, token).apply();
    }

    public static String getToken(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        return prefs.getString(KEY_TOKEN, null);
    }

    public static void clearToken(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        prefs.edit().remove(KEY_TOKEN).apply();
        cookieStore.clear();
    }

    public static String getBaseUrl(Context context) {
        if (context != null) {
            SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
            String saved = prefs.getString(KEY_CUSTOM_BASE_URL, null);
            if (saved != null && !saved.trim().isEmpty()) {
                String trimmed = saved.trim();
                if (!trimmed.endsWith("/")) trimmed += "/";
                return trimmed;
            }
        }
        return DEFAULT_EMARKET_BASE_URL;
    }

    public static String getBaseUrl() {
        return currentBaseUrl != null ? currentBaseUrl : DEFAULT_EMARKET_BASE_URL;
    }

    public static void setBaseUrl(Context context, String newBaseUrl) {
        if (newBaseUrl == null || newBaseUrl.trim().isEmpty()) return;
        String trimmed = newBaseUrl.trim();
        if (!trimmed.endsWith("/")) trimmed += "/";

        if (context != null) {
            SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
            prefs.edit().putString(KEY_CUSTOM_BASE_URL, trimmed).apply();
        }
        currentBaseUrl = trimmed;
        emarketRetrofit = null; // Forces recreate with new base URL
    }

    public static String getFullImageUrl(String relativePath) {
        if (relativePath == null || relativePath.trim().isEmpty()) {
            return null;
        }
        String path = relativePath.trim();
        if (path.startsWith("http://") || path.startsWith("https://")) {
            return path;
        }
        if (path.startsWith("~")) {
            path = path.substring(1);
        }
        if (path.startsWith("/")) {
            path = path.substring(1);
        }
        String baseUrl = getBaseUrl();
        if (!baseUrl.endsWith("/")) {
            baseUrl += "/";
        }
        return baseUrl + path;
    }

    public static Call<LoginResponse> externalLogin(Context context, String provider, String email, String idToken) {
        ExternalLoginRequest request = new ExternalLoginRequest(provider, email, idToken, null);
        return getEMarketService(context).externalLogin(request);
    }
}
