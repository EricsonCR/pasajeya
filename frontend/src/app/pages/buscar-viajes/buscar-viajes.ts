import { Component, inject, OnInit, signal } from '@angular/core';
import { ViajeService } from '../../services/viaje.service';
import { Ciudad } from '../../models/ciudad';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Viaje } from '../../models/viaje';
import { CurrencyPipe, DatePipe } from '@angular/common';

@Component({
  imports: [ReactiveFormsModule, DatePipe, CurrencyPipe],
  selector: 'app-buscar-viajes',
  styleUrl: './buscar-viajes.scss',
  templateUrl: './buscar-viajes.html',
})
export class BuscarViajes implements OnInit {
  private viajeService = inject(ViajeService);
  protected ciudades = signal<Ciudad[]>([]);
  protected viajes = signal<Viaje[]>([]);
  protected flagViajes = signal(false);
  protected errores = signal<string[]>([]);

  protected form = new FormGroup({
    origen: new FormControl<number | null>(null, Validators.required),
    destino: new FormControl<number | null>(null, Validators.required),
    fecha: new FormControl<string | null>('', Validators.required),
  });

  ngOnInit(): void {
    this.viajeService.obtenerCiudades().subscribe({
      next: (ciudades) => {
        this.ciudades.set(ciudades);
      },
      error: (err) => {
        this.errores.set(['No se pudieron cargar las ciudades. Intente más tarde.']);
      },
    });
  }

  buscar() {
    this.errores.set([]);
    this.viajes.set([]);
    this.flagViajes.set(false);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { origen, destino, fecha } = this.form.getRawValue();

    this.viajeService.buscarViajes(origen!, destino!, fecha!).subscribe({
      next: (viajes) => {
        this.viajes.set(viajes);
        this.flagViajes.set(true);
      },
      error: (err) => {
        if (err.status === 400) {
          const mensajes = Object.values(err.error.errors).flat() as string[];
          this.errores.set(mensajes);
        } else {
          this.errores.set(['No se pudo conectar con el servidor. Intente más tarde.']);
        }
      },
    });
  }
}
