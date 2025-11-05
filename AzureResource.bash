# Variables
RESOURCE_GROUP="elastic-observability-rg"
CLUSTER_NAME="elastic-aks"
LOCATION="australiaeast"

# Create Resource Group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create AKS cluster
az aks create \
  --resource-group $RESOURCE_GROUP \
  --name $CLUSTER_NAME \
  --node-count 2 \
  --enable-managed-identity \
  --generate-ssh-keys

# Connect kubectl
# get-credentials: Downloads the Kubernetes configuration (kubeconfig) file for your AKS cluster and merges it with your local ~/.kube/config file. This lets you use kubectl commands to manage your AKS cluster directly from your terminal.
az aks get-credentials --resource-group $RESOURCE_GROUP --name $CLUSTER_NAME

We can run to verify the connection
kubectl get pods --all-namespaces
