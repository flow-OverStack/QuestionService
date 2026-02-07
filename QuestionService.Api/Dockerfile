FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["QuestionService.Api/QuestionService.Api.csproj", "QuestionService.Api/"]
COPY ["QuestionService.Application/QuestionService.Application.csproj", "QuestionService.Application/"]
COPY ["QuestionService.Domain/QuestionService.Domain.csproj", "QuestionService.Domain/"]
COPY ["QuestionService.BackgroundJobs/QuestionService.BackgroundJobs.csproj", "QuestionService.BackgroundJobs/"]
COPY ["QuestionService.Outbox/QuestionService.Outbox.csproj", "QuestionService.Outbox/"]
COPY ["QuestionService.DAL/QuestionService.DAL.csproj", "QuestionService.DAL/"]
COPY ["QuestionService.GraphQl/QuestionService.GraphQl.csproj", "QuestionService.GraphQl/"]
COPY ["QuestionService.Messaging/QuestionService.Messaging.csproj", "QuestionService.Messaging/"]
COPY ["QuestionService.Cache/QuestionService.Cache.csproj", "QuestionService.Cache/"]
COPY ["QuestionService.GrpcClient/QuestionService.GrpcClient.csproj", "QuestionService.GrpcClient/"]
COPY ["QuestionService.GrpcServer/QuestionService.GrpcServer.csproj", "QuestionService.GrpcServer/"]
RUN dotnet restore "QuestionService.Api/QuestionService.Api.csproj"
COPY . .
WORKDIR "/src/QuestionService.Api"
RUN dotnet build "./QuestionService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./QuestionService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuestionService.Api.dll"]
