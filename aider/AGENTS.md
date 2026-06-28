# AGENTS.md

# Radar de Estudos

Este projeto deve ser desenvolvido seguindo estas regras.

## Objetivo

Sistema pessoal para gerenciamento de estudos para concursos públicos.

O foco inicial será o concurso da Polícia Rodoviária Federal (PRF).

---

# Arquitetura

O projeto deverá permanecer como um monólito.

Tecnologias:

* Next.js 16
* TypeScript
* Tailwind CSS
* Prisma
* SQLite
* React Server Components
* App Router

---

# Organização

Sempre respeitar a seguinte estrutura:

app/

components/

lib/

repositories/

services/

actions/

hooks/

types/

utils/

prisma/

public/

---

# Git

Nunca trabalhar diretamente na branch main.

Nunca trabalhar diretamente na branch develop.

Para cada Sprint criar uma branch:

feature/sprint-XX-nome

Exemplo:

feature/sprint-01-prisma

feature/sprint-02-layout

feature/sprint-03-disciplinas

Ao finalizar uma Sprint:

* realizar commit;
* solicitar merge para develop.

Nunca realizar merge automático para main.

---

# Commits

Utilizar Conventional Commits.

Exemplos:

feat:

fix:

refactor:

docs:

style:

test:

chore:

Exemplo:

feat: cria CRUD de disciplinas

---

# Código

Sempre utilizar:

* Clean Code
* SOLID quando fizer sentido
* Componentização
* Tipagem forte
* Evitar duplicação
* Nomes em português para regras de negócio
* Inglês apenas para APIs, bibliotecas e convenções

---

# Desenvolvimento

Nunca criar grandes funcionalidades em uma única tarefa.

Sempre desenvolver uma Sprint por vez.

Ao finalizar cada Sprint:

* validar funcionamento;
* corrigir erros;
* somente então iniciar a próxima.

---

# Regra principal

Nunca inventar que executou comandos.

Nunca afirmar que criou arquivos sem realmente criá-los.

Sempre utilizar as ferramentas disponíveis para editar arquivos e executar comandos.
