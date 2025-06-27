
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["JobAggregator.Worker/JobAggregator.Worker.csproj", "JobAggregator.Worker/"]
COPY ["JobAggregator.Core/JobAggregator.Core.csproj", "JobAggregator.Core/"]
RUN dotnet restore "JobAggregator.Worker/JobAggregator.Worker.csproj"
COPY . .
WORKDIR "/src/JobAggregator.Worker"
RUN dotnet build "JobAggregator.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobAggregator.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY JobAggregator.Worker/appsettings.json .
ENTRYPOINT ["dotnet", "JobAggregator.Worker.dll"]
