package com.example.ev.activities;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import androidx.appcompat.app.AppCompatActivity;

import com.example.ev.R;
import com.example.ev.net.ApiService;
import com.example.ev.util.Prefs;
import com.example.ev.util.Ui;

import org.json.JSONObject;

public class OperatorLoginActivity extends AppCompatActivity {
    EditText etUser, etPass;
    Button btnLogin;

    @Override protected void onCreate(Bundle b) {
        super.onCreate(b);
        setContentView(R.layout.activity_operator_login);

        etUser = findViewById(R.id.etUser);
        etPass = findViewById(R.id.etPass);
        btnLogin = findViewById(R.id.btnLogin);

        btnLogin.setOnClickListener(v -> doLogin());
    }

    private void doLogin() {
        String u = etUser.getText().toString().trim();
        String p = etPass.getText().toString().trim();
        if (u.isEmpty() || p.isEmpty()) { Ui.toast(this, "Username & Password required"); return; }

        new Thread(() -> {
            try {
                String res = new ApiService().operatorLogin(u, p);
                JSONObject j = new JSONObject(res);
                String token = j.optString("token", null); // adapt to your login response
                if (token == null) { Ui.toast(this, "No token"); return; }
                Prefs.put(this, "operatorToken", token);
                runOnUiThread(() -> {
                    startActivity(new Intent(this, OperatorScanActivity.class));
                    finish();
                });
            } catch (Exception e) {
                Ui.toast(this, "Login failed: " + e.getMessage());
            }
        }).start();
    }
}
