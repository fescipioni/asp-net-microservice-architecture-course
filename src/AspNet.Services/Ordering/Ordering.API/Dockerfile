#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AspNet.Services/Ordering/Ordering.API/Ordering.API.csproj", "AspNet.Services/Ordering/Ordering.API/"]
COPY ["AspNet.Services/Ordering/Ordering.Application/Ordering.Application.csproj", "AspNet.Services/Ordering/Ordering.Application/"]
COPY ["AspNet.Services/Ordering/Ordering.Domain/Ordering.Domain.csproj", "AspNet.Services/Ordering/Ordering.Domain/"]
COPY ["AspNet.BuildingBlocks/EventBus.Messages/EventBus.Messages.csproj", "AspNet.BuildingBlocks/EventBus.Messages/"]
COPY ["AspNet.Services/Ordering/Ordering.Infrastructure/Ordering.Infrastructure.csproj", "AspNet.Services/Ordering/Ordering.Infrastructure/"]
RUN dotnet restore "AspNet.Services/Ordering/Ordering.API/Ordering.API.csproj"
COPY . .
WORKDIR "/src/AspNet.Services/Ordering/Ordering.API"
RUN dotnet build "Ordering.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ordering.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordering.API.dll"]