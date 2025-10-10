package com.example.ev.adapters;

import android.content.Context;
import android.content.Intent;
import android.view.*;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ev.R;
import com.example.ev.activities.QrActivity;
import com.example.ev.data.models.Booking;

import java.util.List;

public class BookingAdapter extends RecyclerView.Adapter<BookingAdapter.VH> {
    Context ctx; List<Booking> data;
    public BookingAdapter(Context c, List<Booking> d){ ctx=c; data=d; }

    @NonNull @Override public VH onCreateViewHolder(@NonNull ViewGroup p, int v) {
        View view = LayoutInflater.from(ctx).inflate(R.layout.item_booking, p, false);
        return new VH(view);
    }

    @Override public void onBindViewHolder(@NonNull VH h, int i) {
        Booking b = data.get(i);
        h.tvId.setText("ID: " + b.id);
        h.tvStation.setText("Station: " + b.stationId);
        h.tvTime.setText(b.reservationStartUtc + " → " + b.reservationEndUtc);
        h.tvStatus.setText("Status: " + b.status);
        h.itemView.setOnClickListener(v -> {
            // Tap → show QR if approved
            Intent it = new Intent(ctx, QrActivity.class);
            it.putExtra("bookingId", b.id);
            ctx.startActivity(it);
        });
    }

    @Override public int getItemCount() { return data.size(); }

    static class VH extends RecyclerView.ViewHolder {
        TextView tvId, tvStation, tvTime, tvStatus;
        public VH(@NonNull View v) {
            super(v);
            tvId = v.findViewById(R.id.tvId);
            tvStation = v.findViewById(R.id.tvStation);
            tvTime = v.findViewById(R.id.tvTime);
            tvStatus = v.findViewById(R.id.tvStatus);
        }
    }
}
