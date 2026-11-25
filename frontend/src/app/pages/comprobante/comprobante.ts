import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BoletoService } from '../../services/boleto.service';
import { DetalleBoleto } from '../../models/detalle-boleto';

@Component({
  imports: [],
  selector: 'app-comprobante',
  styleUrl: './comprobante.scss',
  templateUrl: './comprobante.html',
})
export class Comprobante implements OnInit {
  private route = inject(ActivatedRoute);
  private boletoService = inject(BoletoService);
  protected detalleBoleto = signal<DetalleBoleto | null>(null);
  protected error = signal<string | null>(null);

  ngOnInit(): void {
    const idBoleto = this.route.snapshot.paramMap.get('id');
    this.boletoService.obtener(Number(idBoleto)).subscribe({
      next: (data) => {
        this.detalleBoleto.set(data);
      },
      error: () => {
        this.error.set('El boleto no existe');
      },
    });
  }
}
