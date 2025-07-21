# Lab 3 - Deploy the and provision the entire application to Azure

In this lab, you will deploy the entire application to Azure Container Apps (ACA) using the Azure Developer CLI (`azd`).

`azd` is a command line interface tool that helps developers provision resources in and deploy applications to Azure. It provides best practices and developer-friendly commands that map to key stages in the development lifecycle. It provisions Azure resources via Bicep files and can deploy .NET applications to various PaaS services such as Azure Container Apps, Azure Functions, and Azure App Service.

## Expose the store app to the Internet

Before deploying the application, we'll need to update some code to make sure it the app is exposed to the public Internet.

You'll need to add the `WithExternalHttpEndpoints` method to the store project to do so.

1. Get the repository root.

    ```bash
    # bash/zsh
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

1. Navigate to Lab 3.

    ```bash
    cd "$REPOSITORY_ROOT/Labs/Lab 3 - Deploy"
    ```

1. Open the `AppHost.cs` file from the **eShopLite.AppHost** project.
1. Add `.WithExternalHttpEndpoints()` to the **Projects.Store** resource.

    ```csharp
    builder.AddProject<Projects.Store>("store")
           // 👇👇👇 Add 👇👇👇
           .WithExternalHttpEndpoints()
           // 👆👆👆 Add 👆👆👆
           .WithReference(products)
           .WithReference(redis);
    ```

   This will make sure the store app is exposed to the public Internet. Now we can get ready to create our Azure resources and deploy the application.

## Login to Azure

1. Open a terminal and run the following command to login to Azure:

    ```bash
    azd auth login
    ```

## Initialize the deployment environment

1. Make sure you're still in Lab 3.

    ```bash
    cd "$REPOSITORY_ROOT/Labs/Lab 3 - Deploy"
    ```

1. Run the following command to initialize the deployment environment:

    ```bash
    azd init
    ```

1. The Azure Developer CLI will prompt you with several questions. Answer them as follows:

   - `? How do you want to initialize your app?`
     - `> Scan current directory`
   - `? Select an option`
     - `> Confirm and continue initializing my app`
   - `? Enter a unique environment name`
     - `<RANDOM_NAME>`

   > **Note**:
   >
   > Replace `<RANDOM_NAME>` with your preferred environment name. Use something that will be easy to remember and distinct in your environment. For example, `eshoplite-1234`.

1. Now go back to the directory and confirm the following files have been generated:

   - `.azure/.gitignore`
   - `.azure/config.json`
   - `.azure/<RANDOM_NAME>/.env`
   - `.azure/<RANDOM_NAME>/config.json`
   - `azure.yaml`
   - `next-steps.md`

    If those files and directories exist, you've successfully initialized the deployment environment. Now we're ready to provision and deploy the application into Azure.

## Provision and deploy the application

1. Run the following command to provision and deploy the application to Azure Container Apps:

    ```powershell
    azd up
    ```

1. Again, `azd` will prompt you with several questions. Answer them as follows:

   - `? Select an Azure Subscription to use:`
     - `> <AZURE_SUBSCRIPTION>`
   - `? Select an Azure location to use:`
     - `> <AZURE_LOCATION>`

   > **Note**:
   >
   > If you have only one Azure subscription, it will be automatically chosen.
   >
   > Replace `<AZURE_SUBSCRIPTION>` and `<AZURE_LOCATION>` with your Azure subscription and location.

1. Now `azd` will provision the Azure resources your application need and deploy your app to those resources. All from a single command! Wait for the deployment to complete. It may take a few minutes.
1. Once the deployment is over, go to the Azure Portal and navigate to the resource group of `rg-<RANDOM_NAME>` and find the Azure Container Apps instances.

   ![Lab 3 Deploy - results](./images/lab03-01.png)

1. Click the Container Apps instance, **redis**, and notice that the **Application Url** value has the word **internal** in it. This indicates the resource is NOT exposed to the Internet.

   ![Lab 3 Deploy - Redis Cache container](./images/lab03-02.png)

1. Click the Container Apps instance, **products**, again notice the word **internal** in the **Application Url** value.

   ![Lab 3 Deploy - Products container](./images/lab03-03.png)

1. Click the Container Apps instance, **store**, and note that **Application Url** does NOT contain the word **internal**. The store website is available to the Internet.

   ![Lab 3 Deploy - Store container](./images/lab03-04.png)

> **Note**:
>
> At this point, the store app is available but there is no database available on Azure. Therefore, trying to navigate to the product page will result in seeing this message, "There is a problem loading our products. Please try again later". We'll add a database in the next lab.

## Analyze the provisioning

That might have seemed like magic, but we can have `azd` explain what it did by creating Bicep files for the resources it provisioned. This way we could put those infrastructure files in source control.

1. Switch back to the terminal.
1. Then you can generate the Bicep files by running the following commands:

    ```powershell
    azd infra gen
    ```

1. Confirm the following files have been generated:

   - `infra/main.bicep`
   - `infra/main.parameters.json`
   - `infra/resources.bicep`

1. Open those files and see which resources are being provisioned.

## Analyze the deployment

When using `azd` outside of .NET Aspire, you have to specify which applications you want to deploy. But `azd` automatically detects the applications for you with .NET Aspire-based projects. You can still see what `azd` is deploying by generating a manifest file.

1. From the terminal run:

    ```bash
    # bash/zsh
    dotnet run --project ./eShopLite.AppHost \
        -- \
        --publisher manifest \
        --output-path ../aspire-manifest.json
    ```

    ```powershell
    # PowerShell
    dotnet run --project ./eShopLite.AppHost `
        -- `
        --publisher manifest `
        --output-path ../aspire-manifest.json
    ```

1. Confirm the following file has been generated:

   - `aspire-manifest.json`

1. Open the `aspire-manifest.json` file and see which resources are being deployed.

1. To delete all the resources created by the deployment, you can execute the following command:

    ```powershell
    azd down --force --purge
    ```

---

[<- Lab 2 - Add Redis caching to the app](/Labs/Lab%202%20-%20Caching%20and%20Dashboard/README.md) | [Lab 4 - Add a database to the app ->](/Labs/Lab%204%20-%20Data/README.md)
