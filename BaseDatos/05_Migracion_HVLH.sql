/* =============================================================================
   Hospital Nacional Víctor Larco Herrera - HVLH
   Script 05: MIGRACIÓN EVOLUTIVA sobre la base existente

   Este script NO borra ni recrea la base de datos ni las tablas ya existentes.
   Todas las modificaciones están protegidas con comprobaciones de existencia,
   por lo que puede ejecutarse varias veces sin efectos secundarios.

   Alcance:
     1. Amplía dbo.Paciente (tipo de documento, historia clínica, dirección, estado).
     2. Crea dbo.CatalogoCie10  (catálogo CIE-10 MINSA en español, local).
     3. Crea dbo.Cita           (agenda: CITADO / ATENDIDO / NO_ATENDIDO / NO_ACUDIO / CANCELADO).
     4. Enlaza dbo.Atencion con dbo.Cita.
     5. Actualiza vistas y crea los procedimientos de pacientes, citas, CIE-10 y reportes.

   Requisito previo: haber ejecutado 01, 02, 03 y 04.
   ============================================================================= */

USE HospitalAtencionesDB;
GO

SET NOCOUNT ON;
GO

PRINT '=== Migración HVLH: inicio ===';
GO

/* =============================================================================
   1. AMPLIACIÓN DE dbo.Paciente
   Se agregan columnas nuevas como NULL y luego se rellenan, para no fallar sobre
   filas ya existentes. Este es el patrón habitual de migración en caliente.
   ============================================================================= */

IF COL_LENGTH('dbo.Paciente', 'TipoDocumento') IS NULL
    ALTER TABLE dbo.Paciente ADD TipoDocumento VARCHAR(3) NULL;
GO

IF COL_LENGTH('dbo.Paciente', 'HistoriaClinica') IS NULL
    ALTER TABLE dbo.Paciente ADD HistoriaClinica VARCHAR(15) NULL;
GO

IF COL_LENGTH('dbo.Paciente', 'Direccion') IS NULL
    ALTER TABLE dbo.Paciente ADD Direccion NVARCHAR(150) NULL;
GO

IF COL_LENGTH('dbo.Paciente', 'Correo') IS NULL
    ALTER TABLE dbo.Paciente ADD Correo NVARCHAR(100) NULL;
GO

IF COL_LENGTH('dbo.Paciente', 'FechaRegistro') IS NULL
    ALTER TABLE dbo.Paciente ADD FechaRegistro DATETIME2(0) NULL;
GO

IF COL_LENGTH('dbo.Paciente', 'FechaModificacion') IS NULL
    ALTER TABLE dbo.Paciente ADD FechaModificacion DATETIME2(0) NULL;
GO

/* Secuencia para el número de historia clínica.
   Igual que el correlativo de atención: sin tabla de contadores, sin bloqueos
   y seguro en concurrencia. */
IF OBJECT_ID('dbo.SeqHistoriaClinica', 'SO') IS NULL
    CREATE SEQUENCE dbo.SeqHistoriaClinica AS INT START WITH 1 INCREMENT BY 1;
GO

/* Relleno de las columnas nuevas en los pacientes que ya existían. */
UPDATE dbo.Paciente
SET    TipoDocumento = 'DNI'
WHERE  TipoDocumento IS NULL;
GO

UPDATE dbo.Paciente
SET    FechaRegistro = SYSDATETIME()
WHERE  FechaRegistro IS NULL;
GO

/* Historia clínica para los pacientes históricos, en orden de identificador.
   ROW_NUMBER resuelve la numeración en una sola sentencia: no hace falta cursor. */
IF EXISTS (SELECT 1 FROM dbo.Paciente WHERE HistoriaClinica IS NULL)
BEGIN
    WITH Numerados AS
    (
        SELECT IdPaciente,
               ROW_NUMBER() OVER (ORDER BY IdPaciente) AS Orden
        FROM   dbo.Paciente
        WHERE  HistoriaClinica IS NULL
    )
    UPDATE p
    SET    p.HistoriaClinica = 'HC-' + RIGHT('000000' + CAST(n.Orden AS VARCHAR(6)), 6)
    FROM   dbo.Paciente p
    INNER JOIN Numerados n ON n.IdPaciente = p.IdPaciente;

    /* La secuencia continúa a partir del último valor asignado. */
    DECLARE @Siguiente INT = (SELECT ISNULL(MAX(CAST(RIGHT(HistoriaClinica, 6) AS INT)), 0) + 1
                              FROM dbo.Paciente WHERE HistoriaClinica LIKE 'HC-%');
    DECLARE @Reinicio NVARCHAR(200) =
        N'ALTER SEQUENCE dbo.SeqHistoriaClinica RESTART WITH ' + CAST(@Siguiente AS NVARCHAR(10)) + N';';
    EXEC sp_executesql @Reinicio;
