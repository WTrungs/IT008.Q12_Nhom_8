# 🚀 TETRIS SPACE

---

## 🎮 Giới thiệu

Tetris Space là đồ án game xếp gạch cổ điển được phát triển bằng **C# và WPF (Windows Presentation Foundation)** được giới thiệu trong nội dung môn học **Lập trình Trực quan**.

Đồ án tập trung vào trải nghiệm mượt mà, giao diện phong cách Vũ trụ (**Space Theme**) và tích hợp cơ sở dữ liệu (**Database**) để lưu dữ liệu và thông tin của player.

---

## ✨ Tính năng chính

### 🎮 Cơ chế & Gameplay
* **Cơ chế xếp gạch:** Chuẩn 7 loại khối Tetromino.
* **Hệ thống va chạm:** Phát hiện va chạm chính xác dựa trên ma trận **4×4**, đảm bảo xoay và đặt khối luôn hợp lệ.
* **Next Queue:** Hiển thị trước khối gạch tiếp theo, giúp người chơi lên kế hoạch sắp xếp hiệu quả hơn.
* **Hold Mechanism (Giữ khối):** Cho phép lưu (hold) khối gạch hiện tại để sử dụng sau, tăng tính chiến thuật.
* **Ghost Piece:** Hiển thị bóng của khối gạch tại vị trí rơi dự kiến.
* **Pause Game:** Hệ thống tạm dừng trong quá trình chơi.

### ⚙️ Nhịp độ & Độ khó
* **3 Chế độ chơi:** Easy – Normal – Hard.
* **Tốc độ:** Tốc độ rơi tăng theo cấp số nhân dựa trên Level và chế độ chơi, tạo ra độ khó tăng dần theo thời gian.
* **Hệ thống điểm:**
    * Tính khoảng thời gian giữa 2 lần rơi (Tăng dần theo Level).
    * Hàm tính điểm riêng theo Độ Khó.

### ☁️ Hệ thống Tài khoản & Database
* **Tài khoản:** Đăng ký, Đăng nhập, Quên mật khẩu (gửi mã OTP qua Email).
* **Lưu trữ đám mây (Supabase):** Tự động lưu và tải trạng thái game (điểm số, level, vị trí gạch).
* **Global Ranking:** Bảng xếp hạng online, hiển thị Top người chơi và vị trí của bản thân.
* **Settings:** Tùy chỉnh âm lượng (Music/SFX), chọn nhạc nền. Cài đặt được lưu theo tài khoản (thoát ra vào lại vẫn giữ nguyên).
* **Bảo mật:** Hệ thống OTP tự động xóa sau khi sử dụng hoặc hết hạn (5 phút).

### 🎨 Giao diện & Hiệu ứng (UI/UX)
* **Space Theme:** Giao diện hơi hướng vũ trụ với hiệu ứng Neon Glow và nền tinh vân (Nebula).
* **Animations:** Hiệu ứng rung khi đặt gạch, hiệu ứng xóa hàng, gạch ảo.
* **Điều khiển:** Trải nghiệm điều hướng bàn phím tối ưu, không lỗi focus.

---

## 🎹 Hướng dẫn điều khiển

| Phím | Chức năng |
| :--- | :--- |
| **⬅️ / ➡️** | Di chuyển khối sang Trái / Phải |
| **⬇️** | Rơi nhanh (Soft Drop) |
| **⬆️** hoặc **X** | Xoay khối theo chiều kim đồng hồ |
| **Z** | Xoay khối ngược chiều kim đồng hồ |
| **C** / **Shift** | Giữ khối hiện tại (Hold) |
| **Space** | Thả khối xuống ngay lập tức (Hard Drop) |
| **Esc** | Tạm dừng game |

---

## 🕹️ Cách chơi

1.  Khởi động game và đăng ký / đăng nhập tài khoản.
2.  Nhấn **Play Game**, chọn độ khó. Các khối Tetris sẽ rơi từ trên xuống (căn cứ theo độ khó đã chọn).
3.  Sử dụng phím điều khiển để di chuyển và xoay khối.
4.  Xếp các khối để tạo thành hàng ngang hoàn chỉnh.
5.  Hàng hoàn chỉnh sẽ biến mất và bạn được cộng điểm.
6.  Game kết thúc khi các khối chạm đến đỉnh màn hình.
7.  Điểm cao nhất của bạn sẽ được lưu tự động.

---

## 🗃️ Schema Database

Game sử dụng bảng `Players` trên **Supabase** với cấu trúc chi tiết như sau:

| Tên trường | Kiểu dữ liệu | Mô tả |
| :--- | :--- | :--- |
| `id` | `int8` | Mã độc nhất cho mỗi tài khoản (tự động tạo) |
| `username` | `text` | Tên tài khoản |
| `password` | `text` | Mật khẩu |
| `email` | `text` | Email đăng ký (Dùng để khôi phục/nhận OTP) |
| `highscore` | `int8` | Điểm số cao nhất |
| `game_save_data`| `jsonb` | Dữ liệu game đã lưu (Level, trạng thái...) |
| `music_enabled` | `bool` | Tình trạng bật/tắt nhạc |
| `music_vol` | `float8` | Âm lượng nhạc |
| `sfx_vol` | `float8` | Âm lượng hiệu ứng |
| `selected_track`| `text` | Bài nhạc đang chọn |
| `otp_code` | `text` | Mã OTP (Tự động xóa khi nhập đúng) |
| `otp_expiry` | `timestampz`| Thời gian hết hạn mã OTP (5p) |

---

## 🛠️ Công nghệ sử dụng

* **Ngôn ngữ:** C# (.NET 6.0 / 8.0)
* **Framework:** WPF (Windows Presentation Foundation)
* **Backend / Database:** Supabase (PostgreSQL, Auth, Edge Functions)
* **Thư viện:**
    * `Newtonsoft.Json` (Serialization)
    * `Supabase-csharp` (Client SDK)

---
🚀 **Tetris Space** – Đồ án Lập trình Trực quan.
