# Azure Monitor Combined with Elastic Observability

## Steps for sets up
Create Azure Resources

Connect Azure Monitor to Elastic Cloud

Deploy Elastic Agent on AKS

APM Tracing for App in AKS

Set Alerts in Elastic

## Create Azure Resources
Variables
```sh
RESOURCE_GROUP="elastic-observability-rg"
CLUSTER_NAME="elastic-aks"
LOCATION="australiaeast"
```

```sh
az group create --name $RESOURCE_GROUP --location $LOCATION
```

```sh
az aks create \
  --resource-group $RESOURCE_GROUP \
  --name $CLUSTER_NAME \
  --node-count 2 \
  --enable-managed-identity \
  --generate-ssh-keys
```

Connect kubectl

get-credentials: Downloads the Kubernetes configuration (kubeconfig) file for your AKS cluster and merges it with your local ~/.kube/config file. This lets you use kubectl commands to manage your AKS cluster directly from your terminal.
```sh
az aks get-credentials --resource-group $RESOURCE_GROUP --name $CLUSTER_NAME
```

We can run to verify the connection
```sh
kubectl get pods --all-namespaces
```

## Deploy Elastic Agent on AKS
```sh
az elastic monitor create \
  --name "elastic-monitoring" \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --elastic-cloud-id " Cls_Elastic:dXMtY2VudHJhbDEuZ2NwLmNsb3VkLmVzLmlvOjQ0MyQ43MjFjYWI2YjIzNzc0MmQxYjU5NDBlMzczOTk89OTNmNyRlNWQyOTY4MmE1NjE0NjYyOTkwNjI0MmFmOGI5YjMxNQ==" \
  --elastic-apm-key " essu_U43kU1NWVmFiMEpCWjNkWFZWaHpWRFZEY89kwNlJWaFRTMGRHVWxVMWJuQTJNRE0zTTJsUGVFUkdRUT09AAAAAOzgMYM= "
```


