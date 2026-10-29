export interface ReservarBoletoRequest {
  viajeId: number;
  numeroAsiento: number;
  dni: string;
  nombres: string;
  apellidos: string;
  correo: string;
  celular?: string;
}
