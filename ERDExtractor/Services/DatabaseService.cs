using Microsoft.Data.SqlClient;
using ERDExtractor.Models;

namespace ERDExtractor.Services;

public class DatabaseService
{
    public async Task<bool> TestConnectionAsync(DatabaseConnection connection)
    {
        try
        {
            using var sqlConnection = new SqlConnection(connection.ConnectionString);
            await sqlConnection.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<DatabaseSchema> GetDatabaseSchemaAsync(DatabaseConnection connection)
    {
        var schema = new DatabaseSchema();
        
        using var sqlConnection = new SqlConnection(connection.ConnectionString);
        await sqlConnection.OpenAsync();

        // Get all tables
        var tables = await GetTablesAsync(sqlConnection);
        
        foreach (var table in tables)
        {
            var tableSchema = new TableSchema
            {
                TableName = table.TableName,
                Schema = table.Schema
            };

            // Get columns for this table
            tableSchema.Columns = await GetColumnsAsync(sqlConnection, table.Schema, table.TableName);
            
            // Get indexes for this table
            tableSchema.Indexes = await GetIndexesAsync(sqlConnection, table.Schema, table.TableName);
            
            // Get foreign keys for this table
            tableSchema.ForeignKeys = await GetForeignKeysAsync(sqlConnection, table.Schema, table.TableName);

            schema.Tables.Add(tableSchema);
        }

        return schema;
    }

    private async Task<List<(string Schema, string TableName)>> GetTablesAsync(SqlConnection connection)
    {
        var tables = new List<(string Schema, string TableName)>();
        
        var query = @"
            SELECT 
                TABLE_SCHEMA,
                TABLE_NAME
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE'
            ORDER BY TABLE_SCHEMA, TABLE_NAME";

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            tables.Add((reader.GetString(0), reader.GetString(1)));
        }

        return tables;
    }

    private async Task<List<ColumnSchema>> GetColumnsAsync(SqlConnection connection, string schema, string tableName)
    {
        var columns = new List<ColumnSchema>();
        
        var query = @"
            SELECT 
                c.COLUMN_NAME,
                c.DATA_TYPE,
                c.IS_NULLABLE,
                c.CHARACTER_MAXIMUM_LENGTH,
                c.COLUMN_DEFAULT,
                CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IS_PRIMARY_KEY,
                ep.value AS DESCRIPTION
            FROM INFORMATION_SCHEMA.COLUMNS c
            LEFT JOIN (
                SELECT ku.TABLE_SCHEMA, ku.TABLE_NAME, ku.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                    ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY' 
                    AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                    AND tc.TABLE_SCHEMA = ku.TABLE_SCHEMA
                    AND tc.TABLE_NAME = ku.TABLE_NAME
            ) pk ON c.TABLE_SCHEMA = pk.TABLE_SCHEMA 
                AND c.TABLE_NAME = pk.TABLE_NAME 
                AND c.COLUMN_NAME = pk.COLUMN_NAME
            LEFT JOIN sys.extended_properties ep
                ON ep.major_id = OBJECT_ID(@schema + '.' + @tableName)
                AND ep.minor_id = COLUMNPROPERTY(OBJECT_ID(@schema + '.' + @tableName), c.COLUMN_NAME, 'ColumnId')
                AND ep.name = 'MS_Description'
            WHERE c.TABLE_SCHEMA = @schema 
                AND c.TABLE_NAME = @tableName
            ORDER BY c.ORDINAL_POSITION";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@schema", schema);
        command.Parameters.AddWithValue("@tableName", tableName);
        
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            columns.Add(new ColumnSchema
            {
                ColumnName = reader.GetString(0),
                DataType = reader.GetString(1),
                IsNullable = reader.GetString(2) == "YES",
                MaxLength = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                DefaultValue = reader.IsDBNull(4) ? null : reader.GetString(4),
                IsPrimaryKey = reader.GetInt32(5) == 1,
                Description = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }

        return columns;
    }

    private async Task<List<IndexSchema>> GetIndexesAsync(SqlConnection connection, string schema, string tableName)
    {
        var indexes = new List<IndexSchema>();
        
        var query = @"
            SELECT 
                i.name AS INDEX_NAME,
                i.is_unique AS IS_UNIQUE,
                i.is_primary_key AS IS_PRIMARY_KEY,
                STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) AS COLUMNS
            FROM sys.indexes i
            INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
            INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            WHERE i.object_id = OBJECT_ID(@schema + '.' + @tableName)
                AND i.name IS NOT NULL
            GROUP BY i.name, i.is_unique, i.is_primary_key
            ORDER BY i.name";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@schema", schema);
        command.Parameters.AddWithValue("@tableName", tableName);
        
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            indexes.Add(new IndexSchema
            {
                IndexName = reader.GetString(0),
                IsUnique = reader.GetBoolean(1),
                IsPrimaryKey = reader.GetBoolean(2),
                Columns = reader.GetString(3).Split(", ").ToList()
            });
        }

        return indexes;
    }

    private async Task<List<ForeignKeySchema>> GetForeignKeysAsync(SqlConnection connection, string schema, string tableName)
    {
        var foreignKeys = new List<ForeignKeySchema>();
        
        var query = @"
            SELECT 
                fk.name AS FK_NAME,
                c.name AS COLUMN_NAME,
                OBJECT_SCHEMA_NAME(fk.referenced_object_id) AS REFERENCED_SCHEMA,
                OBJECT_NAME(fk.referenced_object_id) AS REFERENCED_TABLE,
                rc.name AS REFERENCED_COLUMN
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
            INNER JOIN sys.columns rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
            WHERE fk.parent_object_id = OBJECT_ID(@schema + '.' + @tableName)
            ORDER BY fk.name";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@schema", schema);
        command.Parameters.AddWithValue("@tableName", tableName);
        
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            foreignKeys.Add(new ForeignKeySchema
            {
                ForeignKeyName = reader.GetString(0),
                ColumnName = reader.GetString(1),
                ReferencedSchema = reader.GetString(2),
                ReferencedTable = reader.GetString(3),
                ReferencedColumn = reader.GetString(4)
            });
        }

        return foreignKeys;
    }
}
