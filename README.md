# ApiClinica com ORM

API REST em ASP.NET Core com Entity Framework Core e SQLite para gerenciamento de pacientes.

> Projeto de faculdade da disciplina de Server-Side.
> 
> A ideia aqui e manter a execucao simples para aula e avaliacao: o banco de dados e local (SQLite) e sobe junto com a aplicacao quando o projeto e executado no ambiente de desenvolvimento.

## Requisitos

- .NET SDK (versao compatível com o projeto)

## Como executar

1. Restaurar dependencias:
   ```bash
   dotnet restore
   ```
2. Aplicar migrations (se necessario):
   ```bash
   dotnet ef database update
   ```
3. Executar a API:
   ```bash
   dotnet run
   ```

A API sobe em `http://localhost:5070` (conforme `launchSettings.json`).

O banco SQLite usado no projeto e o arquivo local `clinica.db`.

## Endpoints principais

- `GET /api/pacientes`
- `GET /api/pacientes/{id}`
- `POST /api/pacientes`

## Testes de requisicao

Use o arquivo `ApiClinica.http` no VS Code para enviar requests rapidamente.
