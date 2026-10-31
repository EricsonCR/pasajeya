# Técnico – PasajeYa

## Arquitectura
Capas del backend, organización del frontend y estructura del repositorio en el [README](../README.md#arquitectura).

## Decisiones técnicas

| Clave | Decisión |
|---|---|
| Plataforma | .NET 10 (LTS), solución `.slnx` y API con controllers: soporte largo y estilo común en empresas. |
| BaseDatos | SQL Server 2022 local (Docker más adelante): motor habitual con .NET. |
| Secretos | Cadena de conexión en User Secrets (`ConnectionStrings:PasajeYa`): no se suben credenciales. |
| Swagger | Swagger UI para probar los endpoints desde el navegador. |
| Idioma | Dominio en español (`Viaje`), términos técnicos en inglés (`Controller`, `Dto`). |
| Repositorios | Interfaz en Application e implementación en Infrastructure: Application no depende de EF Core. |
| Errores | `AppException` → `ValidacionException` (400), `NoEncontradoException` (404), `ConflictoException` (409); `AppExceptionHandler` las convierte en `ProblemDetails`. |
| Reloj | `TimeProvider` inyectado: permite probar reglas de "hoy" o "ahora" con un reloj fijo. |
| Zona | Hora local de Perú (`DateTime`), sin UTC: un solo país. |
| Concurrencia | Índice único filtrado en Boleto; `DbUpdateException` → 409: la base de datos evita la doble reserva. |
| Vencimiento | La reserva vence al consultar (`ReservadoHasta > ahora`), sin procesos en segundo plano. |
| CORS | Orígenes en `appsettings` (`Cors:OrigenesPermitidos`): se cambian sin tocar código. |
| Frontend | Angular 22 + SCSS, sin SSR, Reactive Forms y signals; siempre se importa el `environment` base. |

## Modelo de datos y API
- Tablas, campos y reglas de la base de datos: [DATOS.md](DATOS.md)
- Contratos de la API y códigos de error: [API.md](API.md)

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
- Se hizo la Review y la Retro, a más tardar al inicio de la siguiente sesión después del merge
