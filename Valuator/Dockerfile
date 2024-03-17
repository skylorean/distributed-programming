FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Valuator.csproj", "./"]
RUN dotnet restore "Valuator.csproj"
COPY . .

RUN dotnet build "Valuator.csproj" -c Release -o /app/build

RUN dotnet publish "Valuator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Valuator.dll"]