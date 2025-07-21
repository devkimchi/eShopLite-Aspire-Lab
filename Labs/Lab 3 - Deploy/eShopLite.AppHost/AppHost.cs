var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

var products = builder.AddProject<Projects.Products>("products");

builder.AddProject<Projects.Store>("store")
       .WithReference(products)
       .WithReference(redis)
       .WaitFor(products)
       .WaitFor(redis);
    
builder.Build().Run();
