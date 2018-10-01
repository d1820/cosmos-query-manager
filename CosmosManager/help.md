# Cosmos Manager Help Guide

## Setting up connections
Create a .json file that contains this structure for storing all of your cosmos connections. This is a user owned file so ConnectionKeys and Endpoints stay within the users environment.

```
[
	{
		"Name": "Local Cosmos",
		"EndpointUrl": "https://localhost:8081/",
		"ConnectionKey": "KEY",
		"Database": "DBNAME"
	}
]
```
## Query files
Cosmos Manager uses a custom sql query language that is a mixture of Cosmos and SQL and Razor syntax.

**Tip** When writing select statements the FROM must use the collection name that you wish to query against. The syntax parsers will read that and use it in the requests

```
SELECT * FROM CollectionName WHERE CollectionName.id = 'test'
```

## Supported Applications
- Cosmos Emulator Required Version 1.17.x