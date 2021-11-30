# Build image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
# Изменяем текущий каталог в контейнере 
WORKDIR /app

COPY . ./

# указываем точку монтирования для внешних данных внутри контейнера (как мы помним, это Линукс)
# VOLUME /tmp ? https://habr.com/ru/post/448094/

# Восстановление зависимостей указанных в файле проекта
RUN dotnet restore

RUN dotnet build ./Server/Server.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Server.dll"]
