package com.example.ev.activities;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import androidx.appcompat.app.AppCompatActivity;

import com.example.ev.R;
import com.example.ev.data.UserDao;
import com.example.ev.net.ApiService;
import com.example.ev.util.Prefs;
import com.example.ev.util.Ui;

import org.json.JSONObject;

public class LoginActivity extends AppCompatActivity {
    EditText etNic;
    Button btnLogin, btnRegister, btnOperator;

    @Override protected void onCreate(Bundle b) {
        super.onCreate(b);
        setContentView(R.layout.activity_login);

        etNic = findViewById(R.id.etNic);
        btnLogin = findViewById(R.id.btnLogin);
        btnRegister = findViewById(R.id.btnRegister);
        btnOperator = findViewById(R.id.btnOperator);

        btnLogin.setOnClickListener(v -> loginOwner());
        btnRegister.setOnClickListener(v -> startActivity(new Intent(this, RegisterActivity.class)));
        btnOperator.setOnClickListener(v -> startActivity(new Intent(this, OperatorLoginActivity.class)));
    }

    private void loginOwner() {
        String nic = etNic.getText().toString().trim();
        if (nic.isEmpty()) { Ui.toast(this, "NIC required"); return; }

        new Thread(() -> {
            try {
                String res = new ApiService().getOwner(nic);
                JSONObject j = new JSONObject(res);
                // Expecting API to return an owner or 404; adapt to your shape:
                String name = j.optString("name", "");
                String email = j.optString("email", "");
                String phone = j.optString("phone", "");
                boolean active = j.optBoolean("active", true);

                if (!active) {
                    Ui.toast(this, "Account deactivated. Contact Backoffice.");
                    return;
                }

                new UserDao(this).upsertOwner(nic, name, email, phone, true);
                Prefs.put(this, "ownerNic", nic);
                Prefs.put(this, "ownerName", name);
                runOnUiThread(() -> {
                    startActivity(new Intent(this, DashboardActivity.class));
                    finish();
                });
            } catch (Exception e) {
                Ui.toast(this, "Login failed: " + e.getMessage());
            }
        }).start();
    }
}
