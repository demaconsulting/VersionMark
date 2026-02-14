@echo off
REM Build and test VersionMark (Windows)

echo Building VersionMark...
dotnet build --configuration Release
if %errorlevel% neq 0 exit /b %errorlevel%

echo Running unit tests...
dotnet test --configuration Release
if %errorlevel% neq 0 exit /b %errorlevel%

echo Running self-validation...
dotnet run --project src/DemaConsulting.VersionMark --configuration Release --framework net10.0 --no-build -- --validate
if %errorlevel% neq 0 exit /b %errorlevel%

echo Build, tests, and validation completed successfully!
