FROM nginx:alpine

WORKDIR /app

COPY ssl/live/jasonderulo.live/fullchain.pem /etc/nginx/ssl/fullchain.pem
COPY ssl/live/jasonderulo.live/privkey.pem /etc/nginx/ssl/privkey.pem

COPY . .

COPY ./nginx.conf /etc/nginx/nginx.conf