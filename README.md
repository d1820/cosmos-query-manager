
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

![](.\content\CMNewQueryFile.png)
- From the File List View

![](.\content\CMNewQueryFileList.png)
- From double clicking in the tabs bar

![](.\content\CMNewQueryDoubleClickTabs.png)


## Working with existing query files
YOu can open any directory folder to load all the .cSQL files. This also loads all children folders and files as well. This provides the easy of working across many files, but still being able to organize them.

## Loading Connections
To create the connections file view the Help Guide.
![](.\content\CosmosManagerConnec.jpg)

### Changing connections
In the query options toolbar on the far right is a dropdown to change the connection string of where the query gets executed. Each connection string will color the current tab to a set color. This helps in keeping track of what environment a query has been ran against at a quick glance.

![](.\content\CMConnectionChange.png)


## Query Options

![](.\content\CMQueryOptions.png)

- ![](.\content\CMBeautify.png) Formats the query statement
- ![](.\content\CMWordWrap.png) Controls word wrap of the query statement
- ![](.\content\CMFontSize.png) Changes the font size of the query statement
- ![](.\content\CMSave.png) Saves the query statement
- ![](.\content\CMRunQuery.png) Runs the query statement

## Result Options
On selects we have options available to manipulate the results list

![](.\content\CMResults.png)

- ![](.\content\CMUpdateSelected.png) Send all the selected documents to a New Query Update statement

- ![](.\content\CmDeleteSelected.png) Send all the selected documents to a New Query Delete statement


## Single Document Options

![](.\content\CMDocumentOptions.png)

- ![](.\content\CMBeautify.png) Formats the document
- ![](.\content\CMWordWrap.png) Controls word wrap of the document
- ![](.\content\CMFontSize.png) Changes the font size of the document
- ![](.\content\CMExport.png) Allows to export the single document or all the document results
- ![](.\content\CMDeleteDocument.png) Delete the document
- ![](.\content\CMSave.png) Save the document

## Query Output
In the output tab we can see all information related to the executed query. On selects we output all the cosmos metrics and costs, on other queries information related to the success of the query are displayed.

![](.\content\CMQueryOutput.png)

## Transaction Cache
The transaction cache is the storage location where rollback files are stored. The application never deletes from this folder so there is always a rollback 
history available as a data safety precaution. As this folder grows with backup data for transactions it may need to be cleaned out or have the files archived somewhere.