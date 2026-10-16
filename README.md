# PasajeYa

Sistema de venta de pasajes de bus: búsqueda de viajes, selección de asientos, reserva, pago y emisión de boleto.

## Objetivo
Proyecto de portafolio que demuestra los perfiles:
- Backend
- Frontend
- QA
- Datos e IA
- DevOps
- Metodología ágil (Scrum)

## Funcionalidades
- Búsqueda de viajes
- Selección y reserva de asientos
- Pago y emisión de boleto
- Reportes y predicción de demanda

## Stack
| Área | Tecnología |
|---|---|
| Backend | .NET 10 · ASP.NET Core · EF Core |
| Base de datos | SQL Server |
| Frontend | Angular 22 |
| Datos e IA | Python |
| DevOps | Docker · GitHub Actions |

## Arquitectura
- **Backend:** Clean Architecture en 4 capas (Domain, Application, Infrastructure, Api).
- **Frontend:** Angular con componentes standalone y signals.
- **Monorepo:** `backend/`, `frontend/`, `docs/`.

Detalle en [docs/TECNICO.md](docs/TECNICO.md).

## Cómo ejecutar
**Requisitos:** .NET 10 SDK, Node.js + Angular CLI, SQL Server y la herramienta `dotnet-ef`.

**Backend**
```bash
cd backend
dotnet user-secrets set "ConnectionStrings:PasajeYa" "<connection string>" --project src/PasajeYa.Api
dotnet ef database update -p src/PasajeYa.Infrastructure -s src/PasajeYa.Api
dotnet run --project src/PasajeYa.Api
```
La API queda en `http://localhost:5178` (Swagger en `/swagger`). En desarrollo crea viajes de prueba para los próximos 7 días.

**Pruebas**
```bash
cd backend
dotnet test
```

**Frontend**
```bash
cd frontend
npm install
ng serve -o
```
La app queda en `http://localhost:4200`.

## Capturas
_Pendiente_

## Autor
Ericson
