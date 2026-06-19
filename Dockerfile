FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app
COPY Nutrisense.Nutrisense.Platform/*.csproj Nutrisense.Nutrisense.Platform/
RUN dotnet restore ./Nutrisense.Nutrisense.Platform
COPY . .
RUN dotnet publish ./Nutrisense.Nutrisense.Platform -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=builder /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Nutrisense.Nutrisense.Platform.dll"]
