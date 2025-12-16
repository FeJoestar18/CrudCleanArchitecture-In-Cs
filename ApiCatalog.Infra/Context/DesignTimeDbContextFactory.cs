// Explanation: Create design-time factory so dotnet-ef can instantiate AppDbContext during migrations.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using System.Text.Json;

namespace ApiCatalog.Infra.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath
            (Path.Combine(Directory.GetCurrentDirectory(), "..", "ApiCatalog.Api"));
        
        var settingsPath = Path.Combine(basePath, "appsettings.json");

        string? connectionString = null;

        if (File.Exists(settingsPath))
        {
            var json = File.ReadAllText(settingsPath);
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("ConnectionStrings", out var csSection) &&
                    csSection.TryGetProperty("DefaultConnection", out var csValue))
                {
                    connectionString = csValue.GetString();
                }
            }
            catch
            {
                // ignore parse errors and fall back to default
            }
        }

        connectionString ??= "Server=localhost;Database=ApiCatalog;User=root;Password=;";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new AppDbContext(optionsBuilder.Options);
    }
}
