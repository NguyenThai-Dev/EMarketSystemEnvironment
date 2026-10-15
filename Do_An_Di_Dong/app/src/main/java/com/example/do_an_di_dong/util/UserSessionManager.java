package com.example.do_an_di_dong.util;

import android.content.Context;
import android.content.SharedPreferences;

import com.example.do_an_di_dong.model.Branch;
import com.example.do_an_di_dong.model.User;
import com.example.do_an_di_dong.model.Warehouse;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

public class UserSessionManager {
    private static final String PREF_NAME = "EMarketUserSession";
    private static final String KEY_USER_JSON = "key_user_json";
    private static final String KEY_USER_ID = "key_user_id";
    private static final String KEY_USER_FULL_NAME = "key_user_full_name";
    private static final String KEY_BRANCH_ID = "key_branch_id";
    private static final String KEY_BRANCH_NAME = "key_branch_name";
    private static final String KEY_CACHED_BRANCHES = "key_cached_branches";
    private static final String KEY_CACHED_WAREHOUSES = "key_cached_warehouses";

    private static final Gson gson = new Gson();

    public static void saveUserSession(Context context, User user) {
        if (context == null || user == null) return;
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = prefs.edit();
        editor.putInt(KEY_USER_ID, user.getUserId());
        editor.putString(KEY_USER_FULL_NAME, user.getFullName());
        editor.putInt(KEY_BRANCH_ID, user.getBranchId() != null ? user.getBranchId() : 1);
        editor.putString(KEY_USER_JSON, gson.toJson(user));
        editor.apply();
    }

    public static int getCurrentUserId(Context context) {
        if (context == null) return 1;
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        return prefs.getInt(KEY_USER_ID, 1);
    }

    public static int getCurrentBranchId(Context context) {
        if (context == null) return 1;
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        return prefs.getInt(KEY_BRANCH_ID, 1);
    }

    public static void setCurrentBranchId(Context context, int branchId, String branchName) {
        if (context == null) return;
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        prefs.edit()
                .putInt(KEY_BRANCH_ID, branchId)
                .putString(KEY_BRANCH_NAME, branchName)
                .apply();
    }

    public static String getCurrentBranchName(Context context) {
        if (context == null) return "Chi nhánh";
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        return prefs.getString(KEY_BRANCH_NAME, "Chi nhánh");
    }

    public static String getCurrentUserFullName(Context context) {
        if (context == null) return "Thu Ngân";
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        return prefs.getString(KEY_USER_FULL_NAME, "Thu Ngân");
    }

    public static User getCurrentUser(Context context) {
        if (context == null) return null;
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        String json = prefs.getString(KEY_USER_JSON, null);
        if (json == null) return null;
        try {
            return gson.fromJson(json, User.class);
        } catch (Exception e) {
            return null;
        }
    }

    // Branch list cache
    public static void saveBranches(Context context, List<Branch> branches) {
        if (context == null || branches == null) return;
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        prefs.edit().putString(KEY_CACHED_BRANCHES, gson.toJson(branches)).apply();
    }

    public static List<Branch> getCachedBranches(Context context) {
        if (context == null) return new ArrayList<>();
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        String json = prefs.getString(KEY_CACHED_BRANCHES, null);
        if (json == null) return new ArrayList<>();
        try {
            Type listType = new TypeToken<List<Branch>>() {}.getType();
            List<Branch> list = gson.fromJson(json, listType);
            return list != null ? list : new ArrayList<>();
        } catch (Exception e) {
            return new ArrayList<>();
        }
    }

    // Warehouse list cache
    public static void saveWarehouses(Context context, List<Warehouse> warehouses) {
        if (context == null || warehouses == null) return;
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        prefs.edit().putString(KEY_CACHED_WAREHOUSES, gson.toJson(warehouses)).apply();
    }

    public static List<Warehouse> getCachedWarehouses(Context context) {
        if (context == null) return new ArrayList<>();
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        String json = prefs.getString(KEY_CACHED_WAREHOUSES, null);
        if (json == null) return new ArrayList<>();
        try {
            Type listType = new TypeToken<List<Warehouse>>() {}.getType();
            List<Warehouse> list = gson.fromJson(json, listType);
            return list != null ? list : new ArrayList<>();
        } catch (Exception e) {
            return new ArrayList<>();
        }
    }

    public static boolean hasPermission(Context context, String moduleOrPermission) {
        User user = getCurrentUser(context);
        return user != null && user.hasPermission(moduleOrPermission);
    }

    public static boolean hasAnyPermission(Context context, String... modulesOrPermissions) {
        User user = getCurrentUser(context);
        return user != null && user.hasAnyPermission(modulesOrPermissions);
    }

    public static boolean isAdmin(Context context) {
        User user = getCurrentUser(context);
        return user != null && user.isAdmin();
    }

    public static String getRoleName(Context context) {
        User user = getCurrentUser(context);
        return user != null ? user.getPrimaryRoleName() : "Khách";
    }

    public static void clearSession(Context context) {
        if (context == null) return;
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        prefs.edit().clear().apply();
    }
}
