@echo off

cd D:\nats-server
start cmd /k nats-server.exe

cd D:\nginx
start cmd /k nginx.exe

cd D:\VOLGATECH\6semestr\distributed-programming

cd ./RankCalculator
start cmd /k dotnet run

cd ../Valuator
start cmd /k dotnet run --urls "http://0.0.0.0:5001"
start cmd /k dotnet run --urls "http://0.0.0.0:5002"

