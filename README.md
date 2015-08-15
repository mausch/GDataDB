# GDataDB

GDataDB is a database-like interface to Google Spreadsheets for .Net.<br/>
It models spreadsheets as databases, worksheets as database tables, and rows as records.

[NuGet package here.](https://www.nuget.org/packages/GDataDB/)

A few articles documenting it:

* http://bugsquash.blogspot.co.uk/search/label/gdatadb
* http://ryanfarley.com/blog/archive/2010/09/24/using-google-docs-as-an-online-database.aspx
 
Google has changed the authentication scheme since those articles were written. Now it requires OAuth2. <br/>
To set this up visit http://console.developers.google.com , create a new project, enable the Drive API, create a new client ID of type "service account" and download the P12 key. Use the service account email address and the P12 key [as shown in this example](https://github.com/mausch/GDataDB/blob/master/GDataDB.Tests/IntegrationTests.cs#L27-L29) .<br/>
If you want to access any documents from your personal gmail account, share them (edit permissions) with the service account email address (the one that looks like "`987191231-asdfkasjdjfk@developer.gserviceaccount.com`") and make sure that they're in the root folder of Drive.

Original idea for GDataDB borrowed from https://github.com/google/gdata-python-client/blob/master/src/gdata/spreadsheet/text_db.py

[![Please donate](http://www.pledgie.com/campaigns/11248.png)](http://www.pledgie.com/campaigns/11248)
