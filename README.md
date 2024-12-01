# **StayTrack - PG Management Software**  
A comprehensive solution for managing paying guest (PG) accommodations, offering efficient tools for admins and seamless experiences for members.

---

## **Table of Contents**  
1. [About the Project](#about-the-project)  
2. [Features](#features)  
   - [Admin Panel](#admin-panel)  
   - [User Panel (Member Login)](#user-panel-member-login)  
3. [Technologies Used](#technologies-used)  
4. [Getting Started](#getting-started)  
5. [Installation](#installation)  
6. [Usage](#usage)  
7. [Contributing](#contributing)  
8. [License](#license)  

---

## **About the Project**  
This project is designed to simplify the management of paying guest accommodations. It provides robust features for both administrators and members, ensuring smooth operations and improved user experience. The system is built using **ASP.NET Core MVC** and **SQL Server**, offering scalability, reliability, and a modern web interface.

---

## **Features**

### **Admin Panel**
- **Member Management**  
  - Add, update, and remove members.  
  - View detailed member profiles and payment history.  
- **Room Management**  
  - Assign rooms to members.  
  - Monitor room occupancy and availability.  
- **Billing Management**  
  - Generate and track monthly fees.  
  - Integrate current bill data from external providers.  
- **Query Management**  
  - Track and respond to member complaints/requests.  
  - Send automated notifications for query updates.  
- **Reports & Analytics**  
  - View monthly income, pending dues, and occupancy rates.  
- **Role Management**  
  - Create admin roles for owners, managers, and accountants.  

### **User Panel (Member Login)**  
- **Profile Management**  
  - View and update personal details.  
  - Access room details, such as room number and type.  
- **Payment Portal**  
  - Pay hostel fees online (UPI, credit/debit cards, etc.).  
  - View payment history and download receipts.  
- **Utility Bill Access**  
  - View current bill details fetched from external providers.  
  - Use a bill split calculator for sharing costs among roommates.  
- **Query Submission**  
  - Submit and track complaints or requests (e.g., maintenance).  
  - View admin responses and resolution status.  
- **Notifications**  
  - Receive alerts for payment deadlines, announcements, and events.  

---

## **Technologies Used**  
- **Frontend**: Razor Pages, Bootstrap, HTML, CSS, JavaScript  
- **Backend**: ASP.NET Core MVC  
- **Database**: SQL Server  
- **Payment Integration**: Payment Gateway APIs (e.g., Razorpay/PayU)  
- **External Integration**: APIs for utility bill management  

---

## **Getting Started**

### **Prerequisites**  
- .NET Core SDK  
- SQL Server  
- Visual Studio or Visual Studio Code  
- A configured payment gateway account  

---

## **Installation**

1. **Clone the repository**:  
   ```bash
   git clone https://github.com/yourusername/pg-management-software.git
   cd pg-management-software
   ```

2. **Configure the database**:  
   - Update the connection string in `appsettings.json`.  

3. **Restore dependencies**:  
   ```bash
   dotnet restore
   ```

4. **Apply database migrations**:  
   ```bash
   dotnet ef database update
   ```

5. **Run the application**:  
   ```bash
   dotnet run
   ```

---

## **Usage**  

1. **Admin Login**:  
   - Access the admin dashboard to manage members, rooms, billing, and queries.  

2. **Member Login**:  
   - Access personal details, make payments, and submit queries through the user portal.  

---

## **Contributing**  
Contributions are welcome! Please follow these steps:  
1. Fork the repository.  
2. Create a new branch:  
   ```bash
   git checkout -b feature-name
   ```  
3. Commit your changes:  
   ```bash
   git commit -m "Add feature-name"
   ```  
4. Push the branch:  
   ```bash
   git push origin feature-name
   ```  
5. Submit a Pull Request.  

---

## **License**  
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.  

---
## Features
- ⚙️ Member management (add, update, remove members).  
- ⚙️ Room assignment and occupancy tracking.  
- ⏳ Payment portal
- ⏳ Utility bill integration.

- ~ ✅ (completed), ⚙️ (in progress), or ⏳ (planned).


**Note**: Some features mentioned are under development or planned for future releases. Contributions are welcome!
