dotnet tool install -g coverlet.console
dotnet tool install -g dotnet-reportgenerator-globaltool
@RD /S /Q "./Coverage"

cd..
dotnet test --collect:"XPlat Code Coverage" --results-directory:"./DevelopmentScripts/Coverage/CoverageResults"
reportgenerator -reports:"./DevelopmentScripts/Coverage/CoverageResults/**/coverage.cobertura.xml" -targetdir:"./DevelopmentScripts/Coverage/CoverageReport" -reporttypes:Html
start "" "./DevelopmentScripts/Coverage/CoverageReport/index.html"

