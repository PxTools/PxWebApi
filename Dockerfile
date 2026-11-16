# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:11.0-alpine@sha256:52556c0fb9d0f609ae9aeab9efeeba96f4acc2f7038f7696f2235ccbdbb8e290 AS build
ARG TARGETARCH
WORKDIR /source

COPY . .

RUN \
    mv docker/pxwebapi/app.config PxWeb && \
    mv docker/pxwebapi/log4net.config PxWeb && \
    dotnet publish -a "$TARGETARCH" -o /app "PxWeb/PxWeb.csproj"


# Enable globalization and time zones:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/enable-globalization.md
# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine@sha256:fc9d161eefc515dd65749a45189ac9691cf9954a044fef23ac278e039c6e2ac4
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

COPY --from=build /app .
USER $APP_UID
ENTRYPOINT [ "./PxWeb" ]
