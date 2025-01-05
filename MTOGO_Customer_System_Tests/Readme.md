# Test plan for MTOGO exam project

## Test strategy

Each system should have its own test project attached to it containing unit tests and integration tests.

Moq will be used for unit testing.

A separate database will be used for integration testing. This database will be dropped and restored whenever the test suite is executed, to ensure consistency among tests.

1 test project will handle system testing, where the goal is to verify that the system works as a whole thus meeting the specified requirements and functionality.

Acceptance tests will be tested through the user stories using postman.

## Scope
The customer system will be tested but testing the other projects should be done in a similar fashion.

The only exception would be the gateway API as its sole purpose is to redirect requests to other services. As such this testing project should only have integration tests. 

## Testing approach

- Unit tests

Use MOQ to validate a succesfull and unsuccesfull action for each function.

- Integration tests

Use test database to validate a succesfull and unsuccesfull CRUD action for each function.

- System tests

Use test database to validate a succesfull interaction between projects for each API endpoint.

- Acceptance tests

Will be tested through the user stories that already defines the criteria for the story. Use postman.

## Test coverage
The goal is to have an overall test coverage of +80%. Reports will be generated with the tools that is installed by default when creating an xunit test project. Using the following commands.

dotnet test --collect:"XPlat Code Coverage"

Creates a folder containing an xml file, that can then be turned into an html file with the following command.

reportgenerator -reports:".\TestResults\38fe755b-1d88-4e80-8414-d27c4008abed\coverage.cobertura.xml" -targetdir:"coverageresults" -reporttypes: Html

The report can be found here.

