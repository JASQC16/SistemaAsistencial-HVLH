/* =============================================================================
   Sistema de Atenciones Ambulatorias - Hospital
   Script 01: Base de datos, tablas, restricciones e índices
   Motor: SQL Server 2016 o superior
   ============================================================================= */

IF DB_ID('HospitalAtencionesDB') IS NULL
    CREATE DATABASE HospitalAtencionesDB;
GO

USE HospitalAtencionesDB;
GO

/* ---------------------------------------------------------------------------
   Limpieza (permite re-ejecutar el script en un entorno de pruebas)
   Se eliminan en orden inverso a las dependencias de claves foráneas.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('dbo.AtencionDetalle', 'U') IS NOT NULL DROP TABLE dbo.AtencionDetalle;
IF OBJECT_ID('dbo.Atencion', 'U')        IS NOT NULL DROP TABLE dbo.Atencion;
IF OBJECT_ID('dbo.Medico', 'U')          IS NOT NULL DROP TABLE dbo.Medico;
IF OBJECT_ID('dbo.Especialidad', 'U')    IS NOT NULL DROP TABLE dbo.Especialidad;
IF OBJECT_ID('dbo.Paciente', 'U')        IS NOT NULL DROP TABLE dbo.Paciente;
IF OBJECT_ID('dbo.Usuario', 'U')         IS NOT NULL DROP TABLE dbo.Usuario;
IF OBJECT_ID('dbo.SeqNumeroAtencion', 'SO') IS NOT NULL DROP SEQUENCE dbo.SeqNumeroAtencion;
GO

/* ---------------------------------------------------------------------------
   Seguridad
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.Usuario
(
    IdUsuario       INT IDENTITY(1,1) NOT NULL,
    NombreUsuario   VARCHAR(30)    NOT NULL,
    ClaveHash       VARCHAR(200)   NOT NULL,   -- PBKDF2-SHA256, 20.000 iteraciones (Base64)
    ClaveSalt       VARCHAR(50)    NOT NULL,   -- Salt aleatorio por usuario (Base64)
    NombreCompleto  NVARCHAR(120)  NOT NULL,
    Rol             VARCHAR(20)    NOT NULL,
    Activo          BIT            NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT (1),
    UltimoAcceso    DATETIME2(0)   NULL,
    FechaRegistro   DATETIME2(0)   NOT NULL CONSTRAINT DF_Usuario_FechaReg DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Usuario          PRIMARY KEY CLUSTERED (IdUsuario),
    CONSTRAINT UQ_Usuario_Nombre   UNIQUE (NombreUsuario),
    CONSTRAINT CK_Usuario_Rol      CHECK (Rol IN ('ADMINISTRADOR','MEDICO','ASISTENCIAL'))
);
GO

/* ---------------------------------------------------------------------------
   Maestros asistenciales
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.Especialidad
(
    IdEspecialidad  INT IDENTITY(1,1) NOT NULL,
    Nombre          NVARCHAR(80)   NOT NULL,
    Activo          BIT            NOT NULL CONSTRAINT DF_Especialidad_Activo DEFAULT (1),
    CONSTRAINT PK_Especialidad        PRIMARY KEY CLUSTERED (IdEspecialidad),
    CONSTRAINT UQ_Especialidad_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE dbo.Medico
(
    IdMedico          INT IDENTITY(1,1) NOT NULL,
    NumeroColegiatura VARCHAR(15)    NOT NULL,
    Nombres           NVARCHAR(60)   NOT NULL,
    Apellidos         NVARCHAR(60)   NOT NULL,
    IdEspecialidad    INT            NOT NULL,
    Activo            BIT            NOT NULL CONSTRAINT DF_Medico_Activo DEFAULT (1),
    CONSTRAINT PK_Medico                PRIMARY KEY CLUSTERED (IdMedico),
    CONSTRAINT UQ_Medico_Colegiatura    UNIQUE (NumeroColegiatura),
    CONSTRAINT FK_Medico_Especialidad   FOREIGN KEY (IdEspecialidad)
        REFERENCES dbo.Especialidad (IdEspecialidad)
);
GO

CREATE TABLE dbo.Paciente
(
    IdPaciente      INT IDENTITY(1,1) NOT NULL,
    NumeroDocumento VARCHAR(15)    NOT NULL,
    Nombres         NVARCHAR(60)   NOT NULL,
    ApellidoPaterno NVARCHAR(40)   NOT NULL,
    ApellidoMaterno NVARCHAR(40)   NULL,
    FechaNacimiento DATE           NOT NULL,
    Sexo            CHAR(1)        NOT NULL,
    Telefono        VARCHAR(15)    NULL,
    Activo          BIT            NOT NULL CONSTRAINT DF_Paciente_Activo DEFAULT (1),
    CONSTRAINT PK_Paciente            PRIMARY KEY CLUSTERED (IdPaciente),
    CONSTRAINT UQ_Paciente_Documento  UNIQUE (NumeroDocumento),
    CONSTRAINT CK_Paciente_Sexo       CHECK (Sexo IN ('M','F')),
    CONSTRAINT CK_Paciente_FechaNac   CHECK (FechaNacimiento <= CAST(SYSDATETIME() AS DATE))
);
GO

/* Índice de apoyo para la búsqueda por apellidos desde la pantalla de consulta */
CREATE NONCLUSTERED INDEX IX_Paciente_Apellidos
    ON dbo.Paciente (ApellidoPaterno, ApellidoMaterno)
    INCLUDE (Nombres, NumeroDocumento);
