#!/bin/bash

# Generate self-signed certificate
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes -subj '/CN=localhost'

# Trust the certificate in Gitpod
sudo mkdir -p /etc/gitpod/tls/
sudo cp cert.pem /etc/gitpod/tls/
sudo cp key.pem /etc/gitpod/tls/

# Restart Gitpod
sudo gitpod restart