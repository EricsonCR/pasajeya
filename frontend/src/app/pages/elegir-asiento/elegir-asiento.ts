import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ViajeService } from '../../services/viaje.service';
import { MapaAsientos } from '../../models/mapa-asientos';
import { Asiento } from '../../models/asiento';
import { CurrencyPipe, DatePipe } from '@angular/common';

@Component({
  imports: [DatePipe, CurrencyPipe],
  selector: 'app-elegir-asiento',
  styleUrl: './elegir-asiento.scss',
  templateUrl: './elegir-asiento.html',
})
export class ElegirAsiento implements OnInit {
  private route = inject(ActivatedRoute);
  private viajeService = inject(ViajeService);
  protected mapa = signal<MapaAsientos | null>(null);
  protected asientoElegido = signal<number | null>(null);
  protected error = signal(false);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.viajeService.obtenerMapaAsientos(id).subscribe({
      next: (data) => {
        this.mapa.set(data);
      },
      error: (err) => {
        this.error.set(true);
      },
    });
  }

  elegirAsiento(asiento: Asiento) {
    if (asiento.ocupado) return;
    this.asientoElegido.set(asiento.numero);
  }
}
