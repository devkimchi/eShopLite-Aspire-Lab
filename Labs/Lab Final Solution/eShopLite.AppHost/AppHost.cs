var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

var productsdb = builder.AddPostgres("pg")
                        .WithPgAdmin()
                        .AddDatabase("productsdb");

var products = builder.AddProject<Projects.Products>("products")
                      .WithReference(productsdb)
                      .WaitFor(productsdb);

builder.AddProject<Projects.Store>("store")
       .WithExternalHttpEndpoints()
       .WithReference(products)
       .WithReference(redis)
       .WaitFor(products)
       .WaitFor(redis);
    
builder.Build().Run();
