# ⚡ EV Charging Station Booking System  
### End-to-End Solution using Client–Server Architecture  

---

## 📘 Overview
The **EV Charging Station Booking System** is a full-stack, end-to-end application built using a **client–server architecture**.  
It enables **Backoffice administrators**, **Station operators**, and **EV owners** to efficiently manage electric vehicle (EV) charging reservations through **centralized services**.  

The project includes:
- 🧩 **Web API (Server)** — Core backend service for authentication, booking, and data management.  
- 💻 **Web Application (Client)** — Administrative portal for backoffice operations.  
- 📱 **Mobile Application (Client)** — Android app for EV owners and station operators.

---

## ⚙️ Components Breakdown

### 1️⃣ **ASP.NET Core Web API**
- Acts as the **central backend** for all clients.  
- Handles business logic, validation, and database operations.  
- Implements **JWT-based authentication** and **role-based authorization** for:
  - `Backoffice` users (system admins)
  - `StationOperator` users (charging operators)

#### 🧾 Features
- EV Owner registration and management (via NIC).  
- Charging Station management (create, update, deactivate).  
- Booking lifecycle (Create → Approve → Complete / Cancel).  
- QR Code generation for approved bookings.  
- Atomic slot capacity management per station.  

#### 🗂️ Technologies
- **.NET 8 Web API**  
- **MongoDB (NoSQL)**  
- **JWT Authentication**  
- **QRCoder** for QR generation  
- **Swagger (OpenAPI)** for endpoint testing  

---

### 2️⃣ **Web Application (Backoffice Portal)**
- Built using **ASP.NET Core MVC (CoreUI Template)**.  
- Provides user-friendly dashboards and management tools for backoffice users.  

#### 🧾 Features
- Backoffice user login via JWT token.  
- Manage EV Owners, Stations, and Bookings.  
- Approve and monitor pending reservations.  
- Responsive dashboard UI (CoreUI 5.2.0).  

#### 🗂️ Technologies
- **ASP.NET Core MVC**  
- **CoreUI + Bootstrap 5**  
- **Axios / Fetch API** for backend communication  

---

### 3️⃣ **Mobile Application (Android)**
- Developed as a **native Android application (Java)**.  
- Provides separate access for **EV Owners** and **Station Operators**.  
- Uses a **local SQLite database** for offline user data storage.  

#### 🧾 Features
- **EV Owners:**
  - Register using NIC.
  - View available charging stations (Google Maps API).
  - Create, modify, or cancel bookings.
  - View upcoming and past reservations.
- **Station Operators:**
  - Login using credentials from Web API.
  - Scan QR codes to verify and complete bookings.
  - View assigned bookings and update completion status.

#### 🗂️ Technologies
- **Android Studio (Java)**  
- **SQLite** (local user storage)  
- **Retrofit** (for API communication)  
- **Google Maps API** (for station map view)  

---

## 🧠 Business Rules
- Bookings must be created **within 7 days** of the current date.  
- Bookings can be **updated or cancelled** only **12 hours** before the start time.  
- Booking status follows:
  Pending → Approved → Completed / Cancelled / Expired
- A station **cannot be deactivated** while active bookings exist.  

---

## 🧩 Database Design
**MongoDB Collections:**
- `users` — Stores system users (Backoffice & Station Operators).  
- `owners` — EV Owner data (NIC as logical key).  
- `stations` — Station info + embedded availability slots.  
- `bookings` — Reservation records with timestamps and QR payloads.  

Indexes:
- `owners.NIC` → Unique  
- `users.Username` → Unique  
- `bookings.OwnerNic`, `bookings.StationId` → Indexed  

## 🏁 Conclusion
This project demonstrates a fully functional **EV Charging Management Platform** integrating web, mobile, and backend layers under a client–server architecture.  
It fulfills all functional and technical requirements while adhering to software engineering best practices in authentication, data management, and modular design.

---

### 📄 License
This project was developed as part of the **SE4040 – Application Frameworks** module (2025).  
All source code is intended for academic and educational use only.

