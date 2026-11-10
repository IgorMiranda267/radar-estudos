import { PrismaClient } from "@prisma/client";

declare global {
  var prisma: PrismaClient | undefined;
}

const resolvedDatabaseUrl = process.env.DATABASE_URL?.startsWith("file:")
  ? process.env.DATABASE_URL
  : "file:./dev.db";

if (!process.env.DATABASE_URL || !process.env.DATABASE_URL.startsWith("file:")) {
  process.env.DATABASE_URL = resolvedDatabaseUrl;
}

const client = globalThis.prisma || new PrismaClient({
  datasources: {
    db: {
      url: resolvedDatabaseUrl,
    },
  },
});

if (process.env.NODE_ENV !== "production") {
  globalThis.prisma = client;
}

export default client;
