#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG GH_USER
ARG GH_PKGS_TOKEN
COPY ["tests/BizCover.Api.PoliciesFake/BizCover.Api.PoliciesFake.csproj", "tests/BizCover.Api.PoliciesFake/"]
RUN dotnet nuget add source https://nuget.pkg.github.com/bizcover/index.json -n github --username $GH_USER --password $GH_PKGS_TOKEN --store-password-in-clear-text
RUN dotnet restore "tests/BizCover.Api.PoliciesFake/BizCover.Api.PoliciesFake.csproj"
COPY . .
WORKDIR "/src/tests/BizCover.Api.PoliciesFake"
RUN dotnet build "BizCover.Api.PoliciesFake.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BizCover.Api.PoliciesFake.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BizCover.Api.PoliciesFake.dll"]