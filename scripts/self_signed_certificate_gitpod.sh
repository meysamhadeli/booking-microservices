#!/bin/bash

# install mkcert
curl -fsSL https://github.com/FiloSottile/mkcert/releases/download/v1.4.1/mkcert-v1.4.1-linux-amd64 -o mkcert
chmod +x mkcert
sudo mv mkcert /usr/local/bin

# install root CA
mkcert -install

# create certificate
mkcert gitpod.local

# move certificate files to gitpod directory
mkdir -p ~/.gitpod/certs
mv gitpod.local.pem ~/.gitpod/certs/
mv gitpod.local-key.pem ~/.gitpod/certs/

# configure nginx to use the certificate
echo "server { listen 80; listen [::]:80; server_name gitpod.local; ssl_certificate /home/gitpod/.gitpod/certs/gitpod.local.pem; ssl_certificate_key /home/gitpod/.gitpod/certs/gitpod.local-key.pem; }" | sudo tee /etc/nginx/conf.d/gitpod.local.conf
sudo systemctl reload nginx