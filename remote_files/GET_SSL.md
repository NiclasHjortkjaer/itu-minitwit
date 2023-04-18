## for ssl (replace jasonderulo.live with domain):
- sudo apt install certbot
- sudo certbot certonly --manual --preferred-challenges=dns --email your@email.com --work-dir=./ssl --config-dir=./ssl --logs-dir=./ssl --agree-tos -d jasonderulo.live -d www.jasonderulo.live
- sudo apt install nginx
- sudo cp ssl/live/jasonderulo.live/fullchain.pem /etc/nginx/ssl/fullchain.pem
- sudo cp ssl/live/jasonderulo.live/privkey.pem /etc/nginx/ssl/privkey.pem
- sudo cp nginx.conf /etc/nginx/nginx.conf
- sudo systemctl reload nginx

[//]: # (- cd nginx)
[//]: # (- docker build -t minitwitnginx .)
[//]: # (- docker run --network=minitwit_outside -p 80:80 -p 443:443 -d minitwitnginx)