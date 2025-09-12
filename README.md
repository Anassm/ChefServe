# ChefServe
Serving files like dishes! Cloud storage application for Web App Development project @ Rotterdam University of Applied Sciences

## 🍽️ Overview

ChefServe is a full-stack cloud storage application that allows users to upload, organize, and manage their files in a Google Drive-like interface. The application features a robust backend API built with ASP.NET Core and a modern React frontend with TypeScript and Next.js.

## 🏗️ Architecture

### Backend
- **ASP.NET Core 8.0** - Web API
- **Entity Framework Core** - ORM with SQLite database
- **ASP.NET Core Identity** - User authentication and authorization
- **JWT Authentication** - Secure token-based authentication
- **Clean Architecture** - Separation of concerns with Core, Infrastructure, and API layers

### Frontend
- **Next.js 15** - React framework with App Router
- **TypeScript** - Type-safe development
- **Tailwind CSS** - Utility-first styling
- **TanStack Query** - Server state management
- **Axios** - HTTP client for API communication
- **Lucide React** - Modern icons

## 🚀 Features

### Authentication
- User registration and login
- JWT token-based authentication
- Secure password requirements
- User profile management

### File Management
- **Upload files** - Drag & drop or click to upload
- **Create folders** - Organize files in hierarchical structure
- **Download files** - Download individual files
- **Rename files/folders** - In-place editing
- **Delete files/folders** - With confirmation
- **Move files/folders** - Drag & drop organization
- **File explorer** - Windows Explorer-like interface

### User Interface
- Responsive design for desktop and mobile
- Real-time file operations
- Loading states and error handling
- Intuitive file browser with grid view
- Breadcrumb navigation
- Multi-select operations

## 🛠️ Setup Instructions

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and npm
- Git

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/Anassm/ChefServe.git
   cd ChefServe
   ```

2. **Restore .NET packages**
   ```bash
   cd Backend
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the API**
   ```bash
   cd ChefServe.API
   dotnet run
   ```

   The API will be available at `https://localhost:7144`

### Frontend Setup

1. **Install dependencies**
   ```bash
   cd frontend
   npm install
   ```

2. **Configure environment**
   ```bash
   cp .env.local.example .env.local
   # Edit .env.local to set NEXT_PUBLIC_API_URL if needed
   ```

3. **Run the development server**
   ```bash
   npm run dev
   ```

   The frontend will be available at `http://localhost:3000`

## 📁 Project Structure

```
ChefServe/
├── Backend/
│   ├── ChefServe.API/           # Web API controllers and configuration
│   ├── ChefServe.Core/          # Domain entities, DTOs, and interfaces
│   └── ChefServe.Infrastructure/ # Data access and external services
├── frontend/                    # Next.js React application
│   ├── src/
│   │   ├── app/                # App Router pages
│   │   ├── components/         # React components
│   │   ├── contexts/           # React contexts
│   │   └── lib/                # Utilities and API client
│   └── public/                 # Static assets
└── ChefServe.sln               # .NET solution file
```

## 🔧 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

### Files
- `GET /api/files` - Get files in folder
- `GET /api/files/{id}` - Get file details
- `POST /api/files/upload` - Upload file
- `POST /api/files/create-folder` - Create folder
- `GET /api/files/{id}/download` - Download file
- `PUT /api/files/move` - Move file/folder
- `PUT /api/files/rename` - Rename file/folder
- `DELETE /api/files/{id}` - Delete file/folder

## 🔒 Security Features

- JWT token authentication
- Password requirements (minimum 6 characters, uppercase, lowercase, digit)
- User-specific file access (users can only access their own files)
- CORS configuration for frontend integration
- Secure file upload with validation

## 📊 Database Schema

### Users (ASP.NET Core Identity)
- Extended ApplicationUser with FirstName, LastName, CreatedAt

### FileItems
- Hierarchical structure supporting files and folders
- User ownership through foreign key
- File metadata (name, size, content type, timestamps)
- Self-referencing for folder structure

## 🎨 UI Components

- **LoginForm** - User authentication
- **RegisterForm** - User registration  
- **FileExplorer** - Main file management interface
- **Header** - Navigation and user actions
- **AuthContext** - Authentication state management

## 🧪 Development

### Building
```bash
# Backend
cd Backend && dotnet build

# Frontend
cd frontend && npm run build
```

### Running Tests
```bash
# Backend (if tests are added)
cd Backend && dotnet test

# Frontend (if tests are added)
cd frontend && npm test
```

## 🚀 Deployment

### Backend
The ASP.NET Core API can be deployed to:
- Azure App Service
- Docker containers
- IIS
- Linux servers with .NET runtime

### Frontend
The Next.js application can be deployed to:
- Vercel (recommended)
- Netlify
- Azure Static Web Apps
- Any hosting service that supports Node.js

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📝 License

This project is developed for educational purposes as part of the Web App Development course at Rotterdam University of Applied Sciences.

## 👥 Authors

- **Anass M** - Initial development

---
*Serving files like dishes - bon appétit! 🍽️*
