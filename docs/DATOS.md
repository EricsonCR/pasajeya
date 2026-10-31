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
