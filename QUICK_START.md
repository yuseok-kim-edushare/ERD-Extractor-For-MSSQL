# Quick Start Guide - ERD Extractor for MS SQL Server

## Running the Application

### Option 1: Run with .NET CLI
```bash
cd ERDExtractor
dotnet run
```
The application will be available at `https://localhost:5001` or `http://localhost:5000`

### Option 2: Run with Visual Studio
1. Open `ERDExtractor.sln` in Visual Studio
2. Press F5 or click the Run button
3. The application will launch in your browser

## Using the Application

### Method 1: Demo Mode (No SQL Server Required)
1. Click the **"Try Demo (Sample E-commerce DB)"** button
2. Click **"Generate ERD"** to create the diagram
3. Click **"Download ERD (XML)"** to save the file
4. Open the file in [diagrams.net](https://app.diagrams.net)

### Method 2: Connect to Your SQL Server
1. Enter your database connection details:
   - **Host**: Your SQL Server hostname (e.g., `localhost`, `192.168.1.100`)
   - **Port**: SQL Server port (default: `1433`)
   - **Username**: SQL Server username (e.g., `sa`)
   - **Password**: Your SQL Server password
   - **Database Name**: The database to extract schema from

2. Click **"Connect & Extract Schema"**
3. Click **"Generate ERD"** 
4. Click **"Download ERD (XML)"**
5. Open in diagrams.net for editing

## Editing ERDs in Diagrams.net

1. Go to https://app.diagrams.net/
2. Click **File** → **Open from** → **Device**
3. Select your downloaded `.drawio` file
4. Edit the diagram:
   - Move tables by dragging
   - Resize tables
   - Change colors and styles
   - Add notes and annotations
   - Rearrange relationships
5. Export as PNG, SVG, PDF, or other formats

## SQL Server Connection Requirements

### For Windows Authentication
Not currently supported. Use SQL Server Authentication.

### For SQL Server Authentication
1. Ensure SQL Server is configured for mixed mode authentication
2. Enable TCP/IP protocol in SQL Server Configuration Manager
3. Restart SQL Server service
4. Create a SQL Server login with appropriate permissions

### Required Permissions
The user needs at least the following permissions:
- `VIEW DEFINITION` on the database
- `SELECT` permission on `INFORMATION_SCHEMA` views
- Access to system catalog views (`sys.*`)

### Example: Create a Read-Only User
```sql
-- Create login
CREATE LOGIN erd_reader WITH PASSWORD = 'YourStrongPassword';

-- Create user in your database
USE YourDatabase;
CREATE USER erd_reader FOR LOGIN erd_reader;

-- Grant permissions
GRANT VIEW DEFINITION TO erd_reader;
GRANT SELECT ON SCHEMA::dbo TO erd_reader;
```

## Troubleshooting

### Cannot Connect to SQL Server
**Check:**
- SQL Server is running
- Firewall allows connections on port 1433
- SQL Server is configured for TCP/IP connections
- Credentials are correct
- SQL Server Authentication is enabled

**Solution:**
```sql
-- Enable SQL Server Authentication (requires restart)
-- Run in SSMS as admin
EXEC xp_instance_regwrite 
    N'HKEY_LOCAL_MACHINE', 
    N'Software\Microsoft\MSSQLServer\MSSQLServer',
    N'LoginMode', 
    REG_DWORD, 
    2;
```

### No Tables Found
**Check:**
- Database name is correct
- User has permissions to view schema
- Database actually contains tables

### Download Not Working
**Check:**
- Browser allows downloads
- Pop-up blocker is not blocking the download
- Try a different browser

### ERD File Won't Open in Diagrams.net
**Solution:**
- Ensure file has `.drawio` extension
- Try renaming to `.xml` and opening
- Check file is not corrupted (should be XML format)

## Advanced Usage

### Custom Connection Strings
The application uses this connection string format:
```
Server={Host},{Port};Database={DatabaseName};User Id={Username};Password={Password};TrustServerCertificate=True;
```

### For Production
Change `TrustServerCertificate=True` to use proper SSL certificates:
```csharp
// In DatabaseConnection.cs
public string ConnectionString => 
    $"Server={Host},{Port};Database={DatabaseName};User Id={Username};Password={Password};Encrypt=True;";
```

## Sample Databases to Try

### 1. AdventureWorks
Microsoft's sample database with complex schema
- Download: https://github.com/Microsoft/sql-server-samples/releases/tag/adventureworks

### 2. Northwind
Classic sample database
- Download: https://github.com/microsoft/sql-server-samples/tree/master/samples/databases/northwind-pubs

### 3. Demo Mode
The built-in demo shows an e-commerce database with:
- Users, Orders, OrderItems, Products, Categories tables
- Primary keys and foreign key relationships
- Various data types

## Support

For issues or questions:
- Check the [README.md](README.md)
- Open an issue on GitHub
- Review the [LICENSE](LICENSE) file

## Tips

1. **Large Databases**: For databases with 50+ tables, the ERD may be crowded. Consider:
   - Filtering specific schemas
   - Extracting subsets of tables
   - Manual rearrangement in diagrams.net

2. **Performance**: Schema extraction is fast for most databases (< 5 seconds for 100 tables)

3. **Security**: Never commit connection strings with passwords to version control

4. **Customization**: The ERD layout can be customized by modifying `ErdService.cs`
