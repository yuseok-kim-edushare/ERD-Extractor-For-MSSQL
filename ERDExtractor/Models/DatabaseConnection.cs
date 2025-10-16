namespace ERDExtractor.Models;

public class DatabaseConnection
{
    public string Host { get; set; } = "localhost";
    public string Port { get; set; } = "1433";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string DatabaseName { get; set; } = "";
    
    public string ConnectionString => 
        $"Server={Host},{Port};Database={DatabaseName};User Id={Username};Password={Password};TrustServerCertificate=True;";
}
