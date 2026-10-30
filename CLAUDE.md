# PasajeYa – Instrucciones para Claude

## Contexto
Sistema de venta de pasajes de bus (búsqueda, asientos, reserva, pago).
Antes de cambiar algo, leer `README.md` y `docs/` (PRODUCTO.md y TECNICO.md).

## Reglas
- Respetar la arquitectura y las convenciones de `docs/TECNICO.md`
  (Clean Architecture en 4 capas, nombres en español, Conventional Commits).
- Si un cambio contradice lo definido en `docs/`, avisar antes de hacerlo.
- Soluciones simples: es un proyecto de portafolio, no sobre-diseñar.
- Una HU por rama (`feature/HU-XX-nombre`), PR con `Closes #N`.

## Comandos
Backend (desde `backend/`):
- Compilar: `dotnet build`
- Pruebas: `dotnet test`
- Nueva migración: `dotnet ef migrations add <Nombre> -p src/PasajeYa.Infrastructure -s src/PasajeYa.Api -o Data/Migrations`
- Aplicar migraciones: `dotnet ef database update -p src/PasajeYa.Infrastructure -s src/PasajeYa.Api`
- Ejecutar API: `dotnet run --project src/PasajeYa.Api`

Frontend (desde `frontend/`):
- Ejecutar: `ng serve -o`
- Compilar: `ng build`
