# Finding a bug

Team D introduced a bug, which hardcoded the username when someone was trying to log in, such that the login would fail unless you were that exact user.

Team O discovered the bug by going to our Kibana Discover section. Here we could look at the Kibana Query "fields.error : * ". This way we could see errors popping up when someone tried to register their user. We looked through our /register endpoint, but spend a lot of time not finding the error. We then noticed that the endpoint described in Kibana was actually /Login, and was then able to find the bug. 

The logging message was misleading, because it referenced a wrong endpoint being called. This made us aware of how careful we have to be when writing code in general, but also when trying to provide useful information to others (like via logging).