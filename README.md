# Sistema de Atenciones Ambulatorias — HVLH

Aplicación de escritorio para el registro de pacientes, citas y atenciones ambulatorias,
desarrollada como prueba técnica .NET.

**Stack:** C# · .NET Framework 4.7.2 · Windows Forms · SQL Server · ADO.NET · ReportViewer (RDLC)

El módulo está ambientado en el **Hospital Nacional Víctor Larco Herrera (HVLH)**, hospital
nacional de salud mental, lo que determinó el catálogo de diagnósticos cargado y la
identidad visual de la aplicación.

---

## Índice

1. [Puesta en marcha en 5 pasos](#1-puesta-en-marcha-en-5-pasos)
2. [Requisitos previos](#2-requisitos-previos)
3. [Desplegar la base de datos](#3-desplegar-la-base-de-datos)
4. [Configurar la conexión](#4-configurar-la-conexión)
5. [Compilar y ejecutar](#5-compilar-y-ejecutar)
6. [Recorrido de verificación](#6-recorrido-de-verificación)
7. [Solución de problemas](#7-solución-de-problemas)
8. [Arquitectura y decisiones técnicas](#8-arquitectura-y-decisiones-técnicas)
9. [Modelo de datos y validaciones](#9-modelo-de-datos-y-validaciones)
10. [Catálogo CIE-10 del MINSA](#10-catálogo-cie-10-del-minsa)
11. [Consumo de API REST](#11-consumo-de-api-rest)
12. [Reportes](#12-reportes)
13. [Cinco buenas prácticas de SQL Server](#13-cinco-buenas-prácticas-de-sql-server)
14. [Cobertura de los requisitos](#14-cobertura-de-los-requisitos)
15. [Limitaciones conocidas](#15-limitaciones-conocidas)

---

## 1. Puesta en marcha en 5 pasos

```
1. Abrir SSMS y ejecutar, en orden, los 7 scripts de la carpeta BaseDatos/
2. Abrir SistemaAtenciones.sln en Visual Studio
3. Ajustar el Data Source en src/Hospital.Presentacion/App.config
4. Compilar (Ctrl+Shift+B) y ejecutar (F5)
5. Entrar con   admin / Admin123$
```

El resto del documento detalla cada paso y explica las decisiones de diseño.

---

## 2. Requisitos previos

| Componente | Versión mínima | Nota |
|---|---|---|
| Visual Studio | 2019 o 2022 | Carga de trabajo *Desarrollo de escritorio de .NET* |
| .NET Framework Developer Pack | 4.7.2 | Si falta, Visual Studio lo ofrece al abrir la solución |
| SQL Server | 2016 | Express es suficiente |
| SQL Server Management Studio | Cualquiera | Para ejecutar los scripts |

**Paquetes NuGet.** Se restauran solos al abrir la solución. Si no ocurre: clic derecho
sobre la solución → *Restaurar paquetes NuGet*.

- `Newtonsoft.Json` 13.0.3 — deserialización de la API REST.
- `Microsoft.ReportingServices.ReportViewerControl.Winforms` 150.1652.0 — visor de reportes.

**Conexión a internet.** Solo se necesita para restaurar los paquetes la primera vez. La
aplicación funciona sin conexión: el catálogo CIE-10 vive en la base de datos local.

---

## 3. Desplegar la base de datos

Los scripts están en `BaseDatos/` y se ejecutan **en orden numérico** desde SSMS. Abrir cada
archivo y pulsar `F5`; cada uno imprime su progreso en la pestaña *Messages*.

| Orden | Script | Qué hace |
|---|---|---|
| 1 | `01_Tablas.sql` | Crea la base `HospitalAtencionesDB`, tablas, PK, FK, CHECK e índices |
| 2 | `02_Vistas.sql` | Vista de resumen de atenciones |
| 3 | `03_StoredProcedures.sql` | Procedimientos del núcleo asistencial |
| 4 | `04_DatosIniciales.sql` | Usuarios, especialidades, médicos, pacientes y atenciones de ejemplo |
| 5 | `05_Migracion_HVLH.sql` | Amplía `Paciente`; crea `CatalogoCie10` y `Cita`; enlaza atención con cita; actualiza vistas |
| 6 | `06_Procedimientos_HVLH.sql` | Procedimientos de pacientes, citas, CIE-10 y reportes |
| 7 | `07_Catalogo_Cie10_MINSA.sql` | Carga el catálogo CIE-10 en español y genera agenda de ejemplo |

`00_Script_Completo.sql` equivale a los pasos 1 a 4 unidos, por comodidad. Después hay que
ejecutar igualmente 05, 06 y 07.

El script 07 termina mostrando un resumen con el número de registros cargados. Sirve como
comprobación de que todo salió bien.

### Por qué la migración va en scripts separados

Los scripts 05, 06 y 07 son **evolutivos**: no borran ni recrean nada que ya exista.

- Las columnas nuevas se agregan como `NULL`, se rellenan con valores válidos y recién
  entonces pasan a `NOT NULL`. Es el patrón habitual de migración en caliente: hacerlo en
  un solo paso fallaría sobre las filas que ya están en la tabla.
- Las tablas y secuencias nuevas se crean solo si no existen (`OBJECT_ID`, `COL_LENGTH`).
- Los procedimientos y vistas se eliminan y recrean: se reemplaza código, nunca datos.

En consecuencia **los tres pueden ejecutarse varias veces sin efectos secundarios**. Si ya
había una versión anterior desplegada, basta con correr 05, 06 y 07: no se pierde ningún
dato, y las atenciones ya registradas se enlazan automáticamente con una cita coherente del
mismo paciente y profesional, para que el histórico no quede desconectado del modelo nuevo.

---

## 4. Configurar la conexión

Editar `src/Hospital.Presentacion/App.config` y ajustar el `Data Source` al nombre de la
instancia local de SQL Server:

```xml
<connectionStrings>
  <add name="HospitalDB"
       connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=HospitalAtencionesDB;Integrated Security=True;Connect Timeout=30;Application Name=HospitalAtenciones"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Valores habituales de `Data Source`:

| Instalación | Valor |
|---|---|
| SQL Server Express | `.\SQLEXPRESS` |
| Instancia por defecto | `.` o `localhost` |
| LocalDB | `(localdb)\MSSQLLocalDB` |
| Servidor remoto | `nombre-servidor\instancia` |

Con autenticación de SQL Server en lugar de Windows, reemplazar `Integrated Security=True`
por `User ID=sa;Password=TuClave;`.

La cadena vive en el archivo de configuración y no en el código, de modo que cambiar de
servidor no obliga a recompilar. El nombre de la institución se lee de
`appSettings/NombreInstitucion` por el mismo motivo.

---

## 5. Compilar y ejecutar

1. Abrir `SistemaAtenciones.sln` en Visual Studio.
2. Comprobar que **Hospital.Presentacion** es el proyecto de inicio: aparece en negrita en
   el Explorador de soluciones. Si no, clic derecho → *Establecer como proyecto de inicio*.
3. Compilar con `Ctrl + Shift + B`.
4. Ejecutar con `F5`.

### Credenciales de prueba

| Usuario | Contraseña | Rol | Puede |
|---|---|---|---|
| `admin` | `Admin123$` | Administrador | Todo, incluida la eliminación física de atenciones |
| `jperez` | `Medico123$` | Médico | Registrar y consultar |
| `mtorres` | `Enfer123$` | Asistencial | Registrar y consultar |

Las contraseñas se almacenan con **PBKDF2-SHA256**, 20 000 iteraciones y salt aleatorio por
usuario, con comparación en tiempo constante. Los hashes ya vienen calculados en
`04_DatosIniciales.sql`.

---

## 6. Recorrido de verificación

**Acceso al módulo de pacientes**

```
Login con admin  →  Menú principal  →  PACIENTES  →  Formulario de gestión de pacientes
```

Hay tres caminos al mismo formulario: el menú *Registro asistencial → PACIENTES*, el botón
**PACIENTES** de la barra de accesos directos, y el atajo `Ctrl+P`.

**Prueba funcional completa**

1. **PACIENTES → Nuevo paciente.** Registrar uno. Después intentar registrar otro con el
   mismo documento: el sistema lo impide e indica el nombre y la historia clínica del
   paciente que ya existe.
2. **CITAS → Nueva cita.** Programar una para ese paciente. Probar a marcarla como *No
   acudió* antes de que llegue la hora: se rechaza.
3. **ATENCIONES → Nueva atención.** Seleccionar el paciente y, en *Cita de origen*, elegir
   la cita creada. Buscar diagnósticos escribiendo `F20` (por código) o `esquizofrenia`
   (por texto). Probar las validaciones: guardar sin diagnósticos, con temperatura 50 °C, o
   marcando *Atendida* sin diagnóstico definitivo.
4. Volver a **CITAS**: la cita pasó automáticamente a **ATENDIDO**.
5. **REPORTES.** Elegir un periodo, filtrar por *No acudieron* y exportar a PDF o Excel
   desde la barra del visor.

---

## 7. Solución de problemas

**`No fue posible conectarse a la base de datos` al iniciar sesión.**
El `Data Source` del `App.config` no coincide con la instancia instalada. Verificar el
nombre en la ventana de conexión de SSMS y comprobar que el servicio *SQL Server
(SQLEXPRESS)* esté iniciado en `services.msc`.

**`Invalid object name 'dbo.Cita'` o `Could not find stored procedure`.**
Falta ejecutar alguno de los scripts 05, 06 o 07. Ejecutarlos en orden; son
re-ejecutables.

**Error al abrir el módulo de atenciones, citas o reportes.**
Suele significar que `06_Procedimientos_HVLH.sql` no se ejecutó o quedó desactualizado.
Volver a correrlo: reemplaza procedimientos sin tocar datos.

**El reporte aparece en blanco o da error de definición.**
El archivo `rptGeneralHVLH.rdlc` debe tener *Acción de compilación* = **Recurso
incrustado**. El formulario lo busca primero como recurso incrustado y usa el archivo de la
carpeta de salida como respaldo, así que basta con que una de las dos vías funcione.

**No se encuentra el ensamblado de ReportViewer.**
Restaurar los paquetes NuGet y recompilar. Si persiste, borrar las carpetas `bin` y `obj` y
compilar de nuevo.

**La búsqueda de CIE-10 no devuelve nada.**
Falta ejecutar `07_Catalogo_Cie10_MINSA.sql`. Comprobar con
`SELECT COUNT(1) FROM dbo.CatalogoCie10;` — debe devolver algo más de 200 filas.

**Los textos aparecen cortados o superpuestos.**
La interfaz usa medidas fijas (`AutoScaleMode.None`) para verse igual en todos los equipos.
Con escalado de pantalla superior al 125 % conviene bajarlo a 100 % en la configuración de
Windows.

---

## 8. Arquitectura y decisiones técnicas

```
SistemaAtenciones.sln
├── BaseDatos/                      Scripts SQL (base + migración evolutiva)
├── herramientas/                   Guion generador del RDLC
└── src/
    ├── Hospital.Entidades          POCOs; sin dependencias
    ├── Hospital.Utilidades         Hash de claves, log, sesión, excepciones propias
    ├── Hospital.AccesoDatos        ADO.NET, repositorios, transacciones
    ├── Hospital.Integracion        Cliente HttpClient del catálogo CIE-10 externo
    ├── Hospital.Negocio            Validaciones y reglas del proceso asistencial
    └── Hospital.Presentacion       Windows Forms, identidad visual y reportes RDLC
```

Las dependencias van en un solo sentido:

```
Presentación  →  Negocio  →  Acceso a Datos / Integración  →  Entidades + Utilidades
```

La capa de presentación no referencia `System.Data.SqlClient` ni construye consultas: arma
la entidad, la entrega al servicio y muestra el resultado. Si mañana el consumidor fuera
una API web en lugar de un WinForms, las tres capas inferiores se reutilizan sin cambios.

### ADO.NET con procedimientos almacenados, no un ORM

La prueba pide procedimientos almacenados, al menos una vista y control explícito de la
transacción cabecera–detalle. ADO.NET deja esa frontera visible; poner un ORM sobre
procedimientos que ya existen agregaría una capa de traducción sin aportar nada. En un
proyecto donde el modelo lo definiera la aplicación, la elección sería la contraria.

### La transacción vive en el repositorio, no en el procedimiento

Registrar una atención ejecuta **varios** procedimientos: la cabecera, N líneas de detalle y
el cambio de estado de la cita. La unidad de trabajo es la operación completa, así que se
abre una `SqlTransaction` en `AtencionRepositorio` y se confirma solo si todo terminó bien.
El nivel de aislamiento es `ReadCommitted`: no hay relecturas dentro de la transacción, de
modo que las garantías de `Serializable` solo agregarían bloqueos con varias personas
registrando en paralelo.

Por eso los procedimientos de escritura **no** abren transacciones propias. Si cada uno
hiciera su propio `ROLLBACK`, destruiría la transacción externa y dejaría `@@TRANCOUNT`
inconsistente. Propagan el error con `THROW` y el cliente decide.

Las excepciones son `usp_Atencion_Anular` y `usp_Atencion_Eliminar`, operaciones
autocontenidas —un solo llamado sobre dos tablas—, donde la transacción sí vive dentro del
procedimiento con `SET XACT_ABORT ON`, `TRY…CATCH` y `XACT_STATE()`.

La regla seguida es: **la responsabilidad transaccional está en un único lugar por
operación, nunca repartida entre el procedimiento y el cliente.**

### Al editar, el detalle se reemplaza completo

Borrar e insertar dentro de la misma transacción es más simple y menos propenso a errores
que sincronizar línea por línea, y el volumen es de pocos diagnósticos por atención. Con
detalles de cientos de filas la decisión sería otra.

### El estado de la cita se registra, nunca se deduce

`NO_ACUDIO` solo existe si alguien lo marca e indica el motivo. Que no exista una atención
asociada no convierte la cita en inasistencia: podría simplemente no haber llegado su
turno. En el sentido inverso, `ATENDIDO` no se puede asignar a mano; lo fija el registro de
la atención dentro de la misma transacción, de manera que no exista forma de declarar
atendido a alguien sin un acto clínico detrás. Anular la atención devuelve la cita a
`CITADO`.

### La atención admite `IdCita` nulo

El hospital atiende también por demanda espontánea. Obligar a elegir una cita llevaría al
personal a inventar citas ficticias para poder registrar la atención, que es exactamente lo
que arruinaría los indicadores de inasistencia que este módulo existe para medir.

### Unicidad del documento por tipo + número

Un DNI y un carné de extranjería pueden coincidir en dígitos sin ser la misma persona. La
comprobación previa en `PacienteServicio` existe para avisar con el nombre del paciente ya
registrado; la garantía real la da la restricción `UQ_Paciente_TipoNumeroDoc`, porque entre
la comprobación y el `INSERT` podría colarse otro registro concurrente.

### Correlativos con `SEQUENCE`

Número de atención, número de cita y número de historia clínica se generan con secuencias:
sin tabla de contadores, sin bloqueos, sin cursores y seguro en concurrencia.

### Sin eliminación de pacientes

Un paciente concentra su historia clínica, sus citas y sus atenciones. Se desactiva, y ni
siquiera eso mientras tenga citas pendientes: quedaría una agenda apuntando a alguien
inhabilitado.

### Manejo de errores en dos niveles

`NegocioException` lleva mensajes redactados para el usuario y se muestra tal cual;
`DatosException` envuelve la falla técnica, muestra un texto neutro y deja el detalle en
`Logs/`. La clasificación vive en un único lugar, `ErroresSql`:

| Origen | Tratamiento |
|---|---|
| Número ≥ 50000 (`THROW` desde un procedimiento) | Regla de negocio: el mensaje se muestra al usuario |
| 2627 / 2601 | Violación de unicidad: mensaje de duplicado |
| 547 | Integridad referencial o `CHECK` |
| −2 | Tiempo de espera agotado |
| 4060 / 18456 | Problema de conexión o credenciales |
| Resto | Falla técnica: texto neutro y detalle al log |

`Program.cs` instala además un manejador global para que ninguna excepción no controlada
cierre la aplicación de golpe.

### Formularios construidos por código

Los formularios se arman en un método `InitializeComponent()` escrito a mano, equivalente al
que genera el diseñador de Visual Studio. El motivo es práctico: un `.resx` binario no se
puede revisar ni fusionar en un repositorio Git, mientras que un layout escrito se lee, se
comenta y se compara en un *diff*.

### Identidad visual centralizada

`Tema.cs` concentra la paleta institucional y `Recursos.cs` el acceso al logo, incrustado en
el ensamblado y con respaldo en disco. El control `Controles/EncabezadoHvlh` reúne logo,
nombre y título de módulo, y se reutiliza en las cuatro pantallas principales: un cambio de
logo o de denominación oficial se hace en un solo archivo.

El color se reserva para comunicar estado, no para decorar: azul = atendido, naranja =
citado, rojo = no acudió, gris = cancelado. Si el logo fallara al cargarse, la aplicación
sigue funcionando sin imagen: un problema gráfico nunca debe impedir atender pacientes.

---

## 9. Modelo de datos y validaciones

```
PACIENTE  ──1:N──►  CITA  ──1:0..1──►  ATENCIÓN  ──1:N──►  ATENCIÓN_DETALLE (CIE-10)
```

Un paciente se registra una sola vez y se reutiliza en todas sus citas y atenciones. Una
cita puede terminar en atención o no, y ese desenlace se registra explícitamente. La clave
foránea va del lado de la atención, lo que evita una dependencia circular entre ambas
tablas.

**Tablas (10):** `Usuario`, `Especialidad`, `Medico`, `Paciente`, `Cita`, `Atencion`,
`AtencionDetalle`, `CatalogoCie10`, `CatalogoCie10_Staging`, `CatalogoCie10_Carga`.

**Secuencias (3):** `SeqNumeroAtencion`, `SeqNumeroCita`, `SeqHistoriaClinica`.

**Vistas (2):**

- `vw_AtencionResumen` — cabecera con paciente, médico, especialidad, cita de origen, edad
  calculada a la fecha de la atención y conteo de diagnósticos.
- `vw_CitaResumen` — fuente única de la agenda: la consulta de citas, los filtros y el
  reporte leen exactamente la misma definición de "cita".

**Procedimientos almacenados (33):**

| Grupo | Procedimientos |
|---|---|
| Seguridad | `usp_Usuario_ObtenerPorNombre`, `usp_Usuario_RegistrarAcceso` |
| Maestros | `usp_Medico_Listar`, `usp_Especialidad_Listar` |
| Pacientes | `usp_Paciente_Listar`, `usp_Paciente_Buscar`, `usp_Paciente_ObtenerPorId`, `usp_Paciente_ExisteDocumento`, `usp_Paciente_Insertar`, `usp_Paciente_Actualizar`, `usp_Paciente_CambiarEstado` |
| Citas | `usp_Cita_Listar`, `usp_Cita_ObtenerPorId`, `usp_Cita_ListarPendientesPorPaciente`, `usp_Cita_Insertar`, `usp_Cita_Actualizar`, `usp_Cita_CambiarEstado`, `usp_Cita_MarcarAtendida`, `usp_Cita_Liberar` |
| Atenciones | `usp_Atencion_Listar`, `usp_Atencion_ObtenerPorId`, `usp_Atencion_Insertar`, `usp_Atencion_Actualizar`, `usp_AtencionDetalle_Insertar`, `usp_AtencionDetalle_EliminarPorAtencion`, `usp_Atencion_Anular`, `usp_Atencion_Eliminar` |
| CIE-10 | `usp_Cie10_Buscar`, `usp_Cie10_ObtenerPorCodigo`, `usp_Cie10_ObtenerVersion`, `usp_Cie10_ImportarDesdeStaging` |
| Reportes | `usp_Reporte_General`, `usp_Reporte_Atenciones` |

### Validaciones implementadas

**Pacientes.** DNI de exactamente 8 dígitos; otros documentos de 6 a 15 caracteres
alfanuméricos; nombres y apellido paterno obligatorios; fecha de nacimiento no futura y con
edad no superior a 120 años; formato de correo y teléfono. Los datos se normalizan al
guardar —espacios sobrantes, mayúsculas— para que `12345678 ` y `12345678` no convivan como
dos pacientes distintos.

**Citas.** Paciente y profesional obligatorios; horario de consulta externa de 07:00 a
20:00, sin domingos; no se programa con más de un año de anticipación ni hacia atrás; los
estados distintos de `CITADO` exigen motivo de al menos 5 caracteres; no se registra
inasistencia de una cita cuya hora aún no ha llegado.

**Atenciones.** Paciente, profesional y motivo obligatorios (motivo ≥ 5 caracteres); fecha
no futura ni con más de 5 años de antigüedad; temperatura 30–45 °C; frecuencia cardiaca
20–250 lpm; peso ≤ 400 kg; talla ≤ 2,50 m; presión arterial con formato
`sistólica/diastólica`; entre 1 y 20 diagnósticos, sin códigos repetidos; para marcar
*Atendida* debe existir al menos un diagnóstico definitivo; una atención anulada no puede
modificarse.

En los tres casos los errores se **acumulan y se muestran juntos**: quien registra decenas
de fichas al día no debería corregirlas de una en una.

---

## 10. Catálogo CIE-10 del MINSA

La fuente de diagnósticos es el catálogo oficial del MINSA **en español**, almacenado en la
propia base de datos:

```
Archivo oficial MINSA  →  CatalogoCie10_Staging  →  MERGE  →  CatalogoCie10  →  Sistema
```

Guardarlo localmente hace que la búsqueda responda en milisegundos y funcione sin conexión,
que es lo que un hospital necesita.

### Origen de los datos

**Fuente:** Ministerio de Salud del Perú — **REUNIS** (Repositorio Único Nacional de
Información en Salud), sección de catálogos y tablas maestras del sistema HIS-MINSA:

> https://www.minsa.gob.pe/reunis/

Allí se publican **CIE-10 (MINSA – Excel)**, el catálogo de diagnósticos en español que usa
el HIS, junto con las listas de **uso** y **cese de uso** de códigos CIE-10 de la
RM 447-2024.

Espejo en datos abiertos del Estado peruano, dataset **Tablas Maestras HISMINSA**, que
incluye la tabla de diagnósticos del HIS (`TB_DIAGNOSTICOS`):

> https://datosabiertos.gob.pe/

La estructura de `dbo.CatalogoCie10` está alineada con esa tabla maestra: código,
descripción, restricción de sexo, rango etario, estado y fecha de cese.

> Los enlaces del MINSA se reorganizan cada cierto tiempo. Si alguno no responde, buscar
> "REUNIS CIE-10 MINSA" o el dataset "Tablas Maestras HISMINSA" en datosabiertos.gob.pe.

### Qué carga el script 07

Una carga de arranque para que el sistema sea usable desde el primer día sin descargar
nada: el **capítulo V completo** a nivel de categoría (F00–F99), las subcategorías de las
familias más usadas en salud mental (F00, F10, F20, F31, F32, F33, F41, F43, F60, F84,
F90…), y códigos de morbilidad general y del capítulo XXI.

### Cómo importar el catálogo completo

1. Descargar el Excel del MINSA y guardarlo como CSV con las columnas
   `CodigoCie10, Descripcion, Sexo, EdadMinima, EdadMaxima, Estado`.
2. Volcarlo en la tabla de staging:

   ```sql
   TRUNCATE TABLE dbo.CatalogoCie10_Staging;

   BULK INSERT dbo.CatalogoCie10_Staging
   FROM 'C:\temp\cie10_minsa.csv'
   WITH (FIRSTROW = 2, FIELDTERMINATOR = ';', ROWTERMINATOR = '\n', CODEPAGE = '65001');
   ```

3. Consolidar:

   ```sql
   EXEC dbo.usp_Cie10_ImportarDesdeStaging
        @VersionCatalogo = '2024.2',
        @Fuente          = 'MINSA REUNIS - CIE-10 (Excel) RM 447-2024',
        @Usuario         = 'nombre.apellido';
   ```

El procedimiento devuelve cuántos códigos se insertaron, actualizaron y cesaron, y deja
constancia en `dbo.CatalogoCie10_Carga`.

### Cómo actualizarlo después

Exactamente igual: volcar el archivo nuevo en el staging y volver a ejecutar el
procedimiento. Es el mismo camino que usa la carga inicial, de modo que no existe un
procedimiento "de instalación" y otro "de actualización" que puedan divergir con el tiempo.

### Cómo se protegen los diagnósticos históricos

Tres mecanismos, en capas:

1. **Los códigos que desaparecen del archivo no se borran: se marcan como cesados**
   (`Estado = 'C'` con `FechaCese`). El propio MINSA publica listas de cese de uso; un
   código cesado sigue siendo válido para lo que ya se registró con él.
2. **`AtencionDetalle` guarda su propia copia del código y de la descripción.** El detalle
   no es una referencia al catálogo: es una fotografía de lo que el profesional escribió.
   Una actualización posterior no puede reescribir una historia clínica cerrada.
3. **Cada línea de detalle guarda la versión del catálogo** (`VersionCatalogoCie10`) con la
   que se codificó, de modo que siempre se puede saber contra qué edición se interpretó ese
   diagnóstico.

Por eso las búsquedas nuevas solo ofrecen códigos vigentes, pero la consulta de atenciones
antiguas sigue mostrando lo que se registró en su momento.

### Búsqueda

`usp_Cie10_Buscar` acepta indistintamente código o texto, con coincidencias parciales:

| Se escribe | Devuelve |
|---|---|
| `F20` | F20 y toda la familia F20.x |
| `F20.0` | El punto se normaliza automáticamente |
| `esquizofrenia` | Coincidencias por descripción |
| `depres` | Episodio depresivo, trastorno depresivo recurrente, depresión postesquizofrénica… |

Los resultados se ordenan poniendo primero la coincidencia exacta, luego las de prefijo de
código y luego las de prefijo de descripción.

---

## 11. Consumo de API REST

`Hospital.Integracion.Cie10ApiCliente` consulta el **Clinical Table Search Service** de la
U.S. National Library of Medicine. Es pública, gratuita y no requiere API key:

```
GET https://clinicaltables.nlm.nih.gov/api/icd10cm/v3/search?sf=code,name&maxList=25&terms=diabetes
```

Actúa como **fuente de respaldo**: solo se consulta cuando la búsqueda contra el catálogo
local del MINSA no devuelve resultados, y sus respuestas se marcan como referencia externa
porque vienen en inglés. La codificación oficial siempre es la del MINSA.

Puntos relevantes de la implementación:

- **`HttpClient` estático y reutilizado.** Crear una instancia por llamada agota los sockets
  disponibles —quedan en `TIME_WAIT`—, un problema clásico en aplicaciones de escritorio de
  larga ejecución.
- **`async` / `await` de extremo a extremo**, con `ConfigureAwait(false)` en las capas
  inferiores. La interfaz no se congela mientras se espera la red.
- **`CancellationToken`:** una búsqueda nueva cancela la anterior, de modo que escribir
  rápido no encole peticiones.
- **Deserialización con `JArray` (Json.NET)**, no con una clase POCO. La respuesta no es un
  objeto con propiedades fijas sino un arreglo posicional heterogéneo
  (`[total, [códigos], null, [[código, texto]]]`), contra el que no hay nada que mapear; se
  recorre la posición 3 y se proyecta a `DiagnosticoCie10`.
- **Degradación elegante:** si no hay internet, el formulario lo informa y permite ingresar
  el código y la descripción manualmente. Un fallo de red nunca impide registrar la
  atención.
- Se fuerza **TLS 1.2**, porque .NET Framework 4.7 puede negociar por defecto un protocolo
  que el servicio ya rechaza.

---

## 12. Reportes

Se renderizan con **ReportViewer** en modo local (RDLC), lo que resuelve la impresión y la
exportación a **PDF, Excel y Word** desde la propia barra del visor.

**Periodo:** rango de fechas libre —un día, una semana, un mes, varios meses o cualquier
periodo personalizado—. Los atajos de periodo solo rellenan las fechas; el criterio real
sigue siendo el rango.

**Filtros:** estado (todos, atendidos, no atendidos, citados, no acudieron, cancelados),
profesional, servicio o especialidad, documento del paciente y código CIE-10.

El reporte se construye sobre la unión de dos conjuntos:

- **toda cita del periodo**, con su atención si la tuvo;
- **toda atención del periodo sin cita previa** (demanda espontánea).

Esta unión es lo que hace que el filtro *No acudieron* muestre exactamente a quienes tenían
cita y no se presentaron, y no a cualquiera que simplemente no tenga una atención
registrada.

### Diseño del RDLC

```
[LOGO HVLH]  HOSPITAL NACIONAL VÍCTOR LARCO HERRERA - HVLH
             REPORTE DE PACIENTES Y ATENCIONES
             Periodo consultado: dd/MM/yyyy al dd/MM/yyyy     Generado el dd/MM/yyyy HH:mm
             Filtro aplicado: ...                             Usuario: ...
─────────────────────────────────────────────────────────────────────────────
 TOTAL REGISTROS │ ATENDIDOS │ NO ATENDIDOS │ CITADOS │ NO ACUDIERON │ CANCELADOS
─────────────────────────────────────────────────────────────────────────────
 tabla de detalle (12 columnas), con la cabecera repetida en cada página
─────────────────────────────────────────────────────────────────────────────
 pie institucional                                            Página N de M
```

El logo va incrustado en el propio RDLC como imagen embebida, de modo que aparece también en
el PDF, el Excel y el Word exportados, sin depender de rutas externas.

Lo exportado coincide siempre con lo que se ve, porque pantalla y exportación consumen el
mismo `DataTable`: no hay una consulta para mostrar y otra para exportar.

El archivo se genera con `herramientas/generar_rdlc_hvlh.py` porque el RDLC es XML muy
repetitivo —cada celda son doce líneas prácticamente idénticas—; generarlo garantiza
consistencia de anchos y estilos y convierte "agregar una columna" en cambiar una línea.

---

## 13. Cinco buenas prácticas de SQL Server

**1. Todo el acceso pasa por procedimientos almacenados con parámetros tipados.**
En ningún punto se concatena SQL. Esto elimina la inyección SQL de raíz, permite otorgar
permisos de `EXECUTE` sobre los procedimientos sin dar acceso directo a las tablas, y
favorece la reutilización del plan de ejecución en caché. *Aplicado en:* los 33
procedimientos y en `SqlAyudante.Agregar`, que además convierte nulos y cadenas vacías a
`DBNull`.

**2. Integridad declarada en el motor, no solo en la aplicación.**
Claves primarias y foráneas, `UNIQUE` (documento por tipo, número de cita, CIE-10 no
repetido dentro de una atención) y `CHECK` (estados válidos, sexo, temperatura entre 30 y
45 °C, frecuencia cardiaca entre 20 y 250). La base es la última línea de defensa: sigue
siendo válida aunque alguien inserte desde SSMS o desde otra aplicación. La FK del detalle
usa `ON DELETE CASCADE` para que no queden diagnósticos huérfanos.

Merecen mención aparte los **índices únicos filtrados**, que resuelven casos que un `UNIQUE`
normal no puede:

| Índice | Condición | Por qué |
|---|---|---|
| `UX_Paciente_HistoriaClinica` | `WHERE HistoriaClinica IS NOT NULL` | Única cuando existe; un `UNIQUE` normal solo toleraría un NULL en toda la tabla |
| `UX_Cita_Medico_Horario` | `WHERE Estado <> 'CANCELADO'` | Un profesional no tiene dos citas vigentes a la misma hora, pero sí puede reprogramar sobre un horario liberado |
| `UX_Atencion_Cita` | `WHERE IdCita IS NOT NULL AND Estado <> 'N'` | Una cita no genera dos atenciones vigentes |

**3. Índices diseñados a partir de las consultas reales, no "por si acaso".**
`IX_Atencion_Fecha_Estado` e `IX_Cita_Fecha_Estado` cubren el filtro principal de cada
grilla —rango de fechas más estado— e incluyen con `INCLUDE` las columnas que la consulta
devuelve, evitando el *key lookup*. También hay índices sobre las claves foráneas, que SQL
Server no crea automáticamente y que son necesarios para los JOIN de las vistas. Cada índice
acelera lecturas pero encarece escrituras, por eso solo se crearon los que sostienen
consultas concretas del sistema.

**4. Transacciones cortas, con `SET XACT_ABORT ON` y `TRY…CATCH` con `THROW`.**
La transacción abarca únicamente las escrituras de cabecera, detalle y estado de la cita,
sin trabajo de interfaz ni llamadas a la API dentro de ella, para minimizar el tiempo de
bloqueo. `XACT_ABORT ON` garantiza que cualquier error revierta la transacción completa, y
`THROW` —en lugar de `RAISERROR`— conserva número, mensaje y severidad originales, lo que
permite a la capa de datos clasificarlos. La responsabilidad transaccional está en un único
lugar por operación: nunca anidada entre el procedimiento y el cliente.

**5. Consultas SARGable y control del *parameter sniffing*.**
Los filtros por fecha usan `FechaCita >= @Desde AND FechaCita < DATEADD(DAY, 1, @Hasta)` en
lugar de `CONVERT(date, FechaCita) = @Fecha`: aplicar una función sobre la columna la vuelve
no SARGable y anula el índice. Los procedimientos con parámetros opcionales (`@Estado`,
`@IdMedico`, `@Busqueda`) usan `OPTION (RECOMPILE)` para que el motor genere un plan
adecuado a la combinación de filtros realmente enviada, en vez de reutilizar uno cacheado
para otra combinación muy distinta. Además, `SET NOCOUNT ON` en todos los procedimientos
evita el tráfico innecesario de mensajes `rows affected`.

**Sin cursores.** Ninguna operación los utiliza. La numeración de historias clínicas de los
pacientes históricos se resolvió con `ROW_NUMBER()`, la importación del catálogo con un
único `MERGE`, la clasificación por capítulos con `UPDATE` por rangos, y la concatenación de
diagnósticos del reporte con `STUFF` + `FOR XML PATH` —esta forma en lugar de `STRING_AGG`
para mantener compatibilidad con SQL Server 2016—.

---

## 14. Cobertura de los requisitos

| Requisito de la prueba | Dónde está resuelto |
|---|---|
| Login | `FrmLogin`, `UsuarioServicio`, PBKDF2-SHA256 en `Seguridad.cs` |
| CRUD de un proceso asistencial | Módulos de pacientes, citas y atenciones |
| Registro cabecera–detalle | `Atencion` + `AtencionDetalle`, formulario `FrmAtencionEdicion` |
| Validaciones de campos y reglas de negocio | `PacienteServicio`, `CitaServicio`, `AtencionServicio` |
| Transacciones cabecera + detalle | `AtencionRepositorio.Insertar` / `.Actualizar` |
| DataGridView con filtros | Grillas de pacientes, citas y atenciones |
| Reportes en ReportViewer | `FrmReportes` + `rptGeneralHVLH.rdlc` |
| Tablas relacionadas, PK / FK | `01_Tablas.sql`, `05_Migracion_HVLH.sql` |
| Stored Procedures | 33 procedimientos en `03` y `06` |
| Al menos una View | `vw_AtencionResumen` y `vw_CitaResumen` |
| Arquitectura por capas | 6 proyectos con dependencias unidireccionales |
| API REST con HttpClient, async/await y JSON | `Cie10ApiCliente` |
| Manejo de errores y excepciones | `Excepciones.cs`, `ErroresSql.cs`, `Registro.cs`, manejador global |
| 5 buenas prácticas de SQL Server | Sección 13 de este documento |
| Sin cursores | Verificado: ninguna operación los usa |

---

## 15. Limitaciones conocidas

- El alcance cubre pacientes, citas, atenciones y reportes. El mantenimiento de médicos,
  especialidades y usuarios queda fuera; esos datos se cargan por script.
- La interfaz usa medidas fijas y `AutoScaleMode.None` para verse igual en distintos
  equipos. En pantallas con escalado superior al 125 % conviene revisar los tamaños.
- El catálogo CIE-10 inicial es un subconjunto orientado a salud mental. El catálogo
  completo del MINSA se incorpora con la importación descrita en la sección 10.
- La búsqueda por descripción usa `LIKE '%texto%'`, que no es SARGable. Para un catálogo de
  decenas de miles de filas el índice de cobertura la resuelve sin problema; si creciera
  mucho más, la solución correcta sería un índice de texto completo (`FULLTEXT`) sobre
  `Descripcion`.
- No hay pruebas unitarias. Con más tiempo, la capa de negocio sería el primer lugar donde
  añadirlas, seguida de inyección de dependencias para poder sustituir los repositorios por
  dobles de prueba.
