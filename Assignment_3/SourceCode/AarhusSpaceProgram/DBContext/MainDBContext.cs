using Microsoft.EntityFrameworkCore;

public class MyDBContext : DbContext {
	private const string DbName = "EFGetStarted";
    private const string ConnectionString = $"Data Source=localhost;Initial Catalog={DbName};User ID=sa;Password=Abcd123456!;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Authentication=SqlPassword;Application Intent=ReadWrite;";
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(ConnectionString);

}