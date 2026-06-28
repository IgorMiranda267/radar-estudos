import { PrismaClient } from '@prisma/client'

if (process.env.NODE_ENV === 'production') {
  throw new Error(
    'Prisma Client cannot be used in production. ' +
    'See https://pris.ly/d/client-production for more info'
  )
}

const prisma = new PrismaClient()

export default prisma
