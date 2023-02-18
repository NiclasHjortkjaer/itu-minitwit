# itu-minitwit

### <a href="http://143.244.205.161">Live at 143.244.205.161</a>

## Run with docker-compose

To run the program in Docker use the following command:

```
% docker-compose up
```

MiniTwit will run on: <a href="http://localhost:8765">`http://localhost:8765`</a>

## Deploy to DigitalOcean with vagrant

```
% export SSH_KEY_NAME=<Name of ssh key on DO>
% export DIGITAL_OCEAN_TOKEN=<DO API access token>
% export POSTGRES_PASSWORD=<Password for database>
% export DB_IP=<IP for existing database> (!ONLY IF DATABASE ALREADY EXISTS!) 
% vagrant up
```
## Run for development

```
% docker run --name minitwitdb -e POSTGRES_PASSWORD=postgres -p 2345:5432 -d postgres
```
(^^^ or run docker-compose instead ^^^) <br />
then:
```
% cd Minitwit
% dotnet run
```
(^^^ Or select launch configuration with Rider/Visual Studio ^^^)