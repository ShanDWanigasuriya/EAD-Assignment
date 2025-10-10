package com.example.ev.activities;

import android.graphics.Bitmap;
import android.os.Bundle;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.example.ev.R;
import com.google.zxing.BarcodeFormat;
import com.journeyapps.barcodescanner.BarcodeEncoder;

public class QrActivity extends AppCompatActivity {
    @Override protected void onCreate(Bundle b) {
        super.onCreate(b);
        setContentView(R.layout.activity_qr);

        String bookingId = getIntent().getStringExtra("bookingId");
        ImageView img = findViewById(R.id.imgQr);
        TextView tv = findViewById(R.id.tvHint);

        try {
            BarcodeEncoder enc = new BarcodeEncoder();
            Bitmap bmp = enc.encodeBitmap(bookingId, BarcodeFormat.QR_CODE, 700, 700);
            img.setImageBitmap(bmp);
            tv.setText("Booking: " + bookingId);
        } catch (Exception e) {
            tv.setText("QR failed: " + e.getMessage());
        }
    }
}
