# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Kcl.Reso.CleanTemplate.Web.Api/Kcl.Reso.CleanTemplate.Web.Api.csproj", "src/Kcl.Reso.CleanTemplate.Web.Api/"]
COPY ["src/Kcl.Reso.CleanTemplate.Infrastructure/Kcl.Reso.CleanTemplate.Infrastructure.csproj", "src/Kcl.Reso.CleanTemplate.Infrastructure/"]
COPY ["src/Kcl.Reso.CleanTemplate.Application/Kcl.Reso.CleanTemplate.Application.csproj", "src/Kcl.Reso.CleanTemplate.Application/"]
COPY ["src/Kcl.Reso.CleanTemplate.Domain/Kcl.Reso.CleanTemplate.Domain.csproj", "src/Kcl.Reso.CleanTemplate.Domain/"]
COPY ["src/Kcl.Reso.CleanTemplate.SharedKernel/Kcl.Reso.CleanTemplate.SharedKernel.csproj", "src/Kcl.Reso.CleanTemplate.SharedKernel/"]
RUN dotnet restore "./src/Kcl.Reso.CleanTemplate.Web.Api/Kcl.Reso.CleanTemplate.Web.Api.csproj"
COPY . .
WORKDIR "/src/src/Kcl.Reso.CleanTemplate.Web.Api"
RUN dotnet build "./Kcl.Reso.CleanTemplate.Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Kcl.Reso.CleanTemplate.Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kcl.Reso.CleanTemplate.Web.Api.dll"]