# AbhiShop: Microservices with .NET and Docker

AbhiShop is a sample e-commerce application built to explore how a traditional web application can be organized as microservices. It began as an M.S. capstone project at Rochester Institute of Technology and is the code companion to the [Understanding Microservices using AbhiShop](#the-article-series) article series.

The solution currently targets **.NET 8**. It contains three application projects and a SQL Server database, with Docker Compose to run them together. The design is intentionally focused on this example: it does not add a message broker, API gateway, or extra service layers.

## What the application contains

| Component | Responsibility |
| --- | --- |
| **AbhiShop.Webmvc** | The customer-facing ASP.NET Core MVC storefront. It displays the shop and uses the other applications for inventory and sign-in. |
| **InventoryManagementAPI** | An ASP.NET Core API for the product catalog, including inventory data and product images. It also exposes Swagger documentation. |
| **TokenServiceAPI** | The sign-in and token service. It uses ASP.NET Identity and Duende IdentityServer to authenticate the storefront. |
| **SQL Server** | Stores inventory and identity data in the `InventoryDb` and `TokenServiceDb` databases. |

At a high level, the browser uses the storefront, which requests product data from the inventory API. When a customer signs in, the storefront redirects to the token service and uses its authentication response. The applications connect to their respective databases.

## Repository layout

```text
PROJECT_AbhiShop/
└── Development/
    ├── Development.sln
    ├── docker-compose.yml
    └── src/
        ├── Services/
        │   ├── InventoryManagementAPI/
        │   └── TokenServiceAPI/
        └── Web/
            └── AbhiShop.Webmvc/
```

The solution, Compose file, and all three applications are under `PROJECT_AbhiShop/Development`. Each application has its own project file and Dockerfile. The [migration guide](PROJECT_AbhiShop/Development/NET8_MIGRATION.md) covers the .NET 8 update, local and Docker run instructions, database setup, configuration, and validation details.

## Run and explore

To build the solution locally, install the .NET 8 SDK and run:

```sh
cd PROJECT_AbhiShop/Development
dotnet restore Development.sln
dotnet build Development.sln
```

To run the full stack in containers, install Docker and follow the [Docker setup instructions](PROJECT_AbhiShop/Development/NET8_MIGRATION.md#running-the-complete-docker-stack). The identity-service address must be reachable by both your browser and the storefront container, so the guide explains how to set `IDENTITY_URL` for your network.

## The article series

The articles explain the ideas behind the project and the steps taken to build it:

1. [Introduction (theory)](https://medium.com/@abhikempanna/blog-series-understanding-microservices-using-abhishop-app-3a02b2f9a36f)
2. [Traditional / monolithic design](https://medium.com/@abhikempanna/blog-series-understanding-microservices-using-abhishop-app-f8a184f4f184)
3. [Roadmap towards microservices](https://medium.com/@abhikempanna/understanding-microservices-using-abhishop-app-68bab7054744)
4. [Developing AbhiShop using a microservices approach](https://medium.com/@abhikempanna/blog-series-understanding-microservices-using-abhishop-app-76c19ad849ee)
5. [Running AbhiShop as microservices: DevOps and limitations](https://medium.com/@abhikempanna/blog-series-understanding-microservices-using-abhishop-app-a03c9265d66a)
