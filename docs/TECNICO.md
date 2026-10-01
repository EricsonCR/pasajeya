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
- SQL Server local (Docker más adelante)
- Swagger UI para probar la API
- Idioma: dominio en español (`Viaje`, `Ruta`), términos técnicos en inglés (`Controller`, `Dto`)

## Modelo de datos
| Tabla | Campos |
|---|---|
| Ciudad | Id, Nombre |
| Ruta | Id, OrigenId → Ciudad, DestinoId → Ciudad |
| Bus | Id, Placa, TipoServicio, Capacidad |
| Viaje | Id, RutaId, BusId, Salida, Llegada, Precio |
| Boleto | Id, ViajeId, NumeroAsiento, Estado |

- Ruta: origen distinto de destino, sin rutas duplicadas, `DeleteBehavior.Restrict` en ambas FK.
- Asientos disponibles = Capacidad del bus − boletos activos del viaje.
- Boleto se amplía en HU-03 y HU-05.

## API
_Pendiente_

## Convenciones
**Git:**
- Una rama por HU: `feature/HU-XX-nombre`
- Ramas técnicas: `chore/nombre`
- Un commit por tarea
- Un Pull Request por HU con `Closes #N`, integrado con merge commit
- Commits: Conventional Commits (feat, fix, test, docs, chore)

**Código:**
| Qué | Convención | Ejemplo |
|---|---|---|
| Carpetas | Plural | `Controllers/`, `Entities/` |
| Entidades | Singular | `Viaje`, `Ruta` |
| Controllers | Plural + Controller | `ViajesController` |
| Servicios | Singular + interfaz | `ViajeService`, `IViajeService` |
| DTOs | Singular + Dto / Request | `ViajeDto`, `BuscarViajesRequest` |

## Pruebas
- Unitarias siempre: Domain y Application (xUnit), un proyecto de pruebas por capa.
- Integración solo cuando se evalúe necesario (ej. concurrencia en HU-03).

## Definición de terminado
Una HU está terminada cuando:
- Cumple sus criterios de aceptación
- Tiene sus pruebas y pasan
- Está integrada a `main`
