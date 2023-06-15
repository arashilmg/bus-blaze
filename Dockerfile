# Arguments
ARG BUILD_VERSION="1.0.0"

# Base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_VERSION
ARG GH_USER
ARG GH_PKGS_TOKEN
WORKDIR /sln
COPY BizCover.Api.Renewals.sln ./
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./src/${file%.*}/ && mv $file ./src/${file%.*}/; done
COPY tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./tests/${file%.*}/ && mv $file ./tests/${file%.*}/; done
RUN dotnet nuget add source https://nuget.pkg.github.com/bizcover/index.json -n github --username $GH_USER --password $GH_PKGS_TOKEN --store-password-in-clear-text
RUN dotnet restore
COPY . .
RUN echo Building ${BUILD_VERSION}
RUN dotnet build --no-restore -c Release -p:Version=$BUILD_VERSION

# Test
FROM build AS tests

# Publish
FROM build AS publish
ARG BUILD_VERSION
WORKDIR /sln/src/BizCover.Api.Renewals
RUN echo Publishing ${BUILD_VERSION}
RUN dotnet publish --no-build -c Release -p:Version=$BUILD_VERSION -o /artifacts/server

# Package
FROM build AS packages
ARG BUILD_VERSION
WORKDIR /sln/src/BizCover.Protos.Renewals
RUN echo Packing ${BUILD_VERSION}
RUN dotnet pack --no-build -c Release -p:Version=$BUILD_VERSION -o /artifacts/packages

# Run
FROM base AS final
WORKDIR /app
COPY --from=publish /artifacts/server .
ENTRYPOINT ["dotnet", "BizCover.Api.Renewals.dll"]
