export interface PagarBoletoRequest {
  token: string;
  medioPago: string;
  correoPagador: string;
  claveIdempotencia: string;
  cuotas: number;
}
