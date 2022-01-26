FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
# Изменяем текущий каталог в контейнере 
WORKDIR /app

COPY ./WebUI/WebUI.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish ./WebUI/WebUI.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000-5001

COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WebUI.dll"]