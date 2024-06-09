# Data Parser ![CI Build](https://github.com/rexcfnghk/data-parser/actions/workflows/dotnet.yml/badge.svg)

This is a simple program that parses data files contained in the `/data` folder using formats specified in the `/specs` folder.

The whole program is written in F#.

## Dependencies

[.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)

## How to run

1. `cd ./DataParser.Console/`
2. Feed your data files under the `/data` folder and your spec files under the `/specs` folder
3. `dotnet run -c Release`
4. The resulting output will be contained under the `/output` folder

## Tests

Unit tests can be found under `./DataParser.Tests`, these can be run with:

1. `cd ./DataParser.Tests/`
2. `dotnet test`

## Docker image

A docker image can be built by using the `Dockerfile`/`docker-compose.yml` file provided in the root directory.

## Limitations

- The program will halt when any spec file cannot be parsed
  - This assumes all spec files must be in the correct formats before the program will continue processing data files
- Current error only contains partial information for troubleshooting
