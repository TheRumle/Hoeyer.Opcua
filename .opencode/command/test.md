---
description: Run the Hoeyer unit test suite.
agent: build
---

# Test framework
Tests use TUnit (compile-time generated).

# Test failure reporting 
Report pass/fail counts and any failures, then fix failures if they are caused by your own changes. 

## Unit test
Run `dotnet test Hoeyer.UnitTests.slnf` from the solution root (C:\Users\rasmus\RiderProjects\Hoeyer.Opcua) to test unit test.

## Integration test - local environment
Run `dotnet test dotnet test Hoeyer.IntegrationTests.slnf` to run intergration tests on a local environment. 

## Playground test - dockerized 
Before running, ensure that docker is running using `docker ps`. To run tests use `dotnet test Hoeyer.ContainerizedTests.slnf`. The tests starts multiple containers.   