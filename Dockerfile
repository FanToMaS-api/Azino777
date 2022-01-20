FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base
# Изменяем текущий каталог в контейнере 
WORKDIR /app

COPY . ./

RUN dotnet restore

RUN dotnet build ./WebUI/WebUI.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY --from=base /out .
ENTRYPOINT ["dotnet", "WebUI.dll"]
