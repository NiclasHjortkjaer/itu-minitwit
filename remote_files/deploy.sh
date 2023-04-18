source ~/.bash_profile

cd /minitwit

docker-compose -f docker-compose.yml pull
docker service update --image ${DOCKER_USERNAME}/itu-minitwit-jason ministack_web
docker pull $DOCKER_USERNAME/itu-minitwit-jason:latest