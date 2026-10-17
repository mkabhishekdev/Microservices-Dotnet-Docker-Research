# AbhiShop migration to .NET 8

The three existing applications have been migrated from .NET Core 2.0 to .NET 8. The solution folders, MVC controllers, Razor views, inventory endpoints, SQL Server databases, and Identity login flow remain in place. There are no additional microservices or architectural layers.

## Framework and dependency changes

All three `.csproj` files now target `net8.0`. `Microsoft.AspNetCore.All` has been removed: the Web SDK supplies the ASP.NET Core shared framework on .NET 8. Packages formerly supplied by that metapackage, such as EF Core SQL Server and OpenID Connect authentication, are now referenced explicitly where needed.

| Dependency | Version | Used by |
| --- | --- | --- |
| Microsoft.EntityFrameworkCore.SqlServer / Design | 8.0.31 | Inventory and token services |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.31 | Token service |
| Microsoft.AspNetCore.Authentication.OpenIdConnect | 8.0.31 | Storefront |
| Microsoft.AspNetCore.Mvc.NewtonsoftJson | 8.0.31 | Inventory API |
| Swashbuckle.AspNetCore / Newtonsoft | 8.1.4 | Inventory API |
| Duende.IdentityServer / AspNetIdentity | 7.4.13 | Token service |
| Newtonsoft.Json | 13.0.4 | Storefront's existing HTTP client |

Old `DotNetCliToolReference` entries and obsolete scaffolding packages were removed. EF Core tooling is now a standard local tool in `.config/dotnet-tools.json`, pinned to 8.0.31. Your globally installed EF tool is left in place. The repository's `global.json` selects the installed SDK 8.0.408 and permits later .NET 8 SDK feature bands.

## Changes to the existing files

Paths below are relative to this `Development` directory.

