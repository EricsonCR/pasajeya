import { Routes } from '@angular/router';
import { BuscarViajes } from './pages/buscar-viajes/buscar-viajes';
import { ElegirAsiento } from './pages/elegir-asiento/elegir-asiento';
import { PagarReserva } from './pages/pagar-reserva/pagar-reserva';
import { Comprobante } from './pages/comprobante/comprobante';

export const routes: Routes = [
  { path: '', component: BuscarViajes },
  { path: 'viajes/:id/asientos', component: ElegirAsiento },
  { path: 'boletos/:id/pago', component: PagarReserva },
  { path: 'boletos/:id/comprobante', component: Comprobante },
];
