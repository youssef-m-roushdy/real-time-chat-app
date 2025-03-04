# **Chat Application**  

## **📌 Description**  
This is a real-time chat application that allows users to exchange messages instantly without needing to reload the page. The application ensures private messaging using authentication, meaning only the sender and receiver can access their conversation.  

### 🔹 **Key Features:**  
✔ **Real-time messaging** using WebSockets (SignalR)  
✔ **User authentication** with JWT  
✔ **Presence notifications** when a user comes online/offline  
✔ **Typing indicator** to
ow when the other user is typing  
✔ **Image upload support** for sending multimedia messages  
✔ **Secure message storage** with a backend database  

---

## **🚀 Tech Stack**  

### **Client (Frontend) - Angular**  
- **Angular** – Component-based frontend framework  
- **RxJS** – Reactive programming for real-time updates  
- **Auth Guard** – Protects private routes  
- **Angular Services** – Manages API requests and authentication  
- **WebSockets (SignalR)** – Establ
es a real-time connection  

### **Server (Backend) - ASP.NET Core**  
- **ASP.NET Core Web API** – Backend service  
- **Entity Framework Core** – ORM for database interactions  
- **JWT (JSON Web Token)** – Authentication & Authorization  
- **SignalR** – Real-time communication between client and server  
- **MediatR** – CQRS pattern for request handling  
- **Cloud Storage (Optional)** – For storing uploaded images  

---

## **📁 Project Structure**  

```
.
├── Client
│   ├── public
│   └── src
│       └── app
│           ├── chat
│           ├── components
│           │   ├── chat-box
│           │   ├── chat-right-sidebar
│           │   ├── chat-sidebar
│           │   ├── chat-window
│           │   └── typing-indicator
│           ├── guards
│           ├── login
│           ├── models
│           ├── register
│           └── services
└── Server
    ├── Common
    ├── Data
    ├── DTOs
    ├── Endpoints
    ├── Extensions
    ├── Hubs
    ├── Migrations
    ├── Models
    ├── Properties
    ├── Services
    └── wwwroot
        └── uploads
```


---

## **🛠️ Setup & Installation**  

### **1️⃣ Clone the repository**  
```
git clone https://github.com/your-username/chat-app.git
cd chat-app
```

### **2️⃣ Backend Setup (ASP.NET Core)**

#### Install Required NuGet Packages
```
dotnet add package Microsoft.AspNetCore.SignalR
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
```

#### Run the Server
```
cd Server
dotnet build
dotnet run
```

### **3️⃣ Frontend Setup (Angular)**

#### Install Angular CLI (if not installed)

```
npm install -g @angular/
```

#### Create Angular Client

```
ng new Client
cd Client
```

#### Install Dependencies

```
npm install @microsoft/signalr
```

#### Start Angular App

```
ng serve
```

## **🖥️ Backend Features**

### 🔹 Authentication & Authorization
* JWT-based authentication for securing API endpoints
* Protects chat messages from unauthorized users
* Implements refresh tokens for maintaining sessions

### 🔹 Real-time Messaging (SignalR)
* Uses SignalR Hubs for instant message delivery
* Notifies users when they receive a message
* Typing Indicator feature for better user experience

### 🔹 User Presence Management
* Detects when users come online or go offline
* Sends notifications to friends when their contacts are available

### 🔹 Image Upload & File Sharing
* Supports uploading images with .jpg, .png, .gif extensions
* Saves files in wwwroot/uploads directory

## **💻 Frontend Features**
### 🔹 Services
* Handles API requests and authentication
* Uses Angular's HttpClient for communication with the backend

### 🔹 Components
* Chat-Box: Displays the chat messages
* Chat-Sidebar: Lists users with online/offline status
* Typing-Indicator: Shows when the other user is typing

### 🔹 Authentication Guards
* Protects private routes (e.g., chat page) from unauthorized users
* Redirects unauthenticated users to the login page

## **📜 API Endpoints**

### 🔹 Authentication Routes
```
Method	Endpoint	Description
POST	/api/auth/register	Register a new user
POST	/api/auth/login	Authenticate and return a JWT
```
### 🔹 Chat Routes
```
Method	Endpoint	Description
GET	/api/chat/messages	Retrieve chat messages
POST	/api/chat/send	Send a new message
```
## **📌 Conclusion**

This chat application provides a real-time communication experience with strong authentication and authorization measures. By using SignalR, it ensures seamless message exchange and presence notifications. With Angular's structured services and authentication guards, the client remains secure and scalable. 🚀