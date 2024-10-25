# CustomEd

## Overview
**CustomEd** is a modular education management system designed to streamline various operations within educational institutions using microservices. It supports features like announcements, assessments, classroom management, and real-time notifications.

[Next.Js Frontend Codebase](https://github.com/Son-OfAnton/custom-ed)

## Features
- **Announcement Service:** Manage announcements and updates.
- **Assessment Service:** Create and manage assessments.
- **Classroom Service:** Organize classroom activities and schedules.
- **Forum Service:** Enable student discussions.
- **OTP Service:** Secure authentication with one-time passwords.
- **Real-Time Notifications:** Receive instant updates.

## Architecture
The system is built on a **microservices architecture** using **C#**. Each service is independently deployable, promoting scalability and flexibility. Docker is used for containerized deployment to simplify the setup and management.

## Setup and Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/Amanuel94/CustomEd.git
   cd CustomEd
   ```
2. Install the required dependencies for each service if applicable:
   ```
   dotnet restore
   ```
4. Ensure Docker and Docker Compose are installed and running.
5. Build and start the services:
   ```
   docker-compose up --build
   ```
7. Access the services through their configured endpoints.

