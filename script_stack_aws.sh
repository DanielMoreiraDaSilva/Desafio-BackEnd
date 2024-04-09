# Comando AWS para criar um bucket no S3
# o Endpoint é para definir que iremos criar dentro do localstack
aws s3 mb s3://storage-cnh-images --endpoint http://localhost:4566 --region us-east-1

# Comando para criar uma fila SQS
aws sqs create-queue --queue-name delivery-order --endpoint http://localhost:4566 --region us-east-1

# Comando para criar um SNS
aws sns create-topic --name delivery-topic --endpoint http://localhost:4566 --region us-east-1


