# PROJECT.md

# Radar de Estudos

## Visão Geral

O Radar de Estudos é um sistema web desenvolvido para gerenciamento completo dos estudos para concursos públicos.

O objetivo é permitir acompanhar a evolução dos estudos através de métricas, estatísticas, gráficos e relatórios inteligentes, identificando pontos fortes e fracos em cada disciplina e assunto.

Inicialmente o sistema será utilizado para preparação para o concurso da Polícia Rodoviária Federal (PRF), porém deverá ser desenvolvido de forma genérica para suportar qualquer concurso futuramente.

---

# Objetivos

O sistema deverá permitir:

* acompanhar a evolução dos estudos;
* registrar sessões de estudo;
* controlar questões resolvidas;
* controlar acertos e erros;
* acompanhar desempenho por disciplina;
* acompanhar desempenho por assunto;
* gerar estatísticas;
* gerar gráficos;
* identificar automaticamente pontos que necessitam de revisão.

O sistema será utilizado apenas por um usuário.

Não haverá autenticação na primeira versão.

---

# Tecnologias

Frontend

* Next.js 16
* React
* TypeScript
* Tailwind CSS

Backend

* Next.js Server Actions
* Prisma ORM

Banco de Dados

* SQLite

Bibliotecas

* Shadcn UI
* Lucide React
* Recharts
* React Hook Form
* Zod

---

# Arquitetura

Aplicação Monolítica.

Toda a aplicação ficará em um único projeto Next.js.

Separação por camadas:

* Components
* Actions
* Services
* Repositories
* Prisma
* Hooks
* Utils

Toda regra de negócio deverá ficar em Services.

Repositories serão responsáveis exclusivamente pelo acesso ao banco.

Components nunca deverão acessar o banco diretamente.

---

# Estrutura

```
app/

components/

components/ui/

components/layout/

components/dashboard/

components/disciplinas/

components/assuntos/

components/estudos/

components/estatisticas/

lib/

repositories/

services/

actions/

hooks/

types/

utils/

prisma/

public/
```

---

# Entidades

## Disciplina

Representa uma matéria do concurso.

Exemplos

* Português
* Direito Constitucional
* Direito Administrativo
* Informática

Campos

* id
* nome
* cor
* ativa
* createdAt
* updatedAt

---

## Assunto

Cada disciplina possui vários assuntos.

Exemplo

Disciplina:

Português

Assuntos

* Crase
* Pontuação
* Concordância
* Regência

Campos

* id
* disciplinaId
* nome
* observacao

---

## Sessão de Estudo

Representa um estudo realizado.

Campos

* id
* disciplinaId
* assuntoId
* data
* tempoEstudo
* quantidadeQuestoes
* acertos
* erros
* observacoes

---

# Dashboard

O Dashboard deverá apresentar:

* Total de disciplinas
* Total de assuntos
* Total de sessões
* Total de questões
* Total de acertos
* Total de erros
* Percentual geral de acertos
* Tempo total estudado

Também deverá possuir gráficos.

---

# Gráficos

Entre os gráficos previstos:

* Evolução diária
* Evolução semanal
* Evolução mensal
* Acertos por disciplina
* Erros por disciplina
* Acertos por assunto
* Erros por assunto
* Ranking de disciplinas
* Ranking de assuntos

---

# Relatórios Inteligentes

O sistema deverá identificar automaticamente:

* disciplina com pior desempenho;
* assunto com maior índice de erro;
* disciplina menos estudada;
* disciplina mais estudada;
* evolução dos últimos 30 dias;
* evolução dos últimos 90 dias;
* percentual geral de aproveitamento.

---

# Funcionalidades Futuras

* Sistema de metas
* Cronograma de estudos
* Revisões 24h / 7 dias / 30 dias
* Importação de questões
* Backup
* Exportação para Excel
* Exportação para PDF
* Tema escuro
* Configurações
* Inteligência Artificial para análise do desempenho

---

# Roadmap

Sprint 0

* Criação do projeto

Sprint 1

* Configuração do Prisma
* SQLite
* Estrutura inicial

Sprint 2

* Arquitetura do projeto

Sprint 3

* Layout principal

Sprint 4

* CRUD de Disciplinas

Sprint 5

* CRUD de Assuntos

Sprint 6

* Registro das Sessões de Estudo

Sprint 7

* Dashboard

Sprint 8

* Estatísticas

Sprint 9

* Relatórios

Sprint 10

* Revisões

Sprint 11

* Metas

Sprint 12

* Melhorias visuais

---

# Padrões de Desenvolvimento

* Clean Code
* SOLID quando aplicável
* Componentização
* Código fortemente tipado
* Evitar duplicação
* Código simples
* Performance antes de otimizações complexas
* Sempre documentar decisões importantes

---

# Objetivo Final

Construir uma aplicação moderna, organizada, escalável e visualmente agradável, que permita acompanhar toda a evolução dos estudos através de indicadores confiáveis e gráficos intuitivos.

O projeto deverá servir tanto como ferramenta pessoal de estudos quanto como exemplo de boas práticas de desenvolvimento Full Stack utilizando Next.js.
