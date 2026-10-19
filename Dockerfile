# Этап 1: сборка
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY VideoLibraryApi/VideoLibraryApi.csproj ./VideoLibraryApi/
RUN dotnet restore VideoLibraryApi/VideoLibraryApi.csproj

# Копируем всё остальное и публикуем
COPY . .
WORKDIR /src/VideoLibraryApi
RUN dotnet publish VideoLibraryApi.csproj -c Release -o /app/publish

# Этап 2: финальный образ (только рантайм)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "VideoLibraryApi.dll"]