# ApiClinica com ORM

API REST em ASP.NET Core com Entity Framework Core e SQLite para gerenciamento de pacientes, médicos e consultas.

> Projeto de faculdade da disciplina de Server-Side.
>
> A execução foi mantida simples para aula e avaliação: o banco é local (SQLite) e sobe junto com a aplicação no ambiente de desenvolvimento.

## Visão geral

O projeto usa:

- ASP.NET Core Web API
- Entity Framework Core
- SQLite local em `clinica.db`
- DTOs e mappers manuais
- Validações de negócio no backend
- Sem injeção de dependência, por decisão do projeto

## Requisitos

- .NET SDK compatível com o projeto
- Node.js 18+ para executar os testes em CLI
- Postman para importar a collection com testes embutidos

## Como executar

1. Restaurar dependências:
   ```bash
   dotnet restore
   ```
2. Aplicar migrations, se necessário:
   ```bash
   dotnet ef database update
   ```
3. Executar a API:
   ```bash
   dotnet run
   ```

A API sobe em `http://localhost:5070`.

## Banco de dados

O banco SQLite usado no projeto é o arquivo local `clinica.db`.

Se quiser reiniciar do zero, basta remover o arquivo e executar a API novamente, ou refazer as migrations conforme o seu fluxo.

## Estrutura principal da API

### Pacientes

- `GET /api/pacientes`
- `GET /api/pacientes/{id}`
- `POST /api/pacientes`
- `PATCH /api/pacientes/{id}`
- `DELETE /api/pacientes/{id}`

### Médicos

- `GET /api/medicos`
- `GET /api/medicos/{id}`
- `POST /api/medicos`
- `PATCH /api/medicos/{id}`
- `DELETE /api/medicos/{id}`

### Consultas

- `GET /api/consultas`
- `GET /api/consultas/{id}`
- `POST /api/consultas`
- `PATCH /api/consultas/{id}`
- `DELETE /api/consultas/{id}`

## Regras de negócio validadas

### Pacientes

- Email com formato válido
- Telefone com 10 ou 11 dígitos
- CPF válido pelo algoritmo mod 11
- CPF duplicado não é permitido
- Data de nascimento não pode ser futura
- CPF é imutável no PATCH

### Médicos

- Email com formato válido
- Telefone com 10 ou 11 dígitos
- CRM é imutável no PATCH

### Consultas

- Data da consulta deve ser no futuro
- Paciente deve existir
- Médico deve existir
- Não pode haver sobreposição de horário menor que 30 minutos

## Testes

O projeto possui três formas principais de teste:

1. Collection do Postman com testes embutidos
2. Script CLI em Node.js
3. Guia de teste direto no Postman GUI

### 1. Collection com testes embutidos

Arquivo:

- `ApiClinica-com-testes.postman_collection.json`

Importe no Postman e cada request já virá com a aba de testes configurada.

#### Como importar

1. Abra o Postman
2. Clique em `Import`
3. Selecione `ApiClinica-com-testes.postman_collection.json`
4. Execute qualquer request para ver os testes rodando automaticamente

#### Observações

- A API precisa estar rodando em `http://localhost:5070`
- Os exemplos já vêm com JSON pronto
- Alguns testes usam IDs fixos para facilitar edição manual

### 2. Teste rápido via CLI

Arquivo:

- `postman-test-cli.js`

Executar:

```bash
cd "c:\Users\pmdon\Downloads\Projetos C#\ApiClinica com ORM"
node postman-test-cli.js
```

Saída esperada:

```text
✅ Passou: 15
❌ Falhou: 2
📊 Taxa: 88.24%
```

### 3. Teste com Newman

Instalação:

```bash
npm install -g newman
```

Execução:

```bash
newman run ApiClinica-com-testes.postman_collection.json
```

Relatório HTML:

```bash
newman run ApiClinica-com-testes.postman_collection.json --reporters cli,html --reporter-html-export test-results.html
```

### 4. Guia direto no Postman GUI

Arquivo de apoio:

- `postman-test-suite.js`

Passos:

1. Abra a collection no Postman
2. Vá para a aba `Tests`
3. Cole o script de teste, se quiser executar manualmente em uma request própria
4. Clique em `Send`
5. Veja os resultados na aba `Test Results`

## Cenários cobertos

### Pacientes

- Criar paciente válido
- Rejeitar email inválido
- Rejeitar CPF duplicado
- Rejeitar data de nascimento futura
- Atualizar paciente
- Impedir alteração de CPF
- Deletar paciente

### Médicos

- Criar médico válido
- Rejeitar email inválido
- Atualizar médico
- Impedir alteração de CRM

### Consultas

- Criar consulta válida
- Rejeitar paciente inexistente
- Rejeitar data no passado
- Rejeitar sobreposição de horário
- Atualizar horário da consulta
- Deletar consulta

## Arquivos importantes

- `ApiClinica-com-testes.postman_collection.json` - collection principal com testes embutidos
- `postman-test-cli.js` - runner em Node.js
- `postman-test-suite.js` - script para uso no Postman GUI
- `ApiClinica.http` - requisições rápidas no VS Code

As variáveis do Postman ficaram dentro da própria collection principal, então agora você só precisa importar um arquivo para testar.

## Ordem recomendada de uso

1. Subir a API com `dotnet run`
2. Validar a collection no Postman
3. Rodar o CLI com `node postman-test-cli.js`
4. Se quiser CI, usar Newman

## Troubleshooting

### A API não sobe

- Verifique se o .NET SDK está instalado
- Rode `dotnet restore`
- Rode `dotnet run` na raiz do projeto

### O Postman não encontra a API

- Confirme que a API está em `http://localhost:5070`
- Veja se o banco `clinica.db` foi criado

### Os testes falham em sequência

- Confirme se os IDs usados nas requests existem
- Execute primeiro os POSTs de criação
- Depois rode os PATCHs e DELETEs

## Observação final

Se quiser, eu também posso deixar o README ainda mais curto e objetivo, ou transformar esta documentação em uma versão mais formal para entrega de faculdade.
