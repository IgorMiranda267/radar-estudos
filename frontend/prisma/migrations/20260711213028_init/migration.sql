-- CreateTable
CREATE TABLE "Disciplina" (
    "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "nome" TEXT NOT NULL,
    "cor" TEXT NOT NULL,
    "icone" TEXT NOT NULL,
    "ativa" BOOLEAN NOT NULL DEFAULT true,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL
);

-- CreateTable
CREATE TABLE "Assunto" (
    "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "disciplinaId" INTEGER NOT NULL,
    "nome" TEXT NOT NULL,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL,
    CONSTRAINT "Assunto_disciplinaId_fkey" FOREIGN KEY ("disciplinaId") REFERENCES "Disciplina" ("id") ON DELETE RESTRICT ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "SessaoEstudo" (
    "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "disciplinaId" INTEGER NOT NULL,
    "assuntoId" INTEGER NOT NULL,
    "banca" TEXT NOT NULL,
    "data" DATETIME NOT NULL,
    "tempoEstudado" REAL NOT NULL,
    "questoes" INTEGER NOT NULL,
    "acertos" INTEGER NOT NULL,
    "erros" INTEGER NOT NULL,
    "observacoes" TEXT,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL,
    CONSTRAINT "SessaoEstudo_disciplinaId_fkey" FOREIGN KEY ("disciplinaId") REFERENCES "Disciplina" ("id") ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT "SessaoEstudo_assuntoId_fkey" FOREIGN KEY ("assuntoId") REFERENCES "Assunto" ("id") ON DELETE RESTRICT ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "Meta" (
    "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "tipo" TEXT NOT NULL,
    "quantidade" INTEGER NOT NULL,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL
);

-- CreateTable
CREATE TABLE "Revisao" (
    "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "assuntoId" INTEGER NOT NULL,
    "data" DATETIME NOT NULL,
    "status" TEXT NOT NULL,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL,
    CONSTRAINT "Revisao_assuntoId_fkey" FOREIGN KEY ("assuntoId") REFERENCES "Assunto" ("id") ON DELETE RESTRICT ON UPDATE CASCADE
);
