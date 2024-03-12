FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 8080

ENV DOTNET_EnableDiagnostics=0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS=http://*:8080

RUN apk add --no-cache icu-libs
# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 1000 --disabled-password --gecos "" appuser && chown -R appuser /app
USER 1000

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine  AS build
WORKDIR /src
COPY . .
RUN dotnet restore "PxWeb.sln"
RUN dotnet build "PxWeb/PxWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PxWeb/PxWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PxWeb.dll"]
