# PosterTester

Basic "postman" clone in .NET 6 in WPF (windows only).

Warning: Still in development, stored data might be destroyed after a update

## Current features
* No external services or call-to-home. 
* Share requests by saving project files to a shared place (shared folder, git repo, etc)
* Compare the output of 2 different requests

## Screenshots

![Main GUI showing groups, url, mehtod dropdown and failed response from localhost since the backend isn't online](data/demo.png)

![Compare dialog comparing 2 requests from 2 different groups](data/compare.png)

## Possible some time in the future (not necessarily in a specifc order)
* Option to send compact json
* Uri editor
* Use output from one request as input to another
* Script tests and input "wizards" with .net?
* Commandline to run tests
* Solarized light + dark theme
* Cross platform
* More features to the requests input
* Login options (auth0 etc...)
