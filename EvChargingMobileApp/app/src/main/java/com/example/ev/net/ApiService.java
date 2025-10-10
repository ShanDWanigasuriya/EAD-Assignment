package com.example.ev.net;

import org.json.JSONObject;

public class ApiService {
    // TODO: set to your API (e.g., "http://10.0.2.2:5000")
    public static final String BASE_URL = "http://YOUR-HOST:PORT";
    private final HttpClient http = new HttpClient(BASE_URL);

    // ---- Owners ----
    public String registerOwner(String nic, String name, String email, String phone) throws Exception {
        JSONObject b = new JSONObject()
                .put("nic", nic).put("name", name)
                .put("email", email).put("phone", phone);
        return http.post("/api/owners/register", b.toString(), null);
    }

    public String getOwner(String nic) throws Exception {
        return http.get("/api/owners/" + nic, null);
    }

    public String updateOwner(String nic, String name, String email, String phone) throws Exception {
        JSONObject b = new JSONObject()
                .put("name", name).put("email", email).put("phone", phone);
        return http.put("/api/owners/" + nic, b.toString(), null);
    }

    public String deactivateOwner(String nic) throws Exception {
        return http.patch("/api/owners/" + nic + "/deactivate", "{}", null);
    }

    // ---- Users (Operator) ----
    public String operatorLogin(String username, String password) throws Exception {
        JSONObject b = new JSONObject().put("username", username).put("password", password);
        return http.post("/api/users/login", b.toString(), null);
    }

    // ---- Stations ----
    public String listStations() throws Exception {
        return http.get("/api/stations", null);
    }

    public String getStationBookings(String stationId, String bearer) throws Exception {
        return http.get("/api/bookings/station/" + stationId, bearer);
    }

    // ---- Bookings (Owner) ----
    public String listOwnerBookings(String nic) throws Exception {
        return http.get("/api/bookings/owner/" + nic, null);
    }

    public String createBooking(String ownerNic, String stationId, String startUtc, String endUtc) throws Exception {
        JSONObject b = new JSONObject()
                .put("ownerNic", ownerNic)
                .put("stationId", stationId)
                .put("reservationStartUtc", startUtc)
                .put("reservationEndUtc", endUtc);
        return http.post("/api/bookings", b.toString(), null);
    }

    public String updateBooking(String bookingId, String stationId, String startUtc, String endUtc) throws Exception {
        JSONObject b = new JSONObject()
                .put("stationId", stationId)
                .put("reservationStartUtc", startUtc)
                .put("reservationEndUtc", endUtc);
        return http.put("/api/bookings/" + bookingId, b.toString(), null);
    }

    public String cancelBooking(String bookingId) throws Exception {
        return http.delete("/api/bookings/" + bookingId, null);
    }

    // ---- Booking Status (Operator) ----
    public String approveBooking(String bookingId, String bearer) throws Exception {
        return http.patch("/api/bookings/" + bookingId + "/approve", "{}", bearer);
    }

    public String completeBooking(String bookingId, String bearer) throws Exception {
        return http.patch("/api/bookings/" + bookingId + "/complete", "{}", bearer);
    }
}
