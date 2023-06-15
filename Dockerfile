FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

ARG BUILD_VERSION
ARG GH_USER
ARG GH_PKGS_TOKEN

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG GH_USER
ARG GH_PKGS_TOKEN
WORKDIR /sln
COPY BizCover.Blaze.Infrastructure.Bus.sln ./
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./src/${file%.*}/ && mv $file ./src/${file%.*}/; done
COPY samples/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./src/${file%.*}/ && mv $file ./src/${file%.*}/; done
COPY tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./tests/${file%.*}/ && mv $file ./tests/${file%.*}/; done
RUN dotnet nuget add source https://nuget.pkg.github.com/bizcover/index.json -n github --username $GH_USER --password $GH_PKGS_TOKEN --store-password-in-clear-text
RUN dotnet restore
COPY . .

FROM build as tests
