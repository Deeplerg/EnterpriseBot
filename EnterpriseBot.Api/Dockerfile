#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["EnterpriseBot.Api/EnterpriseBot.Api.csproj", "EnterpriseBot.Api/"]
COPY ["EnterpriseBot.BackgroundJobs/EnterpriseBot.BackgroundJobs.csproj", "EnterpriseBot.BackgroundJobs/"]
COPY ["EnterpriseBot.ApiWrapper/EnterpriseBot.ApiWrapper.csproj", "EnterpriseBot.ApiWrapper/"]
RUN dotnet restore "EnterpriseBot.Api/EnterpriseBot.Api.csproj"
COPY . .
WORKDIR "/src/EnterpriseBot.Api"
RUN dotnet build "EnterpriseBot.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EnterpriseBot.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EnterpriseBot.Api.dll"]