FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS publish
WORKDIR /src
COPY . .
RUN dotnet publish "Fuel-Georgia-Parser.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-focal AS final
#RUN apt-get -y update
#RUN apt-get -y install git
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/app/Fuel-Georgia-Parser"]