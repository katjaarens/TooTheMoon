# -----------------------------------------
# Build Stage
# -----------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Force rebuild layer
COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o /app

# -----------------------------------------
# Run Stage
# -----------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published output
COPY --from=build /app .

# Expose port for Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TooTheMoon.dll"]
