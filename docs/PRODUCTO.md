# Producto – PasajeYa

## Visión
Que comprar un pasaje de bus en línea sea rápido y confiable para el pasajero,
y que la empresa de buses administre su operación desde un solo lugar.

## Usuarios
| Usuario | Qué hace |
|---|---|
| **Pasajero** | Busca viajes, elige y reserva su asiento, paga y recibe su boleto. Puede tener una cuenta para ver sus datos y compras. |
| **Administrador** | Gestiona ciudades, rutas, buses y viajes, y revisa los boletos vendidos y reservados. |

## Alcance
- Los pagos usan una pasarela en modo de pruebas: se simulan con tarjetas y billeteras
  de prueba (por ejemplo, Yape), sin dinero real.
- La analítica de ventas y la predicción de demanda quedan para una etapa posterior.

## Reglas de negocio

**Búsqueda**
- Origen y destino deben ser ciudades existentes y distintas.
- La fecha va desde hoy hasta 30 días después.
- Si la fecha es hoy, solo se muestran los viajes que aún no salen.
- Los resultados se ordenan por hora de salida.
- Un viaje sin asientos disponibles se muestra como agotado y no permite elegir asiento.

**Asientos**
- Un asiento está ocupado si tiene un boleto pagado o una reserva vigente.
- Asientos disponibles = capacidad del bus − asientos ocupados.

**Reserva**
- Datos obligatorios: DNI (8 dígitos), nombres, apellidos y correo. Celular opcional (9 dígitos).
- Se reserva un asiento a la vez.
- La reserva bloquea el asiento por 15 minutos para pagar.
- Si la reserva vence sin pago, el asiento vuelve a estar libre.
- Un asiento no puede tener dos reservas vigentes: si otro pasajero lo tomó primero, se avisa y se debe elegir otro.

## Historias de usuario
Detalle en GitHub Issues.

| Épica | HU | Issue |
|---|---|---|
| E1 Búsqueda (#1) | HU-01 Buscar viajes por origen, destino y fecha | #2 |
| E2 Compra (#6) | HU-02 Ver asientos de un viaje y elegir uno | #7 |
| E2 Compra (#6) | HU-03 Reservar un asiento | #9 |
