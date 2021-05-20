FROM mcr.microsoft.com/dotnet/sdk:5.0-focal
WORKDIR /src
COPY . .
RUN dotnet publish "Fuel-Georgia-Parser.csproj" -c Release -o /app/publish
WORKDIR /app
ENTRYPOINT ["/app/publish/Fuel-Georgia-Parser"]