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
Cosmos Manager uses a custom SQL query language that is a mixture of Cosmos and SQL syntax.

**Tip** When writing select statements the FROM must use the collection name and must match the casing used in Cosmos. The syntax parsers will read that and use it in the requests

*example:*  From Market

### SELECTS

#### SQL/Cosmos syntax
```
SELECT * FROM Market WHERE Market.id = 'test'
```

### TRANSACTIONS
Cosmos Manager supports transactions for updates and deletes. Due to restrictions on how Cosmos operates this does not work in the traditional sense like SQL Transactions.

Transactions are achieved by creating file backups of each document that is apart of the update or delete statement. When the query statement executes a transactionId is created which when used with the ROLLBACK command allows for restoration of the changed documents.

To use transactions simply place **ASTRANSACTION** at the top of the query statement being ran.

The transactionId is broken down into collectionName_date_time_guid.
```
Market_20181014_060441_02632bc6-17c3-4bd8-a97b-268b2d4dac55
```

### DELETES
Deletes can be done in 2 way: 
* By providing a list of documentIds that need to be deleted for a given collection. 
* By deleting all that fulfill a WHERE clause statement

Deletes do not use transactions by default and the proper SQL transaction syntax must be used to invoke a transaction.

#### SQL/Cosmos syntax

##### Delete by documentIds
```
 ASTRANSACTION
 DELETE 'c32ee161-8dd2-4bf3-8cff-e4eb5acd5fb6','3989a227-ae55-4945-8e73-703ce17f9f78'
 FROM Market
```

##### Delete by WHERE clause
```
ASTRANSACTION
DELETE *
FROM Market
WHERE Market.PartitionKey = 'List'
```

### ROLLBACKS
If transactions are used on a query the output will return the TransactionId and the ROLLBACK statement to use to rollback the changed or affected documents. For a rollback to work successfully the same connection, database, and collection must be used. This is done to help prevent a rollback of the wrong data to the wrong environment.

#### SQL/Cosmos syntax
```
ROLLBACK Market_20181014_060441_02632bc6-17c3-4bd8-a97b-268b2d4dac55
```

## Supported Applications
- Cosmos Emulator Required Version 1.17.x