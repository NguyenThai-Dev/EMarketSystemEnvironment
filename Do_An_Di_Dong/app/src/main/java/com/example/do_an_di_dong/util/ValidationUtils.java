package com.example.do_an_di_dong.util;

import android.text.TextUtils;
import android.util.Patterns;
import java.util.regex.Pattern;

public class ValidationUtils {
    private static final Pattern VN_PHONE_PATTERN = Pattern.compile("^(0[3|5|7|8|9])[0-9]{8}$");

    public static boolean isValidEmail(CharSequence email) {
        return !TextUtils.isEmpty(email) && Patterns.EMAIL_ADDRESS.matcher(email).matches();
    }

    public static boolean isValidVnPhone(String phone) {
        if (TextUtils.isEmpty(phone)) return false;
        String clean = phone.trim().replaceAll("\\s+", "");
        return VN_PHONE_PATTERN.matcher(clean).matches();
    }

    public static boolean isNotEmpty(String text) {
        return text != null && !text.trim().isEmpty();
    }

    public static boolean isPositiveNumber(double number) {
        return number > 0;
    }

    public static boolean isPositiveInteger(int number) {
        return number > 0;
    }
}
