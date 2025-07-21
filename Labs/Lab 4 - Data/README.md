# Lab 4 - Add data access and persistence

.NET Aspire makes it very easy to add a database to your applications. Many SQL-compliant database are already available as .NET Aspire components. In this lab, you will add postgreSQL (or mysql). (This will be optional during the in-person portion of the workshop at Build.)

## Adding PostgreSQL database to **eShopLite.AppHost**

1. Get the repository root.

    ```bash
    # bash/zsh
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

1. Navigate to Lab 4.

    ```bash
    cd "$REPOSITORY_ROOT/Labs/Lab 4 - Data"
    ```

1. Add the **Aspire.Hosting.PostgreSQL** package to the **eShopLite.AppHost** project.

    ```bash
    dotnet add ./eShopLite.AppHost package Aspire.Hosting.PostgreSQL
    ```

1. Open the `AppHost.cs` file from the **eShopLite.AppHost** project.
1. Let's create a database and passes it to the product API by updating the code. Here save the resource in the variable `productsdb`, and pass it to the `products` using the `WithReference` method.

    ```csharp
    var redis = builder.AddRedis("redis");

    // 👇👇👇 Add 👇👇👇
    var productsdb = builder.AddPostgres("pg")
                            .AddDatabase("productsdb");
    // 👆👆👆 Add 👆👆👆

    var products = builder.AddProject<Projects.Products>("products")
                          // 👇👇👇 Add 👇👇👇
                          .WithReference(productsdb)
                          .WaitFor(productsdb);
                          // 👆👆👆 Add 👆👆👆
    ```

1. Some of the .NET Aspire database components also allow you to create a container for database management tools. To add **PgAdmin** to your solution to manage the PostgreSQL database, use this code:

    ``` csharp
    var productsdb = builder.AddPostgres("pg")
                            // 👇👇👇 Add 👇👇👇
                            .WithPgAdmin()
                            // 👆👆👆 Add 👆👆👆
                            .AddDatabase("productsdb");
    ```

The advantage of letting .NET Aspire create the container is that you don't need to do any configuration to connect PgAdmin to the PostgreSQL database, it's all automatic.

## Configure Product API to use a PostgreSQL database

1. Make sure you're still in Lab 4.

    ```bash
    cd "$REPOSITORY_ROOT/Labs/Lab 4 - Data"
    ```

1. Add the **Aspire.Hosting.PostgreSQL** package to the **Products** project.

    ```bash
    dotnet add ./Products package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
    ```

1. Open the `appsettings.json` file from the **Products** project and remove the `ConnectionStrings` section completely. It should look like this with it removed:

    ``` json
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "AllowedHosts": "*"
    }
    ```

1. Open the `Program.cs` file from the **Products** project and replace the following lines of codes.

    ```csharp
    // Before
    builder.Services.AddDbContext<ProductDataContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("ProductsContext") ?? throw new InvalidOperationException("Connection string 'ProductsContext' not found.")));

    // After
    builder.AddNpgsqlDbContext<ProductDataContext>("productsdb");
    ```

   It completely replaces the existing SQLite database with PostgreSQL database declared in the **eShopLite.AppHost** project using the same string `productsdb`.

## Explore the .NET Aspire dashboard

Let's test the whole application.

1. Make sure Docker Desktop is up and running
1. Make sure you're still in Lab 4.

    ```bash
    cd "$REPOSITORY_ROOT/Labs/Lab 4 - Data"
    ```

1. Run the .NET Aspire app.

    ```bash
    dotnet watch run --project ./eShopLite.AppHost
    ```

1. When the .NET Aspire dashboard appears, note the you have many more resources.

    ![Dashboard with PostgreSQL](./images/dashboard-with-postgres.png)

   > **NOTE**: You may be asked to enter an authentication token to access to the dashboard.
   >
   > ![.NET Aspire dashboard login](./images/login.png)
   >
   > The token can be found in the terminal console. Copy and paste it to the field and click "Log in".
   >
   > ![.NET Aspire dashboard access token](./images/console-token.png)

1. Click on the **pg-pgadmin** resource, a new tab will open with the pgAdmin website. It can takes a few seconds to completely load.
1. From the pgAdmin website, you manage the products database. To visualize the products table, expand the **Aspire instances** node, then **pg > Databases > productsdb > Schemas > Tables**.

    ![postgresql admin dashboard](./images/postgres-admin-dashboard.png)

1. You can see all the rows by right-clicking on the **products** table and selecting **View/Edit Data > All Rows**.

    ![all the data in the products table viewed from the postgresql admin dashboard](./images/products-data.png)

1. Now going back the the .NET Aspire dashboard, click on **store** the endpoints, a new tab will open with the store website.
1. The store works like before but now uses a PostgreSQL database in a container.

## Deploy / Re-deploy the solution with a PostgreSQL database

1. If you you are in a folder from where the solution was never deployed, `azd init` then `azd up` will deploy the solution without any problem.

   If you are in the same folder where the solution was previously deployed, then just run `azd up` to deploy the updated application including the PostgreSQL database.

1. Wait for the deployment to complete. It should be faster than the first time because most of the resources are already created.
1. Once the deployment is over, click on the link `store` from the `(azd deploy)` outputs, or go to the Azure Portal and navigate to the resource group of `rg-<RANDOM_NAME>` and find the Azure Container Apps named **store**.
1. The store should be working as before, but now it uses a PostgreSQL database.

### Clean up

To delete all the resources created by the deployment, in Lab 3, you can execute the following command:

```powershell
azd down --force --purge
```

---

[<- Lab 3 - Deploy to Azure Container Apps](/Labs/Lab%203%20-%20Deploy/README.md) | [Final Solution ->](../Lab%20Final%20Solution/)
