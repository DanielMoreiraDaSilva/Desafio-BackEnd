# Desafio-BackEnd
## Documentação do Projeto

### Repositório

O repositório contém uma aplicação distribuída composta por uma API .NET, uma função Lambda, um banco de dados PostgreSQL e um arquivo Docker Compose para execução local.

### Como Executar

#### Softwares Necessários

Certifique-se de ter os seguintes softwares instalados:

- Visual Studio 2022
- pgAdmin 4
- Docker

#### Instalações Necessárias

Além disso, você precisará das seguintes instalações:

- SDK .NET 6
- SDK .NET 7
- AWS CLI
- Amazon.Lambda.TestTool-6.0

#### Configuração do Banco de Dados PostgreSQL

1. Abra o pgAdmin 4 e insira a senha "inicial".
2. Crie uma base de dados com o nome "postgresql".
3. Use a seguinte connection string para conectar à base de dados:

```plaintext
User ID=postgres;Password=inicial;Host=localhost;Port=5432;Database=postgresql;Pooling=true;Connection Lifetime=0;
```

4. Execute o script localizado em `./GerenciadorAluguel/Data/Scripts/Database.sql` dentro do banco de dados "postgresql".

#### Execução do Docker Compose

1. Execute o arquivo `docker-compose.yml` localizado na raiz do projeto.
2. Espere até que o container Docker seja criado e esteja em execução.

#### Configuração da Lambda e AWS

1. Execute o script `script_stack_aws.sh`.
   - Este script criará um bucket S3, uma fila SQS, um tópico SNS e configurará uma função Lambda.
   - Ele também implantará a função Lambda compactada no arquivo `function.zip` localizado na pasta `./LambdaConsumer`.
   - Além disso, o script criará um trigger para a fila SQS, acionando a função Lambda quando mensagens forem consumidas.

#### Execução da API e Testes

1. Execute a API localmente.
2. Utilize o Swagger para fazer testes e interagir com os endpoints da API.
3. Caso queira testar a lambda localmente basta executala e testar utilizando a interface do Amazon.Lambda.TestTool-6.0 simulando o consumo de uma mensagem SQS.

### Observações

Certifique-se de seguir os passos corretamente e de que todas as dependências estejam instaladas corretamente para garantir o funcionamento adequado da aplicação.

---

Esta documentação fornece um guia básico para configurar e executar o projeto localmente. Certifique-se de verificar e seguir os passos com cuidado para evitar problemas durante a execução.