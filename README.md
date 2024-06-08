# Data Parser

This is a simple program that parses data files contained in the `/data` folder using formats specified in the `/specs` folder.

The whole program is written in F#.

## Dependencies

[.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)

## How to run

1. `cd ./DataParser.Console/`
2. `dotnet run`

## Tests

Unit tests can be found under `./DataParser.Tests`, these can be run with:

1. `cd ./DataParser.Tests/`
2. `dotnet test`

## Docker image

A docker image can be built by using the `Dockerfile`/`docker-compose.yml` file provided in the root directory.