GO

/* ---------------------------------------------------------------------------
   Proceso asistencial: CABECERA
   --------------------------------------------------------------------------- */
CREATE SEQUENCE dbo.SeqNumeroAtencion AS INT START WITH 1 INCREMENT BY 1;
GO

CREATE TABLE dbo.Atencion
(
    IdAtencion        INT IDENTITY(1,1) NOT NULL,
    NumeroAtencion    VARCHAR(15)    NOT NULL,
    IdPaciente        INT            NOT NULL,
    IdMedico          INT            NOT NULL,
    FechaAtencion     DATETIME2(0)   NOT NULL,
    MotivoConsulta    NVARCHAR(300)  NOT NULL,
    Temperatura       DECIMAL(4,1)   NULL,
    PresionArterial   VARCHAR(10)    NULL,
    FrecuenciaCardiaca INT           NULL,
    Peso              DECIMAL(5,2)   NULL,
    Talla             DECIMAL(4,2)   NULL,
    Observaciones     NVARCHAR(500)  NULL,
    Estado            CHAR(1)        NOT NULL CONSTRAINT DF_Atencion_Estado DEFAULT ('R'),   -- R=Registrada, A=Atendida, N=Anulada
    IdUsuarioRegistro INT            NOT NULL,
    FechaRegistro     DATETIME2(0)   NOT NULL CONSTRAINT DF_Atencion_FechaReg DEFAULT (SYSDATETIME()),
    FechaModificacion DATETIME2(0)   NULL,
    CONSTRAINT PK_Atencion              PRIMARY KEY CLUSTERED (IdAtencion),
    CONSTRAINT UQ_Atencion_Numero       UNIQUE (NumeroAtencion),
    CONSTRAINT FK_Atencion_Paciente     FOREIGN KEY (IdPaciente) REFERENCES dbo.Paciente (IdPaciente),
    CONSTRAINT FK_Atencion_Medico       FOREIGN KEY (IdMedico)   REFERENCES dbo.Medico (IdMedico),
    CONSTRAINT FK_Atencion_Usuario      FOREIGN KEY (IdUsuarioRegistro) REFERENCES dbo.Usuario (IdUsuario),
    CONSTRAINT CK_Atencion_Estado       CHECK (Estado IN ('R','A','N')),
    CONSTRAINT CK_Atencion_Temperatura  CHECK (Temperatura IS NULL OR Temperatura BETWEEN 30.0 AND 45.0),
    CONSTRAINT CK_Atencion_Frecuencia   CHECK (FrecuenciaCardiaca IS NULL OR FrecuenciaCardiaca BETWEEN 20 AND 250)
);
GO

/* La consulta principal filtra por rango de fechas y estado: índice de cobertura */
CREATE NONCLUSTERED INDEX IX_Atencion_Fecha_Estado
    ON dbo.Atencion (FechaAtencion DESC, Estado)
    INCLUDE (IdPaciente, IdMedico, NumeroAtencion);
