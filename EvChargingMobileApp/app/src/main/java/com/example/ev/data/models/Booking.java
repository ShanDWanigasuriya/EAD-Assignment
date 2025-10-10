package com.example.ev.data.models;

public class Booking {
    public String id;
    public String stationId;
    public String reservationStartUtc;
    public String reservationEndUtc;
    public String status; // Pending/Approved/Completed/Cancelled
}