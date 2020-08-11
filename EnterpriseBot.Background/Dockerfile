FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS publish
WORKDIR /src
COPY . .
RUN dotnet publish "EnterpriseBot.Background/EnterpriseBot.Background.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS final
WORKDIR /app
EXPOSE 80 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EnterpriseBot.Background.dll"]