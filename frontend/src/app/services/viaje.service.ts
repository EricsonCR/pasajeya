import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Ciudad } from '../models/ciudad';
import { Observable } from 'rxjs';
import { Viaje } from '../models/viaje';
import { environment } from '../../environments/environment';
import { MapaAsientos } from '../models/mapa-asientos';

@Injectable({ providedIn: 'root' })
export class ViajeService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  obtenerCiudades(): Observable<Ciudad[]> {
    return this.http.get<Ciudad[]>(`${this.apiUrl}/ciudades`);
  }

  buscarViajes(origen: number, destino: number, fecha: string): Observable<Viaje[]> {
    return this.http.get<Viaje[]>(`${this.apiUrl}/viajes`, { params: { origen, destino, fecha } });
  }

  obtenerMapaAsientos(viajeId: number): Observable<MapaAsientos> {
    return this.http.get<MapaAsientos>(`${this.apiUrl}/viajes/${viajeId}/asientos`);
  }
}
