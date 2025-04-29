# Team Software Engineering 2024 - Feedback Tracker
**Note: Outside contributions will not be considered for this repository**

## What is this?
This is a university project, and is a feedback tracker.
Students can give feedback on teaching or teaching materials to teachers/module teams who can review and react to given feedback as nessicary.


## How do I build this
Please install Visual Studio 2022 with ASP.NET and Blazor Workloads installed, after you should be able build and use feedback tracker.

You will need to supply a .ENV file in the root of the server project with the following fields filled
```
ResetEmail="Email Address"
ResetEmailPassword="Email password"
JWTSecret="Generated JWT Secret Key"
SQLString="Server=MyServer;Database=MyDataBase;User ID=MyAccount;Password=MyPassword"
JWTEndpoint="ServerURL"
```
