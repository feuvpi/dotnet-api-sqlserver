# API de Gestão de Pedidos e Clientes

## Visão Geral
Esta API foi desenvolvida para atender ao teste técnico de criação de um sistema de gestão de pedidos e clientes.

## Arquitetura
A API foi construída seguindo os princípios da Clean Architecture, com uma clara separação de responsabilidades:

- **Core**: Contém as entidades de domínio, interfaces e DTOs
- **Infrastructure**: Implementa o acesso a dados usando Entity Framework Core
- **Service**: Implementa a lógica de negócios
- **API**: Expõe os endpoints REST

Esta estrutura facilita a manutenção, testabilidade e escalabilidade do projeto.

## Tecnologias Utilizadas
- .NET Core 8.0
- Entity Framework Core
- SQL Server
- JWT para autenticação
- Swagger/OpenAPI para documentação
- xUnit, Moq e FluentAssertions para testes

## Funcionalidades Implementadas

### Endpoints para Clientes (/api/clientes)
- [POST] Criar Cliente
- [GET] Listar Clientes
- [GET] Buscar Cliente por ID
- [PUT] Atualizar Cliente
- [DELETE] Excluir Cliente

### Endpoints para Pedidos (/api/pedidos)
- [POST] Criar Pedido (associado a um Cliente)
- [GET] Listar Pedidos
- [GET] Buscar Pedido por ID
- [GET] Buscar Pedido por ClienteID
- [PUT] Atualizar Pedido
- [DELETE] Excluir Pedido

## Regras de Negócio Implementadas
- Um pedido só pode ser criado para um cliente existente
- Não é permitido excluir um cliente que possui pedidos associados
- O valor total do pedido não pode ser negativo

## Como Executar o Projeto

### 1. Clonando o Repositório 
```bash
git clone https://github.com/feuvpi/dotnet-api-sqlserver.git
cd dotnet-api-sqlserver
```

### 2. Configurando o SQL Server com Docker
```bash
   docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Sua_Senha_Forte!123" \
   -p 1433:1433 --name sql_server \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

### 3. Configuração da Aplicação
   #### Desenvolvimento
   Edite o arquivo appsettings.json:

```json
{
    "Logging": {
    "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
    }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=dotnet-api-db;User Id=sa;Password=Sua_Senha_Forte!123;TrustServerCertificate=True"
    },
    "JWT": {
    "SecretKey": "Jz7x9fPq2rT4vW6yBD0gH1jK3lM5nO7pQ9sS1vU3wX5yZ7aB8cD0eF2gH4jK6l"
    }
}
```
#### Produção
Em ambiente de produção, é recomendado utilizar variáveis de ambiente ou serviços de configuração seguros:
```bash
# Exemplo de variáveis de ambiente para produção
export ConnectionStrings__DefaultConnection="Server=prod_server;Database=prod_db;User Id=prod_user;Password=prod_password;TrustServerCertificate=True"
export JWT__SecretKey="sua_chave_secreta_produção"
export JWT__Issuer="seu_issuer_producao"
export JWT__Audience="sua_audience_producao"
```

### 4. Criando e Aplicando Migrations
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

### 5. Executando a Aplicação
```bash
cd API
dotnet run
```

## Autenticação e Segurança
A API utiliza autenticação JWT. Para usar os endpoints protegidos:

1. Registre um usuário:
   - POST /api/auth/register 
   - Body: { "username": "seu_usuario", "email": "seu@email.com", "password": "sua_senha" }
   

2. Faça login:
   - POST /api/auth/login 
   - Body: { "email": "seu@email.com", "password": "sua_senha" }


3. Use o token retornado no header das requisições:
   - Authorization: Bearer {seu_token}

## Testes
O projeto inclui testes unitários utilizando xUnit, Moq e FluentAssertions. Para executar:

```bash
dotnet test
```