END
GO

/* Una vez rellenas, las columnas obligatorias pasan a NOT NULL con su valor por defecto. */
IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID('dbo.Paciente') AND name = 'TipoDocumento' AND is_nullable = 1)
    ALTER TABLE dbo.Paciente ALTER COLUMN TipoDocumento VARCHAR(3) NOT NULL;
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID('dbo.Paciente') AND name = 'FechaRegistro' AND is_nullable = 1)
    ALTER TABLE dbo.Paciente ALTER COLUMN FechaRegistro DATETIME2(0) NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_Paciente_TipoDocumento')
    ALTER TABLE dbo.Paciente ADD CONSTRAINT DF_Paciente_TipoDocumento DEFAULT ('DNI') FOR TipoDocumento;
GO

IF NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_Paciente_FechaRegistro')
    ALTER TABLE dbo.Paciente ADD CONSTRAINT DF_Paciente_FechaRegistro DEFAULT (SYSDATETIME()) FOR FechaRegistro;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Paciente_TipoDocumento')
    ALTER TABLE dbo.Paciente ADD CONSTRAINT CK_Paciente_TipoDocumento
        CHECK (TipoDocumento IN ('DNI','CE','PAS','CNV','OTR'));
GO

/* El teléfono admite ahora prefijos y separadores. */
IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID('dbo.Paciente') AND name = 'Telefono' AND max_length < 20)
    ALTER TABLE dbo.Paciente ALTER COLUMN Telefono VARCHAR(20) NULL;
GO

/* La unicidad del documento pasa a ser por tipo + número: un DNI y un carné de
   extranjería pueden coincidir en dígitos sin ser la misma persona. */
IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_Paciente_Documento')
    ALTER TABLE dbo.Paciente DROP CONSTRAINT UQ_Paciente_Documento;
GO

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_Paciente_TipoNumeroDoc')
    ALTER TABLE dbo.Paciente ADD CONSTRAINT UQ_Paciente_TipoNumeroDoc UNIQUE (TipoDocumento, NumeroDocumento);
GO

/* Índice único filtrado: la historia clínica es única cuando existe, pero admite
   NULL en pacientes aún sin apertura de historia. Un UNIQUE normal solo toleraría
   un único NULL en toda la tabla. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Paciente_HistoriaClinica')
    CREATE UNIQUE NONCLUSTERED INDEX UX_Paciente_HistoriaClinica
        ON dbo.Paciente (HistoriaClinica)
        WHERE HistoriaClinica IS NOT NULL;
GO

PRINT '  [1/5] dbo.Paciente ampliada.';
GO

/* =============================================================================
   2. CATÁLOGO CIE-10 OFICIAL (MINSA - Perú), en español y almacenado localmente

   Estructura alineada con la tabla maestra de diagnósticos del HIS-MINSA
   (TB_DIAGNOSTICOS). Guardar el catálogo en la base evita depender de Internet
   en cada búsqueda y permite responder en milisegundos.
   ============================================================================= */

