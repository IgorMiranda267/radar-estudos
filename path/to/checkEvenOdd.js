/**
 * Verifica se um número é par ou ímpar
 * @param {number} numero - O número a ser verificado
 * @returns {string} Retorna "par" ou "ímpar" conforme o resultado
 */
function verificarParImpar(numero) {
  if (numero % 2 === 0) {
    return 'par';
  } else {
    return 'ímpar';
  }
}

// Exemplo de uso
console.log(verificarParImpar(4)); // Saída: par
console.log(verificarParImpar(7)); // Saída: ímpar
