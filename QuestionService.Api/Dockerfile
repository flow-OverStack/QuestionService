FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["QuestionService.Api/QuestionService.Api.csproj", "QuestionService.Api/"]
COPY ["QuestionService.DAL/QuestionService.DAL.csproj", "QuestionService.DAL/"]
COPY ["QuestionService.Domain/QuestionService.Domain.csproj", "QuestionService.Domain/"]
COPY ["QuestionService.Outbox/QuestionService.Outbox.csproj", "QuestionService.Outbox/"]
COPY ["QuestionService.Grpc/QuestionService.Grpc.csproj", "QuestionService.Grpc/"]
COPY ["QuestionService.Application/QuestionService.Application.csproj", "QuestionService.Application/"]
COPY ["QuestionService.ReputationProducer/QuestionService.ReputationProducer.csproj", "QuestionService.ReputationProducer/"]
RUN dotnet restore "QuestionService.Api/QuestionService.Api.csproj"
COPY . .
WORKDIR "/src/QuestionService.Api"
RUN dotnet build "QuestionService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "QuestionService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuestionService.Api.dll"]
