# itu-minitwit

## Run with Docker

To run the program in Docker, the program is run in production, and requires a SSL certificate. You can generate your own self-signed or buy one.

The program will look for a path and key to the certificate, which you must specify in `itu-minitwit/Server/appsettings.Production.json`
```
{
 ...
  "IdentityServer": {
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

### Generating a self-signed certificate

```
$ openssl genrsa 2048 > server_private.pem
$ openssl req -x509 -days 1000 -new -key server_private.pem -out server_public.pem
$ openssl pkcs12 -export -in server_public.pem -inkey server_private.pem -out server.pfx
```

Note that this certificate will expire after 1000 days. 

Taken from: https://stackoverflow.com/a/61990527
