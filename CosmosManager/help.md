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

**Note:** Deletes do not use transactions by default and the proper SQL transaction syntax must be used to invoke a transaction.

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


### INSERTS
Two types of inserts can be preformed. Single record or and array of records. All insert data must be valid JSON and be able to properly be parsed.

#### SQL/Cosmos syntax
#### Single Document
```
INSERT
{
    "id": "one",
    "PartitionKey": "TestKey",
    "LastModifiedOn": "0001-01-01T00:00:00-07:00",
    "LastModifiedBy": null,
    "CreatedOn": "2018-02-28T16:35:11.1404236-07:00",
    "CreatedBy": null
}
INTO Market
```

#### Multiple Documents
```
INSERT
[{
    "id": "one",
    "PartitionKey": "TestKey",
    "LastModifiedOn": "0001-01-01T00:00:00-07:00",
    "LastModifiedBy": null,
    "CreatedOn": "2018-02-28T16:35:11.1404236-07:00",
    "CreatedBy": null
},
{
    "id": "two",
    "PartitionKey": "TestKey",
    "LastModifiedOn": "0001-01-01T00:00:00-07:00",
    "LastModifiedBy": null,
    "CreatedOn": "2018-02-28T16:35:11.1404236-07:00",
    "CreatedBy": null
}]
INTO Market
```

### Updates
Updates can be done in 2 way:
* By providing a list of documentIds that need to be updated for a given collection.
* By updating all that fulfill a WHERE clause statement

#### Updating an entire document
To update an entire document the Id of the document must be provided in the update statement. 
When doing a full document replace only **one** document can be updated at a time.


##### SQL/Cosmos syntax
```
ASTransaction
UPDATE '14e42d8c-7583-432f-8dd0-d80e699ef41f'
from Marketplace
REPLACE {
    "id": "14e42d8c-7583-432f-8dd0-d80e699ef41f",
    "PartitionKey": "List",
    "Name": {
        "Key": "NewNameKey",
        "Text": "New Name"
    }
}
```

#### Updating a portion of a document
To update a part of the document the SET keyword is used. 
This does an explicit merge of the new structure to the existing documents structure. 
This means:
- Properties provided in the SET that do not exist in the document currently will be added to the document. 
- Properties in the SET that have a **NULL** value will be added to the existing document as a **NULL** value property
- If an array of items is included in the SET will be merged to existing item in the document. 
This is based on the index of the array item. So items in the SET **MUST** be in the same order as the current document else data may get corrupt or out of sync.


**Note:** When doing a partial update any attempt to change the "id" or "PartitionKey" of the document will throw an error.

##### SQL/Cosmos syntax
**ORIGINAL**
```
{
    "Name": {
        "Key": "FirstNameKey",
        "Text": "First Name"
    }
}
```

**QUERY**
```
ASTransaction
UPDATE '14e42d8c-7583-432f-8dd0-d80e699ef41f'
from Marketplace
SET {
    "Name": {
        "Key": "NewNameKey",
        "Text": "New Name"
    },
    "Address": null
}
```

**MERGED RESULT**
```
{
    "Name": {
        "Key": "NewNameKey",
        "Text": "New Name"
    }
    "Address" :null
}
```

**Note:** Updates do not use transactions by default and the proper SQL transaction syntax must be used to invoke a transaction.


### Multi-Statement Queries
Cosmos Manager supports being able to run a group of queries at once. 
These query statements are ran synchronously and in the order written in the query window. 
Each statement must be terminated with a semi-colon **(;)**

While it is not currently possible to have the results of one query fill in the data source to the next query this functionality is on the road map for the future.

##### SQL/Cosmos syntax
```
SELECT *
FROM Market
WHERE Market.PartitionKey = 'List'
;

ASTRANSACTION
UPDATE 'Db71b8bf-2b51-4ed1-9dd6-724706a099e0'
FROM Market
SET
{
  "WalletId": null,
  "Phone": {
    "Number": "555-555-5555"
  }
}
;

DELETE 'test'
FROM Market
WHERE Market.PartitionKey = 'List'

```

## Supported Applications
- Cosmos Emulator Required Version 1.17.x. This requirement is due to the coupling of DocumentDB Nuget packages to the emulator installed locally. If you do not use the emulator this is not a requirement of using the application.