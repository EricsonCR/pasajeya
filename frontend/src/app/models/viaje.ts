export interface Viaje {
  id: number;
  salida: string;
  llegada: string;
  tipoServicio: 'Economico' | 'Ejecutivo' | 'Vip';
  precio: number;
  asientosDisponibles: number;
}
