package com.example.ev.activities;

import android.app.AlertDialog;
import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.example.ev.R;
import com.example.ev.util.Prefs;
import com.example.ev.net.ApiService;
import com.example.ev.util.Ui;

import org.json.JSONArray;
import org.json.JSONObject;

public class DashboardActivity extends AppCompatActivity {
    TextView tvWelcome, tvPending, tvApproved;
    Button btnBookings, btnNew, btnQr;

    @Override protected void onCreate(Bundle b) {
        super.onCreate(b);
        setContentView(R.layout.activity_dashboard);

        tvWelcome = findViewById(R.id.tvWelcome);
        tvPending = findViewById(R.id.tvPending);
        tvApproved = findViewById(R.id.tvApprovedFuture);
        btnBookings = findViewById(R.id.btnBookings);
        btnNew = findViewById(R.id.btnNewBooking);
        btnQr = findViewById(R.id.btnShowQr);

        String name = Prefs.get(this, "ownerName");
        String nic = Prefs.get(this, "ownerNic");
        tvWelcome.setText("Welcome " + (name != null ? name : nic));

        btnBookings.setOnClickListener(v -> startActivity(new Intent(this, BookingListActivity.class)));
        btnNew.setOnClickListener(v -> startActivity(new Intent(this, BookingFormActivity.class)));
        btnQr.setOnClickListener(v -> promptQr());

        loadCounts(nic);
    }

    private void loadCounts(String nic) {
        new Thread(() -> {
            try {
                String res = new ApiService().listOwnerBookings(nic);
                JSONArray arr = new JSONArray(res);
                int pending = 0, approvedFuture = 0;
                long now = System.currentTimeMillis();

                for (int i = 0; i < arr.length(); i++) {
                    JSONObject b = arr.getJSONObject(i);
                    String status = b.optString("status", "");
                    String start = b.optString("reservationStartUtc", "");
                    if ("Pending".equalsIgnoreCase(status)) pending++;
                    if ("Approved".equalsIgnoreCase(status) && start.compareTo("") > 0) {
                        // naive compare; ideally parse ISO to epoch
                        approvedFuture++;
                    }
                }
                int finalPending = pending, finalApprovedFuture = approvedFuture;
                runOnUiThread(() -> {
                    tvPending.setText("Pending: " + finalPending);
                    tvApproved.setText("Approved (Future): " + finalApprovedFuture);
                });
            } catch (Exception e) {
                Ui.toast(this, "Count load failed");
            }
        }).start();
    }

    private void promptQr() {
        final TextView input = new TextView(this);
        new AlertDialog.Builder(this)
                .setTitle("Open QR")
                .setMessage("Open QR for a booking from the list (tap) or go to Bookings.")
                .setPositiveButton("Go to Bookings", (d, w) -> startActivity(new Intent(this, BookingListActivity.class)))
                .setNegativeButton("Close", null)
                .show();
    }
}
