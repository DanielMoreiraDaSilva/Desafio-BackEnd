# Comando AWS para criar um bucket no S3
# o Endpoint é para definir que iremos criar dentro do localstack
aws s3 mb s3://storage-cnh-images --endpoint http://localhost:4566

# Comando para criar uma fila SQS
aws sqs create-queue --queue-name sqs-delivery-order --endpoint http://localhost:4566

# Comando para criar um tópico SNS
aws sns create-topic --name delivery-topic --endpoint http://localhost:4566

# Comando para criar e implantar a lambda
aws lambda create-function --function-name lambda-dotnet-consumer-function --zip-file fileb://function.zip --handler Lambda::Lambda.Function::FunctionHandler --runtime dotnet6 --role arn:aws:iam::308309238958:role/lambda-dotnet-ex --endpoint http://localhost:4566

# Comando para criar a trigger que vincula o SQS a execução da lambda
aws lambda create-event-source-mapping --function-name lambda-dotnet-consumer-function --batch-size 1 --event-source-arn arn:aws:sqs:us-east-1:000000000000:sqs-delivery-order --endpoint http://localhost:4566