IF OBJECT_ID('dbo.CatalogoCie10', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CatalogoCie10
    (
        IdCatalogoCie10 INT IDENTITY(1,1) NOT NULL,
        CodigoCie10     VARCHAR(10)    NOT NULL,   -- Código oficial, sin punto (F200)
        CodigoFormato   VARCHAR(10)    NOT NULL,   -- Código con punto para mostrar (F20.0)
        Descripcion     NVARCHAR(250)  NOT NULL,   -- Descripción oficial en español
        Categoria       VARCHAR(5)     NULL,       -- Categoría de 3 caracteres (F20)
        Grupo           VARCHAR(20)    NULL,       -- Grupo CIE-10 (F20-F29)
        Capitulo        VARCHAR(5)     NULL,       -- Capítulo en romanos (V)
        CapituloNombre  NVARCHAR(150)  NULL,
        Sexo            CHAR(1)        NULL,       -- Restricción de sexo del diagnóstico
        EdadMinima      INT            NULL,       -- Restricciones etarias del HIS (en años)
        EdadMaxima      INT            NULL,
        Estado          CHAR(1)        NOT NULL,   -- V = vigente, C = cesado (RM 447-2024)
        FechaCese       DATE           NULL,
        Fuente          VARCHAR(60)    NOT NULL,
        VersionCatalogo VARCHAR(20)    NOT NULL,   -- Versión del archivo MINSA cargado
        FechaCarga      DATETIME2(0)   NOT NULL,
        CONSTRAINT PK_CatalogoCie10        PRIMARY KEY CLUSTERED (IdCatalogoCie10),
        CONSTRAINT UQ_CatalogoCie10_Codigo UNIQUE (CodigoCie10),
        CONSTRAINT CK_CatalogoCie10_Estado CHECK (Estado IN ('V','C')),
        CONSTRAINT CK_CatalogoCie10_Sexo   CHECK (Sexo IS NULL OR Sexo IN ('M','F'))
    );

    ALTER TABLE dbo.CatalogoCie10 ADD CONSTRAINT DF_CatalogoCie10_Estado     DEFAULT ('V')          FOR Estado;
    ALTER TABLE dbo.CatalogoCie10 ADD CONSTRAINT DF_CatalogoCie10_FechaCarga DEFAULT (SYSDATETIME()) FOR FechaCarga;
END
GO

/* La búsqueda por descripción usa LIKE '%texto%', que no es SARGable. Este índice
   de cobertura permite al menos resolverla con un recorrido del índice (más
   estrecho que la tabla) en lugar de leer la tabla completa. Para un catálogo de
   pocas decenas de miles de filas el tiempo de respuesta es imperceptible; si
   creciera, la solución correcta es un índice de texto completo (ver README). */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CatalogoCie10_Descripcion')
    CREATE NONCLUSTERED INDEX IX_CatalogoCie10_Descripcion
        ON dbo.CatalogoCie10 (Estado, Descripcion)
        INCLUDE (CodigoCie10, CodigoFormato, Categoria);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CatalogoCie10_Categoria')
    CREATE NONCLUSTERED INDEX IX_CatalogoCie10_Categoria
        ON dbo.CatalogoCie10 (Categoria)
        INCLUDE (CodigoCie10, Descripcion);
GO

/* Tabla intermedia para la carga del archivo oficial del MINSA. El archivo se
   vuelca aquí tal cual viene y desde aquí se consolida con MERGE. Así una
   importación defectuosa nunca deja el catálogo real a medio actualizar. */
IF OBJECT_ID('dbo.CatalogoCie10_Staging', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CatalogoCie10_Staging
    (
        CodigoCie10 VARCHAR(20)   NULL,
        Descripcion NVARCHAR(500) NULL,
        Sexo        VARCHAR(5)    NULL,
        EdadMinima  VARCHAR(10)   NULL,
        EdadMaxima  VARCHAR(10)   NULL,
        Estado      VARCHAR(10)   NULL
    );
END
GO

/* Traza de qué versión del catálogo se ha cargado y cuándo. */
IF OBJECT_ID('dbo.CatalogoCie10_Carga', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CatalogoCie10_Carga
    (
        IdCarga         INT IDENTITY(1,1) NOT NULL,
        VersionCatalogo VARCHAR(20)   NOT NULL,
        Fuente          VARCHAR(200)  NOT NULL,
        Insertados      INT           NOT NULL,
        Actualizados    INT           NOT NULL,
        Cesados         INT           NOT NULL,
        FechaCarga      DATETIME2(0)  NOT NULL CONSTRAINT DF_CatalogoCarga_Fecha DEFAULT (SYSDATETIME()),
        Usuario         NVARCHAR(120) NULL,
        CONSTRAINT PK_CatalogoCie10_Carga PRIMARY KEY CLUSTERED (IdCarga)
    );
END
GO

PRINT '  [2/5] Catálogo CIE-10 creado.';
GO

/* =============================================================================
   3. AGENDA DE CITAS

   PACIENTE (1) --- (N) CITA (1) --- (0..1) ATENCIÓN

   El estado de la cita es un dato registrado por el personal, nunca deducido:
   una cita solo es NO_ACUDIO si alguien la marca como tal. La ausencia de
   atención no convierte automáticamente una cita en inasistencia.
   ============================================================================= */

IF OBJECT_ID('dbo.SeqNumeroCita', 'SO') IS NULL
    CREATE SEQUENCE dbo.SeqNumeroCita AS INT START WITH 1 INCREMENT BY 1;
GO

IF OBJECT_ID('dbo.Cita', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cita
    (
        IdCita            INT IDENTITY(1,1) NOT NULL,
        NumeroCita        VARCHAR(15)    NOT NULL,
        IdPaciente        INT            NOT NULL,
        IdMedico          INT            NOT NULL,
        FechaCita         DATETIME2(0)   NOT NULL,
        MotivoCita        NVARCHAR(300)  NULL,
        Estado            VARCHAR(12)    NOT NULL,
        MotivoEstado      NVARCHAR(300)  NULL,   -- Justificación de cancelación / inasistencia
        Observaciones     NVARCHAR(500)  NULL,
        IdUsuarioRegistro INT            NOT NULL,
        FechaRegistro     DATETIME2(0)   NOT NULL,
        FechaModificacion DATETIME2(0)   NULL,
        CONSTRAINT PK_Cita            PRIMARY KEY CLUSTERED (IdCita),
        CONSTRAINT UQ_Cita_Numero     UNIQUE (NumeroCita),
        CONSTRAINT FK_Cita_Paciente   FOREIGN KEY (IdPaciente) REFERENCES dbo.Paciente (IdPaciente),
        CONSTRAINT FK_Cita_Medico     FOREIGN KEY (IdMedico)   REFERENCES dbo.Medico (IdMedico),
        CONSTRAINT FK_Cita_Usuario    FOREIGN KEY (IdUsuarioRegistro) REFERENCES dbo.Usuario (IdUsuario),
        CONSTRAINT CK_Cita_Estado     CHECK (Estado IN ('CITADO','ATENDIDO','NO_ATENDIDO','NO_ACUDIO','CANCELADO'))
    );

    ALTER TABLE dbo.Cita ADD CONSTRAINT DF_Cita_Estado        DEFAULT ('CITADO')      FOR Estado;
    ALTER TABLE dbo.Cita ADD CONSTRAINT DF_Cita_FechaRegistro DEFAULT (SYSDATETIME()) FOR FechaRegistro;
END
GO

/* El reporte filtra por rango de fechas y estado: índice de cobertura alineado
   con esa consulta, que es la única de alto volumen sobre la tabla. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Cita_Fecha_Estado')
    CREATE NONCLUSTERED INDEX IX_Cita_Fecha_Estado
        ON dbo.Cita (FechaCita, Estado)
        INCLUDE (IdPaciente, IdMedico, NumeroCita);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Cita_Paciente')
    CREATE NONCLUSTERED INDEX IX_Cita_Paciente ON dbo.Cita (IdPaciente, FechaCita);
GO

/* Índice único filtrado: un profesional no puede tener dos citas vigentes en el
   mismo instante, pero sí puede reprogramar sobre un horario ya cancelado. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Cita_Medico_Horario')
    CREATE UNIQUE NONCLUSTERED INDEX UX_Cita_Medico_Horario
        ON dbo.Cita (IdMedico, FechaCita)
        WHERE Estado <> 'CANCELADO';
GO

/* =============================================================================
   4. ENLACE Atención -> Cita

   La clave foránea vive en Atencion: una atención puede originarse en una cita
   o ser demanda espontánea (IdCita NULL). Se evita así una dependencia circular
   entre ambas tablas.
   ============================================================================= */

IF COL_LENGTH('dbo.Atencion', 'IdCita') IS NULL
    ALTER TABLE dbo.Atencion ADD IdCita INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Atencion_Cita')
    ALTER TABLE dbo.Atencion ADD CONSTRAINT FK_Atencion_Cita
        FOREIGN KEY (IdCita) REFERENCES dbo.Cita (IdCita);
GO

/* Una cita no puede dar lugar a dos atenciones vigentes. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Atencion_Cita')
    CREATE UNIQUE NONCLUSTERED INDEX UX_Atencion_Cita
        ON dbo.Atencion (IdCita)
        WHERE IdCita IS NOT NULL AND Estado <> 'N';
GO

/* Versión del catálogo CIE-10 vigente cuando se registró el diagnóstico.
   El detalle conserva su propia copia del código y la descripción: una
   actualización posterior del catálogo jamás reescribe la historia clínica. */
IF COL_LENGTH('dbo.AtencionDetalle', 'VersionCatalogoCie10') IS NULL
    ALTER TABLE dbo.AtencionDetalle ADD VersionCatalogoCie10 VARCHAR(20) NULL;
GO

PRINT '  [3/5] Agenda de citas creada y enlazada con atenciones.';
GO

/* =============================================================================
   5. VISTAS
   ============================================================================= */

IF OBJECT_ID('dbo.vw_AtencionResumen', 'V') IS NOT NULL DROP VIEW dbo.vw_AtencionResumen;
GO

CREATE VIEW dbo.vw_AtencionResumen
AS
SELECT
    a.IdAtencion,
    a.NumeroAtencion,
    a.FechaAtencion,
    a.IdCita,
    c.NumeroCita,
    c.FechaCita,
    a.IdPaciente,
    p.TipoDocumento,
    p.NumeroDocumento                                            AS DocumentoPaciente,
    p.HistoriaClinica,
    (p.ApellidoPaterno + ' ' + ISNULL(p.ApellidoMaterno, '') + ', ' + p.Nombres) AS Paciente,
    DATEDIFF(YEAR, p.FechaNacimiento, a.FechaAtencion)
        - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, p.FechaNacimiento, a.FechaAtencion), p.FechaNacimiento)
                    > CAST(a.FechaAtencion AS DATE)
               THEN 1 ELSE 0 END                                 AS EdadPaciente,
    p.Sexo,
    a.IdMedico,
    (m.Apellidos + ', ' + m.Nombres)                             AS Medico,
    m.IdEspecialidad,
    e.Nombre                                                     AS Especialidad,
    a.MotivoConsulta,
    a.Temperatura,
    a.PresionArterial,
    a.FrecuenciaCardiaca,
    a.Peso,
    a.Talla,
    a.Observaciones,
    a.Estado,
    CASE a.Estado WHEN 'R' THEN 'Registrada'
                  WHEN 'A' THEN 'Atendida'
                  WHEN 'N' THEN 'Anulada' END                    AS EstadoDescripcion,
    (SELECT COUNT(1) FROM dbo.AtencionDetalle d WHERE d.IdAtencion = a.IdAtencion) AS TotalDiagnosticos,
    u.NombreCompleto                                             AS UsuarioRegistro,
    a.FechaRegistro,
    a.FechaModificacion
FROM dbo.Atencion      a
INNER JOIN dbo.Paciente     p ON p.IdPaciente     = a.IdPaciente
INNER JOIN dbo.Medico       m ON m.IdMedico       = a.IdMedico
INNER JOIN dbo.Especialidad e ON e.IdEspecialidad = m.IdEspecialidad
INNER JOIN dbo.Usuario      u ON u.IdUsuario      = a.IdUsuarioRegistro
LEFT  JOIN dbo.Cita         c ON c.IdCita         = a.IdCita;
GO

IF OBJECT_ID('dbo.vw_CitaResumen', 'V') IS NOT NULL DROP VIEW dbo.vw_CitaResumen;
GO

/* -----------------------------------------------------------------------------
   vw_CitaResumen
   Fuente única de la agenda: la consulta de citas, los filtros y el reporte leen
   exactamente la misma definición de "cita".
   ----------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_CitaResumen
AS
SELECT
    c.IdCita,
    c.NumeroCita,
    c.FechaCita,
    c.IdPaciente,
    p.TipoDocumento,
    p.NumeroDocumento                                            AS DocumentoPaciente,
    p.HistoriaClinica,
    (p.ApellidoPaterno + ' ' + ISNULL(p.ApellidoMaterno, '') + ', ' + p.Nombres) AS Paciente,
    p.Sexo,
    DATEDIFF(YEAR, p.FechaNacimiento, c.FechaCita)
        - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, p.FechaNacimiento, c.FechaCita), p.FechaNacimiento)
                    > CAST(c.FechaCita AS DATE)
               THEN 1 ELSE 0 END                                 AS EdadPaciente,
    c.IdMedico,
    (m.Apellidos + ', ' + m.Nombres)                             AS Medico,
    m.IdEspecialidad,
    e.Nombre                                                     AS Especialidad,
    c.MotivoCita,
    c.Estado,
    CASE c.Estado WHEN 'CITADO'      THEN 'Citado'
                  WHEN 'ATENDIDO'    THEN 'Atendido'
                  WHEN 'NO_ATENDIDO' THEN 'No atendido'
                  WHEN 'NO_ACUDIO'   THEN 'No acudió'
                  WHEN 'CANCELADO'   THEN 'Cancelado' END        AS EstadoDescripcion,
    c.MotivoEstado,
    c.Observaciones,
    a.IdAtencion,
    a.NumeroAtencion,
    a.FechaAtencion,
    u.NombreCompleto                                             AS UsuarioRegistro,
    c.FechaRegistro,
    c.FechaModificacion
FROM dbo.Cita          c
INNER JOIN dbo.Paciente     p ON p.IdPaciente     = c.IdPaciente
INNER JOIN dbo.Medico       m ON m.IdMedico       = c.IdMedico
INNER JOIN dbo.Especialidad e ON e.IdEspecialidad = m.IdEspecialidad
INNER JOIN dbo.Usuario      u ON u.IdUsuario      = c.IdUsuarioRegistro
LEFT  JOIN dbo.Atencion     a ON a.IdCita         = c.IdCita AND a.Estado <> 'N';
GO

PRINT '  [4/5] Vistas actualizadas.';
GO

PRINT '=== Migración HVLH: estructura completada. Ejecute 06 y 07. ===';
GO
