FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["tests/BizCover.Api.OffersFake/BizCover.Api.OffersFake.csproj", "tests/BizCover.Api.OffersFake/"]
RUN dotnet restore "tests/BizCover.Api.OffersFake/BizCover.Api.OffersFake.csproj"
COPY . .
WORKDIR "/src/tests/BizCover.Api.OffersFake"
RUN dotnet build "BizCover.Api.OffersFake.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BizCover.Api.OffersFake.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BizCover.Api.OffersFake.dll"]