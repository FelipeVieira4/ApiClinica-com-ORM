# ApiClinica (N3) — Instruções rápidas

API REST em ASP.NET Core (NET 10) com EF Core + SQLite para gerenciamento de `Pacientes`, `Medicos` e `Consultas`.

## Participantes

- Pedro Martins
- Felipe Vieira
- Klaus Christoph Emmerich Jourdain

Principais pontos implementados (requisitos do trabalho N3)
- Padrão Service + DI para `Paciente`, `Medico` e `Consulta` (serviços registrados em `Program.cs`).
- Mappers e DTOs para conversão entre entidades e transporte.
- Autenticação JWT e endpoints de `Register` / `Login` (em `Controllers/AuthController.cs`).
- Autorização por papel: apenas `Admin` pode `PATCH` e `DELETE` em recursos sensíveis.
- Seed automático das contas obrigatórias: `admin` / `admin123` e `user` / `user123`.

Pré-requisitos
- .NET 10 SDK
- (Opcional) `dotnet-ef` se for usar comandos EF CLI

Como executar localmente
1. Restaurar e compilar:

```bash
dotnet restore
dotnet build
```

2. Aplicar migrations (opcional — o projeto já inclui migrations):

```bash
dotnet ef database update
```

3. Executar a API:

```bash
dotnet run
```

A API espera em `http://localhost:5070` por padrão.

Credenciais seed (criadas automaticamente no primeiro run)
- Admin: `admin` / `admin123` (Role = Admin)
- Usuário comum: `user` / `user123` (Role = User)

Autenticação
- Endpoints:
  - `POST /api/auth/register` — registrar novo usuário
  - `POST /api/auth/login` — obter JWT
- Para chamadas autenticadas, inclua o header:

```
Authorization: Bearer <token>
```

Segurança e papéis
- Endpoints protegidos usam `[Authorize]`.
- Somente usuários com `Role = Admin` podem executar `PATCH` e `DELETE` em `Pacientes`, `Medicos` e `Consultas`.

Testes e exemplos
- `ApiClinica.http` — conjunto de requisições para VSCode REST Client (fluxo manual de token, conforme preferência).
- `ApiClinica-com-testes.postman_collection.json` — collection com requests e scripts de teste que capturam token automaticamente e validam permissões.

Como usar a collection do Postman
1. Abra o Postman ▶ Import ▶ selecione `ApiClinica-com-testes.postman_collection.json`.
2. Suba a API (`dotnet run`).
3. Use o request de `Auth - Login` para obter token (a collection já salva tokens nos testes automaticamente).

Banco de dados
- Arquivo SQLite local: `clinica.db` na raiz do projeto.
- Para reiniciar do zero, pare a API, remova `clinica.db` e execute `dotnet ef database update` ou apenas `dotnet run` (o DB será recriado).

Notas importantes
- A migration `N3_Auth` foi gerada no desenvolvimento; verifique antes de aplicar em bases críticas.
- O código já implementa as regras de negócio solicitadas no enunciado N3 (validações, checagens de conflito de agenda, bloqueios de exclusão quando houver consultas futuras, etc.).

Links úteis
- Código de bootstrap/DI: [Program.cs](Program.cs)
- Controladores principais: [Controllers/PacientesController.cs](Controllers/PacientesController.cs), [Controllers/MedicosController.cs](Controllers/MedicosController.cs), [Controllers/ConsultasController.cs](Controllers/ConsultasController.cs)
- Collection de testes: `ApiClinica-com-testes.postman_collection.json`

Se quiser, eu deixo o README ainda mais curto (apenas comandos e credenciais) ou abro o branch `feature/n3`, faço o commit com as alterações e crio o PR. O que prefere?
