namespace ERDExtractor.Models;

public class TableSchema
{
    public string TableName { get; set; } = "";
    public string Schema { get; set; } = "dbo";
    public List<ColumnSchema> Columns { get; set; } = new();
    public List<IndexSchema> Indexes { get; set; } = new();
    public List<ForeignKeySchema> ForeignKeys { get; set; } = new();
}

public class ColumnSchema
{
    public string ColumnName { get; set; } = "";
    public string DataType { get; set; } = "";
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public string? DefaultValue { get; set; }
    public int? MaxLength { get; set; }
    public string? Description { get; set; }
}

public class IndexSchema
{
    public string IndexName { get; set; } = "";
    public bool IsUnique { get; set; }
    public bool IsPrimaryKey { get; set; }
    public List<string> Columns { get; set; } = new();
}

public class ForeignKeySchema
{
    public string ForeignKeyName { get; set; } = "";
    public string ColumnName { get; set; } = "";
    public string ReferencedTable { get; set; } = "";
    public string ReferencedSchema { get; set; } = "dbo";
    public string ReferencedColumn { get; set; } = "";
}

public class DatabaseSchema
{
    public List<TableSchema> Tables { get; set; } = new();
}
