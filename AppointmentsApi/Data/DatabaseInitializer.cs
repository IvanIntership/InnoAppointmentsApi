using DbUp;

namespace InnoAppointmentsApi.Data;

public static class DatabaseInitializer
{
    public static void Initialize(string writeConnectionString, string readConnectionString)
    {
        ApplyMigrations(writeConnectionString, "Write Database");
        ApplyMigrations(readConnectionString, "Read Database");
    }

    private static void ApplyMigrations(string connectionString, string dbName)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var engine = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DatabaseInitializer).Assembly)
            .LogToConsole()
            .Build();

        var result = engine.PerformUpgrade();

        if (!result.Successful)
        {
            throw new Exception($"Database migration failed for {dbName}", result.Error);
        }
    }
}