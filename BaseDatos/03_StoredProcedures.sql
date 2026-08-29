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
