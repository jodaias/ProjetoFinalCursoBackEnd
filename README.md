# Library

Sistema de gerenciamento de Library (CRUD de autores, livros e empréstimos) desenvolvido em .NET 8, com Entity Framework Core e API REST.

## Estrutura do Projeto

- **Library.Api**: API REST principal
- **Library.Domain**: Models e interfaces de domínio
- **Library.Infrastructure**: Persistência de dados e repositórios
- **Library.Tests**: Testes automatizados

## Como rodar o projeto

1. **Restaure os pacotes:**
   ```sh
   dotnet restore
   ```
2. **Compile a solução:**
   ```sh
   dotnet build
   ```
3. **Execute a API:**
   ```sh
   dotnet run --project Library.Api
   ```
4. **Acesse a documentação Swagger:**
   - Normalmente em: `https://localhost:5001/swagger` ou `http://localhost:5000/swagger`

## Dependências principais

- .NET 8
- Entity Framework Core (Sqlite)
- Swashbuckle (Swagger)

## Observações

- O banco de dados local é criado automaticamente (Sqlite).
- Os arquivos de configuração sensíveis (como `appsettings.Development.json`) estão no `.gitignore`.

---

> Projeto para fins de estudo e demonstração.
