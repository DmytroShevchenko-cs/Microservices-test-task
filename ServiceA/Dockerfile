FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ServiceA/ServiceA.csproj", "ServiceA/"]
RUN dotnet restore "ServiceA/ServiceA.csproj"

COPY . .
WORKDIR "/src/ServiceA"

RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ServiceA.dll"]
