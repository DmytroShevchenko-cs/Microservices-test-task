FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ServiceC/ServiceC.csproj ServiceC/
COPY Shared/Shared.csproj Shared/

RUN dotnet restore ServiceC/ServiceC.csproj

COPY ServiceC/. ServiceC/
COPY Shared/. Shared/

WORKDIR /src/ServiceC
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ServiceC.dll"]
