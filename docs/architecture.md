# AIStudy Lab

## AIStudy Lab - Software Architecture & Engineering Guide

**Versão:** 1.0
**Status:** Em elaboração
**Autor:** Igor Miranda
**Data:** *(preencher posteriormente)*

---

# 1. Introdução

O **AIStudy Lab** é uma plataforma de estudos potencializada por Inteligência Artificial, desenvolvida com o objetivo de unir organização dos estudos, gestão de conteúdo e experimentação de tecnologias modernas de IA em uma única aplicação.

Diferentemente de um simples sistema para controle de estudos ou de um chatbot baseado em LLM, o AIStudy Lab será um ambiente completo para aprendizado, pesquisa e desenvolvimento, permitindo tanto o acompanhamento da evolução dos estudos quanto a exploração prática de conceitos como Large Language Models (LLMs), Retrieval-Augmented Generation (RAG), bancos vetoriais, processamento inteligente de documentos, visão computacional e agentes de IA.

O projeto nasceu a partir da evolução do sistema **Radar de Estudos**, expandindo seu propósito para se tornar uma plataforma modular, preparada para crescer continuamente com novos recursos e experimentos relacionados à Inteligência Artificial.

---

# 2. Objetivo deste documento

Este documento descreve toda a arquitetura de software do AIStudy Lab e servirá como a principal referência técnica durante o desenvolvimento do projeto.

Seu objetivo é garantir que todas as decisões arquiteturais sejam documentadas antes da implementação, permitindo que o desenvolvimento aconteça de forma organizada, consistente e escalável.

Este documento também será utilizado como base para orientar ferramentas de Inteligência Artificial, como GitHub Copilot, ChatGPT e outros assistentes de programação, reduzindo ambiguidades e garantindo que futuras implementações respeitem a arquitetura definida.

Ao longo do desenvolvimento, este documento será continuamente atualizado para refletir novas decisões arquiteturais.

---

# 3. Objetivos do Projeto

O AIStudy Lab possui dois grandes objetivos.

## Objetivo funcional

Disponibilizar uma plataforma moderna para gerenciamento dos estudos, permitindo que o usuário acompanhe toda sua evolução por meio de dashboards, disciplinas, sessões de estudo, revisões, estatísticas, metas, simulados, flashcards e demais ferramentas de produtividade.

## Objetivo técnico

Servir como um laboratório prático para aprendizado de Inteligência Artificial aplicada ao desenvolvimento de software.

O projeto será utilizado para estudar e implementar tecnologias como:

* Large Language Models (LLMs)
* Retrieval-Augmented Generation (RAG)
* Embeddings
* Bancos Vetoriais
* Processamento de documentos (PDF, Word, Imagens)
* OCR
* Visão Computacional
* Agentes Inteligentes
* Prompt Engineering
* MCP (Model Context Protocol)
* Observabilidade aplicada a IA

Dessa forma, o AIStudy Lab não será apenas um sistema para estudos, mas também uma plataforma de experimentação tecnológica.

---

# 4. Escopo

O sistema será composto por diversos módulos independentes, cada um responsável por uma área específica da aplicação.

Entre eles:

* Dashboard
* Gestão de disciplinas
* Gestão de assuntos
* Sessões de estudo
* Revisões
* Estatísticas
* Biblioteca de documentos
* Tutor Inteligente
* Chat baseado em RAG
* Flashcards
* Gerador de simulados
* Playground de IA
* Laboratório de Embeddings
* Busca Vetorial
* Comparação entre modelos de IA
* Configuração de modelos locais
* Administração do sistema

Todos esses módulos compartilharão a mesma identidade visual e seguirão os mesmos padrões arquiteturais definidos neste documento.

---

# 5. Arquitetura Geral

O projeto será desenvolvido utilizando a abordagem de **Monorepo**.

Isso significa que todas as aplicações farão parte do mesmo repositório Git, compartilhando documentação, infraestrutura e padrões de desenvolvimento, mas permanecendo desacopladas e com responsabilidades bem definidas.

É importante destacar que **Monorepo não significa Monólito**.

Embora todos os projetos estejam armazenados em um único repositório, cada aplicação será executada de forma independente, podendo evoluir, ser implantada e escalada individualmente quando necessário.

Essa abordagem simplifica o desenvolvimento durante as fases iniciais do projeto, mantendo uma arquitetura preparada para crescimento futuro.

A estrutura inicial será composta pelos seguintes componentes:

* Frontend (Next.js)
* Backend (.NET)
* Serviço de Inteligência Artificial (Python)
* Banco de Dados Relacional
* Banco Vetorial
* Infraestrutura Docker
* Documentação

Cada componente possuirá responsabilidades claramente definidas.

---

# 6. Organização do Repositório (Monorepo)

O AIStudy Lab será organizado em um único repositório Git, contendo todas as aplicações necessárias para execução do sistema.

A estrutura inicial será semelhante a:

```text
AIStudyLab/

frontend/

backend/

python/

infra/

docs/

scripts/

tests/
```

Cada diretório representará uma responsabilidade específica da solução.

Essa organização facilita:

* versionamento
* documentação
* automação
* integração contínua
* compartilhamento de código
* manutenção
* evolução da arquitetura

---

# 7. Estrutura deste Documento

Este Software Architecture Document será dividido em diversos capítulos, abordando detalhadamente cada aspecto da solução.

Os principais capítulos serão:

* Visão Geral do Produto
* Arquitetura da Solução
* Organização do Monorepo
* Arquitetura Frontend
* Arquitetura Backend (.NET)
* Arquitetura do Serviço de IA (Python)
* Arquitetura de Dados
* Banco Vetorial
* Processamento de Documentos
* Sistema RAG
* Integração com LLMs
* Infraestrutura
* Segurança
* Observabilidade
* Estratégia de Testes
* Padrões de Desenvolvimento
* Convenções de Código
* Fluxos da Aplicação
* Roadmap de Desenvolvimento
* ADRs (Architecture Decision Records)

O modelo lógico de dados e as diretrizes para sua implementação com Entity Framework Core estão documentados em [database.md](./database.md).

---

# 8. Filosofia do Projeto

O AIStudy Lab será desenvolvido priorizando simplicidade, modularidade e aprendizado contínuo.

As decisões arquiteturais deverão favorecer:

* Baixo acoplamento.
* Alta coesão.
* Separação clara de responsabilidades.
* Evolução incremental.
* Código limpo e de fácil manutenção.
* Facilidade para integração com novas tecnologias de IA.
* Reutilização de componentes.
* Documentação como parte integrante do desenvolvimento.

Toda nova funcionalidade deverá respeitar a arquitetura estabelecida neste documento.

O objetivo não é apenas construir uma aplicação funcional, mas criar uma plataforma de referência para estudos, experimentação e aprendizado em Inteligência Artificial aplicada ao desenvolvimento de software.
