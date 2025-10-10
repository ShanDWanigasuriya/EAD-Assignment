package com.example.ev.data;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;

public class UserDao {
    private final DatabaseHelper dbh;
    public UserDao(Context ctx) { dbh = new DatabaseHelper(ctx); }

    public boolean upsertOwner(String nic, String name, String email, String phone, boolean active) {
        SQLiteDatabase db = dbh.getWritableDatabase();
        ContentValues v = new ContentValues();
        v.put(DatabaseHelper.C_NIC, nic);
        v.put(DatabaseHelper.C_NAME, name);
        v.put(DatabaseHelper.C_EMAIL, email);
        v.put(DatabaseHelper.C_PHONE, phone);
        v.put(DatabaseHelper.C_ACTIVE, active ? 1 : 0);
        long r = db.insertWithOnConflict(DatabaseHelper.T_OWNER, null, v, SQLiteDatabase.CONFLICT_REPLACE);
        return r != -1;
    }

    public boolean isActiveOwner(String nic) {
        SQLiteDatabase db = dbh.getReadableDatabase();
        try (Cursor c = db.query(DatabaseHelper.T_OWNER, new String[]{DatabaseHelper.C_ACTIVE},
                DatabaseHelper.C_NIC + "=?", new String[]{nic}, null, null, null)) {
            if (c.moveToFirst()) return c.getInt(0) == 1;
            return false;
        }
    }

    public String getOwnerName(String nic) {
        SQLiteDatabase db = dbh.getReadableDatabase();
        try (Cursor c = db.query(DatabaseHelper.T_OWNER, new String[]{DatabaseHelper.C_NAME},
                DatabaseHelper.C_NIC + "=?", new String[]{nic}, null, null, null)) {
            if (c.moveToFirst()) return c.getString(0);
            return null;
        }
    }
}
