# NHOM21 FINANCIAL TERMINAL �

Hệ thống Phân phối dữ liệu tài chính thời gian thực dựa trên kiến trúc TCP Socket.

## 🏢 1. Giới Thiệu
Nhom21 Financial Terminal là một hệ thống mô phỏng sàn giao dịch chứng khoán. Server đóng vai trò là "Sàn giao dịch tập trung" tự động giả lập các chỉ số chứng khoán, và các Client là các "Máy trạm phân tích" kết nối vào để nhận dữ liệu thời gian thực.

## 🛠️ 2. Tính Năng Chính
- **Real-time Data Streaming**: Dữ liệu giá nhảy liên tục từng giây qua TCP Socket.
- **Multi-Client Sync**: Mọi máy trạm đều nhận được dữ liệu đồng bộ từ Server.
- **Visual Change Detection**: Giao diện tự động đổi màu (Xanh: Tăng, Đỏ: Giảm) giúp theo dõi biến động thị trường tức thì.

## 📂 3. Cấu Trúc Dự Án
- **Nhom21.FinancialTerminal.Server**: Giả lập dữ liệu và phát sóng (Broadcast).
- **Nhom21.FinancialTerminal.Client**: Giao diện máy trạm phân tích.

## � 4. Hướng Dẫn Chạy
1. **Chạy Server**: Mở dự án Server và nhấn Start. Server sẽ bắt đầu tạo dữ liệu giả lập.
2. **Chạy Client**: Mở dự án Client, nhập IP/Port của Server và nhấn Connect.

---
*Đồ án thực hiện bởi Nhóm 21 - Lập trình ứng dụng mạng.*

