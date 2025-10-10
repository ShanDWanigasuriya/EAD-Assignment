package com.example.ev.util;

import android.app.Activity;
import android.widget.Toast;

public class Ui {
    public static void toast(Activity a, String m) {
        a.runOnUiThread(() -> Toast.makeText(a, m, Toast.LENGTH_SHORT).show());
    }
}
