# DATABASE.md

# Modelagem do Banco de Dados

## Objetivo

O banco de dados deve ser simples, consistente e preparado para crescimento.

O sistema será utilizado inicialmente por apenas um usuário, porém a modelagem deverá permitir futuras expansões sem necessidade de grandes refatorações.

O banco utilizará SQLite através do Prisma ORM.

---

# Convenções

## Nome das tabelas

Sempre utilizar nomes no singular.

Exemplos:

Disciplina

Assunto

SessaoEstudo

Meta

Revisao

Nunca utilizar plural.

---

## Chaves Primárias

Todas as tabelas utilizarão:

* id
* Int
* autoincrement

---

## Datas

Sempre utilizar:

createdAt

updatedAt

Quando necessário:

deletedAt

---

# Entidades

## Concurso

Representa um concurso.

Campos

* id
* nome
* orgao
* cargo
* bancaId
* edital
* ativo
* createdAt
* updatedAt

Relacionamentos

* Um concurso possui várias disciplinas.

---

## Banca

Representa a banca organizadora.

Exemplos

* Cebraspe
* FGV
* FCC
* Cesgranrio

Campos

* id
* nome
* createdAt
* updatedAt

Relacionamentos

* Uma banca possui vários concursos.

---

## Disciplina

Representa uma matéria.

Exemplos

* Português
* Direito Administrativo
* Constitucional
* Informática

Campos

* id
* concursoId
* nome
* descricao
* cor
* icone
* ordem
* ativa
* createdAt
* updatedAt

Relacionamentos

* Uma disciplina possui vários assuntos.
* Uma disciplina possui várias sessões de estudo.

---

## Assunto

Representa um tópico pertencente a uma disciplina.

Exemplo

Disciplina

Português

Assuntos

* Crase
* Regência
* Pontuação
* Concordância

Campos

* id
* disciplinaId
* nome
* descricao
* prioridade
* ativa
* createdAt
* updatedAt

Relacionamentos

* Um assunto possui várias sessões de estudo.
* Um assunto possui várias revisões.

---

## SessaoEstudo

Esta é a entidade principal do sistema.

Todas as estatísticas serão calculadas a partir desta tabela.

Campos

* id
* disciplinaId
* assuntoId
* data
* tipo
* tempoEstudado
* quantidadeQuestoes
* acertos
* erros
* percentualAcerto (calculado em tempo de execução, não armazenado)
* observacoes
* createdAt
* updatedAt

Tipos possíveis

* TEORIA
* QUESTOES
* MISTO

Regras

Quando tipo = TEORIA

* quantidadeQuestoes = 0
* acertos = 0
* erros = 0

Quando tipo = QUESTOES

* tempoEstudado é opcional.

Quando tipo = MISTO

Todos os campos podem ser utilizados.

---

## Revisao

Representa revisões programadas.

Campos

* id
* assuntoId
* dataPrevista
* dataRealizada
* status
* tipo
* createdAt
* updatedAt

Tipos

* 24H
* 7_DIAS
* 30_DIAS
* 90_DIAS

Status

* PENDENTE
* REALIZADA
* ATRASADA

---

## Meta

Representa metas pessoais.

Campos

* id
* titulo
* descricao
* tipo
* valor
* progresso
* dataInicio
* dataFim
* ativa
* createdAt
* updatedAt

Tipos

* QUESTOES
* HORAS
* DIAS_ESTUDADOS
* DISCIPLINAS

---

# Relacionamentos

Banca

└── Concurso

    └── Disciplina

        └── Assunto

            └── SessaoEstudo

            └── Revisao

Concurso

└── Meta

---

# Índices

Criar índices para:

* disciplinaId
* assuntoId
* concursoId
* bancaId
* data

---

# Regras de Negócio

Nunca armazenar estatísticas.

Sempre calcular em tempo de execução.

Exemplos

Não armazenar

* percentual de acertos
* total de questões
* total de horas
* ranking
* médias

Todos esses valores deverão ser obtidos através de consultas ao banco.

---

# Exclusão

Inicialmente utilizar exclusão física.

Caso futuramente seja necessário manter histórico, implementar Soft Delete utilizando o campo deletedAt.

---

# Escalabilidade

A modelagem deverá permitir futuramente:

* múltiplos usuários;
* autenticação;
* sincronização em nuvem;
* importação de questões;
* integração com APIs;
* geração de relatórios;
* Inteligência Artificial para análise do desempenho.

Sem necessidade de alterar a estrutura principal das entidades.
