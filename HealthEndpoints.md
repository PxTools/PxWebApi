# The health endpoints
There are 2 health endpoints:
```
/health/ready
/health/alive
```

Their usefullness depend on how Pxwebapi is installed. 
If you have a single pxwebapi instance and install it as a package,
 you will not find them very usefull. 

Their intended use is if you run Pxwebapi as Docker in a kubernetes cluster or 
have more than one server behind a loadbalancer.

For the later usecase there are 2 more:
```
/admin/MarkForShutdown
/admin/MarkForShutdownUndo
```
Which allows you to tell one of your servers to tell the loadbalancer 
it needs to stop for maintainence.

## Ready
The purpose of the ready endpoint is to be the target of loadbalancer probes.

It returns 503 if the node is being stopped for maintainence or 
the application has problems reaching this external dependencies, 200 otherwise.

Technically: MarkForShutdown sets a flag in ApplicationState which is read by this endpoint.
It may take some seconds to have effect.           
			 
Todo: if datasource is CNMM then a simple db connection test is made.

## Alive
The purpose of the alive endpoint is to show which increment of pxwebapi
 is running.

It typically returns some details of how this increment was made, i.e. tags of source and config repos
 
Technically: It returns the current content of the file alive.json 
             in wwwroot/health/alive. The content of alive.json need not be json.
			 When building the application overwrite the alive.json in the package/artefact with the info you find usefull



			 
			 
