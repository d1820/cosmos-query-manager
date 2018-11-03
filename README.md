
# Cosmos Query Manager for Azure DocumentDB

Cosmos Query Manager is a Windows based query manager for Azure DocumentDb/Cosmos DB

* Develop, run, edit your DocumentDb/Cosmos DB SQL commands
* Add, edit and delete documents
* Work with multiple database

# Install

If you want to install the application please grab the exe installer file from the releases folder in the project source

# Code base

This is a C# application, requiring VS2017+

# Application Walk-through


## Create a new Query Tab
There are 3 ways to create a new query tab
- From File select

![](/Content/CMNewQueryFile.png)
- From the File List View

![](/Content/CMNewQueryFileList.png)
- From double clicking in the tabs bar

![](/Content/CMNewQueryDoubleClickTabs.png)


## Working with existing query files
YOu can open any directory folder to load all the .cSQL files. This also loads all children folders and files as well. This provides the easy of working across many files, but still being able to organize them.

## Loading Connections
To create the connections file view the Help Guide.
![](/Content\CosmosManagerConnec.jpg)

### Changing connections
In the query options toolbar on the far right is a dropdown to change the connection string of where the query gets executed. Each connection string will color the current tab to a set color. This helps in keeping track of what environment a query has been ran against at a quick glance.

![](/Content/CMConnectionChange.png)


## Query Options

![](/Ccontent/CMQueryOptions.png)

- ![](/Content/CMBeautify.png) Formats the query statement
- ![](/Content/CMWordWrap.png) Controls word wrap of the query statement
- ![](/Content/CMFontSize.png) Changes the font size of the query statement
- ![](/Content/CMSave.png) Saves the query statement
- ![](/Content/CMRunQuery.png) Runs the query statement

## Result Options
On selects we have options available to manipulate the results list

![](/Content/CMResults.png)

- ![](/Content/CMUpdateSelected.png) Send all the selected documents to a New Query Update statement

- ![](/Content\CmDeleteSelected.png) Send all the selected documents to a New Query Delete statement


## Single Document Options

![](.\content/CMDocumentOptions.png)

- ![](/Content/CMBeautify.png) Formats the document
- ![](/Content/CMWordWrap.png) Controls word wrap of the document
- ![](/Content/CMFontSize.png) Changes the font size of the document
- ![](/Content/CMExport.png) Allows to export the single document or all the document results
- ![](/Content/CMDeleteDocument.png) Delete the document
- ![](/Content/CMSave.png) Save the document

## Query Output
In the output tab we can see all information related to the executed query. On selects we output all the cosmos metrics and costs, on other queries information related to the success of the query are displayed.

![](/Content/CMQueryOutput.png)

## Transaction Cache
The transaction cache is the storage location where rollback files are stored. The application never deletes from this folder so there is always a rollback 
history available as a data safety precaution. As this folder grows with backup data for transactions it may need to be cleaned out or have the files archived somewhere.
