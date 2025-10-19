# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todo el contenido del proyecto
COPY . .

# Restauramos dependencias
RUN dotnet restore "DemonSlayer.csproj"

# Publicamos en modo Release
RUN dotnet publish "DemonSlayer.csproj" -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiamos la app publicada desde la etapa anterior
COPY --from=build /app/publish .

# Puerto donde correrá la app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "DemonSlayer.dll"]
