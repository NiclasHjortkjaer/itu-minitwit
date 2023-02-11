# itu-minitwit

## Run with Docker 

To run the program with Docker it is necesarry to add your own SSL Certificate (possibly self-signed). Possibly to the directory itu-minitwit/Server.

Then modify itu-minitwit/Server/appsettings.json to contain the following section:

```
{
 ...
  "IdentityServer": {
    "Clients": {
      "itu_minitwit.Client": {
        "Profile": "IdentityServerSPA"
      }
    },
    "Key": {
      "Type": "File",
      "FilePath": "<path-to-certificate>",
      "Password": "<password-to-certificate>"
    }
 },
 ...
```

Now, to run the program in a Docker container do: 

```
docker compose up
```
