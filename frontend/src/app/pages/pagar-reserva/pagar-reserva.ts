import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BoletoService } from '../../services/boleto.service';
import { DetalleBoleto } from '../../models/detalle-boleto';
import { CurrencyPipe, DatePipe } from '@angular/common';

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
}
