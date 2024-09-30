The portal api just aggregates data from other apis.  

PxWeb 2 runs on the users device. So the calls to the apis are made from the device.
The links in the respones from the portal api will point back to the portal api for folders i.e. the menu, but for other things like table data it will point directly to the underlying api.   

Inside one organisation, some data may be in the cloud and some on owned servers. 
```mermaid
  flowchart TB
    subgraph NSI
      subgraph GUI
        User[Users PC/Phone]:::guiStyle
        NSIPXWeb[PXWeb GUI]:::guiStyle
        User <--> NSIPXWeb
        
      end
        NSIPortalApi[Portal API]
        User <-- "navigation and search" --> NSIPortalApi
      
      Div1Api[API with data in the cloud]
      User <-- "specific item like table or codelist" --> Div1Api
      NSIPortalApi  <-- "navigation and search" -->  Div1Api

      Div2Api[API with data on owned servers]
      User <-- "specific item like table or codelist" --> Div2Api
      NSIPortalApi  <-- "navigation and search" -->  Div2Api
      
    end  
      classDef guiStyle fill:green,stroke:black    
```

Another senario is: The government want a site with data from many organisations. The organisations will have separate sites with their own PxWeb

```mermaid
flowchart TB
User[Users PC/Phone]:::guiStyle
   subgraph Goverment Portal site
   PortalPXWeb[PXWeb GUI]:::guiStyle
   PortalPXWebApi[PXWeb Api Portal]
   end 
   
   subgraph Site Organisation1 
   SSBPXWeb[PXWeb GUI]:::guiStyle
   SSBPXWebApi[PXWeb Api]
   SSBBackend[CNMM]
   end
   
   subgraph Site Organisation2 
   SCBPXWeb[PXWeb GUI]:::guiStyle
   SCBPXWebApi[PXWeb Api]
   SCBBackend{{PX-files}}
   end

   User <--> PortalPXWebApi
   
   User <--> PortalPXWeb

   User <--> SSBPXWeb
   User --> SSBPXWebApi
   
   PortalPXWebApi --> SSBPXWebApi
   PortalPXWebApi --> SCBPXWebApi
   User <--> SCBPXWeb
   User <--> SCBPXWebApi
   classDef guiStyle fill:green,stroke:black   
```
      
       
     
