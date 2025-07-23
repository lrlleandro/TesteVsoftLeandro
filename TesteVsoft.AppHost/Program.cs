var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.TesteVsoft_ApiService>("apiservice");

builder.Build().Run();
