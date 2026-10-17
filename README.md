# Planta Minera Dorado S.A. - Sistema de Control de Inventario y Despacho

Sistema de escritorio empresarial desarrollado en **WPF (.NET 8)** y **Entity Framework Core** conectado a **SQL Server**, diseñado específicamente para la gestión operativa, control de herramientas, equipos mineros, equipos de protección personal (EPP), kárdex de almacén y dotación al personal de mina.

---

## 🏗️ Arquitectura del Sistema (N-Capas)

El proyecto está estructurado con una arquitectura limpia desacoplada en capas:
- **`InventarioMinero.Models`**: Entidades maestras de base de datos (14 tablas), DTOs y enumeraciones de negocio.
- **`InventarioMinero.DAL`**: Capa de acceso a datos con Entity Framework Core (`InventarioDbContext`), configuraciones Fluent API y sembrado de datos iniciales (`DbInitializer`).
- **`InventarioMinero.BLL`**: Capa de lógica de negocio y servicios especializados con inyección de dependencias (`Transient`).
- **`InventarioMinero.WPF`**: Interfaz de usuario de alto impacto visual con tema oscuro industrial (`Dark Gold`), arquitectura MVVM (CommunityToolkit.Mvvm), validaciones y modales interactivos.

---

## 📦 Módulos Principales (Cobertura de las 14 Tablas Maestras)

1. **Dashboard General**: Métricas clave en tiempo real, alertas de stock bajo y accesos directos de operación.
2. **Herramientas y Stock**: Catálogo de equipos y herramientas, control de existencias, stock mínimo/máximo y pañol de mina.
3. **EPP y Catálogo**: Elementos de protección personal clasificados por categoría (cascos, respiradores, botas, guantes, etc.).
4. **Requerimientos de Compra**: Generación manual y automatizada de hojas de pedido por stock mínimo con formato imprimible oficial.
5. **Movimientos y Kárdex**: Registro de ingresos y salidas con cálculo de saldos y trazabilidad total por herramienta o EPP.
6. **Préstamos de Herramientas**: Control de asignación y custodia temporal a personal minero con registro de devoluciones y estado físico del equipo.
7. **Dotación de EPP**: Entrega de equipamiento de seguridad a trabajadores con generación e impresión de Acta de Entrega oficial.
8. **Personal Minero y Cargos**: Directorio de operarios, supervisores, cargos de mina y áreas de labor.
9. **Usuarios y Accesos**: Control de cuentas del sistema, asignación de roles (Administrador, Supervisor, Almacenero) y seguridad con cifrado SHA-256.

---

## ⚙️ Requisitos y Puesta en Marcha

1. **Base de Datos**:
   - Microsoft SQL Server (instancia local por defecto: `.\SQLMARX` o la configurada en `appsettings.json`).
   - Base de datos: `InventarioMineroDoradoSA`.
   - Se incluye el script completo en `database_init.sql` o se inicializa automáticamente al primer arranque mediante `DbInitializer`.

2. **Ejecución Directa**:
   - Doble clic en `INICIAR_SISTEMA_MINERO.bat` o el ejecutable compilado en la carpeta `Ejecutable/InventarioMinero.WPF.exe`.

3. **Credenciales por Defecto**:
   - **Usuario**: `admin`
   - **Contraseña**: `Admin123*`

---

## 🛠️ Tecnologías

- **.NET 8.0 Windows Desktop (WPF)**
- **Entity Framework Core 8.0 (SQL Server)**
- **CommunityToolkit.Mvvm 8.4**
- **Microsoft.Extensions.Hosting & DependencyInjection**
