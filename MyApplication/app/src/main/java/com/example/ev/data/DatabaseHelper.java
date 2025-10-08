package com.example.ev.data;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

public class DatabaseHelper extends SQLiteOpenHelper {
    public static final String DB_NAME = "evcharging.db";
    public static final int DB_VERSION = 1;

    // Owner table
    public static final String T_OWNER = "owner";
    public static final String C_NIC = "nic"; // PK
    public static final String C_NAME = "name";
    public static final String C_EMAIL = "email";
    public static final String C_PHONE = "phone";
    public static final String C_ACTIVE = "active"; // 0/1

    // Simple booking cache to show latest fetched list offline (optional)
    public static final String T_BOOKING = "booking_cache";
    public static final String B_ID = "id";
    public static final String B_STATION = "stationId";
    public static final String B_START = "startUtc";
    public static final String B_END = "endUtc";
    public static final String B_STATUS = "status";

    public DatabaseHelper(Context ctx) { super(ctx, DB_NAME, null, DB_VERSION); }

    @Override public void onCreate(SQLiteDatabase db) {
        db.execSQL("CREATE TABLE " + T_OWNER + "(" +
                C_NIC + " TEXT PRIMARY KEY," +
                C_NAME + " TEXT," +
                C_EMAIL + " TEXT," +
                C_PHONE + " TEXT," +
                C_ACTIVE + " INTEGER)");
        db.execSQL("CREATE TABLE " + T_BOOKING + "(" +
                B_ID + " TEXT PRIMARY KEY," +
                B_STATION + " TEXT," +
                B_START + " TEXT," +
                B_END + " TEXT," +
                B_STATUS + " TEXT)");
    }

    @Override public void onUpgrade(SQLiteDatabase db, int ov, int nv) {
        db.execSQL("DROP TABLE IF EXISTS " + T_OWNER);
        db.execSQL("DROP TABLE IF EXISTS " + T_BOOKING);
        onCreate(db);
    }
}
