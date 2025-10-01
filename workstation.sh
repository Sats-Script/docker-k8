#!/bin/bash
# setup-k8s.sh
# Purpose: Install Docker, kubectl, Minikube, mount EBS, and start Minikube on RHEL 9

set -e

# -------------------------------
# 1️⃣ Update system
# -------------------------------
echo "Updating system packages..."
yum update -y

# -------------------------------
# 2️⃣ Install Docker CE
# -------------------------------
echo "Installing Docker CE..."
yum remove -y docker docker-client docker-client-latest docker-common docker-latest docker-latest-logrotate docker-logrotate docker-engine || true
yum install -y yum-utils
yum-config-manager --add-repo https://download.docker.com/linux/rhel/docker-ce.repo
yum install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
systemctl enable --now docker
echo "Docker installed and running."

# -------------------------------
# 3️⃣ Add ec2-user to Docker group
# -------------------------------
if id "ec2-user" &>/dev/null; then
    usermod -aG docker ec2-user
    echo "Added ec2-user to Docker group. Logout and login again for group change."
fi

# -------------------------------
# 4️⃣ Install kubectl
# -------------------------------
echo "Installing kubectl..."
KUBECTL_VERSION=$(curl -L -s https://dl.k8s.io/release/stable.txt)
curl -LO "https://dl.k8s.io/release/${KUBECTL_VERSION}/bin/linux/amd64/kubectl"
chmod +x kubectl
mv kubectl /usr/local/bin/
echo "kubectl installed."
kubectl version --client || true

# -------------------------------
# 5️⃣ Install Minikube
# -------------------------------
echo "Installing Minikube..."
curl -LO https://storage.googleapis.com/minikube/releases/latest/minikube-linux-amd64
chmod +x minikube-linux-amd64
mv minikube-linux-amd64 /usr/local/bin/minikube
echo "Minikube installed."

# -------------------------------
# 6️⃣ Mount extra EBS volume for PV
# -------------------------------
EBS_DEVICE="/dev/nvme1n1"
MOUNT_POINT="/mnt/ebs"

echo "Setting up extra EBS volume..."
mkdir -p $MOUNT_POINT

# Check if already formatted
if ! blkid $EBS_DEVICE &>/dev/null; then
    echo "Formatting $EBS_DEVICE with ext4..."
    mkfs.ext4 $EBS_DEVICE
fi

# Mount and set permissions
mount $EBS_DEVICE $MOUNT_POINT
chmod 777 $MOUNT_POINT

# Add to /etc/fstab to mount on reboot
if ! grep -q "$MOUNT_POINT" /etc/fstab; then
    echo "$EBS_DEVICE $MOUNT_POINT ext4 defaults,nofail 0 2" >> /etc/fstab
fi
echo "EBS volume mounted at $MOUNT_POINT"

# -------------------------------
# 7️⃣ Start Minikube
# -------------------------------
echo "Starting Minikube..."
minikube delete || true
minikube start --cpus=2 --memory=4096 --driver=docker

# -------------------------------
# 8️⃣ Done
# -------------------------------
echo "===================================="
echo "✅ Setup complete!"
echo "Docker, kubectl, Minikube installed."
echo "Minikube is running, PV available at $MOUNT_POINT"
echo "Use 'kubectl get nodes' to verify cluster."
echo "===================================="
