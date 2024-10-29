# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 2222 80 443

# Use the SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files required for the build
COPY ["Visual.Dataroma.BFF/Visual.Dataroma.BFF.csproj", "Visual.Dataroma.BFF/"]
COPY ["Visual.Dataroma.Domain/Visual.Dataroma.Domain.csproj", "Visual.Dataroma.Domain/"]
# Add more projects here if needed

# Restore dependencies
RUN dotnet restore "./Visual.Dataroma.BFF/Visual.Dataroma.BFF.csproj"

# Copy the rest of the source code
COPY . .

# Build the entire solution
RUN dotnet build "./Visual.Dataroma.BFF/Visual.Dataroma.BFF.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Visual.Dataroma.BFF/Visual.Dataroma.BFF.csproj" -c Release -o /app/publish

# Use the base image to run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Visual.Dataroma.BFF.dll"]

