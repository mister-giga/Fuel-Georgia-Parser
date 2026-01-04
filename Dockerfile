FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "Fuel-Georgia-Parser.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app

# GitHub Action needs `git` inside the container (we run git clone/commit/push).
# Also ensure /github/workspace exists since GitHub runs the container with that workdir.
RUN apt-get update \
  && apt-get install -y --no-install-recommends git ca-certificates \
  && rm -rf /var/lib/apt/lists/* \
  && mkdir -p /github/workspace

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "/app/Fuel-Georgia-Parser.dll"]