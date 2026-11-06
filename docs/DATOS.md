# Modelo de datos – PasajeYa

Base de datos SQL Server, creada con migraciones de EF Core. Configuración de cada tabla en
`backend/src/PasajeYa.Infrastructure/Data/Configurations/`.

- Todas las tablas usan `Id` int autoincremental como PK.
- Las FK no borran en cascada (`Restrict`).
- Los enums se guardan como texto.
- Script SQL completo (desde `backend/`):
  `dotnet ef migrations script -p src/PasajeYa.Infrastructure -s src/PasajeYa.Api`

## Ciudad

| Campo | Tipo | Notas |
|---|---|---|
| Id | int | PK |
| Nombre | nvarchar(100) | Único |

## Ruta

| Campo | Tipo | Notas |
|---|---|---|
| Id | int | PK |
| OrigenId | int | FK → Ciudad |
| DestinoId | int | FK → Ciudad. Distinto del origen (CHECK) |

**Índice único:** (OrigenId, DestinoId). No hay dos rutas iguales.

## Bus

| Campo | Tipo | Notas |
|---|---|---|
| Id | int | PK |
| Placa | nvarchar(10) | Única |
| TipoServicio | nvarchar(20) | Economico, Ejecutivo o Vip |
| Capacidad | int | Cantidad de asientos |

## Viaje

| Campo | Tipo | Notas |
|---|---|---|
| Id | int | PK |
| RutaId | int | FK → Ruta |
| BusId | int | FK → Bus |
| Salida | datetime2 | Hora local de Perú |
| Llegada | datetime2 | Hora local de Perú |
| Precio | decimal(10,2) | En soles |

## Boleto

| Campo | Tipo | Notas |
|---|---|---|
| Id | int | PK |
| ViajeId | int | FK → Viaje |
| NumeroAsiento | int | 1 a la capacidad del bus |
| Estado | nvarchar(20) | Reservado, Pagado o Anulado |
| Dni | nchar(8) | |
| Nombres | nvarchar(100) | |
| Apellidos | nvarchar(100) | |
| Correo | nvarchar(150) | |
| Celular | nvarchar(9) | Opcional |
| ReservadoHasta | datetime2 | Opcional. Hora límite para pagar |

**Índice único filtrado:** (ViajeId, NumeroAsiento) solo para Estado `Reservado` o `Pagado`. Evita dos boletos activos en el mismo asiento.

## Datos de prueba

**Catálogo** (en las migraciones): 8 ciudades, 15 rutas y 6 buses (Económico, Ejecutivo y Vip).

**Seed** (`DbSeeder`): corre al arrancar la API en Development y nunca borra datos.

**Viajes:** si no hay viajes desde hoy, crea viajes para hoy y mañana: 3 salidas (06:00, 14:00 y 22:00) por cada ruta existente (45 viajes por día).

**Ruta de pruebas, Lima → Arequipa:** sus boletos se crean una sola vez, junto con los viajes, y no cambian en los arranques siguientes. Hoy:

| Salida | Estado | Para probar |
|---|---|---|
| 06:00 | Vacío | Buscar, elegir asiento y reservar → 201 |
| 14:00 | Lleno (`Pagado`) | Aparece agotado; reservar → 409 |
| 22:00 | Mixto (`Pagado`, `Reservado`, `Anulado`) | Reservar en `Anulado` → 201; en `Pagado` o `Reservado` → 409 |

Mañana, la misma ruta no tiene boletos.

**Resto de rutas:** simulan ventas. En cada arranque, cada viaje con salida posterior a la hora actual (los de hoy que aún no salen y los de mañana) recibe 4 boletos `Pagado` en asientos libres, hasta llenarse. Los viajes que ya salieron no cambian.
