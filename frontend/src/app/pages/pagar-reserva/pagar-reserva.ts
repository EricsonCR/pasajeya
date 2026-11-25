import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BoletoService } from '../../services/boleto.service';
import { DetalleBoleto } from '../../models/detalle-boleto';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { environment } from '../../../environments/environment';
import { PagarBoletoRequest } from '../../models/pagar-boleto-request';
import { firstValueFrom } from 'rxjs';
import { Pago } from '../../models/pago';

declare const MercadoPago: any;

@Component({
  imports: [DatePipe, CurrencyPipe, RouterLink],
  selector: 'app-pagar-reserva',
  styleUrl: './pagar-reserva.scss',
  templateUrl: './pagar-reserva.html',
})
export class PagarReserva implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private boletoService = inject(BoletoService);
  protected detalle = signal<DetalleBoleto | null>(null);
  protected error = signal<string | null>(null);
  protected modalAbierto = signal<boolean>(false);
  private brick: any;
  private claveIdempotencia = '';
  protected pago = signal<Pago | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.boletoService.obtener(Number(id)).subscribe({
      next: (data) => {
        this.detalle.set(data);
      },
      error: () => {
        this.error.set('La reserva no existe');
      },
    });
  }

  abrirModal() {
    this.modalAbierto.set(true);
    this.claveIdempotencia = crypto.randomUUID();
    setTimeout(() => this.crearBrick());
  }

  cerrarModal() {
    this.brick?.unmount();
    this.modalAbierto.set(false);
  }

  private async crearBrick() {
    const mp = new MercadoPago(environment.mercadoPagoPublicKey, { locale: 'es-PE' });
    this.brick = await mp.bricks().create('cardPayment', 'cardPaymentBrick_container', {
      initialization: { amount: this.detalle()!.precio },
      callbacks: {
        onReady: () => {},
        onSubmit: (formData: any) => this.enviarPago(formData),
        onError: (error: any) => console.error(error),
      },
    });
  }

  private async enviarPago(formData: any): Promise<void> {
    const request: PagarBoletoRequest = {
      token: formData.token,
      medioPago: formData.payment_method_id,
      cuotas: formData.installments,
      correoPagador: formData.payer.email,
      claveIdempotencia: this.claveIdempotencia,
    };
    const pago = await firstValueFrom(this.boletoService.pagar(this.detalle()!.id, request));
    this.pago.set(pago);
    this.cerrarModal();
  }

  ngOnDestroy(): void {
    this.brick?.unmount();
  }
}
