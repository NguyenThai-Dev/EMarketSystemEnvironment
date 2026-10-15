package com.example.do_an_di_dong.util;

import android.text.Editable;
import android.text.TextWatcher;
import android.widget.EditText;

import java.lang.ref.WeakReference;
import java.text.DecimalFormat;
import java.text.DecimalFormatSymbols;
import java.util.Locale;

public class MoneyTextWatcher implements TextWatcher {
    public interface OnAmountChangedListener {
        void onAmountChanged(String cleanAmount);
    }

    private final WeakReference<EditText> editTextWeakReference;
    private final DecimalFormat formatter;
    private final OnAmountChangedListener listener;
    private String current = "";

    public MoneyTextWatcher(EditText editText) {
        this(editText, null);
    }

    public MoneyTextWatcher(EditText editText, OnAmountChangedListener listener) {
        this.editTextWeakReference = new WeakReference<>(editText);
        this.listener = listener;
        DecimalFormatSymbols symbols = new DecimalFormatSymbols(Locale.getDefault());
        symbols.setGroupingSeparator(',');
        symbols.setDecimalSeparator('.');
        this.formatter = new DecimalFormat("#,###", symbols);
    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

    @Override
    public void onTextChanged(CharSequence s, int start, int count, int after) {}

    @Override
    public void afterTextChanged(Editable s) {
        EditText editText = editTextWeakReference.get();
        if (editText == null) return;

        String str = s.toString();
        if (str.equals(current)) return;

        String cleanString = str.replaceAll("[^0-9]", "");
        if (cleanString.isEmpty()) {
            current = "";
            editText.setText("");
            if (listener != null) listener.onAmountChanged("");
            return;
        }

        try {
            double parsed = Double.parseDouble(cleanString);
            String formatted = formatter.format(parsed);
            current = formatted;
            editText.setText(formatted);
            editText.setSelection(formatted.length());
            if (listener != null) listener.onAmountChanged(cleanString);
        } catch (NumberFormatException ignored) {
        }
    }

    public static double parseAmount(EditText editText) {
        if (editText == null || editText.getText() == null) return 0.0;
        String cleanString = editText.getText().toString().replaceAll("[^0-9]", "");
        if (cleanString.isEmpty()) return 0.0;
        try {
            return Double.parseDouble(cleanString);
        } catch (NumberFormatException e) {
            return 0.0;
        }
    }

    public static String formatCurrency(double amount) {
        DecimalFormatSymbols symbols = new DecimalFormatSymbols(Locale.getDefault());
        symbols.setGroupingSeparator(',');
        DecimalFormat df = new DecimalFormat("#,### đ", symbols);
        return df.format(amount);
    }
}
