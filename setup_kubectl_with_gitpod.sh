#!/bin/bash

if command -v /usr/local/bin/kubectl &>/dev/null; then
    echo "kubectl is already installed"
    exit
fi

# Setup aws-iam-authenticator

OLD_DIR="$PWD"
TMP_DIR="$(mktemp -d)"
echo "Installing aws-iam-authenticator"
cd "${TMP_DIR}" || exit 1

curl -o aws-iam-authenticator https://s3.us-west-2.amazonaws.com/amazon-eks/1.21.2/2021-07-05/bin/linux/amd64/aws-iam-authenticator
curl -o aws-iam-authenticator.sha256 https://s3.us-west-2.amazonaws.com/amazon-eks/1.21.2/2021-07-05/bin/linux/amd64/aws-iam-authenticator.sha256

if sha256sum -c aws-iam-authenticator.sha256; then
    chmod +x aws-iam-authenticator
    sudo mv aws-iam-authenticator /usr/local/bin/aws-iam-authenticator
fi

cd "${OLD_DIR}" || exit 1
rm -rf "${TMP_DIR}"

# Setup kubectl

echo "Setting-up kubectl"
OLD_DIR="$PWD"
TMP_DIR="$(mktemp -d)"
cd "${TMP_DIR}" || exit 1

# Get the Stable version of k8s
KUBE_REL=$(curl -L -s https://dl.k8s.io/release/stable.txt)

curl -LO "https://dl.k8s.io/release/${KUBE_REL}/bin/linux/amd64/kubectl"
KUBE_SHA=$(curl -sL "https://dl.k8s.io/release/${KUBE_REL}/bin/linux/amd64/kubectl.sha256")

echo "${KUBE_SHA} kubectl" >kubectl.sha256

if sha256sum -c kubectl.sha256; then
    chmod +x kubectl
    sudo mv kubectl /usr/local/bin/kubectl
fi

cd "${OLD_DIR}" || exit 1
rm -rf "${TMP_DIR}"

# Setup cert-manager

echo "Setting-up cert-manager"
TMP_DIR="$(mktemp -d)"
cd "$TMP_DIR" || exit 1

CM_RELEASE=$(curl -sL https://api.github.com/repos/cert-manager/cert-manager/releases/latest | jq -r .tag_name)

OS=$(go env GOOS)
ARCH=$(go env GOARCH)
curl -sSL -o cmctl.tar.gz https://github.com/cert-manager/cert-manager/releases/download/$CM_RELEASE/cmctl-$OS-$ARCH.tar.gz
tar xzf cmctl.tar.gz
sudo mv cmctl /usr/local/bin

cd "$OLD_DIR"
rm -rf "$TMP_DIR"


# Setup kubectl-cert-manager

echo "Setting-up kubectl-cert-manager"
TMP_DIR="$(mktemp -d)"
cd "$TMP_DIR" || exit 1

OS=$(go env GOOS)
ARCH=$(go env GOARCH)
curl -sSL -o kubectl-cert-manager.tar.gz https://github.com/cert-manager/cert-manager/releases/download/$CM_RELEASE/kubectl-cert_manager-$OS-$ARCH.tar.gz
tar xzf kubectl-cert-manager.tar.gz
sudo mv kubectl-cert_manager /usr/local/bin

cd "$OLD_DIR"
rm -rf "$TMP_DIR"

# Setup eksctl

echo "Setting-up eksctl"
TMP_DIR="$(mktemp -d)"
cd "$TMP_DIR" || exit 1

curl --silent --location "https://github.com/weaveworks/eksctl/releases/latest/download/eksctl_$(uname -s)_amd64.tar.gz" | tar xz -C /tmp
sudo mv /tmp/eksctl /usr/local/bin
cd "$OLD_DIR"
rm -rf "$TMP_DIR"

echo "All things which are required for kubectl are Installed & Configured Successfully."
echo "Now, You can Start use kubectl."
