# TechTask

Steps to run application:

1. Clone repository
2. Open solution in Visual Studio
3. Run application
4. I added Swagger so testing will be easier

There is 2 options to run application in DEBUG or RELEASE mode.

If you will run in DEBUG then after first execution, best stories will cache for a week,
if you will run in RELEASE then each execution will call HackerNews API

I implemented Throttling ("efficiently service large numbers of requests without risking overloading") via synchronization of threads and simply counting requests.
It's bad if the amount of users will be huge, since the last users will wait tremendous amount of time.

To fix that issue I could count total entities to retrive from HackerNews API and if that limit did not surpass HackerNews limit we could share it with another user.
Or simply use other Servers.

Also I could store HackerNews data in database instead of memory cache, if we know that data would rarely update

I could also use Redis instead of MemoryCache, if I would have more servers.