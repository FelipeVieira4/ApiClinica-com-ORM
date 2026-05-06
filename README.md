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

---

## Coleção Postman (teste)

Criei uma coleção do Postman com todos os endpoints e cenários de teste (sucesso e falha). Arquivo: `ApiClinica.postman_collection.json` (no diretório raiz do projeto).

Como importar na sua máquina:

1. Abra o Postman
2. Clique em `Import` → `File` → selecione `ApiClinica.postman_collection.json`
3. A coleção aparecerá na barra lateral e você pode executar as requests

Observações:
- A API deve estar rodando em `http://localhost:5070` (rode `dotnet run` no diretório do projeto)
- Os cenários já têm exemplos de JSON prontos; após criar recursos (médico/paciente) atualize os IDs nas requests seguintes.

### Ordem recomendada de testes
1. Criar um Médico (POST /api/medicos)
2. Criar um Paciente (POST /api/pacientes)
3. Criar uma Consulta (POST /api/consultas) — testa regras de agendamento
4. Testar casos de validação (email inválido, data no passado, CPF duplicado, sobreposição)
5. Testar PATCHs e DELETEs nesta ordem

### Pontos importantes
- Telefone: 10 ou 11 dígitos
- CPF: 11 dígitos (apenas números) e validado pelo algoritmo
- DataHora de consulta: formato ISO 8601 `YYYY-MM-DDTHH:mm:ss`
- O arquivo da coleção está em `ApiClinica.postman_collection.json`

---

## Endpoints principais

- `GET /api/pacientes`
- `GET /api/pacientes/{id}`
- `POST /api/pacientes`
- `PATCH /api/pacientes/{id}`
- `DELETE /api/pacientes/{id}`

- `GET /api/medicos`
- `GET /api/medicos/{id}`
- `POST /api/medicos`
- `PATCH /api/medicos/{id}`
- `DELETE /api/medicos/{id}`

- `GET /api/consultas`
- `GET /api/consultas/{id}`
- `POST /api/consultas`
- `PATCH /api/consultas/{id}`
- `DELETE /api/consultas/{id}`

---

Se quiser que eu também atualize a coleção com IDs dinâmicos (variáveis do Postman) ou carregue exemplos adicionais, eu faço em seguida.

## Endpoints principais

- `GET /api/pacientes`
- `GET /api/pacientes/{id}`
- `POST /api/pacientes`

## Testes de requisicao

Use o arquivo `ApiClinica.http` no VS Code para enviar requests rapidamente.
