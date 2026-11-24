# API – PasajeYa

Fechas en hora local de Perú, sin zona horaria (`2026-10-05T06:00:00`).

## Listar ciudades

**URL:** `GET /api/ciudades`

**Response 200:** lista de ciudades.
```json
[
  { "id": 1, "nombre": "Lima" },
  { "id": 2, "nombre": "Arequipa" }
]
```

## Buscar viajes

**URL:** `GET /api/viajes?origen={id}&destino={id}&fecha={yyyy-MM-dd}`
- `origen` y `destino`: ids de ciudades existentes y distintas.
- `fecha`: desde hoy hasta 30 días después.

**Response 200:** viajes ordenados por hora de salida. Sin resultados: `[]`. Si la fecha es hoy, solo trae los viajes que aún no salen. `asientosDisponibles: 0` = viaje agotado.
```json
[ {
    "id": 10,
    "salida": "2026-10-05T06:00:00",
    "llegada": "2026-10-05T14:00:00",
    "tipoServicio": "Ejecutivo",
    "precio": 69.90,
    "asientosDisponibles": 12
} ]
```

**Response 400:** datos inválidos. También ocurre si una ciudad no existe o la fecha está fuera de rango.
```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "destino": ["Origen y destino no pueden ser iguales"]
  }
}
```

## Ver mapa de asientos

**URL:** `GET /api/viajes/{id}/asientos`

**Response 200:** datos del viaje y estado de cada asiento. `asientos` trae todos los asientos del bus, del 1 a la capacidad. `ocupado: true` = el asiento tiene un boleto pagado o una reserva vigente.
```json
{
  "viajeId": 10,
  "origen": "Lima",
  "destino": "Arequipa",
  "salida": "2026-10-17T06:00:00",
  "tipoServicio": "Ejecutivo",
  "precio": 69.90,
  "asientos": [
    { "numero": 1, "ocupado": false },
    { "numero": 2, "ocupado": true }
  ]
}
```

**Response 404:** el viaje no existe.
```json
{ "status": 404, "detail": "El viaje 99999 no existe" }
```

## Reservar asiento

**URL:** `POST /api/boletos`

**Request:** `celular` es opcional.
```json
{
  "viajeId": 10,
  "numeroAsiento": 5,
  "dni": "12345678",
  "nombres": "Ana",
  "apellidos": "Pérez Soto",
  "correo": "ana@mail.com",
  "celular": "987654321"
}
```

**Response 201:** reserva creada. `reservadoHasta` = hora de la reserva + 15 minutos.
```json
{
  "id": 120,
  "viajeId": 10,
  "numeroAsiento": 5,
  "estado": "Reservado",
  "reservadoHasta": "2026-10-27T10:15:00"
}
```

**Response 400:** datos inválidos. También: celular sin 9 dígitos, correo con formato inválido, asiento fuera de rango (1 a la capacidad) o viaje que ya salió.
```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "dni": ["El DNI debe tener 8 dígitos."],
    "correo": ["El correo es obligatorio."]
  }
}
```

**Response 404:** el viaje no existe.
```json
{ "status": 404, "detail": "El viaje 99999 no existe" }
```

**Response 409:** el asiento ya está ocupado (pagado o con una reserva vigente).
```json
{ "status": 409, "detail": "El número asiento 5 ya está ocupado" }
```

## Ver reserva

**URL:** `GET /api/boletos/{id}`

**Response 200:** datos del boleto para la pantalla de pago.
```json
{
  "id": 120,
  "estado": "Reservado",
  "origen": "Lima",
  "destino": "Arequipa",
  "salida": "2026-10-27T14:00:00",
  "numeroAsiento": 5,
  "precio": 69.90,
  "nombres": "Ana",
  "apellidos": "Pérez Soto",
  "dni": "12345678",
  "reservadoHasta": "2026-10-27T10:15:00"
}
```

**Response 404:** el boleto no existe.
```json
{ "status": 404, "detail": "El boleto 99999 no existe" }
```

## Pagar reserva

**URL:** `POST /api/boletos/{id}/pago`

**Request:** `token`, `medioPago` y `cuotas` los entrega el formulario de tarjeta de Mercado Pago. El monto no viaja: sale del precio del viaje. `claveIdempotencia` se genera en el front cada vez que se abre el formulario; si se repite, no se cobra dos veces.
```json
{
  "token": "ff8080814c11e237014c1ff593b57b4d",
  "medioPago": "master",
  "cuotas": 1,
  "correoPagador": "ana@mail.com",
  "claveIdempotencia": "7f3c2a9e-1b4d-4e8a-9c21-5d6f7a8b9c0d"
}
```

**Response 200:** pago aprobado, el boleto pasa a `Pagado`.
```json
{
  "id": 120,
  "estado": "Pagado",
  "numeroOperacion": "1234567890",
  "fechaPago": "2026-10-27T10:05:00",
  "monto": 69.90
}
```

**Response 400:** datos inválidos (token, medio de pago, correo o clave vacíos, o cuotas menores a 1).
```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "token": ["El token es obligatorio."]
  }
}
```

**Response 404:** el boleto no existe.
```json
{ "status": 404, "detail": "El boleto 99999 no existe" }
```
