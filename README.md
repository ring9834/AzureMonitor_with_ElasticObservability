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

## Connect Azure Monitor to Elastic Cloud
```sh
az elastic monitor create \
  --name "elastic-monitoring" \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --elastic-cloud-id " Cls_Elastic:dXMtY2VudHJhbDEuZ2NwLmNsb3VkLmVzLmlvOjQ0MyQ43MjFjYWI2YjIzNzc0MmQxYjU5NDBlMzczOTk89OTNmNyRlNWQyOTY4MmE1NjE0NjYyOTkwNjI0MmFmOGI5YjMxNQ==" \
  --elastic-apm-key " essu_U43kU1NWVmFiMEpCWjNkWFZWaHpWRFZEY89kwNlJWaFRTMGRHVWxVMWJuQTJNRE0zTTJsUGVFUkdRUT09AAAAAOzgMYM= "
```
This will stream Azure resource logs and metrics automatically.

Now, deploy Elastic Agent to collect cluster-level logs and metrics. The Elastic Agent runs as a DaemonSet, collecting logs, metrics, and traces from every node/pod.

This command used to create a Secret object in your cluster to securely store your Elastic API key for use by pods or applications running inside Kubernetes.
```sh
kubectl create secret generic elastic-credentials \
  --from-literal=api_key=" essu_U43kU1NWVmFiMEpCWjNkWFZWaHpWRFZEY89kwNlJWaFRTMGRHVWxVMWJuQTJNRE0zTTJsUGVFUkdRUT09AAAAAOzgMYM= "
```

Create a DaemonSet manifest 
```sh
apiVersion: apps/v1
kind: DaemonSet
metadata:
  name: elastic-agent
  namespace: kube-system
spec:
  selector:
    matchLabels:
      app: elastic-agent
  template:
    metadata:
      labels:
        app: elastic-agent
    spec:
      serviceAccountName: elastic-agent
      containers:
      - name: elastic-agent
        image: docker.elastic.co/beats/elastic-agent:8.15.0
        env:
          - name: FLEET_ENROLLMENT_TOKEN
            valueFrom:
              secretKeyRef:
                name: elastic-credentials
                key: api_key
          - name: FLEET_URL
            value: " https://cls-elastic-f5d375.fleet.us-central1.gcp.cloud.es.io:443"
          - name: KIBANA_FLEET_SETUP
            value: "1"
          - name: ELASTIC_AGENT_LOG_LEVEL
            value: "info"
        volumeMounts:
          - name: varlog
            mountPath: /var/log
          - name: varlibdockercontainers
            mountPath: /var/lib/docker/containers
            readOnly: true
      volumes:
        - name: varlog
          hostPath:
            path: /var/log
        - name: varlibdockercontainers
          hostPath:
            path: /var/lib/docker/containers
```

Deploy it.
```sh
curl -O https://github.com/ring9834/AKS_with_ElasticObservability/main/elastic-agent.yaml
kubectl apply -f elastic-agent.yaml
```

Check that agents are running. 
```sh
kubectl get pods -n kube-system -l app=elastic-agent
```
open Kibana → Observability → Overview in your Elastic Cloud console. 
You’ll see Logs from Kubernetes pods; Metrics from AKS nodes; Azure Monitor resource metrics (VMs, storage, etc.); Traces.

## APM Tracing for App in AKS
Make our ASP.NET Core Web API app to use the Elastic APM Agent. Modify Program.cs of our API service.
```sh
using Elastic.Apm.NetCoreAll;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

// Enable Elastic APM
app.UseAllElasticApm(builder.Configuration);

app.MapControllers();
app.Run();
```

Add Elastic APM configuration in appsettings.json
```sh
{
  "ElasticApm": {
    "ServerUrls": " https://cls-elastic-f5d375.apm.us-central1.gcp.cloud.es.io:443",
    "SecretToken": " RW98kZKX43STaiMzHYCWE70Wgt",
    "ServiceName": "cls-api ",
    "Environment": "production"
  }
}
```

Build Docker Image and push to registry.
```sh
docker build -t https://app.docker.com/accounts/ring9834/elastic-apm-dotnet:latest .
docker push https://app.docker.com/accounts/ring9834/elastic-apm-dotnet:latest
```
```sh
Deploy the App to AKS
apiVersion: apps/v1
kind: Deployment
metadata:
  name: dotnet-apm-cls
  labels:
    app: dotnet-apm- cls
spec:
  replicas: 1
  selector:
    matchLabels:
      app: dotnet-apm- cls
  template:
    metadata:
      labels:
        app: dotnet-apm- cls
    spec:
      containers:
      - name: dotnet-apm- cls
        image: https://app.docker.com/accounts/ring9834/elastic-apm-dotnet:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_URLS
          value: "http://+:80"
        - name: ElasticApm__ServerUrls
          value: "https://your-apm-url:443"
        - name: ElasticApm__SecretToken
          value: "YOUR_SECRET_TOKEN"
        - name: ElasticApm__ServiceName
          value: "dotnet-api-demo"
        - name: ElasticApm__Environment
          value: "production"
---
apiVersion: v1
kind: Service
metadata:
  name: cls-apm-service
spec:
  selector:
    app: cls-apm-cls
  ports:
    - protocol: TCP
      port: 80
      targetPort: 80
  type: LoadBalancer
```

Deploy
```sh
kubectl apply -f dotnet-apm-deploy.yaml
```

Check pods and services
```sh
kubectl get pods,svc
```

Get the external IP of our service
```sh
kubectl get svc cls-apm-service
```

## Set Alerts
In Elastic Cloud, find through Observability → Alerts → Rules→Create rule. and Set:
CPU > 80% for 5 min
Memory > 90%
Container restart count > 3
Application error rate > 10%






