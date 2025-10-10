package com.example.ev.activities;

import android.os.Bundle;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.ev.R;
import com.example.ev.adapters.BookingAdapter;
import com.example.ev.data.models.Booking;
import com.example.ev.net.ApiService;
import com.example.ev.util.Prefs;
import com.example.ev.util.Ui;

import org.json.JSONArray;
import org.json.JSONObject;

import java.util.ArrayList;

public class BookingListActivity extends AppCompatActivity {
    RecyclerView rv;
    ArrayList<Booking> items = new ArrayList<>();

    @Override protected void onCreate(Bundle b) {
        super.onCreate(b);
        setContentView(R.layout.activity_booking_list);
        rv = findViewById(R.id.rvBookings);
        rv.setLayoutManager(new LinearLayoutManager(this));
        rv.setAdapter(new BookingAdapter(this, items));
        load();
    }

    private void load() {
        String nic = Prefs.get(this, "ownerNic");
        new Thread(() -> {
            try {
                String res = new ApiService().listOwnerBookings(nic);
                JSONArray arr = new JSONArray(res);
                items.clear();
                for (int i = 0; i < arr.length(); i++) {
                    JSONObject j = arr.getJSONObject(i);
                    Booking b = new Booking();
                    b.id = j.optString("id");
                    b.stationId = j.optString("stationId");
                    b.reservationStartUtc = j.optString("reservationStartUtc");
                    b.reservationEndUtc = j.optString("reservationEndUtc");
                    b.status = j.optString("status");
                    items.add(b);
                }
                runOnUiThread(() -> rv.getAdapter().notifyDataSetChanged());
            } catch (Exception e) {
                Ui.toast(this, "Load failed: " + e.getMessage());
            }
        }).start();
    }
}
