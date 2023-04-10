## for ssl (replace jasonderulo.live with domain, also in Dockerfile):
- sudo apt install certbot
- sudo certbot certonly --manual --preferred-challenges=dns --email your@email.com --work-dir=./ssl --config-dir=./ssl --logs-dir=./ssl --agree-tos -d jasonderulo.live -d www.jasonderulo.live
- docker build -t minitwitnginx .
- docker run --network=minitwit_outside -p 80:80 -p 443:443 -d minitwitnginx