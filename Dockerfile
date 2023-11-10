FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

#RUN adduser -u 1000 --disabled-password --gecos "" appuser && chown -R appuser /app
#USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine  AS build
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
