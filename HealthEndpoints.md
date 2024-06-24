# The health endpoints
These are technical endpoints of no use to somebody looking for facts.
  
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
/admin/enter-maintance-mode
/admin/exit-maintance-mode
```
Which allows you to tell one of your servers to tell the loadbalancer 
it needs to stop for maintainence.

## Ready
The purpose of the ready endpoint is to be the target of loadbalancer or kubernetes probes.

It returns 503 if the node is being stopped for maintainence or 
the application has problems reaching this external dependencies, 200 otherwise.

Technically: Enter/ExitMaintanceMode sets a flag in ApplicationState which is read by this endpoint.
It may take some seconds to have effect. 
			 
Todo: if datasource is CNMM then a simple db connection test is made.

## Alive
The purpose of the alive endpoint is to provide for kubernetes 
It returns 200 (with something like "yes" in body)



			 
			 
