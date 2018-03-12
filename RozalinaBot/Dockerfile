FROM microsoft/dotnet:2.0-runtime-nanoserver-1709 AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY RozalinaBot.sln ./
COPY RozalinaBot/RozalinaBot.csproj RozalinaBot/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/RozalinaBot
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RozalinaBot.dll"]
