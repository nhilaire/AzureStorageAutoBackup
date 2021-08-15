# AzureStorageAutoBackup

## Backup your directories to Azure File Share

Do you have an Azure Account ? Do you want to backup some directories of your hard drives ?

I use this C# tool and the Azure File Share Api to backup my directories.

With the use of md5 checksum, I upload only what has changed or is new.

## Configuration

Use appsettings.json to set the list of included and excluded paths.

ConnectionString should be the connection string of azure file share founded in the azure portal. ShareReference is the name of the file share. DestinationPathInAzure is a directory in the file share, because I use the same file share for multiple computers.

```
{
  "AppConfiguration": {
    "Paths": [ "C:\\Path\\To\\Backup" ],
    "ExcludedPaths": [ "C:\\Path\\To\\Backup\\ExcludeThisOne" ],
    "DestinationPathInAzure": "backup",
    "ConnectionString": "Put File Share Connection String",
    "ShareReference": "fileshare"
  }
}
```
