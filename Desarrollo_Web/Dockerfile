FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "DemonSlayer.csproj"
RUN dotnet publish "DemonSlayer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 7211
ENV ASPNETCORE_URLS=http://+:7211

ENTRYPOINT ["dotnet", "DemonSlayer.dll"]