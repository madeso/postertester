# PosterTester

Basic "postman" clone in .NET 6 in WPF (windows only).

## Current features
* No external services or call-to-home. 
* Share requests by saving project files to a shared place (shared folder, git repo, etc)
* Compare the output of 2 different requests

## Screenshots

![Main GUI showing groups, url, mehtod dropdown and failed response from localhost since the backend isn't online](data/demo.png)

![Compare dialog comparing 2 requests from 2 different groups](data/compare.png)

## Possible some time in the future
* Display more than request body in the result view
* Solarized light + dark theme
* Script tests and input "wizards" with .net?
* Timing betweeen request start and end
* Option to send compact json
* Serial and parallel attacks (needs test)
* Use output from one request as input to another
* Commandline to run tests
* Cross platform
* More features to the requests input
* Login options (auth0 etc...)
