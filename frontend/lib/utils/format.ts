export function formatHours(hours: number) {
  const rounded = Math.round(hours * 10) / 10;
  return `${rounded.toFixed(1)}h`;
}
