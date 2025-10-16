# ERD-Extractor-For-MSSQL

ERD extractor from existing MS SQL Server - A Blazor Web Application that connects to MS SQL Server databases, extracts schema information, and generates interactive ERD diagrams.

## Features

- **Database Connection**: Securely connect to MS SQL Server databases using host, port, username, password, and database name
- **Schema Extraction**: Automatically extract comprehensive database schema including:
  - Table names and schemas
  - Column names, data types, nullability, and descriptions
  - Primary keys
  - Foreign keys and relationships
  - Indexes
  - Extended properties/comments
- **ERD Generation**: Convert extracted schema to Draw.io (diagrams.net) compatible XML format
- **Interactive Editing**: Download ERD files and edit them interactively in diagrams.net
- **Localhost Support**: Runs completely on localhost for secure database access

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- MS SQL Server database (local or remote)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yuseok-kim-edushare/ERD-Extractor-For-MSSQL.git
cd ERD-Extractor-For-MSSQL
```

### 2. Build the Application

```bash
dotnet build
```

### 3. Run the Application

```bash
cd ERDExtractor
dotnet run
```

The application will start on `https://localhost:5001` (or the port shown in the console).

### 4. Connect to Your Database

1. Open your browser and navigate to the application URL
2. Enter your database connection details:
   - **Host**: Your SQL Server hostname (e.g., `localhost`)
   - **Port**: SQL Server port (default: `1433`)
   - **Username**: SQL Server username (e.g., `sa`)
   - **Password**: Your SQL Server password
   - **Database Name**: The database you want to extract schema from

3. Click **"Connect & Extract Schema"**

### 5. Generate and Download ERD

1. Once connected, you'll see the number of tables found
2. Click **"Generate ERD"** to create the diagram
3. Click **"Download ERD (XML)"** to download the `.drawio` file

### 6. Edit ERD in Diagrams.net

1. Go to [diagrams.net](https://app.diagrams.net/)
2. Click **File** → **Open from** → **Device**
3. Select the downloaded `.drawio` file
4. Edit and customize your ERD diagram interactively
5. Export in various formats (PNG, SVG, PDF, etc.)

## Project Structure

```
ERD-Extractor-For-MSSQL/
├── ERDExtractor/               # Server-side Blazor project
│   ├── Components/
│   │   ├── Pages/
│   │   │   └── Home.razor     # Main ERD extractor page
│   │   └── Layout/
│   ├── Models/                # Data models
│   │   ├── DatabaseConnection.cs
│   │   └── DatabaseSchema.cs
│   ├── Services/              # Business logic
│   │   ├── DatabaseService.cs # SQL Server connection & schema extraction
│   │   └── ErdService.cs      # ERD XML generation
│   └── wwwroot/
│       └── js/
│           └── fileDownload.js # File download utility
└── ERDExtractor.Client/       # Client-side WebAssembly project

```

## Security Considerations

- **TrustServerCertificate**: The application uses `TrustServerCertificate=True` for development. For production, configure proper SSL certificates.
- **Password Storage**: Passwords are not stored; they're only used for the active session.
- **Local Execution**: The application runs on localhost by default for secure database access.

## Technologies Used

- **ASP.NET Core 9.0** - Web framework
- **Blazor Web App** - UI framework with Server and WebAssembly rendering
- **Microsoft.Data.SqlClient** - SQL Server connectivity
- **Bootstrap 5** - UI styling
- **Draw.io XML Format** - ERD diagram format

## Troubleshooting

### Connection Issues

- Verify SQL Server is running and accessible
- Check firewall settings allow connection on the specified port
- Ensure SQL Server authentication is enabled (for username/password login)
- Verify the username and password are correct

### No Tables Found

- Ensure the database name is correct
- Verify the user has permissions to read schema information
- Check that the database contains tables

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
