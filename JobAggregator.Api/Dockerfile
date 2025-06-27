
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["JobAggregator.Api/JobAggregator.Api.csproj", "JobAggregator.Api/"]
COPY ["JobAggregator.Core/JobAggregator.Core.csproj", "JobAggregator.Core/"]
RUN dotnet restore "JobAggregator.Api/JobAggregator.Api.csproj"
COPY . .
WORKDIR "/src/JobAggregator.Api"
RUN dotnet build "JobAggregator.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobAggregator.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobAggregator.Api.dll"]
