FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PersonFinderDotNET.slnx", "./"]
COPY ["src/PersonFinder.API/PersonFinder.API.csproj", "src/PersonFinder.API/"]
COPY ["src/PersonFinder.Application/PersonFinder.Application.csproj", "src/PersonFinder.Application/"]
COPY ["src/PersonFinder.Domain/PersonFinder.Domain.csproj", "src/PersonFinder.Domain/"]
COPY ["src/PersonFinder.Infrastructure/PersonFinder.Infrastructure.csproj", "src/PersonFinder.Infrastructure/"]

RUN dotnet restore "src/PersonFinder.API/PersonFinder.API.csproj"

COPY . .

RUN dotnet publish "src/PersonFinder.API/PersonFinder.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PersonFinder.API.dll"]