var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.TesteVsoft_ApiService>("apiservice");

builder.AddProject<Projects.TesteVsoft_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
