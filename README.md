# Club Deportivo – Sistema de Gestión
### Proyecto Integrador DSOO | WinForms + MySQL + C#

---

## 📁 Estructura del proyecto en Visual Studio

```
ClubDeportivo/
│
├── Program.cs                      ← Punto de entrada
│
├── Data/
│   └── Conexion.cs                 ← Singleton de conexión MySQL
│
├── Models/
│   └── Modelos.cs                  ← Clases: Persona, Socio, NoSocio, Cuota, Pago, etc.
│
├── DAL/
│   ├── UsuarioDAL.cs               ← Acceso a datos: Login
│   └── SocioDAL.cs                 ← Acceso a datos: Socios / Vencimientos
│
└── Forms/
    ├── FrmLogin.cs                 ← Pantalla de login
    ├── FrmMenuPrincipal.cs         ← Menú principal con botones
    ├── FrmAltaSocio.cs             ← Alta de nuevo socio
    ├── FrmListaSocios.cs           ← Listado y búsqueda de socios
    ├── FrmVencimientos.cs          ← Consulta de cuotas vencidas
    ├── FrmCobrarCuota.cs           ← Registro de pago de cuota
    └── FrmAltaNoSocio.cs           ← Alta de no socio
```

---

## ⚙️ Pasos de instalación

### 1. Crear la base de datos en MySQL
1. Abrí **MySQL Workbench** (o tu cliente preferido).
2. Ejecutá el script completo: `ClubDeportivo_DB.sql`
3. Esto crea la base de datos, todas las tablas, stored procedures y datos iniciales.

> Usuario admin por defecto: **admin** / **admin123**

---

### 2. Configurar la conexión en Visual Studio
Abrí `Data/Conexion.cs` y ajustá estas constantes según tu entorno:

```csharp
private const string SERVER   = "localhost";
private const string DATABASE = "ClubDeportivo";
private const string USER     = "root";
private const string PASSWORD = "";       // tu contraseña de MySQL
private const int    PORT     = 3306;
```

---

### 3. Instalar el paquete NuGet de MySQL
En Visual Studio:
1. Click derecho en el proyecto → **Administrar paquetes NuGet**
2. Buscar: `MySql.Data`
3. Instalar la versión más reciente (ej: 8.x)

O desde la **Consola del Administrador de paquetes**:
```
Install-Package MySql.Data
```

---

### 4. Crear el proyecto en Visual Studio
1. **Archivo → Nuevo → Proyecto**
2. Tipo: **Aplicación de Windows Forms (.NET Framework)**
3. Nombre: `ClubDeportivo`
4. Framework: **.NET Framework 4.8** (o superior)

### 5. Agregar los archivos al proyecto
- Crear carpetas: `Data`, `Models`, `DAL`, `Forms`
- Copiar cada `.cs` en su carpeta correspondiente
- Asegurarse que el namespace en cada archivo sea `ClubDeportivo.NombreCarpeta`

### 6. Configurar el punto de entrada
En las propiedades del proyecto, asegurarse que el **objeto de inicio** sea `ClubDeportivo.Program`.

---

## 🗺️ Flujo de la aplicación

```
Program.cs
    ↓ verifica conexión MySQL
FrmLogin
    ↓ valida usuario/contraseña (sp_ValidarLogin)
FrmMenuPrincipal
    ├── FrmAltaSocio      → sp_AltaSocio
    ├── FrmListaSocios    → sp_ObtenerSocios / sp_BuscarSocio
    ├── FrmVencimientos   → sp_ConsultarVencimientos
    ├── FrmCobrarCuota    → sp_CobrarCuota
    └── FrmAltaNoSocio    → sp_AltaNoSocio
```

---

## 📋 Stored Procedures disponibles

| Nombre                    | Descripción                                          |
|---------------------------|------------------------------------------------------|
| `sp_ValidarLogin`         | Autentica usuario del sistema                        |
| `sp_ExisteDNI`            | Verifica si un DNI ya está registrado                |
| `sp_AltaSocio`            | Registra socio + carnet + cuota inicial              |
| `sp_AltaNoSocio`          | Registra no socio                                    |
| `sp_ObtenerSocios`        | Lista todos los socios                               |
| `sp_BuscarSocio`          | Busca por DNI, número de socio o apellido            |
| `sp_CobrarCuota`          | Registra pago mensual y actualiza vencimiento        |
| `sp_ConsultarVencimientos`| Lista socios con cuota vencida o próxima a vencer    |

---

## 🎨 Convenciones del código

- **Namespaces**: `ClubDeportivo.Data`, `ClubDeportivo.Models`, `ClubDeportivo.DAL`, `ClubDeportivo.Forms`
- **DAL**: toda la lógica de base de datos va en la carpeta `DAL`, nunca en los formularios directamente (excepción: `FrmCobrarCuota` llama al SP directamente para simplificar)
- **Diseño de formularios**: hecho en código (sin `.resx` / diseñador visual) usando `InitializeComponent()` vacío
- **Contraseñas**: se almacenan hasheadas con SHA2-256 (manejado por MySQL)
