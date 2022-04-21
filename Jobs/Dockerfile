FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Jobs/ZDC.Jobs.csproj", "Jobs/"]
COPY ["Shared/ZDC.Shared.csproj", "Shared/"]
RUN dotnet restore "Jobs/ZDC.Jobs.csproj"
COPY . .
WORKDIR "/src/Jobs"
RUN dotnet build "ZDC.Jobs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZDC.Jobs.csproj" -c Release -o /app/publish
 
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZDC.Jobs.dll"]