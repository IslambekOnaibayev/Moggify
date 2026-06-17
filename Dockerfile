# syntax=docker/dockerfile:1

# ---- Stage 1: build the Angular client ----
FROM node:22-alpine AS client
WORKDIR /client
COPY Web/ClientApp/package.json Web/ClientApp/package-lock.json ./
RUN npm ci
COPY Web/ClientApp/ ./
RUN npm run build

# ---- Stage 2: build & publish the .NET backend ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . ./
RUN dotnet restore Web/Web.csproj
RUN dotnet publish Web/Web.csproj -c Release -o /app/publish /p:UseAppHost=false
COPY --from=client /client/dist/MoggifyWeb/browser/ /app/publish/wwwroot/

# ---- Stage 3: runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "Web.dll"]