GO

CREATE NONCLUSTERED INDEX IX_Atencion_Paciente ON dbo.Atencion (IdPaciente);
CREATE NONCLUSTERED INDEX IX_Atencion_Medico   ON dbo.Atencion (IdMedico);
GO

/* ---------------------------------------------------------------------------
   Proceso asistencial: DETALLE (diagnósticos CIE-10 de la atención)
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.AtencionDetalle
(
    IdAtencionDetalle     INT IDENTITY(1,1) NOT NULL,
    IdAtencion            INT            NOT NULL,
    Item                  INT            NOT NULL,
    CodigoCie10           VARCHAR(10)    NOT NULL,
    DescripcionDiagnostico NVARCHAR(250) NOT NULL,
    TipoDiagnostico       CHAR(1)        NOT NULL,   -- P=Presuntivo, D=Definitivo, R=Repetitivo
    Indicaciones          NVARCHAR(300)  NULL,
    CONSTRAINT PK_AtencionDetalle          PRIMARY KEY CLUSTERED (IdAtencionDetalle),
    CONSTRAINT FK_AtencionDetalle_Atencion FOREIGN KEY (IdAtencion)
        REFERENCES dbo.Atencion (IdAtencion) ON DELETE CASCADE,
    CONSTRAINT UQ_AtencionDetalle_Item     UNIQUE (IdAtencion, Item),
    CONSTRAINT UQ_AtencionDetalle_Cie      UNIQUE (IdAtencion, CodigoCie10),
    CONSTRAINT CK_AtencionDetalle_Tipo     CHECK (TipoDiagnostico IN ('P','D','R'))
);
GO

CREATE NONCLUSTERED INDEX IX_AtencionDetalle_Atencion
    ON dbo.AtencionDetalle (IdAtencion) INCLUDE (CodigoCie10, DescripcionDiagnostico, TipoDiagnostico);
GO

PRINT 'Script 01 - Tablas creadas correctamente.';
GO
/* =============================================================================
   Script 02: Vistas
   ============================================================================= */
USE HospitalAtencionesDB;
GO

IF OBJECT_ID('dbo.vw_AtencionResumen', 'V') IS NOT NULL DROP VIEW dbo.vw_AtencionResumen;
GO

/* -----------------------------------------------------------------------------
   vw_AtencionResumen
   Centraliza el JOIN de cabecera + maestros para que la grilla, los filtros y el
   reporte consuman exactamente la misma definición de "atención". Si mañana se
   agrega un dato al resumen, se modifica en un solo lugar.
   ----------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_AtencionResumen
AS
SELECT
    a.IdAtencion,
    a.NumeroAtencion,
    a.FechaAtencion,
    a.IdPaciente,
    p.NumeroDocumento                                            AS DocumentoPaciente,
    (p.ApellidoPaterno + ' ' + ISNULL(p.ApellidoMaterno, '') + ', ' + p.Nombres) AS Paciente,
    DATEDIFF(YEAR, p.FechaNacimiento, a.FechaAtencion)
        - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, p.FechaNacimiento, a.FechaAtencion), p.FechaNacimiento)
                    > CAST(a.FechaAtencion AS DATE)
               THEN 1 ELSE 0 END                                 AS EdadPaciente,
    p.Sexo,
    a.IdMedico,
    (m.Apellidos + ', ' + m.Nombres)                             AS Medico,
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
INNER JOIN dbo.Usuario      u ON u.IdUsuario      = a.IdUsuarioRegistro;
GO

PRINT 'Script 02 - Vista vw_AtencionResumen creada correctamente.';
GO
/* =============================================================================
   Script 03: Stored Procedures
   Convención: usp_<Entidad>_<Acción>
   Todos los accesos a datos de la aplicación pasan por estos procedimientos.
   No se utilizan cursores en ninguna operación.
   ============================================================================= */
USE HospitalAtencionesDB;
GO

