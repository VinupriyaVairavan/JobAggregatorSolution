
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["JobAggregator.Dashboard/JobAggregator.Dashboard.csproj", "JobAggregator.Dashboard/"]
COPY ["JobAggregator.Core/JobAggregator.Core.csproj", "JobAggregator.Core/"]
RUN dotnet restore "JobAggregator.Dashboard/JobAggregator.Dashboard.csproj"
COPY . .
WORKDIR "/src/JobAggregator.Dashboard"
RUN dotnet build "JobAggregator.Dashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobAggregator.Dashboard.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
