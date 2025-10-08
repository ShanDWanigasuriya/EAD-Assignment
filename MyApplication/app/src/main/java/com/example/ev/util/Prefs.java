package com.example.ev.util;

import android.content.Context;

public class Prefs {
    private static final String N = "ev_prefs";

    public static void put(Context c, String k, String v) {
        c.getSharedPreferences(N, Context.MODE_PRIVATE).edit().putString(k, v).apply();
    }
    public static String get(Context c, String k) {
        return c.getSharedPreferences(N, Context.MODE_PRIVATE).getString(k, null);
    }

    public static void putBool(Context c, String k, boolean v) {
        c.getSharedPreferences(N, Context.MODE_PRIVATE).edit().putBoolean(k, v).apply();
    }
    public static boolean getBool(Context c, String k, boolean def) {
        return c.getSharedPreferences(N, Context.MODE_PRIVATE).getBoolean(k, def);
    }
}
