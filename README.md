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

## Point of interests
* Using azure storage api via HTTPS (instead of using smb volume, because port 445 can be blocked)
* Files no more presents in hard drive but existing on storage are deleted (synchronisation)
* I'm using my own metadata on files instead of using existing MD5 property because I've seen the MD5 calculation changed on storage, so all files needs to be reuploaded ...
* DeleteEmptyDirectoriesIfExist can probably be optimized to avoid browsing 2 times the storage
* Before, I use the md5.exe tool to improve the performance of MD5 calculation. But I don't want to use an external tool anymore. So now, MD5 calculation is slow. This should be improved too.
* I'm using Windows, so the namimg convention is based on windows (c:\...), but this can be easily adapted to work on other systems