| File(s) | Change and reason |
| --- | --- |
| `src/Services/InventoryManagementAPI/InventoryManagementAPI.csproj` | .NET 8 target, explicit EF/JSON/Swagger packages, obsolete tooling removed. |
| `src/Services/InventoryManagementAPI/Startup.cs` | `AddControllersWithViews`, `UseRouting`, and `UseEndpoints` replace old MVC routing. Original routes are retained. `IWebHostEnvironment` replaces `IHostingEnvironment`. Swagger uses `OpenApiInfo`. Newtonsoft serialization is retained for API response compatibility. Database environment settings still override the appsettings connection string. |
| `src/Services/InventoryManagementAPI/Program.cs` | Uses the generic host with the existing `Startup` class and startup seeding. |
| `src/Services/InventoryManagementAPI/Controllers/ImageController.cs` | Updates the hosting-environment interface; the existing image endpoint is retained. |
| `src/Services/InventoryManagementAPI/Data/InventoryContext.cs` | `UseHiLo` replaces the removed SQL Server HiLo API. Explicit `decimal(18,2)` mapping preserves the existing price column. |
| `src/Services/InventoryManagementAPI/Data/Migrations/20191204022241_InitialMigration.Designer.cs`, `InventoryContextModelSnapshot.cs` | Updates EF metadata, sequence annotations, and SQL types for EF Core 8. The migration ID and original table-creation migration are retained. |
| `src/Services/InventoryManagementAPI/appsettings.json`, `appsettings.Development.json`, `Properties/launchSettings.json` | Local SQL connection uses `localhost,1445`; local development trusts the SQL container certificate; project launch port is 5100. |
| `src/Services/TokenServiceAPI/TokenServiceAPI.csproj` | .NET 8 target, EF/Identity packages, and Duende packages replacing IdentityServer4. |
| `src/Services/TokenServiceAPI/Startup.cs` | Updates MVC routing/environment APIs, removes obsolete BrowserLink/database error middleware, and registers the existing identity resources, clients, and explicit API scopes using Duende. Keeps developer signing and in-memory grants. |
| `src/Services/TokenServiceAPI/Config.cs` | Adds an explicit `cart` API scope, as required by the newer resource model. Retains the `mvc` client, secret, hybrid `code id_token` flow, and redirect URLs. PKCE remains disabled for compatibility with the existing hybrid flow. |
| `src/Services/TokenServiceAPI/Controllers/AccountController.cs`, `HomeController.cs` | Imports Duende and the project's quickstart namespace; existing account actions remain. Logout now renders the existing `SignedOut` view, correcting the old reference to a missing `LoggedOut` view. |
| `src/Services/TokenServiceAPI/Quickstart/Account/*.cs`, `Consent/*.cs`, `Grants/*.cs`, `Home/ErrorViewModel.cs`, `SecurityHeadersAttribute.cs` | Replaces IdentityServer4 namespaces and adapts changed client, resource, consent, grant, and security-header APIs. Quickstart types now belong to `TokenServiceAPI.Quickstart.UI`. |
| `src/Services/TokenServiceAPI/Data/ApplicationDbContext.cs` | Explicitly preserves the original Identity indexes and relationships instead of inheriting changed defaults. |
| `src/Services/TokenServiceAPI/Data/Migrations/00000000000000_CreateIdentitySchema.Designer.cs`, `ApplicationDbContextModelSnapshot.cs` | Updates Identity entity metadata to EF Core 8 while retaining the existing schema and migration ID. |
| `src/Services/TokenServiceAPI/Data/IdentityDbInit.cs`, `Program.cs` | Replaces unawaitable `async void` seeding with `Task` and waits for it in the existing startup sequence, so database errors reach the existing exception handler. |
| `src/Services/TokenServiceAPI/Views/Account/Login.cshtml`, `Views/_ViewImports.cshtml`, `Views/Shared/Error.cshtml` | Removes a deleted authentication namespace and updates references to quickstart types. |
| `src/Services/TokenServiceAPI/Views/Account/SignedOut.cshtml` | Uses the existing logout view model and renders the post-logout callback link and sign-out notification iframe. This completes the existing logout flow without adding a new view. |
| `src/Services/TokenServiceAPI/Views/Manage/ChangePassword.cshtml`, `ExternalLogins.cshtml`, `Index.cshtml`, `SetPassword.cshtml` | Uses asynchronous partial rendering supported by current Razor. |
| `src/Services/TokenServiceAPI/appsettings.Development.json`, `Properties/launchSettings.json` | Uses local SQL on 1445 with the development certificate setting, and token-service port 5200. |
| `src/Web/AbhiShop.Webmvc/AbhiShop.Webmvc.csproj` | .NET 8 target; explicit OIDC and Newtonsoft packages; obsolete metapackage/tool removed. |
| `src/Web/AbhiShop.Webmvc/Startup.cs` | Updates routing and environment APIs, adds authorization middleware, and removes BrowserLink. Preserves OIDC hybrid authentication and maps the existing username claim. Error handling points to the existing `/Inventory/Error` action. |
| `src/Web/AbhiShop.Webmvc/Views/Inventory/Index.cshtml` | Replaces three synchronous partial renders with `await Html.PartialAsync`. |
| `src/Web/AbhiShop.Webmvc/Properties/launchSettings.json` | Uses port 5000, matching the existing callback/client configuration. |
| All three `Dockerfile` files | Uses Microsoft's .NET 8 SDK/runtime images and sets `ASPNETCORE_URLS=http://+:80` explicitly, preserving the original published ports. |
| `docker-compose.yml` | Updates SQL Server to the Microsoft Container Registry 2022 image and its password variable, supplies container database/certificate settings, and makes the existing external identity address configurable with `IDENTITY_URL`. |

The only new supporting files are the SDK selection, EF tool manifest, `.gitignore`, three `.dockerignore` files, and these notes. Docker ignores prevent old Windows build output from entering Linux image builds. Git ignores keep new build output and generated developer signing keys out of source changes. Previously tracked `bin`/`obj` files are still tracked; some were already modified before the migration and restore/build refreshes others.

The original `tempkey.rsa` is retained. Duende creates a separate `tempkey.net8.jwk`, avoiding attempts to deserialize the old signing-key format.

## Running locally

From the repository root:

```sh
cd PROJECT_AbhiShop/Development
dotnet restore Development.sln
dotnet tool restore
dotnet build Development.sln
```

Start Docker Desktop and run the existing database service:

```sh
docker compose up -d mssqlserver
```

Wait until SQL Server is ready, then run each application in a separate terminal from `Development`:

```sh
dotnet run --project src/Services/InventoryManagementAPI
dotnet run --project src/Services/TokenServiceAPI
dotnet run --project src/Web/AbhiShop.Webmvc
```

| Application | Address |
| --- | --- |
| Storefront | http://localhost:5000 |
| Inventory Swagger | http://localhost:5100/swagger |
| Token service | http://localhost:5200 |
| SQL Server for local application processes | `localhost,1445` |

