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
