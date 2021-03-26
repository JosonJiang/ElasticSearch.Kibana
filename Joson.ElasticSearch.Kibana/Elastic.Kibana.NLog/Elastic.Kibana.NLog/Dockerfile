#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Elastic.Kibana.NLog/Elastic.Kibana.NLog.csproj", "Elastic.Kibana.NLog/"]
RUN dotnet restore "Elastic.Kibana.NLog/Elastic.Kibana.NLog.csproj"
COPY . .
WORKDIR "/src/Elastic.Kibana.NLog"
RUN dotnet build "Elastic.Kibana.NLog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Elastic.Kibana.NLog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Elastic.Kibana.NLog.dll"]