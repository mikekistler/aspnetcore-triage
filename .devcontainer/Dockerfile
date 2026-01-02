ARG VARIANT=1-8.0
FROM mcr.microsoft.com/devcontainers/dotnet:${VARIANT}

# Install .NET 9.0
COPY --from=mcr.microsoft.com/dotnet/sdk:9.0 /usr/share/dotnet /usr/share/dotnet
