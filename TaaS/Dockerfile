#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TaaS/TaaS.csproj", "TaaS/"]
RUN dotnet restore "TaaS/TaaS.csproj"
COPY . .
WORKDIR "/src/TaaS"
RUN dotnet build "TaaS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaaS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
VOLUME /data
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaaS.dll"]