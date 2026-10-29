import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ViajeService } from '../../services/viaje.service';
import { MapaAsientos } from '../../models/mapa-asientos';
import { Asiento } from '../../models/asiento';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

@Component({
  imports: [DatePipe, CurrencyPipe, ReactiveFormsModule],
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
  protected form = new FormGroup({
    dni: new FormControl<string | null>(null, [Validators.required, Validators.pattern(/^\d{8}$/)]),
    nombres: new FormControl<string | null>(null, Validators.required),
    apellidos: new FormControl<string | null>(null, Validators.required),
    correo: new FormControl<string | null>(null, [Validators.required, Validators.email]),
    celular: new FormControl<string | null>(null, [Validators.pattern(/^\d{9}$/)]),
  });

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

  reservar() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
  }
}
