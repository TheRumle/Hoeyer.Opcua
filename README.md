# Hoeyer.OpcUa - A small OpcUa framework
A small framework based on OpcFoundations' own framework and using reflection and source generation to create a code-first approach to defining OpcUa Nodes - the class representing the nodes are refered to as entities.
The framework is partitioned into two main parts:
1. A client project, where client services to the defined entities (nodes) are created.
2. A server part, where a basic server is spun up - this is primarily used for testing and creating small simulations to test client behaviours.

## Defining Nodes
To define a node, simply create a class and annotate it with the OpcUaAgentAttribute. Let the framework do its magic and generate client and server structure.

## Browsing an Agent
After defining an Agent class, simply inject an IAgentBrowser<MyAgentClass> where you want to use it. 

## Example
A small example has been included in the project MyOpcUaWebApplication - when starting the application, services are wired up - this includes an OpcUa server with two defined nodes. There will also be autowired a service to browse entities and one to write entities. 
The program also starts a hosted service that will 
An example showing both can be seen in ExampleHost.cs. The example show the browsing of a simple Agent and changing its values semi-randomly. 

It is recommended that another OpcUa browser software is used to verify and play around with the library. 


# Testing 
The project uses TUnit - a currently experimental framework where tests are generated on compiletime. This makes test startup times insanely fast, and any test that does not rely on async operations very fast too. As the framework changes so will this project. 
