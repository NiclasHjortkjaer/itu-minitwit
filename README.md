# itu-minitwit

### <a href="http://143.244.205.161">Live at 143.244.205.161</a>

## Run with docker-compose

To run the program with Docker use the following command:

```
% docker-compose up
```

MiniTwit will run on: <a href="http://localhost:8765">`http://localhost:8765`</a>

## Deploy to DigitalOcean with Vagrant
To use an already existing remote postgres database instance create a file called "db_ip.txt" containing its IP in the root directory. <br>
You must have set a repository up on docker hub called "itu-minitwit-jason" <br>
You must have generated a ssh key called ~/.ssh/do_ssh_key which is your ssh key on Digital Ocean

```
% export SSH_KEY_NAME=<Name of ssh key on DO>
% export DIGITAL_OCEAN_TOKEN=<DO API access token>
% export POSTGRES_PASSWORD=<Password for database>
% export DB_PORT=<Port for database>
% export DB_USER=<Username for database>
% export DOCKER_USERNAME=<Username on docker hub>
% export DOCKER_PASSWORD=<Access token for docker hub>
% export LOKI_HOST=<Host for loki logging server>
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
