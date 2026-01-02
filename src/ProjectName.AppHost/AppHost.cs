var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.ProjectName_Web_Api>("webapi")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.ProjectName_Web>("webapp")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();