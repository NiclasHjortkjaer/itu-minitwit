FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5108

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . ./
RUN dotnet restore "./itu-minitwit/Server/itu-minitwit.Server.csproj"
WORKDIR "/src"
RUN dotnet build "./itu-minitwit/Server/itu-minitwit.Server.csproj" -c release -o /app/build

FROM build AS publish
RUN dotnet publish "./itu-minitwit/Server/itu-minitwit.Server.csproj" -c release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "itu-minitwit.Server.dll"]