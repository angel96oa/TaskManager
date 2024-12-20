FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TaskManager.Service/TaskManager.Service.csproj", "TaskManager.Service/"]
COPY ["TaskManager.Proto/TaskManager.Proto.csproj", "TaskManager.Proto/"]
COPY ["TaskManager.Data/TaskManager.Data.csproj", "TaskManager.Data/"]
COPY ["TaskManager.Identity/TaskManager.Identity.csproj", "TaskManager.Identity/"]
COPY ["TaskManager.Configuration/TaskManager.Configuration.csproj", "TaskManager.Configuration/"]
COPY ["TaskManager.Messaging/TaskManager.Messaging.csproj", "TaskManager.Messaging/"]

RUN dotnet restore "TaskManager.Service/TaskManager.Service.csproj"

COPY "TaskManager.Service/" "TaskManager.Service/"
COPY "TaskManager.Proto/" "TaskManager.Proto/"
COPY "TaskManager.Data/" "TaskManager.Data/"
COPY "TaskManager.Identity/" "TaskManager.Identity/"
COPY "TaskManager.Messaging/" "TaskManager.Messaging/"
COPY "TaskManager.Configuration/" "TaskManager.Configuration/"
WORKDIR "/src/TaskManager.Service"
RUN dotnet build "TaskManager.Service.csproj" -c Release -o /app/build

RUN dotnet publish "TaskManager.Service.csproj" -c Release -o /app/publish

RUN mkdir -p /https && \
    dotnet dev-certs https -ep /https/aspnetapp.pfx -p password

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /https /https
ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=password
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENTRYPOINT ["dotnet", "TaskManager.Service.dll"]
