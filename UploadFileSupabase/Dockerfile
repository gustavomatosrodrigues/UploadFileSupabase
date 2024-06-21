# Use a imagem base do SDK do .NET para construir seu aplicativo
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Define o diretório de trabalho dentro do contêiner
WORKDIR /app

# Copia o arquivo csproj e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante do código-fonte e constrói o aplicativo
COPY . ./
RUN dotnet publish -c Release -o out

# Define a imagem base para a execução do aplicativo
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "UploadFileSupabase.dll"]

