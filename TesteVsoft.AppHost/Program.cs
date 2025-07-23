using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
var databaseName = "teste-vsoft-db";

var messaging = builder.AddRabbitMQ("messaging")
    .WithDataVolume("messaging-volume", false)
    .WithManagementPlugin();

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("postgres-volume", false)
    .WithPgAdmin();

var db = postgres.AddDatabase(databaseName);

var migrationService = builder.AddProject<Projects.TesteVsoft_MigrationService>("migration-service")
    .WithReference(db)
    .WaitFor(db);

var apiService = builder.AddProject<Projects.TesteVsoft_ApiService>("api-service")
    .WithReference(messaging)
    .WithReference(db)
    .WithReference(migrationService)
    .WaitFor(messaging)
    .WaitFor(db)
    .WaitForCompletion(migrationService);

builder.Build().Run();
