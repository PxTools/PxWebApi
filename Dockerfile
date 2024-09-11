# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0.401-1-alpine3.19 AS build
ARG TARGETARCH
WORKDIR /source

COPY . .
RUN dotnet restore -a $TARGETARCH

RUN dotnet publish -a $TARGETARCH --no-restore -o /app "PxWeb/PxWeb.csproj"


# Enable globalization and time zones:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/enable-globalization.md
# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0.8-alpine3.19
EXPOSE 8080

ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8 \
    DOTNET_EnableDiagnostics=0 \
    ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production
RUN apk add --no-cache \
    icu-data-full \
    icu-libs

WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
#RUN adduser -u 1000 --disabled-password --gecos "" appuser && chown -R appuser /app
#USER 1000

COPY --from=build /app .
USER $APP_UID
ENTRYPOINT [ "./PxWeb" ]
