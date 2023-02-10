# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ./ .
RUN dotnet restore "./itu-minitwit/Server/itu-minitwit.Server.csproj" --disable-parallel
RUN dotnet publish "./itu-minitwit/Server/itu-minitwit.Server.csproj" -c release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5000

ENTRYPOINT ["dotnet", "itu-minitwit.Server.dll"]