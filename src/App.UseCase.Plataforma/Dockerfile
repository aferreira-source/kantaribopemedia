#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
#EXPOSE 2222
#EXPOSE 5241
#EXPOSE 7113

#ENV TZ=America/Sao_Paulo


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/app.plataforma.csproj", "App.UseCase.Plataforma/"]
RUN dotnet restore "App.UseCase.Plataforma/app.plataforma.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "./app.plataforma.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./app.plataforma.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "app.plataforma.dll"]