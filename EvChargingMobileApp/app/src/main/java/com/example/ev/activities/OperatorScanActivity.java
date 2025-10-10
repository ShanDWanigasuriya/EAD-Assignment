package com.example.ev.activities;

import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ev.R;
import com.example.ev.net.ApiService;
import com.example.ev.util.Prefs;
import com.example.ev.util.Ui;
import com.google.zxing.integration.android.IntentIntegrator;
import com.google.zxing.integration.android.IntentResult;

public class OperatorScanActivity extends AppCompatActivity {
    Button btnScan, btnComplete; TextView tv;
    String lastBookingId;

    @Override protected void onCreate(Bundle b) {
        super.onCreate(b);
        setContentView(R.layout.activity_operator_scan);
        btnScan = findViewById(R.id.btnScan);
        btnComplete = findViewById(R.id.btnComplete);
        tv = findViewById(R.id.tvResult);

        btnScan.setOnClickListener(v -> new IntentIntegrator(this).initiateScan());
        btnComplete.setOnClickListener(v -> complete());
    }

    @Override protected void onActivityResult(int requestCode, int resultCode, android.content.Intent data) {
        IntentResult result = IntentIntegrator.parseActivityResult(requestCode, resultCode, data);
        super.onActivityResult(requestCode, resultCode, data);
        if (result != null && result.getContents() != null) {
            lastBookingId = result.getContents();
            tv.setText("Scanned booking: " + lastBookingId);
        }
    }

    private void complete() {
        if (lastBookingId == null) { Ui.toast(this, "Scan a QR first"); return; }
        String token = Prefs.get(this, "operatorToken");
        if (token == null) { Ui.toast(this, "Operator not logged in"); return; }

        new Thread(() -> {
            try {
                new ApiService().completeBooking(lastBookingId, token);
                Ui.toast(this, "Marked complete");
            } catch (Exception e) {
                Ui.toast(this, "Failed: " + e.getMessage());
            }
        }).start();
    }
}
