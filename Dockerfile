# -----------------------------------------
# Build Stage
# -----------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

COPY . .

RUN dotnet restore

RUN dotnet publish -c Release -o /app --no-restore

# -----------------------------------------
# Runtime Stage
# -----------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app .

# Render setzt die Variable PORT automatisch.
# 8080 dient nur als Fallback.
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TooTheMoon.dll"]
