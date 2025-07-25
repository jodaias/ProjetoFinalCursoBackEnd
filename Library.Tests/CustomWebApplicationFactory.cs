using System.Linq;
using Library.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Library.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o contexto registrado anteriormente
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LibraryContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Cria um arquivo SQLite temporário para cada execução de teste
            var dbPath = Path.Combine(Path.GetTempPath(), $"Library_test_{Guid.NewGuid()}.db");
            var connectionString = $"Data Source={dbPath}";

            services.AddDbContext<LibraryContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            // Garante que o banco é criado
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
            db.Database.EnsureCreated();
        });
    }
}
