# PxWeb Api[![Mentioned in Awesome Official Statistics ](https://awesome.re/mentioned-badge.svg)](http://www.awesomeofficialstatistics.org)

This is the official source code repository for PxWebApi. PxWeb is a nice web application for dissemination of statistical tables please read more abou it at the official web page on Statistics Sweden web site at [www.scb.se/px-web](https://www.scb.se/px-web).

## Current activities

We are currently developing PxWebApi 2.0

## Development notes

```sh
curl -i -H "API_ADMIN_KEY: test" -X 'PUT'  https://localhost:8443/api/v2/admin/database
curl -i -H "API_ADMIN_KEY: test" -X 'POST' https://localhost:8443/api/v2/admin/searchindex
curl -i -H "API_ADMIN_KEY: test" -X 'PATCH' -H 'Content-Type: application/json' -d '["TAB001", "TAB004"]' https://localhost:8443/api/v2/admin/searchindex
```

### Code formatting

We use [dotnet format](https://github.com/dotnet/format) to clean the source code. The build pipeline aslo checks for formatting error.

If you don't want to manually run `dotnet format` or Code Cleanup in Visual Studio you can use git [pre-commit](https://pre-commit.com/). After installing pre-commit for your operating system, run `pre-commit install`from the root of this repo and you're done.

The rules for formatting are set in the [.editorconfig](.editorconfig) file. Visual Studio supports this automatically, and for VS Code we have the [EditorConfig extension](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig). More information on [EditorConfig](https://editorconfig.org/)

## Initial OpenID Connect (OIDC) Authentication support

This API supports OpenID Connect (OIDC) authentication, allowing secure user authentication via **Keycloak** or another OIDC provider.  

### How It Works  
- If **OIDC is enabled**, all requests **must** include a valid **Bearer token** in the `Authorization` header.  
- Tokens are validated using the configured **OIDC Authority** (e.g., Keycloak).  
- Unauthorized requests receive a **401 Unauthorized** response.  
- The **frontend needs to handle login and JWT decoding**

### Configuration (`appsettings.json`)
Modify the `Authentication` section to enable OIDC:  

```json
"Authentication": {
  "UseOIDC": true,
  "Authority": "http://localhost:8080/realms/myrealm",
  "Audience": "PxWebApi",
  "RequireHttpsMetadata": false
}
```

### Testing Authentication
#### Valid Token (Should return 200 OK)
```sh
curl -H "Authorization: Bearer <VALID_TOKEN>" http://localhost:8080/api/v2/tables
```

#### No Token (Should return 401 Unauthorized)
```sh
curl -v http://localhost:8080/api/v2/tables
```

#### Invalid Token (Should return 401 Unauthorized)
```sh
curl -H "Authorization: Bearer fake-token" http://localhost:8080/api/v2/tables
```

### Implementation Details 
- Authentication is handled **globally** in `Program.cs` via `UseAuthentication()` and `UseAuthorization()`.  
- **All controllers require authentication** (`RequireAuthorization()` is applied globally).  
- If OIDC is disabled, the API functions as usual without authentication.  
