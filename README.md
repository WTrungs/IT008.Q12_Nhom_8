# 🚀 TETRIS SPACE

---

## 🎮 Giới thiệu

**Tetris Space** là phiên bản hiện đại của trò chơi xếp gạch cổ điển, được phát triển bằng **C# và WPF**, tích hợp hệ thống lưu trữ đám mây qua **Supabase**.

[cite_start]Game không chỉ tập trung vào trải nghiệm xếp gạch mượt mà mà còn cung cấp hệ thống tài khoản, bảng xếp hạng toàn cầu (Global Ranking) và giao diện **Space Theme** ấn tượng[cite: 3, 4].

---

## ✨ Tính năng chính

### 🕹️ Cơ chế Gameplay
- [cite_start]**7 loại khối Tetromino** chuẩn cổ điển[cite: 6].
- [cite_start]**Hold Mechanism (Giữ khối):** Cho phép chiến thuật linh hoạt hơn[cite: 14].
- [cite_start]**Next Queue:** Hiển thị trước khối tiếp theo[cite: 15].
- **Ghost Piece:** Hiển thị bóng mờ tại vị trí khối sẽ rơi xuống.
- [cite_start]**Pause Game:** Hệ thống tạm dừng thông minh[cite: 13].

### ⚙️ Chế độ chơi & Hệ thống điểm
- [cite_start]**3 Độ khó:** Easy – Normal – Hard[cite: 8].
- **Thuật toán tính điểm & Tốc độ:**
  - [cite_start]Mỗi độ khó có hàm tính riêng biệt[cite: 33, 34].
  - [cite_start]**Tốc độ rơi:** Khoảng thời gian giữa 2 lần rơi giảm dần (nhanh hơn) theo Level[cite: 35].
  - [cite_start]**Điểm số:** Hàm tính điểm thưởng khác nhau tùy theo độ khó đã chọn[cite: 36].

### ☁️ Hệ thống Tài khoản & Database
- [cite_start]**Đăng ký / Đăng nhập / Quên mật khẩu** (xác thực qua Email)[cite: 7].
- [cite_start]**Lưu trữ đám mây (Supabase):** Tự động lưu điểm cao, level và trạng thái game[cite: 9, 10].
- [cite_start]**Sync Settings:** Cài đặt âm thanh, nhạc nền được lưu theo tài khoản (đăng nhập máy nào cũng giữ nguyên cài đặt)[cite: 11].
- [cite_start]**Bảo mật:** Hệ thống OTP tự động xóa sau khi sử dụng hoặc hết hạn[cite: 51].

---

## 🎹 Hướng dẫn điều khiển

| Phím | Chức năng |
| :--- | :--- |
| **⬅️ / ➡️** | [cite_start]Di chuyển khối sang Trái / Phải [cite: 17] |
| **⬇️** | [cite_start]Rơi nhanh (Soft Drop) [cite: 18] |
| **⬆️** hoặc **X** | [cite_start]Xoay khối theo chiều kim đồng hồ [cite: 19] |
| **Z** | [cite_start]Xoay khối ngược chiều kim đồng hồ [cite: 20] |
| **C** / **Shift** (Trái/Phải) | [cite_start]Giữ khối hiện tại (Hold) [cite: 21] |
| **Space** (Cách) | [cite_start]Thả khối xuống ngay lập tức (Hard Drop) [cite: 22] |
| **Esc** | [cite_start]Tạm dừng trò chơi (Pause) [cite: 23] |

---

## 🕹️ Cách chơi

1.  [cite_start]**Đăng nhập:** Khởi động game và đăng ký hoặc đăng nhập tài khoản để hệ thống tải dữ liệu của bạn[cite: 25].
2.  **Bắt đầu:** Nhấn **Play Game**, chọn độ khó (Easy/Normal/Hard). [cite_start]Các khối sẽ bắt đầu rơi theo tốc độ tương ứng[cite: 26].
3.  [cite_start]**Thao tác:** Sử dụng phím điều khiển để di chuyển, xoay và sắp xếp các khối gạch[cite: 27].
4.  [cite_start]**Ghi điểm:** Xếp kín một hàng ngang để phá hủy nó và nhận điểm cộng[cite: 28, 29].
5.  [cite_start]**Kết thúc:** Game Over khi các khối gạch chạm đến đỉnh màn hình[cite: 30].
6.  [cite_start]**Lưu thành tích:** Điểm số cao nhất (Highscore) sẽ tự động được cập nhật lên Bảng xếp hạng[cite: 31].

---

## 🗃️ Schema Database

[cite_start]Game sử dụng bảng `Players` trên **Supabase** với cấu trúc chi tiết như sau[cite: 40]:

| Tên trường | Kiểu dữ liệu | Mô tả |
| :--- | :--- | :--- |
| `id` | `int8` | [cite_start]Mã định danh độc nhất (Auto-generated) [cite: 41] |
| `username` | `text` | [cite_start]Tên tài khoản hiển thị [cite: 42] |
| `password` | `text` | [cite_start]Mật khẩu người chơi [cite: 43] |
| `email` | `text` | [cite_start]Email đăng ký (Dùng để khôi phục/nhận OTP) [cite: 50] |
| `highscore` | `int8` | [cite_start]Điểm số cao nhất đạt được [cite: 49] |
| `game_save_data`| `jsonb` | [cite_start]Dữ liệu game đã lưu (Level, trạng thái bàn cờ...) [cite: 48] |
| `music_enabled` | `bool` | [cite_start]Trạng thái Bật/Tắt nhạc nền [cite: 44] |
| `music_vol` | `float8` | [cite_start]Mức âm lượng nhạc [cite: 45] |
| `sfx_vol` | `float8` | [cite_start]Mức âm lượng hiệu ứng âm thanh [cite: 46] |
| `selected_track`| `text` | [cite_start]Bài nhạc nền đang được chọn [cite: 47] |
| `otp_code` | `text` | [cite_start]Mã OTP (Tự động xóa khi nhập đúng) [cite: 51] |
| `otp_expiry` | `timestampz`| [cite_start]Thời gian hết hạn của mã OTP (5 phút) [cite: 52] |

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
