# PasajeYa

Plataforma web de venta de pasajes de bus de punta a punta: el pasajero busca
su viaje, elige su asiento, paga en línea y recibe su boleto en PDF por correo.
A futuro, el sistema sumará analítica de ventas y predicción de demanda.

## Qué abarca

| Área | En PasajeYa |
|---|---|
| **Backend** | API en .NET con Clean Architecture, EF Core, SQL Server y pruebas unitarias |
| **Frontend** | Aplicación Angular con componentes standalone y signals |
| **DevOps** | Contenedores con Docker, integración continua con GitHub Actions y despliegue |

## Estado del proyecto

| Área | Módulo | Estado |
|---|---|---|
| Producto | Búsqueda de viajes (origen, destino, fecha) | ✅ Listo |
| Producto | Mapa de asientos | ✅ Listo |
| Producto | Reserva de asiento (bloqueo de 15 min) | ✅ Listo |
| Producto | Navegación: header, menú y footer | ⏳ Pendiente |
| Producto | Login y cuenta (datos, cambio de contraseña) | ⏳ Pendiente |
| Producto | Reserva de varios asientos | ⏳ Pendiente |
| Producto | Pago en línea | ⏳ Pendiente |
| Producto | Boleto en PDF y envío por correo | ⏳ Pendiente |
| Producto | Panel de administración (todos los viajes) | ⏳ Pendiente |
| Calidad | Pruebas unitarias (Domain y Application) | ✅ Listo |
| DevOps | Docker (API, frontend y BD) | ⏳ Pendiente |
| DevOps | Integración continua (build y pruebas en cada PR) | ⏳ Pendiente |
| DevOps | Despliegue en la nube | ⏳ Pendiente |

## Cómo se usa

1. En la pantalla de inicio, elige origen, destino y fecha, y busca.
2. En la lista de viajes, presiona **Elegir asiento** en el que te interese.
3. En el mapa, elige un asiento libre.
4. Completa tus datos (DNI, nombres, apellidos, correo y, si quieres, celular) y presiona **Reservar**.
5. Ves la confirmación con tu asiento y la hora límite para pagar (15 minutos).

Si otro pasajero tomó el asiento segundos antes, verás el aviso "El asiento ya fue tomado"
y el mapa se actualiza para que elijas otro.

**Sin doble reserva:** si dos pasajeros reservan el mismo asiento a la vez, un índice
único filtrado en SQL Server deja pasar solo a uno. Las reservas vencidas no bloquean
el asiento: se calculan por hora, sin procesos en segundo plano.

## Arquitectura

### Backend: Clean Architecture
| Capa | Responsabilidad |
|---|---|
| `Api` | Controllers, manejo global de errores y configuración |
| `Application` | Casos de uso (servicios), DTOs y validaciones |
| `Infrastructure` | Acceso a datos con EF Core y SQL Server |
| `Domain` | Entidades y reglas de negocio |

### Frontend: Angular organizado por responsabilidad
| Carpeta | Responsabilidad |
|---|---|
| `pages/` | Una carpeta por pantalla (componente standalone con su HTML y SCSS). Maneja el estado de la pantalla con signals y los formularios con Reactive Forms. |
| `services/` | Comunicación con la API: un servicio por recurso (`ViajeService`, `BoletoService`), con `HttpClient`. |
| `models/` | Interfaces TypeScript que reflejan los contratos de la API. |

#### Estilos
- `styles.scss`: base común para toda la app (paleta de colores, botones, campos y mensajes de error).
- `.scss` de cada pantalla: solo lo propio de esa pantalla (por ejemplo, el mapa de asientos con forma de bus).

### Estructura del repositorio
```text
pasajeya/
├── backend/
│   ├── src/
│   │   ├── PasajeYa.Api/              Controllers, Handlers, PasajeYa.Api.http
│   │   ├── PasajeYa.Application/      Services, Dtos, Interfaces, Exceptions
│   │   ├── PasajeYa.Infrastructure/   Data (DbContext, Migrations, Seeder), Repositories
│   │   └── PasajeYa.Domain/           Entities, Enums
│   ├── tests/
│   │   ├── PasajeYa.Domain.Tests/
│   │   └── PasajeYa.Application.Tests/
│   └── scripts/sql/                   Consultas de apoyo
├── frontend/
│   └── src/app/
│       ├── pages/                     buscar-viajes, elegir-asiento
│       ├── services/                  viaje.service, boleto.service
│       └── models/                    Interfaces de la API
└── docs/                              PRODUCTO.md (historias) y TECNICO.md (decisiones)
```

Detalle de decisiones, modelo de datos y contratos de la API en [docs/TECNICO.md](docs/TECNICO.md).

## Cómo ejecutarlo

### Requisitos
Versiones con las que se probó el proyecto:

| Herramienta | Versión |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0.403 |
| [Node.js](https://nodejs.org/) | 26.2.0 (incluye npm 11.13) |
| SQL Server | 2022 (instalación local o en Docker) |

Angular 22 y TypeScript no se instalan aparte: `npm install` los descarga según `frontend/package.json`.

Además, la herramienta de migraciones de EF Core:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### 1. Clonar el repositorio
```bash
git clone https://github.com/EricsonCR/pasajeya.git
cd pasajeya
```

### 2. Backend
```bash
cd backend
```

Guardar la cadena de conexión en User Secrets (no se sube al repositorio). Ejemplo con usuario `sa`:
```bash
dotnet user-secrets set "ConnectionStrings:PasajeYa" "Server=localhost;Database=PasajeYa;User Id=sa;Password=<tu-contraseña>;TrustServerCertificate=True" --project src/PasajeYa.Api
```

Crear la base de datos con las migraciones (la API no la crea sola al iniciar):
```bash
dotnet ef database update -p src/PasajeYa.Infrastructure -s src/PasajeYa.Api
```

Levantar la API:
```bash
dotnet run --project src/PasajeYa.Api
```
La API queda en `http://localhost:5178` (Swagger en `http://localhost:5178/swagger`).
Al iniciar en desarrollo, carga datos de prueba: ciudades, rutas, buses y viajes para los próximos días.

### 3. Frontend
En otra terminal, desde la raíz del repositorio:
```bash
cd frontend
npm install
npx ng serve -o
```
La app se abre en `http://localhost:4200`.

## Pruebas
Desde `backend/`:
```bash
dotnet test
```
Cubren las reglas de negocio (`Domain`) y los casos de uso (`Application`) con xUnit y NSubstitute.

## Autor
Ericson Cruz
