# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the CSPROJ file and restore dependencies
COPY medical-record-system-backend.csproj ./
RUN dotnet restore medical-record-system-backend.csproj

# Copy the entire project and publish
COPY . ./
RUN dotnet publish medical-record-system-backend.csproj -c Release -o /publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "medical-record-system-backend.dll"]
