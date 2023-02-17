# itu-minitwit

## Run with Docker

To run the program in Docker use the following command:

```
docker compose up
```

The program will run on:
```
http://localhost:8765
```

## Deploy to DigitalOcean with vagrant

```
% export SSH_KEY_NAME=<Name of ssh key on DO>
% export DIGITAL_OCEAN_TOKEN=<DO API access token>
% export POSTGRES_PASSWORD=<Password for database>
% vagrant up
```