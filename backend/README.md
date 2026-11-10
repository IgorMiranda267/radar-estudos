# Backend

Backend da plataforma AIStudy Lab, organizado em uma solução .NET com camadas de API, aplicação, domínio e infraestrutura.

## Persistência

- PostgreSQL acessado pelo Entity Framework Core através do provider Npgsql.
- Entidades de domínio e `StudyLabDbContext` para o núcleo do MVP.
- Configurações EF Core explícitas para tabelas, colunas, relacionamentos, índices e constraints.
- A connection string não é armazenada no código nem no repositório.

Antes de iniciar a API ou gerar/aplicar migrations, configure `ConnectionStrings__DefaultConnection` no ambiente. Exemplo no PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=aistudylab;Username=postgres;Password=<sua-senha>"
dotnet run --project .\AIStudyLab.Api
```

Para criar e aplicar a migration inicial, a partir da pasta `backend`:

```powershell
dotnet ef migrations add InitialCreate --project .\AIStudyLab.Infrastructure --startup-project .\AIStudyLab.Api --output-dir .\Persistence\Migrations
dotnet ef database update --project .\AIStudyLab.Infrastructure --startup-project .\AIStudyLab.Api
```

O repositório fixa a versão compatível do `dotnet-ef` em `dotnet-tools.json`. Restaure a ferramenta na pasta `backend` antes de executar os comandos:

```powershell
dotnet tool restore
```

A migration `InitialCreate` já foi gerada e versionada. Para mudanças futuras, crie novas migrations com outro nome; não recrie nem edite manualmente uma migration já aplicada. A connection string deve ser mantida fora de arquivos versionados.

## API e autenticação

Os endpoints de autenticação, perfil, disciplinas, assuntos, sessões, metas, revisões, configurações e dashboard estão documentados em `AIStudyLab.Api.http`. Os endpoints de estudo estão disponíveis no grupo `/api/me` e derivam a identidade do usuário do cookie autenticado; nenhum `usuarioId` enviado pelo cliente é usado para autorizar acesso.

O login cria uma sessão em cookie `HttpOnly`, `SameSite=Strict`, com expiração deslizante de sete dias. Em Development, o cookie acompanha o esquema HTTP local; fora de Development, ele é enviado somente em HTTPS. Configure HTTPS no ambiente de hospedagem e defina as origens confiáveis através de `Cors__AllowedOrigins__0`, `Cors__AllowedOrigins__1` etc. A API valida a origem e o cabeçalho `X-AIStudyLab-Request` nas operações que alteram dados; o frontend já envia esse cabeçalho.

Login e cadastro têm limite de tentativas por endereço IP. As rotas de autenticação, perfil e dados são registradas em todos os ambientes, mas as rotas de estudo e perfil exigem sessão autenticada. O OpenAPI continua disponível somente em Development. Senhas são armazenadas usando `PasswordHasher`; hashes nunca são retornados. Use segredos fortes e HTTPS nos ambientes publicados.

Em Development, a API permite por padrão as origens `http://localhost:3000` e `http://127.0.0.1:3000`. As demais origens devem ser configuradas explicitamente. A connection string continua configurada fora dos arquivos versionados.

## Testes

Execute os testes de domínio na pasta `backend`:

```powershell
dotnet test .\AIStudyLab.slnx
```

### Executar localmente

Configure a connection string no User Secrets/ambiente e inicie a API:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project .\AIStudyLab.Api --urls http://localhost:5259
```

O OpenAPI fica disponível em `http://localhost:5259/openapi/v1.json`. Após configurar a connection string e aplicar a migration inicial, execute as requisições do arquivo `AIStudyLab.Api.http` pelo VS Code; mantenha a sessão/cookie criado no cadastro ou login.

## Entidades persistidas nesta etapa

`Usuario`, `PerfilUsuario`, `ConfiguracaoUsuario`, `Disciplina`, `Assunto`, `SessaoEstudo`, `Meta` e `Revisao`. Documentos, conversas e entidades de RAG serão adicionados quando os respectivos fluxos forem iniciados.
