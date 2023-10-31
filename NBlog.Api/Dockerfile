FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NBlog.Api/NBlog.Api.csproj", "NBlog.Api/"]
RUN dotnet restore "NBlog.Api/NBlog.Api.csproj"
COPY . .
WORKDIR "/src/NBlog.Api"
RUN dotnet build "NBlog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NBlog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NBlog.Api.dll"]
