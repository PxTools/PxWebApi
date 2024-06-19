# The health endpoints
There are 2 health endpoints:
```
/health/ready
/health/alive
```

Their usefullness depend on how Pxwebapi is installed. 
If you have a single pxwebapi instance and install it as a package,
 you will not find them very usefull. 

## Ready
The purpose of the ready endpoint is to be the target of loadbalancer probes.

It returns 503 if the node is being stopped for maintainence, 200 otherwise.

Technically: It returns 200 if there exist a file called yes.json 
             in wwwroot/health/ready  , 503 otherwise.

## Alive
The purpose of the alive endpoint is to show which increment of pxwebapi
 is running.

It typically returns some details of how this increment was made, i.e. tags of source and config repos
 
Technically: It returns the current content of the file alive.json 
             in wwwroot/health/alive. The content of alive.json need not be json.


			 
			 
