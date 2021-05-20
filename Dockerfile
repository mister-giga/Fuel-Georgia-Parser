#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
RUN apk add --no-cache git
RUN apk add --no-cache tree
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
RUN dotnet --version
WORKDIR /src
COPY ["Fuel-Georgia-Parser.csproj", "."]
RUN dotnet restore "./Fuel-Georgia-Parser.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Fuel-Georgia-Parser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fuel-Georgia-Parser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#RUN pwd
#RUN ls -a
#RUN tree /app
ENTRYPOINT ["dotnet", "Fuel-Georgia-Parser.dll"]
