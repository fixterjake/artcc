FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Server/ZDC.Server.csproj", "Server/"]
COPY ["Shared/ZDC.Shared.csproj", "Shared/"]
COPY ["Client/ZDC.Client.csproj", "Client/"]
RUN dotnet restore "Server/ZDC.Server.csproj"
COPY . .

WORKDIR "/src/Server"
RUN dotnet build "ZDC.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZDC.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZDC.Server.dll"]