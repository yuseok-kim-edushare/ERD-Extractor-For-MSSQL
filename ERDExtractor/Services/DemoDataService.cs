using ERDExtractor.Models;

namespace ERDExtractor.Services;

public class DemoDataService
{
    public static DatabaseSchema GetDemoSchema()
    {
        var schema = new DatabaseSchema();

        // Users table
        var usersTable = new TableSchema
        {
            TableName = "Users",
            Schema = "dbo",
            Columns = new List<ColumnSchema>
            {
                new() { ColumnName = "UserId", DataType = "int", IsNullable = false, IsPrimaryKey = true, Description = "Unique identifier for user" },
                new() { ColumnName = "Username", DataType = "nvarchar(50)", IsNullable = false, Description = "User login name" },
                new() { ColumnName = "Email", DataType = "nvarchar(100)", IsNullable = false },
                new() { ColumnName = "CreatedDate", DataType = "datetime", IsNullable = false, DefaultValue = "getdate()" }
            },
            Indexes = new List<IndexSchema>
            {
                new() { IndexName = "PK_Users", IsPrimaryKey = true, IsUnique = true, Columns = new List<string> { "UserId" } },
                new() { IndexName = "IX_Users_Email", IsPrimaryKey = false, IsUnique = true, Columns = new List<string> { "Email" } }
            }
        };

        // Orders table
        var ordersTable = new TableSchema
        {
            TableName = "Orders",
            Schema = "dbo",
            Columns = new List<ColumnSchema>
            {
                new() { ColumnName = "OrderId", DataType = "int", IsNullable = false, IsPrimaryKey = true },
                new() { ColumnName = "UserId", DataType = "int", IsNullable = false, Description = "Foreign key to Users table" },
                new() { ColumnName = "OrderDate", DataType = "datetime", IsNullable = false, DefaultValue = "getdate()" },
                new() { ColumnName = "TotalAmount", DataType = "decimal(18,2)", IsNullable = false },
                new() { ColumnName = "Status", DataType = "nvarchar(20)", IsNullable = false }
            },
            Indexes = new List<IndexSchema>
            {
                new() { IndexName = "PK_Orders", IsPrimaryKey = true, IsUnique = true, Columns = new List<string> { "OrderId" } },
                new() { IndexName = "IX_Orders_UserId", IsPrimaryKey = false, IsUnique = false, Columns = new List<string> { "UserId" } }
            },
            ForeignKeys = new List<ForeignKeySchema>
            {
                new() { ForeignKeyName = "FK_Orders_Users", ColumnName = "UserId", ReferencedTable = "Users", ReferencedSchema = "dbo", ReferencedColumn = "UserId" }
            }
        };

        // OrderItems table
        var orderItemsTable = new TableSchema
        {
            TableName = "OrderItems",
            Schema = "dbo",
            Columns = new List<ColumnSchema>
            {
                new() { ColumnName = "OrderItemId", DataType = "int", IsNullable = false, IsPrimaryKey = true },
                new() { ColumnName = "OrderId", DataType = "int", IsNullable = false },
                new() { ColumnName = "ProductId", DataType = "int", IsNullable = false },
                new() { ColumnName = "Quantity", DataType = "int", IsNullable = false },
                new() { ColumnName = "UnitPrice", DataType = "decimal(18,2)", IsNullable = false }
            },
            Indexes = new List<IndexSchema>
            {
                new() { IndexName = "PK_OrderItems", IsPrimaryKey = true, IsUnique = true, Columns = new List<string> { "OrderItemId" } }
            },
            ForeignKeys = new List<ForeignKeySchema>
            {
                new() { ForeignKeyName = "FK_OrderItems_Orders", ColumnName = "OrderId", ReferencedTable = "Orders", ReferencedSchema = "dbo", ReferencedColumn = "OrderId" },
                new() { ForeignKeyName = "FK_OrderItems_Products", ColumnName = "ProductId", ReferencedTable = "Products", ReferencedSchema = "dbo", ReferencedColumn = "ProductId" }
            }
        };

        // Products table
        var productsTable = new TableSchema
        {
            TableName = "Products",
            Schema = "dbo",
            Columns = new List<ColumnSchema>
            {
                new() { ColumnName = "ProductId", DataType = "int", IsNullable = false, IsPrimaryKey = true },
                new() { ColumnName = "ProductName", DataType = "nvarchar(100)", IsNullable = false },
                new() { ColumnName = "CategoryId", DataType = "int", IsNullable = true },
                new() { ColumnName = "Price", DataType = "decimal(18,2)", IsNullable = false },
                new() { ColumnName = "StockQuantity", DataType = "int", IsNullable = false, DefaultValue = "0" }
            },
            Indexes = new List<IndexSchema>
            {
                new() { IndexName = "PK_Products", IsPrimaryKey = true, IsUnique = true, Columns = new List<string> { "ProductId" } }
            },
            ForeignKeys = new List<ForeignKeySchema>
            {
                new() { ForeignKeyName = "FK_Products_Categories", ColumnName = "CategoryId", ReferencedTable = "Categories", ReferencedSchema = "dbo", ReferencedColumn = "CategoryId" }
            }
        };

        // Categories table
        var categoriesTable = new TableSchema
        {
            TableName = "Categories",
            Schema = "dbo",
            Columns = new List<ColumnSchema>
            {
                new() { ColumnName = "CategoryId", DataType = "int", IsNullable = false, IsPrimaryKey = true },
                new() { ColumnName = "CategoryName", DataType = "nvarchar(50)", IsNullable = false },
                new() { ColumnName = "Description", DataType = "nvarchar(255)", IsNullable = true }
            },
            Indexes = new List<IndexSchema>
            {
                new() { IndexName = "PK_Categories", IsPrimaryKey = true, IsUnique = true, Columns = new List<string> { "CategoryId" } }
            }
        };

        schema.Tables.Add(usersTable);
        schema.Tables.Add(ordersTable);
        schema.Tables.Add(orderItemsTable);
        schema.Tables.Add(productsTable);
        schema.Tables.Add(categoriesTable);

        return schema;
    }
}
