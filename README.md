# Messenger Backend API

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-RealTime-blue?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/Microsoft%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

A robust backend service for a real-time messaging application built with **ASP.NET Core**. This project demonstrates the implementation of Clean Architecture principles, utilizing the **CQRS** pattern and **SignalR** for instant communication.

## 🚀 Key Features

* **Real-Time Communication:** Instant messaging delivery using **SignalR** WebSockets.
* **CQRS Architecture:** Implementation of Command Query Responsibility Segregation using the **MediatR** library to decouple command processing from query execution.
* **Secure Authentication:** User registration and login system with cryptographic protection.
* **Private Messaging:** Support for 1-on-1 direct messages between users.
* **Data Persistence:** Persistent storage of user profiles and chat history using **Microsoft SQL Server**.
* **Security:** Applied encryption and cryptography best practices for sensitive data.

## 🛠 Tech Stack

* **Framework:** ASP.NET Core Web API
* **Language:** C#
* **Architecture Pattern:** CQRS (Command Query Responsibility Segregation)
* **Messaging Implementation:** MediatR
* **Real-Time Protocol:** SignalR
* **Database:** Microsoft SQL Server

## 📂 Architecture Overview

The application logic is structured around the CQRS pattern:

* **Commands:** Handle write operations.
* **Queries:** Handle read operations.
* **Handlers:** Isolated logic components that process specific commands or queries via MediatR.

## ⚠️ Project Status

This is an archival project designed to demonstrate backend architecture skills (specifically CQRS and Real-time communication implementation). It is currently provided as-is.

---
