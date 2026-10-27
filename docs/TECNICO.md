# Técnico – PasajeYa

## Arquitectura
Clean Architecture ligera:
- **Domain:** entidades y reglas de negocio
- **Application:** casos de uso y DTOs
- **Infrastructure:** EF Core y SQL Server
- **Api:** controllers y configuración

Monorepo: `backend/`, `frontend/`, `analytics/`, `docs/`.

## Decisiones técnicas
- .NET 10 (LTS), solución `.slnx`, API con controllers
- SQL Server 2022 local (Docker más adelante); connection string en User Secrets (`ConnectionStrings:PasajeYa`)
- Swagger UI para probar la API
- Idioma: dominio en español (`Viaje`, `Ruta`), términos técnicos en inglés (`Controller`, `Dto`)
- Patrón repositorio: interfaz en Application, implementación en Infrastructure
- Errores: `AppException` (abstracta, con `StatusCode`) → `ValidacionException` (400), `NoEncontradoException` (404) y `ConflictoException` (409); `AppExceptionHandler` global responde con `ProblemDetails`.
- `TimeProvider` inyectado para reglas que dependen de "hoy" o "ahora"
- CORS en Api, orígenes en `appsettings` (`Cors:OrigenesPermitidos`)
- Frontend: Angular 22 + SCSS, sin SSR, Reactive Forms y signals; carpetas `models/`, `services/`, `pages/`; siempre se importa el `environment` base

## Modelo de datos
| Tabla | Campos |
|---|---|
| Ciudad | Id, Nombre |
| Ruta | Id, OrigenId → Ciudad, DestinoId → Ciudad |
| Bus | Id, Placa, TipoServicio, Capacidad |
| Viaje | Id, RutaId, BusId, Salida, Llegada, Precio |
| Boleto | Id, ViajeId, NumeroAsiento, Estado, Dni, Nombres, Apellidos, Correo, Celular, ReservadoHasta |

- Id `int` autoincremental; sin auditoría por ahora.
- `DeleteBehavior.Restrict` en todas las FK.
- Ruta: origen distinto de destino (CHECK) y sin rutas duplicadas (índice único).
- `Ciudad.Nombre`: máx. 100, único. `Bus.Placa`: máx. 10, única.
- Enums guardados como texto (máx. 20).
- `Boleto.Estado`: Reservado, Pagado, Anulado.
- Asientos disponibles = Capacidad del bus − boletos activos del viaje (regla en `Viaje.AsientosDisponibles()`).
- Configuración por entidad con `IEntityTypeConfiguration` en `Infrastructure/Data/Configurations/`.
- Boleto: `Dni` char(8); `Nombres`/`Apellidos` máx. 100; `Correo` máx. 150; `Celular` máx. 9 y opcional; `ReservadoHasta` opcional.
- Activos = Pagado + Reservado con ReservadoHasta > ahora. La reserva dura 15 min.
- Índice único filtrado (ViajeId, NumeroAsiento) para Estado IN (`Reservado`,`Pagado`).
- Boleto se amplía en HU-05.

## API
**Zona horaria:** hora local de Perú (`DateTime`), sin UTC.

**GET /api/ciudades** → 200
```json
[ {
    "id": 1,
    "nombre": "Lima"
} ]
```

**GET /api/viajes?origen={id}&destino={id}&fecha={yyyy-MM-dd}** → 200 / 400
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
- Agotado = `asientosDisponibles: 0`
- Sin resultados = `[]`
- Errores 400 con `ProblemDetails` estándar (agrupados por campo)

**GET /api/viajes/{id}/asientos** → 200 / 404
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
- **asientos:** incluye todos los de la capacidad del bus, por ejemplo: si el bus tiene capacidad=34, se incluye 34 items en el arreglo de asientos.
- **ocupado:** solo es true si el boleto está activo (ver Modelo de datos).
- El 404 cuando el viaje no existe y responde con ProblemDetails.

**POST /api/boletos** → 201 / 400 / 404 / 409
```json
// request
{
  "viajeId": 10,
  "numeroAsiento": 5,
  "dni": "12345678",
  "nombres": "Ana",
  "apellidos": "Pérez Soto",
  "correo": "ana@mail.com",
  "celular": "987654321"
}

// 201
{
  "id": 120,
  "viajeId": 10,
  "numeroAsiento": 5,
  "estado": "Reservado",
  "reservadoHasta": "2026-10-27T10:15:00"
}
```
- 400: faltan datos obligatorios, el DNI no tiene 8 dígitos, el correo es inválido o el asiento está fuera de 1..capacidad.
- 404: el viaje no existe.
- 409: el asiento ya está ocupado (pagado, o reservado y vigente).
- celular es opcional.

## Convenciones
**Git:**
- Una rama por HU: `feature/HU-XX-nombre`
- Ramas técnicas: `chore/nombre`
- Un commit por tarea
- Un Pull Request por HU con `Closes #N`, integrado con merge commit
- Commits: Conventional Commits (feat, fix, refactor, test, docs, chore). `style` es solo formato de código, no CSS.

**Código:**
| Qué | Convención | Ejemplo |
|---|---|---|
| Carpetas | Plural | `Controllers/`, `Entities/` |
| Entidades | Singular | `Viaje`, `Ruta` |
| Controllers | Plural + Controller | `ViajesController` |
| Servicios | Singular + interfaz | `ViajeService`, `IViajeService` |
| DTOs | Singular + Dto / Request | `ViajeDto`, `BuscarViajesRequest` |

## Pruebas
- Unitarias siempre: Domain y Application (xUnit + NSubstitute), un proyecto de pruebas por capa.
- Nombres `Metodo_Escenario_Resultado` y patrón AAA (Arrange, Act, Assert).
- Integración solo cuando se evalúe necesario (ej. concurrencia en HU-03).
- Pruebas manuales: archivo `.http` con un request por criterio de aceptación.
- Frontend sin pruebas unitarias por ahora.

## Definición de listo
Una HU puede empezar (crear la rama) cuando tiene:
- Criterios de aceptación claros
- Contrato de API definido
- Cambios al modelo de datos definidos
- Decisiones técnicas resueltas

## Definición de terminado
Una HU está terminada cuando:
- Cumple sus criterios de aceptación
- Tiene sus pruebas y pasan
- Está integrada a `main`
- Se hizo la Review y la Retro el mismo día del merge
