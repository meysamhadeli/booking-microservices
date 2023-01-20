#!/bin/bash

# Download kubectl binary
curl -LO "https://storage.googleapis.com/kubernetes-release/release/$(curl -L -s https://storage.googleapis.com/kubernetes-release/release/stable.txt)/bin/linux/amd64/kubectl"

# Make the kubectl binary executable
chmod +x kubectl

# Move the binary in to PATH
sudo mv kubectl /usr/local/bin/