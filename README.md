# PxWeb Api[![Mentioned in Awesome Official Statistics ](https://awesome.re/mentioned-badge.svg)](http://www.awesomeofficialstatistics.org)
This is the official source code repository for PxWebApi. PxWeb is a nice web application for dissemination of statistical tables please read more abou it at the official web page on Statistics Sweden web site at [www.scb.se/px-web](https://www.scb.se/px-web).

## Current activities
We are currently developing PxWebApi 2.0

## Development notes

```sh
curl -i -H "API_ADMIN_KEY: test" -X 'PUT'  https://localhost:5001/api/v2/admin/database
curl -i -H "API_ADMIN_KEY: test" -X 'POST' https://localhost:5001/api/v2/admin/searchindex
curl -i -H "API_ADMIN_KEY: test" -X 'PATCH' -H 'Content-Type: application/json' -d '["TAB001", "TAB004"]' https://localhost:5001/api/v2/admin/searchindex
```
