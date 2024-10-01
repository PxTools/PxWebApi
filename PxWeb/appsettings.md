# fields in appsettings.json
The fields baseUrl and routePrefix should be
so that f.x. the url to the config endpoint is baseUrl + routePrefix + "/config"
Neither should end with a "/". If not empty routePrefix should start with a "/"    
The baseUrl should hold the scheme (https normally) + the host + on IIS the virutal Path to the application if any.
