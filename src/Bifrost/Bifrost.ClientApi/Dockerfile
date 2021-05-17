FROM microsoft/aspnetcore:2.0.0-jessie
WORKDIR /app
COPY out .
EXPOSE 80
ENTRYPOINT ["dotnet", "Bifrost.ClientAPI.dll"]
