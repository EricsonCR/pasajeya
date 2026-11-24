export interface DetalleBoleto {
  id: number;
  estado: string;
  origen: string;
  destino: string;
  salida: string;
  numeroAsiento: number;
  precio: number;
  nombres: string;
  apellidos: string;
  dni: string;
  reservadoHasta: string;
}
