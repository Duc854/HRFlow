# HRFlow
A modern HR Management System built with .NET 8 Web API &amp; ASP.NET MVC. Leverages HTMX for seamless, "no-JS" reactive UI and JWT for secure cross-origin authentication

---

## Tech Stack

### Backend (Web API)
* **Framework:** .NET 8.0
* **Architecture:** Layered Architecture (Infrastructure, Application, Domain, API)
* **Database:** SQL Server
* **ORM:** Entity Framework Core (Code First)
* **Security:** JWT (JSON Web Token) với cơ chế xác thực phân quyền (RBAC)

### Frontend (Web Portal)
* **Framework:** ASP.NET Core MVC (BFF Pattern - Backend for Frontend)
* **UI Interactivity:** [HTMX](https://htmx.org/) (AJAX-based partial rendering)
* **Styling:** Bootstrap 5
* **Storage:** LocalStorage / HttpOnly Cookie

---

## 📊 Database Schema

Hệ thống được tổ chức thành 4 nhóm bảng chính:

1.  **Identity & Security:** `Users`, `Roles`, `UserRoles` - Quản lý tài khoản và phân quyền JWT.
2.  **Core HR:** `Employees`, `Departments`, `Positions` - Thông tin gốc của nhân sự và tổ chức.
3.  **Time & Attendance:** `TimeLogs`, `LeaveRequests` - Chấm công và quy trình duyệt nghỉ phép.
4.  **Payroll:** `Payrolls`, `Contracts` - Quản lý hợp đồng và bảng tính lương hàng tháng.

---

## Key Features

### Security & Access
- Xác thực dựa trên JWT Token.
- Phân quyền người dùng: **Admin, HR, Manager, Employee**.

### For HR & Admin
- **Dashboard:** Thống kê biến động nhân sự và trạng thái đi làm real-time.
- **Employee Management:** Quản lý vòng đời nhân viên từ lúc vào đến lúc nghỉ.
- **Approval Flow:** Phê duyệt đơn xin nghỉ phép trực tiếp qua UI.
- **Payroll Engine:** Tự động quét công và hợp đồng để tính lương định kỳ.

### For Employees (Self-Service)
- **Profile:** Tự quản lý thông tin cá nhân.
- **Attendance:** Theo dõi lịch sử vào/ra và tạo đơn nghỉ phép trực tuyến.
- **Digital Payslip:** Xem và tải phiếu lương hàng tháng.

---

## System Architecture

Dự án được tổ chức theo mô hình phân lớp (Layered Architecture) nhằm đảm bảo tính tách biệt giữa các thành phần (Separation of Concerns) và khả năng mở rộng
- **Backend (Web API):**
- **Phân chia theo 3 lớp chính (Core Layers):**
   - **API (Presentation Layer):** Chịu trách nhiệm tiếp nhận HTTP Requests, xử lý xác thực JWT và điều hướng dữ liệu.

   - **Business (Logic Layer):** Nơi chứa toàn bộ nghiệp vụ (Business Logic), các dịch vụ (Services) và xử lý tính toán.

   - **Data (Infrastructure Layer):** Quản lý kết nối SQL Server thông qua Entity Framework Core, Repository Pattern và thực hiện các thao tác Migrations.
- **Frontend (Web Portal):**
   - **WebPortal (ASP.NET MVC):** Đóng vai trò là lớp hiển thị và điều phối giao diện. Thay vì xử lý logic nặng, nó gọi qua Web API để lấy dữ liệu.
   - **HTMX Integration:** Được tích hợp vào các View của MVC để thực hiện các yêu cầu AJAX không đồng bộ, giúp cập nhật từng phần của giao diện mà không cần tải lại toàn bộ trang (Partial Rendering)

---

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB hoặc Docker)
- Visual Studio 2022 / JetBrains Rider

### Installation

1. **Clone repo**
   ```bash
   git clone https://github.com/Duc854/HRFlow.git