/* ============================ SEGURIDAD ==================================== */
IF OBJECT_ID('dbo.usp_Usuario_ObtenerPorNombre', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Usuario_ObtenerPorNombre;
GO
CREATE PROCEDURE dbo.usp_Usuario_ObtenerPorNombre
    @NombreUsuario VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdUsuario, NombreUsuario, ClaveHash, ClaveSalt, NombreCompleto, Rol, Activo, UltimoAcceso
    FROM   dbo.Usuario
    WHERE  NombreUsuario = @NombreUsuario;
END
GO

IF OBJECT_ID('dbo.usp_Usuario_RegistrarAcceso', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Usuario_RegistrarAcceso;
GO
CREATE PROCEDURE dbo.usp_Usuario_RegistrarAcceso
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Usuario SET UltimoAcceso = SYSDATETIME() WHERE IdUsuario = @IdUsuario;
END
GO

/* ============================ MAESTROS ===================================== */
IF OBJECT_ID('dbo.usp_Medico_Listar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Medico_Listar;
GO
CREATE PROCEDURE dbo.usp_Medico_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT m.IdMedico,
           m.NumeroColegiatura,
           m.Nombres,
           m.Apellidos,
           m.IdEspecialidad,
           e.Nombre AS Especialidad
    FROM   dbo.Medico m
    INNER JOIN dbo.Especialidad e ON e.IdEspecialidad = m.IdEspecialidad
    WHERE  m.Activo = 1
    ORDER BY m.Apellidos, m.Nombres;
END
GO

IF OBJECT_ID('dbo.usp_Paciente_Buscar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Paciente_Buscar;
GO
CREATE PROCEDURE dbo.usp_Paciente_Buscar
    @Busqueda NVARCHAR(100) = NULL   -- documento, nombres o apellidos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (50)
           IdPaciente, NumeroDocumento, Nombres, ApellidoPaterno, ApellidoMaterno,
           FechaNacimiento, Sexo, Telefono
    FROM   dbo.Paciente
    WHERE  Activo = 1
      AND (@Busqueda IS NULL
           OR NumeroDocumento LIKE @Busqueda + '%'
           OR ApellidoPaterno LIKE @Busqueda + '%'
           OR Nombres         LIKE @Busqueda + '%')
    ORDER BY ApellidoPaterno, ApellidoMaterno, Nombres
    OPTION (RECOMPILE);   -- filtros opcionales: evita reutilizar un plan inadecuado
END
GO

/* ==================== PROCESO ASISTENCIAL - CONSULTAS ====================== */
IF OBJECT_ID('dbo.usp_Atencion_Listar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_Listar;
GO
CREATE PROCEDURE dbo.usp_Atencion_Listar
    @FechaDesde DATE          = NULL,
    @FechaHasta DATE          = NULL,
    @Busqueda   NVARCHAR(100) = NULL,   -- N° de atención, documento o apellido del paciente
    @IdMedico   INT           = NULL,
    @Estado     CHAR(1)       = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdAtencion, NumeroAtencion, FechaAtencion, DocumentoPaciente, Paciente,
           EdadPaciente, Medico, Especialidad, MotivoConsulta, Estado, EstadoDescripcion,
           TotalDiagnosticos, UsuarioRegistro
    FROM   dbo.vw_AtencionResumen
    WHERE  (@FechaDesde IS NULL OR FechaAtencion >= @FechaDesde)
      AND  (@FechaHasta IS NULL OR FechaAtencion <  DATEADD(DAY, 1, @FechaHasta))
      AND  (@IdMedico   IS NULL OR IdMedico = @IdMedico)
      AND  (@Estado     IS NULL OR Estado   = @Estado)
      AND  (@Busqueda   IS NULL
            OR NumeroAtencion    LIKE @Busqueda + '%'
            OR DocumentoPaciente LIKE @Busqueda + '%'
            OR Paciente          LIKE '%' + @Busqueda + '%')
    ORDER BY FechaAtencion DESC, IdAtencion DESC
    OPTION (RECOMPILE);
END
GO

IF OBJECT_ID('dbo.usp_Atencion_ObtenerPorId', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_ObtenerPorId;
GO
CREATE PROCEDURE dbo.usp_Atencion_ObtenerPorId
    @IdAtencion INT
AS
BEGIN
    SET NOCOUNT ON;

    /* Resultset 1: cabecera */
    SELECT a.IdAtencion, a.NumeroAtencion, a.IdPaciente, a.IdMedico, a.FechaAtencion,
           a.MotivoConsulta, a.Temperatura, a.PresionArterial, a.FrecuenciaCardiaca,
           a.Peso, a.Talla, a.Observaciones, a.Estado, a.IdUsuarioRegistro,
           a.FechaRegistro, a.FechaModificacion,
           p.NumeroDocumento AS DocumentoPaciente,
           (p.ApellidoPaterno + ' ' + ISNULL(p.ApellidoMaterno,'') + ', ' + p.Nombres) AS NombrePaciente
    FROM   dbo.Atencion a
    INNER JOIN dbo.Paciente p ON p.IdPaciente = a.IdPaciente
    WHERE  a.IdAtencion = @IdAtencion;

    /* Resultset 2: detalle */
    SELECT IdAtencionDetalle, IdAtencion, Item, CodigoCie10, DescripcionDiagnostico,
           TipoDiagnostico, Indicaciones
    FROM   dbo.AtencionDetalle
    WHERE  IdAtencion = @IdAtencion
    ORDER BY Item;
END
GO

/* ==================== PROCESO ASISTENCIAL - ESCRITURA ===================== */
/*
   Nota de diseño: estos procedimientos de escritura NO abren ni confirman
   transacciones propias. La transacción que agrupa cabecera + detalle se abre
   desde la capa de acceso a datos (SqlTransaction), porque una sola operación
   de negocio ejecuta varios SP. Si cada SP hiciera ROLLBACK por su cuenta,
   anularía la transacción externa y dejaría el @@TRANCOUNT inconsistente.
   Los errores se propagan al cliente con THROW y allí se decide el rollback.
*/
IF OBJECT_ID('dbo.usp_Atencion_Insertar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_Insertar;
GO
CREATE PROCEDURE dbo.usp_Atencion_Insertar
    @IdPaciente        INT,
    @IdMedico          INT,
    @FechaAtencion     DATETIME2(0),
    @MotivoConsulta    NVARCHAR(300),
    @Temperatura       DECIMAL(4,1)  = NULL,
    @PresionArterial   VARCHAR(10)   = NULL,
    @FrecuenciaCardiaca INT          = NULL,
    @Peso              DECIMAL(5,2)  = NULL,
    @Talla             DECIMAL(4,2)  = NULL,
    @Observaciones     NVARCHAR(500) = NULL,
    @Estado            CHAR(1),
    @IdUsuarioRegistro INT,
    @IdAtencion        INT OUTPUT,
    @NumeroAtencion    VARCHAR(15) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Paciente WHERE IdPaciente = @IdPaciente AND Activo = 1)
        THROW 50001, 'El paciente indicado no existe o está inactivo.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Medico WHERE IdMedico = @IdMedico AND Activo = 1)
        THROW 50002, 'El médico indicado no existe o está inactivo.', 1;

    /* Correlativo con SEQUENCE: sin bloqueos ni cursores, seguro en concurrencia */
    SET @NumeroAtencion = 'AT-' + CAST(YEAR(@FechaAtencion) AS VARCHAR(4)) + '-'
                        + RIGHT('000000' + CAST(NEXT VALUE FOR dbo.SeqNumeroAtencion AS VARCHAR(6)), 6);

    INSERT INTO dbo.Atencion
        (NumeroAtencion, IdPaciente, IdMedico, FechaAtencion, MotivoConsulta, Temperatura,
         PresionArterial, FrecuenciaCardiaca, Peso, Talla, Observaciones, Estado,
         IdUsuarioRegistro, FechaRegistro)
    VALUES
        (@NumeroAtencion, @IdPaciente, @IdMedico, @FechaAtencion, @MotivoConsulta, @Temperatura,
         @PresionArterial, @FrecuenciaCardiaca, @Peso, @Talla, @Observaciones, @Estado,
         @IdUsuarioRegistro, SYSDATETIME());

    SET @IdAtencion = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.usp_Atencion_Actualizar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_Actualizar;
GO
CREATE PROCEDURE dbo.usp_Atencion_Actualizar
    @IdAtencion        INT,
    @IdPaciente        INT,
    @IdMedico          INT,
    @FechaAtencion     DATETIME2(0),
    @MotivoConsulta    NVARCHAR(300),
    @Temperatura       DECIMAL(4,1)  = NULL,
    @PresionArterial   VARCHAR(10)   = NULL,
    @FrecuenciaCardiaca INT          = NULL,
    @Peso              DECIMAL(5,2)  = NULL,
    @Talla             DECIMAL(4,2)  = NULL,
    @Observaciones     NVARCHAR(500) = NULL,
    @Estado            CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Atencion WHERE IdAtencion = @IdAtencion)
        THROW 50003, 'La atención que intenta modificar no existe.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Atencion WHERE IdAtencion = @IdAtencion AND Estado = 'N')
        THROW 50004, 'No se puede modificar una atención anulada.', 1;

    UPDATE dbo.Atencion
    SET    IdPaciente         = @IdPaciente,
           IdMedico           = @IdMedico,
           FechaAtencion      = @FechaAtencion,
           MotivoConsulta     = @MotivoConsulta,
           Temperatura        = @Temperatura,
           PresionArterial    = @PresionArterial,
           FrecuenciaCardiaca = @FrecuenciaCardiaca,
           Peso               = @Peso,
           Talla              = @Talla,
           Observaciones      = @Observaciones,
           Estado             = @Estado,
           FechaModificacion  = SYSDATETIME()
    WHERE  IdAtencion = @IdAtencion;
END
GO

IF OBJECT_ID('dbo.usp_AtencionDetalle_Insertar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_AtencionDetalle_Insertar;
GO
CREATE PROCEDURE dbo.usp_AtencionDetalle_Insertar
    @IdAtencion            INT,
    @Item                  INT,
    @CodigoCie10           VARCHAR(10),
    @DescripcionDiagnostico NVARCHAR(250),
    @TipoDiagnostico       CHAR(1),
    @Indicaciones          NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.AtencionDetalle
        (IdAtencion, Item, CodigoCie10, DescripcionDiagnostico, TipoDiagnostico, Indicaciones)
    VALUES
        (@IdAtencion, @Item, @CodigoCie10, @DescripcionDiagnostico, @TipoDiagnostico, @Indicaciones);
END
GO

IF OBJECT_ID('dbo.usp_AtencionDetalle_EliminarPorAtencion', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_AtencionDetalle_EliminarPorAtencion;
GO
CREATE PROCEDURE dbo.usp_AtencionDetalle_EliminarPorAtencion
    @IdAtencion INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.AtencionDetalle WHERE IdAtencion = @IdAtencion;
END
GO

IF OBJECT_ID('dbo.usp_Atencion_Anular', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_Anular;
GO
CREATE PROCEDURE dbo.usp_Atencion_Anular
    @IdAtencion INT,
    @Motivo     NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Atencion WHERE IdAtencion = @IdAtencion)
        THROW 50005, 'La atención indicada no existe.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Atencion WHERE IdAtencion = @IdAtencion AND Estado = 'N')
        THROW 50006, 'La atención ya se encuentra anulada.', 1;

    /* Baja lógica: la historia clínica no se borra, se marca como anulada */
    UPDATE dbo.Atencion
    SET    Estado            = 'N',
           Observaciones     = LEFT(ISNULL(Observaciones + ' | ', '') + 'ANULADA: ' + @Motivo, 500),
           FechaModificacion = SYSDATETIME()
    WHERE  IdAtencion = @IdAtencion;
END
GO

IF OBJECT_ID('dbo.usp_Atencion_Eliminar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_Eliminar;
GO
/*
   Eliminación física. A diferencia de los SP anteriores, esta operación es
   autocontenida (cabecera + detalle en un solo llamado), por lo que aquí sí
   corresponde manejar la transacción dentro del procedimiento.
*/
CREATE PROCEDURE dbo.usp_Atencion_Eliminar
    @IdAtencion INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;   -- cualquier error aborta y revierte la transacción

    BEGIN TRY
        BEGIN TRANSACTION;

            IF NOT EXISTS (SELECT 1 FROM dbo.Atencion WHERE IdAtencion = @IdAtencion)
                THROW 50007, 'La atención indicada no existe.', 1;

            DELETE FROM dbo.AtencionDetalle WHERE IdAtencion = @IdAtencion;
            DELETE FROM dbo.Atencion        WHERE IdAtencion = @IdAtencion;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;   -- se conserva el número, mensaje y severidad originales
    END CATCH
END
GO

/* ============================== REPORTE ==================================== */
IF OBJECT_ID('dbo.usp_Reporte_Atenciones', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Reporte_Atenciones;
GO
CREATE PROCEDURE dbo.usp_Reporte_Atenciones
    @FechaDesde DATE,
    @FechaHasta DATE,
    @IdMedico   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.NumeroAtencion,
           r.FechaAtencion,
           r.DocumentoPaciente,
           r.Paciente,
           r.EdadPaciente,
           r.Sexo,
           r.Medico,
           r.Especialidad,
           r.MotivoConsulta,
           r.EstadoDescripcion,
           ISNULL(d.CodigoCie10, '-')            AS CodigoCie10,
           ISNULL(d.DescripcionDiagnostico, 'Sin diagnóstico registrado') AS Diagnostico,
           CASE d.TipoDiagnostico WHEN 'P' THEN 'Presuntivo'
                                  WHEN 'D' THEN 'Definitivo'
                                  WHEN 'R' THEN 'Repetitivo'
                                  ELSE '-' END   AS TipoDiagnostico
    FROM   dbo.vw_AtencionResumen r
    LEFT JOIN dbo.AtencionDetalle d ON d.IdAtencion = r.IdAtencion
    WHERE  r.FechaAtencion >= @FechaDesde
      AND  r.FechaAtencion <  DATEADD(DAY, 1, @FechaHasta)
      AND  (@IdMedico IS NULL OR r.IdMedico = @IdMedico)
      AND  r.Estado <> 'N'
    ORDER BY r.FechaAtencion, r.NumeroAtencion, d.Item
    OPTION (RECOMPILE);
END
GO

PRINT 'Script 03 - Stored Procedures creados correctamente.';
GO
/* =============================================================================
   Script 04: Datos iniciales de prueba
   Claves generadas con PBKDF2-SHA256, 20.000 iteraciones, salt por usuario.
   Credenciales de prueba:
       admin    / Admin123$
       jperez   / Medico123$
       mtorres  / Enfer123$
   ============================================================================= */
USE HospitalAtencionesDB;
GO

SET NOCOUNT ON;

/* --------------------------------- Usuarios -------------------------------- */
INSERT INTO dbo.Usuario (NombreUsuario, ClaveHash, ClaveSalt, NombreCompleto, Rol, Activo)
VALUES
 ('admin',   '4teZLt90x3UwDOm2FqgaCJUk9oI0S7fF3yCbBlMtz1E=', 'jGl25bVBBBW96Qi9Te4V3w==', N'Administrador del Sistema', 'ADMINISTRADOR', 1),
 ('jperez',  '8+zKk/Tm5pNF7Ngu6UOHdRO1ezD8oYzA2KjRyPtHwzg=', 'grCE8yCiyXdYXsGZJtfvRQ==', N'Juan Pérez Salazar',        'MEDICO',        1),
 ('mtorres', 'IRF1rhwi/RVTUNyNrhZur9wG3dEA6z/w4Tr6+le3Lkc=', 'uVUraPbC9Nqz0ZRtCFFCTQ==', N'María Torres Aguirre',      'ASISTENCIAL',   1);
GO

/* ------------------------------ Especialidades ----------------------------- */
INSERT INTO dbo.Especialidad (Nombre) VALUES
 (N'Medicina General'), (N'Pediatría'), (N'Cardiología'),
 (N'Ginecología'), (N'Traumatología');
GO

/* ---------------------------------- Médicos -------------------------------- */
INSERT INTO dbo.Medico (NumeroColegiatura, Nombres, Apellidos, IdEspecialidad) VALUES
 ('CMP-45120', N'Juan Carlos',  N'Pérez Salazar',    1),
 ('CMP-38765', N'Lucía',        N'Ramírez Vega',     2),
 ('CMP-51203', N'Andrés',       N'Quispe Mendoza',   3),
 ('CMP-29844', N'Rosa María',   N'Ferrer Campos',    4),
 ('CMP-60117', N'Diego',        N'Alarcón Ríos',     5);
GO

/* --------------------------------- Pacientes ------------------------------- */
INSERT INTO dbo.Paciente (NumeroDocumento, Nombres, ApellidoPaterno, ApellidoMaterno, FechaNacimiento, Sexo, Telefono) VALUES
 ('45781203', N'Carmen Rosa',  N'Huamán',   N'Ccahuana', '1985-03-14', 'F', '987654321'),
 ('10293847', N'Luis Alberto', N'Sánchez',  N'Portal',   '1971-11-02', 'M', '999112233'),
 ('72639184', N'Sofía',        N'Delgado',  N'Vargas',   '2016-07-21', 'F', '955443322'),
 ('08475619', N'Manuel',       N'Castro',   N'Ibáñez',   '1948-01-30', 'M', '944556677'),
 ('61728394', N'Andrea',       N'Molina',   N'Reyes',    '1999-09-09', 'F', '966778899'),
 ('33445566', N'Pedro',        N'Vilca',    N'Quispe',   '1990-05-18', 'M', '911223344');
GO

/* ---------------------- Atenciones de ejemplo (cabecera) ------------------- */
DECLARE @IdAtencion INT, @Numero VARCHAR(15);

EXEC dbo.usp_Atencion_Insertar
     @IdPaciente = 1, @IdMedico = 1, @FechaAtencion = '2026-08-20T09:30:00',
     @MotivoConsulta = N'Dolor abdominal de 3 días de evolución',
     @Temperatura = 37.8, @PresionArterial = '120/80', @FrecuenciaCardiaca = 88,
     @Peso = 62.5, @Talla = 1.58, @Observaciones = N'Se solicita ecografía abdominal',
     @Estado = 'A', @IdUsuarioRegistro = 2,
     @IdAtencion = @IdAtencion OUTPUT, @NumeroAtencion = @Numero OUTPUT;

EXEC dbo.usp_AtencionDetalle_Insertar @IdAtencion, 1, 'K297', N'Gastritis, no especificada', 'D', N'Omeprazol 20 mg cada 24 h por 14 días';
EXEC dbo.usp_AtencionDetalle_Insertar @IdAtencion, 2, 'R101', N'Dolor abdominal localizado en parte superior', 'P', N'Control en 7 días';

EXEC dbo.usp_Atencion_Insertar
     @IdPaciente = 3, @IdMedico = 2, @FechaAtencion = '2026-08-22T11:00:00',
     @MotivoConsulta = N'Fiebre y tos seca de 2 días',
     @Temperatura = 38.5, @PresionArterial = '100/60', @FrecuenciaCardiaca = 110,
     @Peso = 28.0, @Talla = 1.24, @Observaciones = NULL,
     @Estado = 'A', @IdUsuarioRegistro = 2,
     @IdAtencion = @IdAtencion OUTPUT, @NumeroAtencion = @Numero OUTPUT;

EXEC dbo.usp_AtencionDetalle_Insertar @IdAtencion, 1, 'J069', N'Infección aguda de las vías respiratorias superiores', 'D', N'Paracetamol 250 mg condicional a fiebre';

EXEC dbo.usp_Atencion_Insertar
     @IdPaciente = 4, @IdMedico = 3, @FechaAtencion = '2026-08-25T08:15:00',
     @MotivoConsulta = N'Control de presión arterial',
     @Temperatura = 36.6, @PresionArterial = '150/95', @FrecuenciaCardiaca = 76,
     @Peso = 81.2, @Talla = 1.70, @Observaciones = N'Paciente refiere olvido de medicación',
     @Estado = 'R', @IdUsuarioRegistro = 1,
     @IdAtencion = @IdAtencion OUTPUT, @NumeroAtencion = @Numero OUTPUT;

EXEC dbo.usp_AtencionDetalle_Insertar @IdAtencion, 1, 'I10', N'Hipertensión esencial (primaria)', 'R', N'Enalapril 10 mg cada 12 h';
GO

PRINT 'Script 04 - Datos iniciales cargados correctamente.';
GO

/* Verificación rápida */
SELECT NumeroAtencion, FechaAtencion, Paciente, Medico, EstadoDescripcion, TotalDiagnosticos
FROM   dbo.vw_AtencionResumen
ORDER BY FechaAtencion;
GO
