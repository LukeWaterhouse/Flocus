dotnet tool install -g coverlet.console
dotnet tool install -g dotnet-reportgenerator-globaltool
@RD /S /Q "./Coverage"

cd..
dotnet test --collect:"XPlat Code Coverage" --results-directory:"./Scripts/Coverage/CoverageResults"
reportgenerator -reports:"./Scripts/Coverage/CoverageResults/**/coverage.cobertura.xml" -targetdir:"./Scripts/Coverage/CoverageReport" -reporttypes:Html
start "" "./Scripts/Coverage/CoverageReport/index.html"

