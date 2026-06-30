IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

_ = builder.AddProject<Projects.ProjectName_Web_Api>("webapi")
    .WithHttpHealthCheck("/health");

builder.Build().Run();
