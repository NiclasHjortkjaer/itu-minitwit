FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MiniTwit/MiniTwit.csproj", "MiniTwit/"]
RUN dotnet restore "MiniTwit/MiniTwit.csproj"
COPY . .
WORKDIR "/src/MiniTwit"
RUN dotnet build "MiniTwit.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MiniTwit.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MiniTwit.dll"]
