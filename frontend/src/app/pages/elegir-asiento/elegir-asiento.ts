import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ViajeService } from '../../services/viaje.service';
import { MapaAsientos } from '../../models/mapa-asientos';
import { Asiento } from '../../models/asiento';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { BoletoService } from '../../services/boleto.service';
import { ReservarBoletoRequest } from '../../models/reservar-boleto-request';
import { Boleto } from '../../models/boleto';

@Component({
  imports: [DatePipe, CurrencyPipe, ReactiveFormsModule, DatePipe],
  selector: 'app-elegir-asiento',
  styleUrl: './elegir-asiento.scss',
  templateUrl: './elegir-asiento.html',
})
export class ElegirAsiento implements OnInit {
  private route = inject(ActivatedRoute);
  private viajeService = inject(ViajeService);
  private boletoService = inject(BoletoService);
  protected mapa = signal<MapaAsientos | null>(null);
  protected asientoElegido = signal<number | null>(null);
  protected boletoReservado = signal<Boleto | null>(null);
  protected error = signal(false);
  protected errores = signal<string | null>(null);
  protected form = new FormGroup({
    dni: new FormControl<string | null>(null, [Validators.required, Validators.pattern(/^\d{8}$/)]),
    nombres: new FormControl<string | null>(null, Validators.required),
    apellidos: new FormControl<string | null>(null, Validators.required),
    correo: new FormControl<string | null>(null, [Validators.required, Validators.email]),
    celular: new FormControl<string | null>(null, [Validators.pattern(/^\d{9}$/)]),
  });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.cargarMapa(id);
  }

  elegirAsiento(asiento: Asiento) {
    if (asiento.ocupado) return;
    this.asientoElegido.set(asiento.numero);
  }

  reservar() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const datos = this.form.getRawValue();

    const request: ReservarBoletoRequest = {
      viajeId: this.mapa()!.viajeId,
      numeroAsiento: this.asientoElegido()!,
      dni: datos.dni!,
      nombres: datos.nombres!,
      apellidos: datos.apellidos!,
      correo: datos.correo!,
      celular: datos.celular || undefined,
    };

    this.boletoService.reservar(request).subscribe({
      next: (boleto) => {
        this.boletoReservado.set(boleto);
        this.errores.set(null);
      },
      error: (err) => {
        if (err.status === 409) {
          this.errores.set('El asiento ya fue tomado');
          this.asientoElegido.set(null);
          this.cargarMapa(this.mapa()!.viajeId);
        }
      },
    });
  }

  private cargarMapa(id: number) {
    this.viajeService.obtenerMapaAsientos(id).subscribe({
      next: (data) => {
        this.mapa.set(data);
      },
      error: (err) => {
        this.error.set(true);
      },
    });
  }
}
