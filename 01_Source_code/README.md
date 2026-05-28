###### \# Smart Time Manager

###### 

###### **## 1. Giới thiệu dự án**

###### 

###### Smart Time Manager là ứng dụng quản lý thời gian được xây dựng bằng C# WinForms và SQLite. Ứng dụng hỗ trợ người dùng quản lý công việc, nhắc nhở theo thời gian, theo dõi mục tiêu cá nhân và thống kê tiến độ làm việc thông qua giao diện dashboard trực quan.

###### 

###### **## 2. Chức năng chính**

###### 

###### \* Thêm, hiển thị và quản lý danh sách công việc.

###### \* Phân loại trạng thái công việc:

###### 

###### &#x20; \* Pending

###### &#x20; \* In Progress

###### &#x20; \* Overdue

###### &#x20; \* Completed

###### \* Nhắc nhở công việc khi tới thời gian đã cài đặt.

###### \* Phát âm thanh chuông báo khi reminder tới hạn.

###### \* Hiển thị cửa sổ thông báo và cho phép tắt thông báo.

###### \* Quản lý mục tiêu dài hạn bằng Goals.

###### \* Theo dõi tiến độ mục tiêu bằng progress bar.

###### \* Dashboard thống kê tổng số task, task đã hoàn thành, task đang thực hiện và task quá hạn.

###### \* Hiển thị danh sách Upcoming Tasks và Completed Tasks.

###### \* Thống kê công việc theo category.

###### \* Cập nhật thông tin người dùng.

###### \* Reset dữ liệu khi cần làm mới hệ thống.

###### 

###### **## 3. Công nghệ sử dụng**

###### 

###### \* Ngôn ngữ lập trình: C#

###### \* Giao diện: Windows Forms

###### \* Framework: .NET Framework 4.7.2

###### \* Cơ sở dữ liệu: SQLite

###### \* IDE: Visual Studio

###### \* Quản lý mã nguồn: GitHub

###### 

###### **## 4. Cấu trúc thư mục**

###### 

###### ```text

###### SmartTimeManager/

###### ├── SmartTimeManager.slnx

###### ├── SmartTimeManager/

###### │   ├── Forms/

###### │   ├── Models/

###### │   ├── Services/

###### │   ├── UI/

###### │   ├── Assets/

###### │   ├── Properties/

###### │   └── SmartTimeManager.csproj

###### ├── README.md

###### └── .gitignore

###### ```

###### 

###### **## 5. Phân công thành viên**

###### 

###### | STT | Thành viên       | Vai trò                       | Chức năng phụ trách                                                               |

###### | --- | ---------------- | ----------------------------- | --------------------------------------------------------------------------------- |

###### | 1   | Lê Minh Anh      |  Database, GitHub             | Quản lý repository, SQLite database, TaskModel, DatabaseService, tích hợp project |

###### | 2   | Nguyễn Duy Duy   | Dashboard, Statistics         | Thiết kế Dashboard, biểu đồ thống kê, Upcoming Tasks, Completed Tasks, Statistics |

###### | 3   | Hoàng Mạnh Dũng  | Reminder, Notification        | Reminder, lọc task theo thời gian, chuông báo, cửa sổ thông báo, trạng thái task  |

###### | 4   | Nguyễn Hạ Giang  | Goals, UI, Report             | Goals, giao diện tổng thể, icon category, sidebar, báo cáo và hình ảnh demo       |

###### 

###### **## 6. Mô tả logic xử lý**

###### 

###### \### 6.1. Trạng thái công việc

###### 

###### Ứng dụng phân loại task theo các trạng thái:

###### 

###### ```text

###### Pending:

###### \- Task mới được tạo.

###### \- Chưa bắt đầu thực hiện.

###### 

###### In Progress:

###### \- Task đã được người dùng chọn Start task.

###### 

###### Overdue:

###### \- Task chưa hoàn thành và đã quá thời gian DueDate/ReminderTime.

###### 

###### Completed:

###### \- Task đã được đánh dấu hoàn thành.

###### ```

###### 

###### \### 6.2. Reminder

###### 

###### Khi task tới thời gian nhắc nhở, ứng dụng sẽ:

###### 

###### ```text

###### 1\. Kiểm tra các task chưa hoàn thành.

###### 2\. So sánh DueDate và ReminderTime với thời gian hiện tại.

###### 3\. Nếu task vừa tới hạn, phát chuông thông báo.

###### 4\. Hiển thị cửa sổ notification.

###### 5\. Người dùng có thể tắt thông báo hoặc đánh dấu task đã hoàn thành.

###### ```

###### 

###### \### 6.3. Dashboard

###### 

###### Dashboard thống kê dữ liệu theo công thức:

###### 

###### ```text

###### Total Tasks = tổng số task

###### Completed = số task đã hoàn thành

###### In Progress = số task đang thực hiện

###### Overdue = số task quá hạn

###### Pending = số task chưa bắt đầu

###### ```

###### 

###### **## 7. Hướng dẫn chạy project**

###### 

###### \### Bước 1: Clone repository

###### 

###### ```bash

###### git clone https://github.com/CuAnhProvip/SmartTimeManager.git

###### ```

###### 

###### \### Bước 2: Mở project

###### 

###### Mở file:

###### 

###### ```text

###### SmartTimeManager.slnx

###### ```

###### 

###### bằng Visual Studio.

###### 

###### \### Bước 3: Restore package nếu cần

###### 

###### Nếu Visual Studio yêu cầu, chọn Restore NuGet Packages.

###### 

###### \### Bước 4: Build project

###### 

###### Trong Visual Studio chọn:

###### 

###### ```text

###### Build → Clean Solution

###### Build → Rebuild Solution

###### ```

###### 

###### \### Bước 5: Chạy chương trình

###### 

###### Bấm:

###### 

###### ```text

###### F5

###### ```

###### 

###### hoặc chọn:

###### 

###### ```text

###### Debug → Start Debugging

###### ```

###### 

###### **## 8. Lưu ý khi chạy SQLite**

###### 

###### Project sử dụng SQLite nên cần có file:

###### 

###### ```text

###### SQLite.Interop.dll

###### ```

###### 

###### Nếu chạy bị lỗi thiếu SQLite, cần đảm bảo file `SQLite.Interop.dll` đã được copy vào thư mục output như:

###### 

###### ```text

###### bin/Debug

###### ```

###### 

###### hoặc:

###### 

###### ```text

###### bin/Release

###### ```

###### 

###### **## 9. Kết quả đạt được**

###### 

###### Sau khi hoàn thành, ứng dụng có thể:

###### 

###### \* Quản lý công việc cá nhân.

###### \* Theo dõi tiến độ công việc.

###### \* Nhắc nhở người dùng khi task tới hạn.

###### \* Thống kê trạng thái công việc bằng giao diện trực quan.

###### \* Quản lý mục tiêu cá nhân.

###### \* Hỗ trợ reset dữ liệu và cập nhật thông tin người dùng.

###### 

###### **## 10. Kết luận**

###### 

###### Smart Time Manager là ứng dụng hỗ trợ quản lý thời gian đơn giản, trực quan và dễ sử dụng. Dự án giúp nhóm thực hành các kiến thức về lập trình C# WinForms, xử lý giao diện, làm việc với SQLite, quản lý mã nguồn bằng GitHub và phân chia công việc nhóm trong quá trình phát triển phần mềm.

###### 

