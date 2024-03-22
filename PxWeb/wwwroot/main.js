function main() {
  console.log("runes-cors-test");

  const fetchPromise = fetch(
    //"https://pxapi2.staging-bip-app.ssb.no/api/v2/tables/TAB004/data?outputFormat=json-stat2",
    "https://localhost:5001/api/v2/tables/TAB004/data?outputFormat=json-stat2",

    {
      method: "POST",
      mode: "cors",
      headers: {
        "Content-Type": "application/json",
      },
      body: "{foo: bar}",
    }
  );

  fetchPromise.then((response) => {
    console.log(response.status);
  });
}
/*

curl -I -X OPTIONS -H "Origin: http://localhost:3000" -H 'Access-Control-Request-Method: POST' -H "Access-Control-Request-Headers: Content-Type" "http://localhost:43071/api/v2/tables/TAB004/data?outputFormat=json-stat2"
curl -I -X OPTIONS -H "Origin: http://localhost:3000" -H 'Access-Control-Request-Method: GET' "http://localhost:43071/api/v2/tables/TAB004/data?outputFormat=json-stat2"
curl -I -X OPTIONS -H "Origin: http://localhost:3000" -H 'Access-Control-Request-Method: POST' "http://localhost:43071/api/v2/tables/TAB004/data?outputFormat=json-stat2"
curl -I -X OPTIONS -H "Origin: http://localhost:3000" -H 'Access-Control-Request-Method: POST' "https://pxapi2.staging-bip-app.ssb.no/api/v2/tables/TAB004/data?outputFormat=json-stat2"

curl -I -X OPTIONS -H "Origin: http://localhost:3000" "http://localhost:43071/api/v2/tables/TAB004/data?outputFormat=json-stat2"
curl -I -X OPTIONS -H "Origin: http://localhost:3000" "https://pxapi2.staging-bip-app.ssb.no/api/v2/tables/TAB004/data?outputFormat=json-stat2"

*/
