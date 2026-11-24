import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BoletoService } from '../../services/boleto.service';
import { DetalleBoleto } from '../../models/detalle-boleto';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { environment } from '../../../environments/environment';

declare const MercadoPago: any;

@Component({
  imports: [DatePipe, CurrencyPipe],
  selector: 'app-pagar-reserva',
  styleUrl: './pagar-reserva.scss',
  templateUrl: './pagar-reserva.html',
})
export class PagarReserva implements OnInit {
  private route = inject(ActivatedRoute);
  private boletoService = inject(BoletoService);
  protected detalle = signal<DetalleBoleto | null>(null);
  protected error = signal<string | null>(null);
  protected modalAbierto = signal<boolean>(false);
  private brick: any;

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
        onSubmit: (formData: any) => console.log(formData),
        onError: (error: any) => console.error(error),
      },
    });
  }
}
