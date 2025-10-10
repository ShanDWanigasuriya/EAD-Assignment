package com.example.ev.activities;

import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import androidx.appcompat.app.AppCompatActivity;

import com.example.ev.R;
import com.example.ev.data.UserDao;
import com.example.ev.net.ApiService;
import com.example.ev.util.Ui;

public class RegisterActivity extends AppCompatActivity {
    EditText etNic, etName, etEmail, etPhone;
    Button btnRegister;

    @Override protected void onCreate(Bundle b) {
        super.onCreate(b);
        setContentView(R.layout.activity_register);

        etNic = findViewById(R.id.etNic);
        etName = findViewById(R.id.etName);
        etEmail = findViewById(R.id.etEmail);
        etPhone = findViewById(R.id.etPhone);
        btnRegister = findViewById(R.id.btnRegister);

        btnRegister.setOnClickListener(v -> doRegister());
    }

    private void doRegister() {
        String nic = etNic.getText().toString().trim();
        String name = etName.getText().toString().trim();
        String email = etEmail.getText().toString().trim();
        String phone = etPhone.getText().toString().trim();
        if (nic.isEmpty() || name.isEmpty()) { Ui.toast(this, "NIC & Name required"); return; }

        new Thread(() -> {
            try {
                new ApiService().registerOwner(nic, name, email, phone);
                new UserDao(this).upsertOwner(nic, name, email, phone, true);
                Ui.toast(this, "Registered!");
                finish();
            } catch (Exception e) {
                Ui.toast(this, "Failed: " + e.getMessage());
            }
        }).start();
    }
}
