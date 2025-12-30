# 🚀 TETRIS SPACE

---

## 🎮 Giới thiệu

**Tetris Space** là phiên bản hiện đại của trò chơi xếp gạch cổ điển, được phát triển bằng **C# và WPF**, tích hợp hệ thống lưu trữ đám mây qua **Supabase**.

Game không chỉ tập trung vào trải nghiệm xếp gạch mượt mà mà còn cung cấp hệ thống tài khoản, bảng xếp hạng toàn cầu (Global Ranking) và giao diện **Space Theme** ấn tượng.

---

## ✨ Tính năng chính

### 🕹️ Cơ chế Gameplay
- **7 loại khối Tetromino** chuẩn cổ điển.
- **Hold Mechanism (Giữ khối):** Cho phép chiến thuật linh hoạt hơn.
- **Next Queue:** Hiển thị trước khối tiếp theo.
- **Ghost Piece:** Hiển thị bóng mờ tại vị trí khối sẽ rơi xuống.
- **Pause Game:** Hệ thống tạm dừng thông minh.

### ⚙️ Chế độ chơi & Hệ thống điểm
- **3 Độ khó:** Easy – Normal – Hard.
- **Thuật toán tính điểm & Tốc độ:**
  - Mỗi độ khó có hàm tính riêng biệt.
  - **Tốc độ rơi:** Khoảng thời gian giữa 2 lần rơi giảm dần (nhanh hơn) theo Level.
  - **Điểm số:** Hàm tính điểm thưởng khác nhau tùy theo độ khó đã chọn.

### ☁️ Hệ thống Tài khoản & Database
- **Đăng ký / Đăng nhập / Quên mật khẩu** (xác thực qua Email).
- **Lưu trữ đám mây (Supabase):** Tự động lưu điểm cao, level và trạng thái game.
- **Sync Settings:** Cài đặt âm thanh, nhạc nền được lưu theo tài khoản (đăng nhập máy nào cũng giữ nguyên cài đặt).
- **Bảo mật:** Hệ thống OTP tự động xóa sau khi sử dụng hoặc hết hạn.

---

## 🎹 Hướng dẫn điều khiển

| Phím | Chức năng |
| :--- | :--- |
| **⬅️ / ➡️** | Di chuyển khối sang Trái / Phải |
| **⬇️** | Rơi nhanh (Soft Drop) |
| **⬆️** hoặc **X** | Xoay khối theo chiều kim đồng hồ |
| **Z** | Xoay khối ngược chiều kim đồng hồ |
| **C** / **Shift** (Trái/Phải) | Giữ khối hiện tại (Hold) |
| **Space** (Cách) | Thả khối xuống ngay lập tức (Hard Drop) |
| **Esc** | Tạm dừng trò chơi (Pause) |

---

## 🕹️ Cách chơi

1.  **Đăng nhập:** Khởi động game và đăng ký hoặc đăng nhập tài khoản để hệ thống tải dữ liệu của bạn.
2.  **Bắt đầu:** Nhấn **Play Game**, chọn độ khó (Easy/Normal/Hard). Các khối sẽ bắt đầu rơi theo tốc độ tương ứng.
3.  **Thao tác:** Sử dụng phím điều khiển để di chuyển, xoay và sắp xếp các khối gạch.
4.  **Ghi điểm:** Xếp kín một hàng ngang để phá hủy nó và nhận điểm cộng.
5.  **Kết thúc:** Game Over khi các khối gạch chạm đến đỉnh màn hình.
6.  **Lưu thành tích:** Điểm số cao nhất (Highscore) sẽ tự động được cập nhật lên Bảng xếp hạng.

---

## 🗃️ Schema Database

Game sử dụng bảng `Players` trên **Supabase** với cấu trúc chi tiết như sau:

| Tên trường | Kiểu dữ liệu | Mô tả |
| :--- | :--- | :--- |
| `id` | `int8` | Mã định danh độc nhất (Auto-generated) |
| `username` | `text` | Tên tài khoản hiển thị |
| `password` | `text` | Mật khẩu người chơi |
| `email` | `text` | Email đăng ký (Dùng để khôi phục/nhận OTP) |
| `highscore` | `int8` | Điểm số cao nhất đạt được |
| `game_save_data`| `jsonb` | Dữ liệu game đã lưu (Level, trạng thái bàn cờ...) |
| `music_enabled` | `bool` | Trạng thái Bật/Tắt nhạc nền |
| `music_vol` | `float8` | Mức âm lượng nhạc |
| `sfx_vol` | `float8` | Mức âm lượng hiệu ứng âm thanh |
| `selected_track`| `text` | Bài nhạc nền đang được chọn |
| `otp_code` | `text` | Mã OTP (Tự động xóa khi nhập đúng) |
| `otp_expiry` | `timestampz`| Thời gian hết hạn của mã OTP (5 phút) |

---

## 🛠️ Công nghệ sử dụng

* **Ngôn ngữ:** C# (.NET 6.0 / 8.0)
* **Giao diện:** WPF (Windows Presentation Foundation)
* **Backend:** Supabase (PostgreSQL, Auth, Edge Functions)
* **Thư viện:**
    * `Newtonsoft.Json` (Xử lý dữ liệu JSON)
    * `Supabase-csharp` (Kết nối Database)

---
🚀 **Tetris Space** – Đồ án Lập trình Trực quan.
