#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MessagingService/NuGet.Config", "."]
COPY ["MessagingService/MessagingService.csproj", "MessagingService/"]
COPY ["MessagingService.BusinessLogic/MessagingService.BusinessLogic.csproj", "MessagingService.BusinessLogic/"]
COPY ["MessagingService.EmailMessageAggregate/MessagingService.EmailMessageAggregate.csproj", "MessagingService.EmailMessageAggregate/"]
COPY ["MessagingService.EmailMessage.DomainEvents/MessagingService.EmailMessage.DomainEvents.csproj", "MessagingService.EmailMessage.DomainEvents/"]
COPY ["MessagingService.DataTransferObjects/MessagingService.DataTransferObjects.csproj", "MessagingService.DataTransferObjects/"]
RUN dotnet restore "MessagingService/MessagingService.csproj"
COPY . .
WORKDIR "/src/MessagingService"
RUN dotnet build "MessagingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessagingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessagingService.dll"]