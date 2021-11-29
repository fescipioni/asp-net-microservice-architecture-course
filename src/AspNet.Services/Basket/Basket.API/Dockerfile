#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AspNet.Services/Basket/Basket.API/Basket.API.csproj", "AspNet.Services/Basket/Basket.API/"]
COPY ["AspNet.BuildingBlocks/EventBus.Messages/EventBus.Messages.csproj", "AspNet.BuildingBlocks/EventBus.Messages/"]
RUN dotnet restore "AspNet.Services/Basket/Basket.API/Basket.API.csproj"
COPY . .
WORKDIR "/src/AspNet.Services/Basket/Basket.API"
RUN dotnet build "Basket.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Basket.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Basket.API.dll"]