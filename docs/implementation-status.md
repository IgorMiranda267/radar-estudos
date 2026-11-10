# Acompanhamento da Implementação

Este documento acompanha o que já foi implementado no AI Study Lab e o que falta para chegar ao escopo definido. Atualize os itens conforme cada etapa for concluída.

## Status atual

O núcleo de acompanhamento de estudos está integrado entre o frontend Next.js, a API ASP.NET Core e o PostgreSQL. Autenticação por cookie HttpOnly, autorização baseada na sessão e perfil do usuário já foram implementados.

## Implementado

### Base técnica

- [x] Monorepo com frontend Next.js/TypeScript e backend .NET/C#.
- [x] PostgreSQL acessado pelo Entity Framework Core.
- [x] Entidades e migration inicial para usuários, perfis, configurações, disciplinas, assuntos, sessões, metas e revisões.
- [x] API com OpenAPI, tratamento de erros e CORS local.
- [x] Endpoints de estudo autorizados pela sessão autenticada; OpenAPI fica limitado a `Development`.

### Fluxos do núcleo de estudos

- [x] Registrar uma conta e entrar/sair com sessão protegida por cookie HttpOnly.
- [x] Criar, editar e desativar disciplinas.
- [x] Criar, editar e excluir assuntos; agendar revisões a partir deles.
- [x] Cronometrar e registrar sessões de estudo.
- [x] Consultar o dashboard com dados reais.
- [x] Consultar estatísticas e histórico com base nas sessões registradas.
- [x] Criar, editar e excluir metas; calcular progresso com sessões e revisões.
- [x] Consultar, concluir, reabrir, cancelar, reagendar e excluir revisões.
- [x] Carregar e salvar preferências de usuário.
- [x] Consultar e editar o perfil com validação dos campos e da data de nascimento.
- [x] Estados de carregamento, erro e conteúdo vazio nas telas conectadas.

## Pendente para completar o MVP definido

### Prioridade 1 — Contas e segurança

- [x] Implementar autenticação real e emissão/validação de sessão por cookie HttpOnly.
- [x] Implementar autorização e garantir que cada usuário só acesse os próprios dados.
- [x] Substituir a seleção local de conta e o `usuarioId` enviado pelo navegador como identificador de acesso.
- [x] Criar testes de autenticação, autorização, origem e isolamento de dados.
- [x] Exigir autenticação para todas as rotas de estudo; somente cadastro e login são públicos.

### Prioridade 2 — Perfil

- [x] Criar endpoints e interface para consultar e editar o perfil do usuário.
- [x] Validar comprimentos dos campos e impedir data de nascimento futura.

### Prioridade 3 — Biblioteca de documentos

- [ ] Criar a tela de biblioteca e o fluxo de upload.
- [ ] Definir limites de tamanho e tipos de arquivo aceitos.
- [ ] Definir armazenamento dos arquivos; guardar no banco somente metadados e referências.
- [ ] Criar entidades, migration e endpoints para documentos.
- [ ] Permitir associar documentos a assuntos, conforme o modelo planejado.
- [ ] Cobrir upload, remoção, validação e acesso por usuário com testes.

### Prioridade 4 — Tutor IA e conversas

- [ ] Definir e implementar o serviço de IA em Python e sua integração com a API .NET.
- [ ] Escolher/configurar provedor ou modelo de IA.
- [ ] Criar telas do tutor e do histórico de chat.
- [ ] Criar entidades, migration e endpoints para sessões e mensagens.
- [ ] Exibir estados de erro e indisponibilidade do serviço de IA sem simular respostas.

### Prioridade 5 — RAG e processamento de documentos

- [ ] Extrair texto dos documentos e acompanhar o estado do processamento.
- [ ] Dividir o conteúdo em trechos e persistir os resultados.
- [ ] Escolher o modelo de embeddings, dimensão vetorial e estratégia de atualização.
- [ ] Avaliar/habilitar `pgvector` e implementar recuperação de fontes.
- [ ] Responder no chat com referências aos documentos usados.

## Melhorias e decisões em aberto

- [ ] Aplicar na interface as preferências de tema e idioma; atualmente são persistidas, mas a interface permanece em tema claro e português.
- [ ] Remover o acesso Prisma/SQLite legado do frontend depois de confirmar que nenhum fluxo ativo depende dele.
- [ ] Definir política de exclusão/anonimização de conta e retenção dos dados.
- [ ] Configurar origens do frontend e HTTPS para cada ambiente de implantação.
- [ ] Definir notificações e estratégia de envio; a preferência pode ser salva, mas o envio de notificações não está implementado.
- [ ] Decidir se flashcards, simulados e ferramentas de laboratório de IA entram no primeiro MVP ou em versões posteriores.
- [ ] Reavaliar lint frontend: o build de produção passou, mas a regra `react-hooks/set-state-in-effect` aponta carregamentos em efeitos em várias telas.

## Validações já realizadas

- [x] Build de produção do frontend concluído.
- [x] Build da solução .NET concluído sem avisos ou erros.
- [x] Testes de domínio e integração da API para autenticação, autorização, perfil e isolamento.
- [x] Smoke test local: `/health` e `/openapi/v1.json` responderam com HTTP 200.
- [ ] Testes dos fluxos completos usando o PostgreSQL de desenvolvimento.
- [ ] Executar novamente build e testes depois de cada etapa pendente.

## Referências

- Modelo de dados e etapas: [database.md](./database.md)
- Arquitetura planejada: [architecture.md](./architecture.md)
- Telas MVP inicialmente definidas: [MVP_Screens.md](./09_UI_UX/MVP_Screens.md)
- Navegação planejada: [Navigation.md](./09_UI_UX/Navigation.md)
- Instruções do backend: [README.md](../backend/README.md)