The original startup database migration/seeding remains. On a new database, it creates the original tables and sample inventory/user. The existing sample login is `me@myemail.com` with password `P@ssword1`.

In `Development`, the token service uses `SameSite=Lax` for its Identity application/external cookies and IdentityServer session cookies. These services share the same site on local HTTP, and browsers reject `SameSite=None` cookies without `Secure`, which otherwise causes a successful password submission to loop back to the login form. The overrides are registered after `AddAspNetIdentity` so that they take effect. After changing these settings, rebuild and restart the token service before trying a fresh login from `http://localhost:5000`.

Local connection settings can be overridden through configuration or environment variables, for example `ConnectionStrings__DefaultConnection` for the token service and `ConnectionString` for inventory. Development certificate trust addresses the newer SQL client's encryption default for a local SQL container.

## Running the complete Docker stack

The browser and MVC container must both be able to reach the same identity address. The original Docker setup used the host's LAN IP for this purpose; `localhost` inside a container points at that container.

Set `IDENTITY_URL` to your host's current LAN IP before launching. For example, if your Mac's LAN IP is `192.168.1.20`:

```sh
IDENTITY_URL=http://192.168.1.20:5200 docker compose up --build
```

Compose preserves the original `192.168.1.2` default for compatibility, so override it when it differs from your machine. Local `dotnet run` uses `localhost` and does not need this override. The service/container topology and host ports remain 5000/5100/5200/1445.

## Database tools and reviewing changes

Use the local EF Core 8 tool from `Development`. These commands generate SQL without applying it:

```sh
dotnet ef migrations script --project src/Services/InventoryManagementAPI --output /tmp/abhishop-inventory.sql
dotnet ef migrations script --project src/Services/TokenServiceAPI --output /tmp/abhishop-token.sql
```

To focus a Git review on source changes and exclude the repository's already tracked build output, run this from the repository root:

```sh
git diff -- . ':!**/bin/**' ':!**/obj/**'
```

## IdentityServer dependency

IdentityServer4 is obsolete. Duende IdentityServer 7 is its .NET 8-compatible successor, allowing the closest migration of the existing client, MVC login, ASP.NET Identity, and quickstart code. Duende permits development/testing and personal use; production deployment requires checking its license terms. See [Duende licensing](https://docs.duendesoftware.com/general/licensing/) and [the version 7 upgrade guide](https://docs.duendesoftware.com/identityserver/upgrades/v6_3-to-v7_0/).

Microsoft documents the [MVC/shared-framework migration changes](https://learn.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-8.0) and [the .NET 8 container port change](https://learn.microsoft.com/en-us/dotnet/core/compatibility/containers/8.0/aspnet-port). These explain the startup/package edits and explicit Docker port setting.

## Validation

- The complete solution builds on the installed .NET SDK 8.0.408 / runtime 8.0.15 with **zero warnings and zero errors**.
- All three Docker images build successfully, including Linux Release publishing of the Razor views. Compose configuration validates successfully.
- NuGet's vulnerability check reports no known vulnerable direct or transitive packages in any of the three projects.
- EF Core 8 reports **no pending model changes** for both databases. Temporary validation harnesses also compare the relational models and generate idempotent SQL scripts without schema differences.
- Against a separate temporary SQL Server 2022 container, both original database migrations apply and startup seeding succeeds. Inventory returns the original 21 sample products.
- Inventory checks pass for paging SQL, the home redirect, Swagger document/UI, image delivery, and an invalid-ID response.
- Token-service checks pass for discovery/JWKS, home, login/registration Razor forms, protected grants, and the existing hybrid authorization redirect.
- The native storefront renders seeded products through the inventory API. An integrated HTTP test passes login with the original sample user, hybrid callback/code redemption, username claims, authentication-cookie persistence, logout through the existing signed-out page, and a fresh login requirement after logout.
- A Chrome browser test passes the actual login-button submission and automatic OIDC callback, displays the sample username on `/Inventory/About`, preserves authentication after reload, and requires a fresh login after logout. The development cookie settings prevent the browser's `SameSiteNoneInsecure` rejection.

The validation harnesses live outside the repository; no test project or replacement application flow was added. The database tests use a separate temporary container, leaving the existing `sql2022` container in place. The complete Compose stack's browser authentication address must still be configured for your host as described above.
