export DIGITALOCEAN_PRIVATE_NETWORKING=true
export DROPLETS_API="https://api.digitalocean.com/v2/droplets"
export BEARER_AUTH_TOKEN="Authorization: Bearer $DIGITAL_OCEAN_TOKEN"
export JSON_CONTENT="Content-Type: application/json"

echo "creating manager"
export MANAGER_ID="$(curl -X POST $DROPLETS_API\
       -d'{"name":"manager","tags":["swarm"],"region":"fra1",
       "size":"s-1vcpu-1gb","image":"docker-20-04",
       "ssh_keys":["ed:f0:8f:a6:e3:30:fb:1b:91:30:8e:47:cb:01:a6:fa"]}'\
       -H "$BEARER_AUTH_TOKEN" -H "$JSON_CONTENT"\
       | jq -r .droplet.id )"\
       && sleep 3 && echo "$MANAGER_ID"

echo "creating worker1"
export WORKER1_ID="$(curl -X POST $DROPLETS_API\
       -d'{"name":"worker1","tags":["swarm"],"region":"fra1",
       "size":"s-1vcpu-1gb","image":"docker-20-04",
       "ssh_keys":["ed:f0:8f:a6:e3:30:fb:1b:91:30:8e:47:cb:01:a6:fa"]}'\
       -H "$BEARER_AUTH_TOKEN" -H "$JSON_CONTENT"\
       | jq -r .droplet.id )"\
       && sleep 3 && echo "$WORKER1_ID"

echo "creating worker2"
export WORKER2_ID="$(curl -X POST $DROPLETS_API\
       -d'{"name":"worker2","tags":["swarm"],"region":"fra1",
       "size":"s-1vcpu-1gb","image":"docker-20-04",
       "ssh_keys":["ed:f0:8f:a6:e3:30:fb:1b:91:30:8e:47:cb:01:a6:fa"]}'\
       -H "$BEARER_AUTH_TOKEN" -H "$JSON_CONTENT"\
       | jq -r .droplet.id )"\
       && sleep 3 && echo "$WORKER2_ID"

echo "sleep 2 minutes waiting for droplets to be ready"
sleep 120

export JQFILTER1='.droplets | .[] | select (.name == "manager") | .networks.v4 | .[]| select (.type == "public") | .ip_address'

export MANAGER_IP="$(curl -s GET $DROPLETS_API\
    -H "$BEARER_AUTH_TOKEN" -H "$JSON_CONTENT"\
    | jq -r "$JQFILTER1")"
echo "manager ip: $MANAGER_IP"

export JQFILTER2='.droplets | .[] | select (.name == "worker1") | .networks.v4 | .[]| select (.type == "public") | .ip_address'

export WORKER1_IP="$(curl -s GET $DROPLETS_API\
    -H "$BEARER_AUTH_TOKEN" -H "$JSON_CONTENT"\
    | jq -r "$JQFILTER2")"
echo "worker1 ip: $WORKER1_IP"

export JQFILTER3='.droplets | .[] | select (.name == "worker2") | .networks.v4 | .[]| select (.type == "public") | .ip_address'

export WORKER2_IP="$(curl -s GET $DROPLETS_API\
    -H "$BEARER_AUTH_TOKEN" -H "$JSON_CONTENT"\
    | jq -r "$JQFILTER3")"
echo "worker2 ip: $WORKER2_IP"

ssh -o StrictHostKeyChecking=accept-new root@"$MANAGER_IP" "ufw allow 22/tcp && ufw allow 2376/tcp &&\
ufw allow 2377/tcp && ufw allow 7946/tcp && ufw allow 7946/udp &&\
ufw allow 4789/udp &&\
ufw allow 8765 && ufw allow 3000&&\
ufw reload && ufw --force enable &&\
systemctl restart docker"

ssh -o StrictHostKeyChecking=accept-new root@"$WORKER1_IP" "ufw allow 22/tcp && ufw allow 2376/tcp &&\
ufw allow 2377/tcp && ufw allow 7946/tcp && ufw allow 7946/udp &&\
ufw allow 4789/udp &&\
ufw allow 8765 &&\
ufw reload && ufw --force enable &&\
systemctl restart docker"

ssh -o StrictHostKeyChecking=accept-new root@"$WORKER2_IP" "ufw allow 22/tcp && ufw allow 2376/tcp &&\
ufw allow 2377/tcp && ufw allow 7946/tcp && ufw allow 7946/udp &&\
ufw allow 4789/udp &&\
ufw allow 8765 &&\
ufw reload && ufw --force enable &&\
systemctl restart docker"

echo "init swarm"
ssh root@"$MANAGER_IP" "docker swarm init --advertise-addr $MANAGER_IP"
export WORKER_TOKEN="$(ssh root@"$MANAGER_IP" "docker swarm join-token worker -q")"
echo "worker token: $WORKER_TOKEN"

echo "join workers"
REMOTE_JOIN_CMD="docker swarm join --token $WORKER_TOKEN $MANAGER_IP:2377"
ssh root@"$WORKER1_IP" "$REMOTE_JOIN_CMD"
ssh root@"$WORKER2_IP" "$REMOTE_JOIN_CMD"

ssh root@"$MANAGER_IP" "mkdir /minitwit"

echo "syncing remote_files"
rsync -a remote_files/ root@"$MANAGER_IP":/minitwit

export exCD="cd /minitwit"
ssh root@"$MANAGER_IP" "echo $exCD >> ~/.bash_profile &&\
chmod +x /minitwit/deploy.sh"

echo "setting env variables"
export exDockerUsername="export DOCKER_USERNAME=$DOCKER_USERNAME"
ssh root@"$MANAGER_IP" "echo $exDockerUsername >> ~/.bash_profile"
export exDockerPw="export DOCKER_PASSWORD=$DOCKER_PASSWORD"
ssh root@"$MANAGER_IP" "echo $exDockerPw >> ~/.bash_profile"
export exDbPw="export DB_PASSWORD=$DB_PASSWORD"
ssh root@"$MANAGER_IP" "echo $exDbPw >> ~/.bash_profile"
export exDbU="export DB_USER=$DB_USER"
ssh root@"$MANAGER_IP" "echo $exDbU >> ~/.bash_profile"
export exDbIP="export DB_HOST=$DB_HOST"
ssh root@"$MANAGER_IP" "echo $exDbIP >> ~/.bash_profile"
export exDbPort="export DB_PORT=$DB_PORT"
ssh root@"$MANAGER_IP" "echo $exDbPort >> ~/.bash_profile"

echo "deploying"
ssh root@"$MANAGER_IP" "cd /minitwit &&\
apt install -y docker-compose &&\
export DOCKER_USERNAME=$DOCKER_USERNAME &&\
export DB_PASSWORD=$DB_PASSWORD &&\
docker stack deploy -c docker-compose.swarm.yml ministack"