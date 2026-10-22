import { Asiento } from './asiento';

export interface MapaAsientos {
  viajeId: number;
  origen: string;
  destino: string;
  salida: string;
  tipoServicio: 'Economico' | 'Ejecutivo' | 'Vip';
  precio: number;
  asientos: Asiento[];
}
