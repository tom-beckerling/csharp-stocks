
## Front End
A simple Console App that displays stock price model data.
Data is fetched from the backend server, over http requests.
A built in timer refreshes the data request every 1 second (http polling)
The data is displayed on the console for example
$ Microsoft (MSFT) $110.1
  Apple (APL) $170.1


## Back End
A simple http server, that provides stock price model data at a localhost address
Stock price model data is generated randomly on request

### Stock price model
Name
Price

# App architecture
single app.
seperate threads for back end and front end.
written in C#
use only .NET CORE framework


