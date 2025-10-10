package com.example.ev.activities;

import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import androidx.appcompat.app.AppCompatActivity;

import com.example.ev.R;
import com.example.ev.net.ApiService;
import com.example.ev.util.Prefs;
import com.example.ev.util.Ui;

public class BookingFormActivity extends AppCompatActivity {
    EditText etStation, etStart, etEnd;
    Button btnCreate;

    @Override protected void onCreate(Bundle b) {
        super.onCreate(b);
        setContentView(R.layout.activity_booking_form);

        etStation = findViewById(R.id.etStationId);
        etStart = findViewById(R.id.etStart);
        etEnd = findViewById(R.id.etEnd);
        btnCreate = findViewById(R.id.btnCreate);

        btnCreate.setOnClickListener(v -> create());
    }

    private void create() {
        String nic = Prefs.get(this, "ownerNic");
        String st = etStation.getText().toString().trim();
        String s = etStart.getText().toString().trim();
        String e = etEnd.getText().toString().trim();
        if (st.isEmpty() || s.isEmpty() || e.isEmpty()) { Ui.toast(this, "All fields required"); return; }

        new Thread(() -> {
            try {
                new ApiService().createBooking(nic, st, s, e);
                Ui.toast(this, "Created");
                finish();
            } catch (Exception ex) {
                Ui.toast(this, "Failed: " + ex.getMessage());
            }
        }).start();
    }
}
