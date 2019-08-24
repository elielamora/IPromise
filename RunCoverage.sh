#!/bin/bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
reportgenerator "-reports:IPromise.Tests/coverage.opencover.xml" "-targetdir:.coverage"