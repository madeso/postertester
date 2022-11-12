# PosterTester

Basic "postman" clone in .NET 6 in WPF (windows only).

Warning: Still in development, stored data might be destroyed after a update

## Current features
* No external services or call-to-home. 
* Share requests by saving project files to a shared place (shared folder, git repo, etc)
* Compare the output of 2 different requests

## Screenshots

![Main GUI showing groups, url, mehtod dropdown and failed response from localhost since the backend isn't online, response headers are hidden](data/demo.png)
Main GUI showing groups, url, mehtod dropdown and failed response from localhost since the backend isn't online. Response headers are hidden

![Compare dialog comparing 2 requests from 2 different groups](data/compare.png)
Compare dialog comparing 2 requests from 2 different groups


![Histogram showing the response time of 50 attacks](data/attack-single.png)
"Attack" a endpoint and display the response times.

![A histogram showing 2 distinct response times. A blueish that never goes about 2.5 seconds and a redish that never goes below 5 seconds. The actual endooints are blured.](data/attack-compare.png)
Compare the response time for 2 requests. Does 2 endpoins take the same time to "call"? Duplicate one request and test out some optimization? Your imagination is the limit!


## Possible some time in the future (not necessarily in a specifc order)
* Option to send compact json
* Uri editor
* Use output from one request as input to another
* Script tests and input "wizards" with .net
* Commandline to run tests
* Solarized light + dark theme
* Cross platform
* More features to the requests input
* Login options (auth0 etc...)
