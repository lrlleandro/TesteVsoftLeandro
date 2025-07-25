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

builder.AddContainer("frontend", "node:20-alpine")
    .WithImage("frontend:dev")
    .WithDockerfile("../TesteVsoft.Frontend")
    .WithContainerName("frontend")
    .WithImageTag("dev")
    .WithEnvironment("NODE_ENV", "development")
    .WithExternalHttpEndpoints()
    .WithEndpoint(port: 5173, targetPort: 5173, scheme: "http");

builder.Build().Run();
