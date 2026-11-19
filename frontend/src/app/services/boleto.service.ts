import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ReservarBoletoRequest } from '../models/reservar-boleto-request';
import { Observable } from 'rxjs';
import { Boleto } from '../models/boleto';
import { DetalleBoleto } from '../models/detalle-boleto';

@Injectable({ providedIn: 'root' })
export class BoletoService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  reservar(request: ReservarBoletoRequest): Observable<Boleto> {
    return this.http.post<Boleto>(`${this.apiUrl}/boletos`, request);
  }

  obtener(id: number): Observable<DetalleBoleto> {
    return this.http.get<DetalleBoleto>(`${this.apiUrl}/boletos/${id}`);
  }
}
