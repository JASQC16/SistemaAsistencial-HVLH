/* =============================================================================
   Hospital Nacional Víctor Larco Herrera - HVLH
   Script 06: PROCEDIMIENTOS ALMACENADOS de la migración

   Crea los procedimientos de pacientes, citas, catálogo CIE-10 y reportes, y
   actualiza los de atención para enlazarlos con la cita.

   Todos los procedimientos se eliminan y recrean, por lo que el script puede
   ejecutarse tantas veces como haga falta. Ningún dato se pierde: solo se
   reemplaza código, nunca tablas.

   Requisito previo: 01, 02, 03, 04 y 05.
   ============================================================================= */

USE HospitalAtencionesDB;
GO

SET NOCOUNT ON;
GO

PRINT '=== Procedimientos HVLH: inicio ===';
GO

/* =============================================================================
   1. MAESTROS
   ============================================================================= */

IF OBJECT_ID('dbo.usp_Especialidad_Listar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Especialidad_Listar;
GO
CREATE PROCEDURE dbo.usp_Especialidad_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdEspecialidad, Nombre
    FROM   dbo.Especialidad
    WHERE  Activo = 1
    ORDER BY Nombre;
END
GO

/* =============================================================================
   2. PACIENTES

   Se sustituye usp_Paciente_Buscar por una versión que devuelve también las
   columnas nuevas, y se agrega el CRUD completo del módulo.
   ============================================================================= */

IF OBJECT_ID('dbo.usp_Paciente_Buscar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Paciente_Buscar;
GO
/* -----------------------------------------------------------------------------
   Búsqueda rápida usada por el selector de pacientes de atenciones y citas.
   Solo devuelve pacientes activos: no tiene sentido citar a alguien dado de baja.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Paciente_Buscar
    @Busqueda NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (100)
           IdPaciente, TipoDocumento, NumeroDocumento, HistoriaClinica,
           Nombres, ApellidoPaterno, ApellidoMaterno,
           FechaNacimiento, Sexo, Telefono, Direccion, Correo,
           Activo, FechaRegistro, FechaModificacion
    FROM   dbo.Paciente
    WHERE  Activo = 1
      AND (@Busqueda IS NULL
           OR NumeroDocumento  LIKE @Busqueda + '%'
           OR HistoriaClinica  LIKE @Busqueda + '%'
           OR ApellidoPaterno  LIKE @Busqueda + '%'
           OR ApellidoMaterno  LIKE @Busqueda + '%'
           OR Nombres          LIKE @Busqueda + '%')
    ORDER BY ApellidoPaterno, ApellidoMaterno, Nombres
    OPTION (RECOMPILE);
END
GO

IF OBJECT_ID('dbo.usp_Paciente_Listar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Paciente_Listar;
GO
/* -----------------------------------------------------------------------------
   Listado del módulo de pacientes, con filtros opcionales.
   @Activo NULL = todos; 1 = solo activos; 0 = solo inactivos.
   OPTION (RECOMPILE) evita que un plan cacheado para "sin filtros" se reutilice
   para una búsqueda muy selectiva, y viceversa.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Paciente_Listar
    @Busqueda      NVARCHAR(100) = NULL,
    @TipoDocumento VARCHAR(3)    = NULL,
    @Activo        BIT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.IdPaciente,
           p.TipoDocumento,
           p.NumeroDocumento,
           p.HistoriaClinica,
           p.Nombres,
           p.ApellidoPaterno,
           p.ApellidoMaterno,
           (p.ApellidoPaterno + ' ' + ISNULL(p.ApellidoMaterno, '') + ', ' + p.Nombres) AS NombreCompleto,
           p.FechaNacimiento,
           DATEDIFF(YEAR, p.FechaNacimiento, GETDATE())
               - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, p.FechaNacimiento, GETDATE()), p.FechaNacimiento) > CAST(GETDATE() AS DATE)
                      THEN 1 ELSE 0 END                       AS Edad,
           p.Sexo,
           p.Telefono,
           p.Direccion,
           p.Correo,
           p.Activo,
           p.FechaRegistro,
           p.FechaModificacion,
           (SELECT COUNT(1) FROM dbo.Atencion a WHERE a.IdPaciente = p.IdPaciente AND a.Estado <> 'N') AS TotalAtenciones,
           (SELECT COUNT(1) FROM dbo.Cita     c WHERE c.IdPaciente = p.IdPaciente)                     AS TotalCitas
    FROM   dbo.Paciente p
    WHERE (@Activo IS NULL OR p.Activo = @Activo)
      AND (@TipoDocumento IS NULL OR p.TipoDocumento = @TipoDocumento)
      AND (@Busqueda IS NULL
           OR p.NumeroDocumento LIKE '%' + @Busqueda + '%'
           OR p.HistoriaClinica LIKE '%' + @Busqueda + '%'
           OR p.ApellidoPaterno LIKE '%' + @Busqueda + '%'
           OR p.ApellidoMaterno LIKE '%' + @Busqueda + '%'
           OR p.Nombres         LIKE '%' + @Busqueda + '%')
    ORDER BY p.ApellidoPaterno, p.ApellidoMaterno, p.Nombres
    OPTION (RECOMPILE);
END
GO

IF OBJECT_ID('dbo.usp_Paciente_ObtenerPorId', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Paciente_ObtenerPorId;
GO
CREATE PROCEDURE dbo.usp_Paciente_ObtenerPorId
    @IdPaciente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdPaciente, TipoDocumento, NumeroDocumento, HistoriaClinica,
           Nombres, ApellidoPaterno, ApellidoMaterno,
           FechaNacimiento, Sexo, Telefono, Direccion, Correo,
           Activo, FechaRegistro, FechaModificacion
    FROM   dbo.Paciente
    WHERE  IdPaciente = @IdPaciente;
END
GO

IF OBJECT_ID('dbo.usp_Paciente_ExisteDocumento', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Paciente_ExisteDocumento;
GO
/* -----------------------------------------------------------------------------
   Comprobación previa de duplicados. Devuelve el paciente ya registrado con ese
   documento, si existe, para poder avisar al usuario con nombre y apellidos en
   lugar de un simple "ya existe".

   Esta validación es de conveniencia: la garantía real la da la restricción
   UQ_Paciente_TipoNumeroDoc. Entre la comprobación y el INSERT podría colarse
   otro registro concurrente, y en ese caso el motor rechaza la inserción.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Paciente_ExisteDocumento
    @TipoDocumento   VARCHAR(3),
    @NumeroDocumento VARCHAR(15),
    @IdPaciente      INT = NULL   -- se excluye a sí mismo al editar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
           IdPaciente, TipoDocumento, NumeroDocumento, HistoriaClinica,
           Nombres, ApellidoPaterno, ApellidoMaterno,
           FechaNacimiento, Sexo, Telefono, Direccion, Correo,
           Activo, FechaRegistro, FechaModificacion
    FROM   dbo.Paciente
    WHERE  TipoDocumento   = @TipoDocumento
      AND  NumeroDocumento = @NumeroDocumento
      AND (@IdPaciente IS NULL OR IdPaciente <> @IdPaciente);
END
GO

IF OBJECT_ID('dbo.usp_Paciente_Insertar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Paciente_Insertar;
GO
/* -----------------------------------------------------------------------------
   Alta de paciente. La historia clínica se genera aquí con una SEQUENCE:
   sin tabla de contadores, sin bloqueos y sin cursores.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Paciente_Insertar
    @TipoDocumento   VARCHAR(3),
    @NumeroDocumento VARCHAR(15),
    @Nombres         NVARCHAR(60),
    @ApellidoPaterno NVARCHAR(40),
    @ApellidoMaterno NVARCHAR(40) = NULL,
    @FechaNacimiento DATE,
    @Sexo            CHAR(1),
    @Telefono        VARCHAR(20)   = NULL,
    @Direccion       NVARCHAR(150) = NULL,
    @Correo          NVARCHAR(100) = NULL,
    @IdPaciente      INT         OUTPUT,
    @HistoriaClinica VARCHAR(15) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Paciente
               WHERE TipoDocumento = @TipoDocumento AND NumeroDocumento = @NumeroDocumento)
    BEGIN
        THROW 50010, 'Ya existe un paciente registrado con ese tipo y número de documento.', 1;
    END

    IF @FechaNacimiento > CAST(GETDATE() AS DATE)
        THROW 50011, 'La fecha de nacimiento no puede ser posterior a la fecha actual.', 1;

    SET @HistoriaClinica = 'HC-' +
        RIGHT('000000' + CAST(NEXT VALUE FOR dbo.SeqHistoriaClinica AS VARCHAR(10)), 6);

    INSERT INTO dbo.Paciente
        (TipoDocumento, NumeroDocumento, HistoriaClinica, Nombres, ApellidoPaterno,
         ApellidoMaterno, FechaNacimiento, Sexo, Telefono, Direccion, Correo,
         Activo, FechaRegistro)
    VALUES
        (@TipoDocumento, @NumeroDocumento, @HistoriaClinica, @Nombres, @ApellidoPaterno,
         @ApellidoMaterno, @FechaNacimiento, @Sexo, @Telefono, @Direccion, @Correo,
         1, SYSDATETIME());

    SET @IdPaciente = CAST(SCOPE_IDENTITY() AS INT);
END
GO

IF OBJECT_ID('dbo.usp_Paciente_Actualizar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Paciente_Actualizar;
GO
CREATE PROCEDURE dbo.usp_Paciente_Actualizar
    @IdPaciente      INT,
    @TipoDocumento   VARCHAR(3),
    @NumeroDocumento VARCHAR(15),
    @Nombres         NVARCHAR(60),
    @ApellidoPaterno NVARCHAR(40),
    @ApellidoMaterno NVARCHAR(40) = NULL,
    @FechaNacimiento DATE,
    @Sexo            CHAR(1),
    @Telefono        VARCHAR(20)   = NULL,
    @Direccion       NVARCHAR(150) = NULL,
    @Correo          NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Paciente WHERE IdPaciente = @IdPaciente)
        THROW 50012, 'El paciente indicado no existe.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Paciente
               WHERE TipoDocumento = @TipoDocumento
                 AND NumeroDocumento = @NumeroDocumento
                 AND IdPaciente <> @IdPaciente)
    BEGIN
        THROW 50010, 'Ya existe otro paciente registrado con ese tipo y número de documento.', 1;
    END

    IF @FechaNacimiento > CAST(GETDATE() AS DATE)
        THROW 50011, 'La fecha de nacimiento no puede ser posterior a la fecha actual.', 1;

    UPDATE dbo.Paciente
    SET    TipoDocumento     = @TipoDocumento,
           NumeroDocumento   = @NumeroDocumento,
           Nombres           = @Nombres,
           ApellidoPaterno   = @ApellidoPaterno,
           ApellidoMaterno   = @ApellidoMaterno,
           FechaNacimiento   = @FechaNacimiento,
           Sexo              = @Sexo,
           Telefono          = @Telefono,
           Direccion         = @Direccion,
           Correo            = @Correo,
           FechaModificacion = SYSDATETIME()
    WHERE  IdPaciente = @IdPaciente;
END
GO

IF OBJECT_ID('dbo.usp_Paciente_CambiarEstado', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Paciente_CambiarEstado;
GO
/* -----------------------------------------------------------------------------
   Activación / desactivación. Nunca se elimina un paciente: su historia clínica
   y sus atenciones deben conservarse. Un paciente con citas pendientes no puede
   desactivarse, porque quedaría una agenda apuntando a alguien inhabilitado.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Paciente_CambiarEstado
    @IdPaciente INT,
    @Activo     BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Paciente WHERE IdPaciente = @IdPaciente)
        THROW 50012, 'El paciente indicado no existe.', 1;

    IF @Activo = 0 AND EXISTS (SELECT 1 FROM dbo.Cita
                               WHERE IdPaciente = @IdPaciente AND Estado = 'CITADO')
    BEGIN
        THROW 50013, 'No se puede desactivar el paciente: tiene citas pendientes de atención.', 1;
    END

    UPDATE dbo.Paciente
    SET    Activo            = @Activo,
           FechaModificacion = SYSDATETIME()
    WHERE  IdPaciente = @IdPaciente;
END
GO

PRINT '  [1/5] Procedimientos de pacientes creados.';
GO

/* =============================================================================
   3. CATÁLOGO CIE-10 (consulta local, sin Internet)
   ============================================================================= */

IF OBJECT_ID('dbo.usp_Cie10_Buscar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cie10_Buscar;
GO
/* -----------------------------------------------------------------------------
   Búsqueda unificada por código o por texto, tal como la necesita el usuario:
   escribir "F20" devuelve la familia F20.x, y escribir "esquizofrenia" devuelve
   las coincidencias por descripción aunque no se conozca el código.

   El orden de los resultados prioriza las coincidencias exactas y por prefijo,
   porque son las que el usuario espera ver primero.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Cie10_Buscar
    @Termino        NVARCHAR(100),
    @SoloVigentes   BIT = 1,
    @Maximo         INT = 50
AS
BEGIN
    SET NOCOUNT ON;

    IF @Termino IS NULL OR LEN(LTRIM(RTRIM(@Termino))) = 0 RETURN;

    DECLARE @Buscado VARCHAR(20)   = REPLACE(LTRIM(RTRIM(@Termino)), '.', '');
    DECLARE @Texto   NVARCHAR(100) = LTRIM(RTRIM(@Termino));

    SELECT TOP (@Maximo)
           c.CodigoCie10,
           c.CodigoFormato,
           c.Descripcion,
           c.Categoria,
           c.Grupo,
           c.Capitulo,
           c.CapituloNombre,
           c.Sexo,
           c.Estado,
           c.VersionCatalogo
    FROM   dbo.CatalogoCie10 c
    WHERE (@SoloVigentes = 0 OR c.Estado = 'V')
      AND (c.CodigoCie10 LIKE @Buscado + '%'
           OR c.Descripcion LIKE '%' + @Texto + '%')
    ORDER BY
           CASE WHEN c.CodigoCie10 = @Buscado                THEN 0
                WHEN c.CodigoCie10 LIKE @Buscado + '%'       THEN 1
                WHEN c.Descripcion LIKE @Texto + '%'         THEN 2
                ELSE 3 END,
           c.CodigoCie10
    OPTION (RECOMPILE);
END
GO

IF OBJECT_ID('dbo.usp_Cie10_ObtenerPorCodigo', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cie10_ObtenerPorCodigo;
GO
CREATE PROCEDURE dbo.usp_Cie10_ObtenerPorCodigo
    @CodigoCie10 VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CodigoCie10, CodigoFormato, Descripcion, Categoria, Grupo,
           Capitulo, CapituloNombre, Sexo, Estado, VersionCatalogo
    FROM   dbo.CatalogoCie10
    WHERE  CodigoCie10 = REPLACE(@CodigoCie10, '.', '');
END
GO

IF OBJECT_ID('dbo.usp_Cie10_ObtenerVersion', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cie10_ObtenerVersion;
GO
CREATE PROCEDURE dbo.usp_Cie10_ObtenerVersion
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1) VersionCatalogo, Fuente, FechaCarga, Insertados, Actualizados, Cesados
    FROM   dbo.CatalogoCie10_Carga
    ORDER BY IdCarga DESC;
END
GO

IF OBJECT_ID('dbo.usp_Cie10_ImportarDesdeStaging', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cie10_ImportarDesdeStaging;
GO
/* -----------------------------------------------------------------------------
   Consolida en dbo.CatalogoCie10 lo que se haya volcado en la tabla de staging
   desde el archivo oficial del MINSA.

   Puntos importantes de esta importación:

   * Es un MERGE en una sola sentencia: sin cursores y sin recorrer fila a fila.
   * Los códigos que ya no vienen en el archivo NO se borran: se marcan como
     cesados (Estado = 'C'). Borrarlos rompería los diagnósticos históricos, y
     el MINSA publica precisamente listas de cese de uso (RM 447-2024).
   * Las atenciones ya registradas guardan su propia copia del código y de la
     descripción en dbo.AtencionDetalle, de modo que una actualización del
     catálogo nunca reescribe una historia clínica cerrada.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Cie10_ImportarDesdeStaging
    @VersionCatalogo VARCHAR(20),
    @Fuente          VARCHAR(200),
    @Usuario         NVARCHAR(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoCie10_Staging)
        THROW 50020, 'La tabla de staging está vacía: cargue primero el archivo del MINSA.', 1;

    DECLARE @Resumen TABLE (Accion NVARCHAR(10));
    DECLARE @Insertados INT, @Actualizados INT, @Cesados INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        /* Normalización: el archivo oficial trae el código sin punto y en
           mayúsculas, pero conviene no confiar en ello. */
        ;WITH Origen AS
        (
            SELECT UPPER(REPLACE(LTRIM(RTRIM(CodigoCie10)), '.', ''))       AS CodigoCie10,
                   LTRIM(RTRIM(Descripcion))                                AS Descripcion,
                   NULLIF(UPPER(LTRIM(RTRIM(ISNULL(Sexo, '')))), '')        AS Sexo,
                   TRY_CAST(EdadMinima AS INT)                              AS EdadMinima,
                   TRY_CAST(EdadMaxima AS INT)                              AS EdadMaxima,
                   CASE WHEN UPPER(LTRIM(RTRIM(ISNULL(Estado, 'V')))) LIKE 'C%'
                        THEN 'C' ELSE 'V' END                               AS Estado,
                   ROW_NUMBER() OVER (
                       PARTITION BY UPPER(REPLACE(LTRIM(RTRIM(CodigoCie10)), '.', ''))
                       ORDER BY LEN(LTRIM(RTRIM(Descripcion))) DESC)        AS Fila
            FROM   dbo.CatalogoCie10_Staging
            WHERE  CodigoCie10 IS NOT NULL
              AND  LTRIM(RTRIM(CodigoCie10)) <> ''
              AND  Descripcion IS NOT NULL
        )
        MERGE dbo.CatalogoCie10 AS destino
        USING (SELECT * FROM Origen WHERE Fila = 1) AS origen
              ON destino.CodigoCie10 = origen.CodigoCie10
        WHEN MATCHED THEN
            UPDATE SET destino.Descripcion     = origen.Descripcion,
                       destino.CodigoFormato   = CASE WHEN LEN(origen.CodigoCie10) > 3
                                                      THEN LEFT(origen.CodigoCie10, 3) + '.' + SUBSTRING(origen.CodigoCie10, 4, 7)
                                                      ELSE origen.CodigoCie10 END,
                       destino.Categoria       = LEFT(origen.CodigoCie10, 3),
                       destino.Sexo            = CASE WHEN origen.Sexo IN ('M','F') THEN origen.Sexo ELSE NULL END,
                       destino.EdadMinima      = origen.EdadMinima,
                       destino.EdadMaxima      = origen.EdadMaxima,
                       destino.Estado          = origen.Estado,
                       destino.FechaCese       = CASE WHEN origen.Estado = 'C' AND destino.FechaCese IS NULL
                                                      THEN CAST(SYSDATETIME() AS DATE) ELSE destino.FechaCese END,
                       destino.Fuente          = @Fuente,
                       destino.VersionCatalogo = @VersionCatalogo,
                       destino.FechaCarga      = SYSDATETIME()
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (CodigoCie10, CodigoFormato, Descripcion, Categoria, Sexo,
                    EdadMinima, EdadMaxima, Estado, Fuente, VersionCatalogo, FechaCarga)
            VALUES (origen.CodigoCie10,
                    CASE WHEN LEN(origen.CodigoCie10) > 3
                         THEN LEFT(origen.CodigoCie10, 3) + '.' + SUBSTRING(origen.CodigoCie10, 4, 7)
                         ELSE origen.CodigoCie10 END,
                    origen.Descripcion,
                    LEFT(origen.CodigoCie10, 3),
                    CASE WHEN origen.Sexo IN ('M','F') THEN origen.Sexo ELSE NULL END,
                    origen.EdadMinima, origen.EdadMaxima, origen.Estado,
                    @Fuente, @VersionCatalogo, SYSDATETIME())
        OUTPUT $action INTO @Resumen;

        /* Códigos que estaban vigentes y ya no vienen en el archivo:
           se cesan, nunca se eliminan. */
        UPDATE c
        SET    c.Estado    = 'C',
               c.FechaCese = CAST(SYSDATETIME() AS DATE)
        FROM   dbo.CatalogoCie10 c
        WHERE  c.Estado = 'V'
          AND  NOT EXISTS (SELECT 1 FROM dbo.CatalogoCie10_Staging s
                           WHERE UPPER(REPLACE(LTRIM(RTRIM(s.CodigoCie10)), '.', '')) = c.CodigoCie10);

        SET @Cesados = @@ROWCOUNT;

        SELECT @Insertados   = SUM(CASE WHEN Accion = 'INSERT' THEN 1 ELSE 0 END),
               @Actualizados = SUM(CASE WHEN Accion = 'UPDATE' THEN 1 ELSE 0 END)
        FROM   @Resumen;

        INSERT INTO dbo.CatalogoCie10_Carga (VersionCatalogo, Fuente, Insertados, Actualizados, Cesados, Usuario)
        VALUES (@VersionCatalogo, @Fuente, ISNULL(@Insertados, 0), ISNULL(@Actualizados, 0), ISNULL(@Cesados, 0), @Usuario);

        /* El staging queda limpio para la próxima importación. */
        TRUNCATE TABLE dbo.CatalogoCie10_Staging;

        COMMIT TRANSACTION;

        SELECT ISNULL(@Insertados, 0)   AS Insertados,
               ISNULL(@Actualizados, 0) AS Actualizados,
               ISNULL(@Cesados, 0)      AS Cesados;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

PRINT '  [2/5] Procedimientos del catálogo CIE-10 creados.';
GO

/* =============================================================================
   4. CITAS
   ============================================================================= */

IF OBJECT_ID('dbo.usp_Cita_Listar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cita_Listar;
GO
CREATE PROCEDURE dbo.usp_Cita_Listar
    @FechaDesde     DATE          = NULL,
    @FechaHasta     DATE          = NULL,
    @Busqueda       NVARCHAR(100) = NULL,
    @IdMedico       INT           = NULL,
    @IdEspecialidad INT           = NULL,
    @Estado         VARCHAR(12)   = NULL
AS
BEGIN
    SET NOCOUNT ON;

    /* Comparación SARGable: no se aplica ninguna función sobre FechaCita,
       de modo que el índice IX_Cita_Fecha_Estado sigue siendo utilizable. */
    SELECT *
    FROM   dbo.vw_CitaResumen
    WHERE (@FechaDesde IS NULL OR FechaCita >= @FechaDesde)
      AND (@FechaHasta IS NULL OR FechaCita <  DATEADD(DAY, 1, @FechaHasta))
      AND (@IdMedico       IS NULL OR IdMedico       = @IdMedico)
      AND (@IdEspecialidad IS NULL OR IdEspecialidad = @IdEspecialidad)
      AND (@Estado         IS NULL OR Estado         = @Estado)
      AND (@Busqueda IS NULL
           OR NumeroCita        LIKE '%' + @Busqueda + '%'
           OR DocumentoPaciente LIKE '%' + @Busqueda + '%'
           OR HistoriaClinica   LIKE '%' + @Busqueda + '%'
           OR Paciente          LIKE '%' + @Busqueda + '%')
    ORDER BY FechaCita DESC
    OPTION (RECOMPILE);
END
GO

IF OBJECT_ID('dbo.usp_Cita_ObtenerPorId', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cita_ObtenerPorId;
GO
CREATE PROCEDURE dbo.usp_Cita_ObtenerPorId
    @IdCita INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM dbo.vw_CitaResumen WHERE IdCita = @IdCita;
END
GO

IF OBJECT_ID('dbo.usp_Cita_ListarPendientesPorPaciente', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cita_ListarPendientesPorPaciente;
GO
/* -----------------------------------------------------------------------------
   Citas de un paciente que aún pueden convertirse en atención: las que están en
   estado CITADO y todavía no tienen atención vigente asociada. La usa el
   formulario de atenciones para enlazar la atención con su cita de origen.
   @IdCitaActual permite que, al editar una atención ya guardada, su propia cita
   siga apareciendo en la lista.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Cita_ListarPendientesPorPaciente
    @IdPaciente   INT,
    @IdCitaActual INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.IdCita, c.NumeroCita, c.FechaCita, c.MotivoCita, c.Estado,
           (m.Apellidos + ', ' + m.Nombres) AS Medico,
           e.Nombre                         AS Especialidad
    FROM   dbo.Cita c
    INNER JOIN dbo.Medico       m ON m.IdMedico       = c.IdMedico
    INNER JOIN dbo.Especialidad e ON e.IdEspecialidad = m.IdEspecialidad
    WHERE  c.IdPaciente = @IdPaciente
      AND (c.IdCita = @IdCitaActual
           OR (c.Estado = 'CITADO'
               AND NOT EXISTS (SELECT 1 FROM dbo.Atencion a
                               WHERE a.IdCita = c.IdCita AND a.Estado <> 'N')))
    ORDER BY c.FechaCita DESC;
END
GO

IF OBJECT_ID('dbo.usp_Cita_Insertar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cita_Insertar;
GO
CREATE PROCEDURE dbo.usp_Cita_Insertar
    @IdPaciente        INT,
    @IdMedico          INT,
    @FechaCita         DATETIME2(0),
    @MotivoCita        NVARCHAR(300) = NULL,
    @Observaciones     NVARCHAR(500) = NULL,
    @IdUsuarioRegistro INT,
    @IdCita            INT         OUTPUT,
    @NumeroCita        VARCHAR(15) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Paciente WHERE IdPaciente = @IdPaciente AND Activo = 1)
        THROW 50030, 'El paciente indicado no existe o está inactivo.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Medico WHERE IdMedico = @IdMedico AND Activo = 1)
        THROW 50031, 'El profesional indicado no existe o está inactivo.', 1;

    /* Una cita ya cerrada no bloquea el horario, pero una vigente sí. */
    IF EXISTS (SELECT 1 FROM dbo.Cita
               WHERE IdMedico = @IdMedico AND FechaCita = @FechaCita AND Estado <> 'CANCELADO')
        THROW 50032, 'El profesional ya tiene una cita registrada en esa fecha y hora.', 1;

    SET @NumeroCita = 'CI-' + CAST(YEAR(@FechaCita) AS VARCHAR(4)) + '-' +
        RIGHT('000000' + CAST(NEXT VALUE FOR dbo.SeqNumeroCita AS VARCHAR(10)), 6);

    INSERT INTO dbo.Cita
        (NumeroCita, IdPaciente, IdMedico, FechaCita, MotivoCita,
         Estado, Observaciones, IdUsuarioRegistro, FechaRegistro)
    VALUES
        (@NumeroCita, @IdPaciente, @IdMedico, @FechaCita, @MotivoCita,
         'CITADO', @Observaciones, @IdUsuarioRegistro, SYSDATETIME());

    SET @IdCita = CAST(SCOPE_IDENTITY() AS INT);
END
GO

IF OBJECT_ID('dbo.usp_Cita_Actualizar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cita_Actualizar;
GO
/* -----------------------------------------------------------------------------
   Reprogramación. Solo se admite sobre citas en estado CITADO: una cita ya
   atendida o cerrada no debe poder moverse de fecha, porque su información ya
   forma parte de la producción asistencial reportada.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Cita_Actualizar
    @IdCita        INT,
    @IdPaciente    INT,
    @IdMedico      INT,
    @FechaCita     DATETIME2(0),
    @MotivoCita    NVARCHAR(300) = NULL,
    @Observaciones NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoActual VARCHAR(12) = (SELECT Estado FROM dbo.Cita WHERE IdCita = @IdCita);

    IF @EstadoActual IS NULL
        THROW 50033, 'La cita indicada no existe.', 1;

    IF @EstadoActual <> 'CITADO'
        THROW 50034, 'Solo puede modificarse una cita que siga en estado CITADO.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Cita
               WHERE IdMedico = @IdMedico AND FechaCita = @FechaCita
                 AND Estado <> 'CANCELADO' AND IdCita <> @IdCita)
        THROW 50032, 'El profesional ya tiene una cita registrada en esa fecha y hora.', 1;

    UPDATE dbo.Cita
    SET    IdPaciente        = @IdPaciente,
           IdMedico          = @IdMedico,
           FechaCita         = @FechaCita,
           MotivoCita        = @MotivoCita,
           Observaciones     = @Observaciones,
           FechaModificacion = SYSDATETIME()
    WHERE  IdCita = @IdCita;
END
GO

IF OBJECT_ID('dbo.usp_Cita_CambiarEstado', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cita_CambiarEstado;
GO
/* -----------------------------------------------------------------------------
   Registro explícito del desenlace de la cita.

   Regla central del módulo: el estado NO_ACUDIO es un dato que alguien registra,
   no una deducción del sistema. Una cita sin atención no se convierte sola en
   inasistencia; puede simplemente no haber llegado todavía su turno.

   ATENDIDO no se asigna por esta vía: lo fija el registro de la atención, para
   que no exista forma de marcar como atendida una cita sin acto clínico detrás.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Cita_CambiarEstado
    @IdCita       INT,
    @Estado       VARCHAR(12),
    @MotivoEstado NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoActual VARCHAR(12) = (SELECT Estado FROM dbo.Cita WHERE IdCita = @IdCita);

    IF @EstadoActual IS NULL
        THROW 50033, 'La cita indicada no existe.', 1;

    IF @Estado NOT IN ('CITADO','NO_ATENDIDO','NO_ACUDIO','CANCELADO')
        THROW 50035, 'Estado no válido. El estado ATENDIDO lo asigna el registro de la atención.', 1;

    IF @EstadoActual = 'ATENDIDO'
        THROW 50036, 'La cita ya fue atendida: anule primero la atención asociada.', 1;

    IF @Estado IN ('NO_ATENDIDO','NO_ACUDIO','CANCELADO')
       AND (@MotivoEstado IS NULL OR LEN(LTRIM(RTRIM(@MotivoEstado))) < 5)
        THROW 50037, 'Debe indicar el motivo con al menos 5 caracteres.', 1;

    UPDATE dbo.Cita
    SET    Estado            = @Estado,
           MotivoEstado      = CASE WHEN @Estado = 'CITADO' THEN NULL ELSE @MotivoEstado END,
           FechaModificacion = SYSDATETIME()
    WHERE  IdCita = @IdCita;
END
GO

IF OBJECT_ID('dbo.usp_Cita_MarcarAtendida', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cita_MarcarAtendida;
GO
/* -----------------------------------------------------------------------------
   Lo invoca el repositorio de atenciones dentro de la misma transacción que
   registra la atención: o se guardan la atención y el cambio de estado de la
   cita, o no se guarda nada. Por eso no abre transacción propia.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Cita_MarcarAtendida
    @IdCita INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Cita
    SET    Estado            = 'ATENDIDO',
           MotivoEstado      = NULL,
           FechaModificacion = SYSDATETIME()
    WHERE  IdCita = @IdCita
      AND  Estado <> 'ATENDIDO';
END
GO

IF OBJECT_ID('dbo.usp_Cita_Liberar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Cita_Liberar;
GO
/* Al anular o eliminar la atención, la cita vuelve a quedar pendiente. */
CREATE PROCEDURE dbo.usp_Cita_Liberar
    @IdCita INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Cita
    SET    Estado            = 'CITADO',
           FechaModificacion = SYSDATETIME()
    WHERE  IdCita = @IdCita
      AND  Estado = 'ATENDIDO';
END
GO

PRINT '  [3/5] Procedimientos de citas creados.';
GO

/* =============================================================================
   5. ATENCIONES: actualización para trabajar con la cita y con la versión
      del catálogo CIE-10
   ============================================================================= */

IF OBJECT_ID('dbo.usp_Atencion_Listar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_Listar;
GO
/* -----------------------------------------------------------------------------
   Listado de atenciones para la grilla de consulta.

   Se recrea aquí porque la vista vw_AtencionResumen ahora expone el tipo de
   documento, el número de historia clínica y la cita de origen: la aplicación
   lee esas tres columnas, de modo que el procedimiento debe devolverlas.

   La búsqueda por texto incluye también la historia clínica y el número de
   cita, que es como el personal identifica a un paciente en ventanilla.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Atencion_Listar
    @FechaDesde DATE          = NULL,
    @FechaHasta DATE          = NULL,
    @Busqueda   NVARCHAR(100) = NULL,   -- N.° de atención o de cita, documento, historia o apellido
    @IdMedico   INT           = NULL,
    @Estado     CHAR(1)       = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdAtencion, NumeroAtencion, FechaAtencion, NumeroCita,
           TipoDocumento, DocumentoPaciente, HistoriaClinica, Paciente,
           EdadPaciente, Medico, Especialidad, MotivoConsulta, Estado,
           EstadoDescripcion, TotalDiagnosticos, UsuarioRegistro
    FROM   dbo.vw_AtencionResumen
    WHERE  (@FechaDesde IS NULL OR FechaAtencion >= @FechaDesde)
      AND  (@FechaHasta IS NULL OR FechaAtencion <  DATEADD(DAY, 1, @FechaHasta))
      AND  (@IdMedico   IS NULL OR IdMedico = @IdMedico)
      AND  (@Estado     IS NULL OR Estado   = @Estado)
      AND  (@Busqueda   IS NULL
            OR NumeroAtencion    LIKE @Busqueda + '%'
            OR NumeroCita        LIKE @Busqueda + '%'
            OR DocumentoPaciente LIKE @Busqueda + '%'
            OR HistoriaClinica   LIKE @Busqueda + '%'
            OR Paciente          LIKE '%' + @Busqueda + '%')
    ORDER BY FechaAtencion DESC, IdAtencion DESC
    OPTION (RECOMPILE);
END
GO

IF OBJECT_ID('dbo.usp_Atencion_Insertar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_Insertar;
GO
/* -----------------------------------------------------------------------------
   Este procedimiento NO abre transacción: la transacción la controla el
   repositorio, porque el alta completa de una atención abarca la cabecera, N
   líneas de detalle y el cambio de estado de la cita. Si abriera y revirtiera
   una transacción propia, destruiría la del cliente y dejaría @@TRANCOUNT
   inconsistente. Los errores se propagan con THROW.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Atencion_Insertar
    @IdPaciente         INT,
    @IdMedico           INT,
    @IdCita             INT           = NULL,
    @FechaAtencion      DATETIME2(0),
    @MotivoConsulta     NVARCHAR(300),
    @Temperatura        DECIMAL(4,1)  = NULL,
    @PresionArterial    VARCHAR(10)   = NULL,
    @FrecuenciaCardiaca INT           = NULL,
    @Peso               DECIMAL(5,2)  = NULL,
    @Talla              DECIMAL(4,2)  = NULL,
    @Observaciones      NVARCHAR(500) = NULL,
    @Estado             CHAR(1),
    @IdUsuarioRegistro  INT,
    @IdAtencion         INT         OUTPUT,
    @NumeroAtencion     VARCHAR(15) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Paciente WHERE IdPaciente = @IdPaciente AND Activo = 1)
        THROW 50001, 'El paciente indicado no existe o está inactivo.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Medico WHERE IdMedico = @IdMedico AND Activo = 1)
        THROW 50002, 'El profesional indicado no existe o está inactivo.', 1;

    IF @IdCita IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.Cita WHERE IdCita = @IdCita AND IdPaciente = @IdPaciente)
            THROW 50038, 'La cita seleccionada no corresponde al paciente de la atención.', 1;

        IF EXISTS (SELECT 1 FROM dbo.Atencion WHERE IdCita = @IdCita AND Estado <> 'N')
            THROW 50039, 'Esa cita ya tiene una atención registrada.', 1;
    END

    SET @NumeroAtencion = 'AT-' + CAST(YEAR(@FechaAtencion) AS VARCHAR(4)) + '-' +
        RIGHT('000000' + CAST(NEXT VALUE FOR dbo.SeqNumeroAtencion AS VARCHAR(10)), 6);

    INSERT INTO dbo.Atencion
        (NumeroAtencion, IdPaciente, IdMedico, IdCita, FechaAtencion, MotivoConsulta,
         Temperatura, PresionArterial, FrecuenciaCardiaca, Peso, Talla,
         Observaciones, Estado, IdUsuarioRegistro, FechaRegistro)
    VALUES
        (@NumeroAtencion, @IdPaciente, @IdMedico, @IdCita, @FechaAtencion, @MotivoConsulta,
         @Temperatura, @PresionArterial, @FrecuenciaCardiaca, @Peso, @Talla,
         @Observaciones, @Estado, @IdUsuarioRegistro, SYSDATETIME());

    SET @IdAtencion = CAST(SCOPE_IDENTITY() AS INT);
END
GO

IF OBJECT_ID('dbo.usp_Atencion_Actualizar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_Actualizar;
GO
CREATE PROCEDURE dbo.usp_Atencion_Actualizar
    @IdAtencion         INT,
    @IdPaciente         INT,
    @IdMedico           INT,
    @IdCita             INT           = NULL,
    @FechaAtencion      DATETIME2(0),
    @MotivoConsulta     NVARCHAR(300),
    @Temperatura        DECIMAL(4,1)  = NULL,
    @PresionArterial    VARCHAR(10)   = NULL,
    @FrecuenciaCardiaca INT           = NULL,
    @Peso               DECIMAL(5,2)  = NULL,
    @Talla              DECIMAL(4,2)  = NULL,
    @Observaciones      NVARCHAR(500) = NULL,
    @Estado             CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoActual CHAR(1) = (SELECT Estado FROM dbo.Atencion WHERE IdAtencion = @IdAtencion);

    IF @EstadoActual IS NULL
        THROW 50003, 'La atención indicada no existe.', 1;

    IF @EstadoActual = 'N'
        THROW 50004, 'No es posible modificar una atención anulada.', 1;

    IF @IdCita IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Atencion
                                       WHERE IdCita = @IdCita AND Estado <> 'N' AND IdAtencion <> @IdAtencion)
        THROW 50039, 'Esa cita ya tiene una atención registrada.', 1;

    UPDATE dbo.Atencion
    SET    IdPaciente         = @IdPaciente,
           IdMedico           = @IdMedico,
           IdCita             = @IdCita,
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
/* -----------------------------------------------------------------------------
   El detalle guarda su propia copia del código y de la descripción del
   diagnóstico, más la versión del catálogo vigente en ese momento. Es
   deliberado: la historia clínica debe poder leerse dentro de diez años tal
   como se escribió, aunque el MINSA haya cesado ese código entre medias.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_AtencionDetalle_Insertar
    @IdAtencion             INT,
    @Item                   INT,
    @CodigoCie10            VARCHAR(10),
    @DescripcionDiagnostico NVARCHAR(250),
    @TipoDiagnostico        CHAR(1),
    @Indicaciones           NVARCHAR(300) = NULL,
    @VersionCatalogoCie10   VARCHAR(20)   = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Atencion WHERE IdAtencion = @IdAtencion)
        THROW 50005, 'No existe la atención sobre la que se intenta registrar el diagnóstico.', 1;

    INSERT INTO dbo.AtencionDetalle
        (IdAtencion, Item, CodigoCie10, DescripcionDiagnostico,
         TipoDiagnostico, Indicaciones, VersionCatalogoCie10)
    VALUES
        (@IdAtencion, @Item, @CodigoCie10, @DescripcionDiagnostico,
         @TipoDiagnostico, @Indicaciones, @VersionCatalogoCie10);
END
GO

IF OBJECT_ID('dbo.usp_Atencion_ObtenerPorId', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_ObtenerPorId;
GO
/* Devuelve dos conjuntos de resultados: cabecera y detalle, en una sola ida
   al servidor en lugar de dos llamadas separadas. */
CREATE PROCEDURE dbo.usp_Atencion_ObtenerPorId
    @IdAtencion INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT a.IdAtencion, a.NumeroAtencion, a.IdPaciente, a.IdMedico, a.IdCita,
           c.NumeroCita, c.FechaCita,
           a.FechaAtencion, a.MotivoConsulta, a.Temperatura, a.PresionArterial,
           a.FrecuenciaCardiaca, a.Peso, a.Talla, a.Observaciones, a.Estado,
           a.IdUsuarioRegistro, a.FechaRegistro, a.FechaModificacion,
           p.TipoDocumento, p.NumeroDocumento AS DocumentoPaciente, p.HistoriaClinica,
           (p.ApellidoPaterno + ' ' + ISNULL(p.ApellidoMaterno, '') + ', ' + p.Nombres) AS NombrePaciente
    FROM   dbo.Atencion a
    INNER JOIN dbo.Paciente p ON p.IdPaciente = a.IdPaciente
    LEFT  JOIN dbo.Cita     c ON c.IdCita     = a.IdCita
    WHERE  a.IdAtencion = @IdAtencion;

    SELECT IdAtencionDetalle, IdAtencion, Item, CodigoCie10, DescripcionDiagnostico,
           TipoDiagnostico, Indicaciones, VersionCatalogoCie10
    FROM   dbo.AtencionDetalle
    WHERE  IdAtencion = @IdAtencion
    ORDER BY Item;
END
GO

IF OBJECT_ID('dbo.usp_Atencion_Anular', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_Anular;
GO
/* -----------------------------------------------------------------------------
   Baja lógica de la atención. Además de marcarla como anulada, devuelve la cita
   de origen al estado CITADO: si el acto clínico se deshace, la agenda no puede
   seguir afirmando que ese paciente fue atendido.

   Es una operación autocontenida (dos tablas, un solo llamado), así que aquí sí
   corresponde que la transacción viva dentro del procedimiento.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Atencion_Anular
    @IdAtencion INT,
    @Motivo     NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

            DECLARE @EstadoActual CHAR(1), @IdCita INT;

            SELECT @EstadoActual = Estado, @IdCita = IdCita
            FROM   dbo.Atencion
            WHERE  IdAtencion = @IdAtencion;

            IF @EstadoActual IS NULL
                THROW 50005, 'La atención indicada no existe.', 1;

            IF @EstadoActual = 'N'
                THROW 50006, 'La atención ya se encuentra anulada.', 1;

            UPDATE dbo.Atencion
            SET    Estado            = 'N',
                   Observaciones     = LEFT(ISNULL(Observaciones + ' | ', '') + 'ANULADA: ' + @Motivo, 500),
                   FechaModificacion = SYSDATETIME()
            WHERE  IdAtencion = @IdAtencion;

            IF @IdCita IS NOT NULL
                EXEC dbo.usp_Cita_Liberar @IdCita = @IdCita;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

IF OBJECT_ID('dbo.usp_Atencion_Eliminar', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Atencion_Eliminar;
GO
/* -----------------------------------------------------------------------------
   Eliminación física, reservada al rol administrador. También libera la cita.
   ----------------------------------------------------------------------------- */
CREATE PROCEDURE dbo.usp_Atencion_Eliminar
    @IdAtencion INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

            DECLARE @IdCitaEliminar INT =
                (SELECT IdCita FROM dbo.Atencion WHERE IdAtencion = @IdAtencion);

            IF NOT EXISTS (SELECT 1 FROM dbo.Atencion WHERE IdAtencion = @IdAtencion)
                THROW 50007, 'La atención indicada no existe.', 1;

            DELETE FROM dbo.AtencionDetalle WHERE IdAtencion = @IdAtencion;
            DELETE FROM dbo.Atencion        WHERE IdAtencion = @IdAtencion;

            IF @IdCitaEliminar IS NOT NULL
                EXEC dbo.usp_Cita_Liberar @IdCita = @IdCitaEliminar;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

PRINT '  [4/5] Procedimientos de atención actualizados.';
GO

/* =============================================================================
   6. REPORTES

   Un único procedimiento alimenta el RDLC. Devuelve una fila por encuentro
   asistencial, entendiendo por tal:

     a) toda cita del periodo, con su atención si la tuvo;
     b) toda atención del periodo por demanda espontánea, es decir sin cita.

   Esta unión es lo que permite que el filtro "No acudió" muestre exactamente a
   quienes tenían cita y no se presentaron, y no a cualquiera que simplemente no
   tenga una atención registrada.
   ============================================================================= */

IF OBJECT_ID('dbo.usp_Reporte_General', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Reporte_General;
GO
CREATE PROCEDURE dbo.usp_Reporte_General
    @FechaDesde     DATE,
    @FechaHasta     DATE,
    @Estado         VARCHAR(12)   = NULL,   -- NULL = todos
    @IdPaciente     INT           = NULL,
    @Documento      VARCHAR(15)   = NULL,
    @IdMedico       INT           = NULL,
    @IdEspecialidad INT           = NULL,
    @CodigoCie10    VARCHAR(10)   = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @FechaDesde > @FechaHasta
        THROW 50040, 'La fecha inicial no puede ser posterior a la fecha final.', 1;

    DECLARE @Limite DATETIME2(0) = DATEADD(DAY, 1, CAST(@FechaHasta AS DATETIME2(0)));
    DECLARE @Codigo VARCHAR(10) = NULLIF(REPLACE(ISNULL(@CodigoCie10, ''), '.', ''), '');

    ;WITH Encuentros AS
    (
        /* a) Citas del periodo */
        SELECT  'CITA'                AS Origen,
                c.IdCita,
                c.NumeroCita,
                c.FechaCita,
                c.IdPaciente,
                c.IdMedico,
                c.IdEspecialidad,
                c.Estado              AS EstadoCita,
                c.EstadoDescripcion,
                c.MotivoEstado,
                c.MotivoCita          AS Motivo,
                c.IdAtencion,
                c.NumeroAtencion,
                c.FechaAtencion,
                c.FechaCita           AS FechaReferencia
        FROM    dbo.vw_CitaResumen c
        WHERE   c.FechaCita >= @FechaDesde
          AND   c.FechaCita <  @Limite

        UNION ALL

        /* b) Atenciones del periodo sin cita previa (demanda espontánea).
              Se reportan siempre como ATENDIDO: hubo acto clínico. */
        SELECT  'ESPONTANEA'          AS Origen,
                NULL                  AS IdCita,
                NULL                  AS NumeroCita,
                NULL                  AS FechaCita,
                a.IdPaciente,
                a.IdMedico,
                a.IdEspecialidad,
                'ATENDIDO'            AS EstadoCita,
                'Atendido'            AS EstadoDescripcion,
                NULL                  AS MotivoEstado,
                a.MotivoConsulta      AS Motivo,
                a.IdAtencion,
                a.NumeroAtencion,
                a.FechaAtencion,
                a.FechaAtencion       AS FechaReferencia
        FROM    dbo.vw_AtencionResumen a
        WHERE   a.IdCita IS NULL
          AND   a.Estado <> 'N'
          AND   a.FechaAtencion >= @FechaDesde
          AND   a.FechaAtencion <  @Limite
    )
    SELECT  e.Origen,
            e.NumeroCita,
            e.FechaCita,
            e.NumeroAtencion,
            e.FechaAtencion,
            e.FechaReferencia,
            e.EstadoCita,
            e.EstadoDescripcion,
            e.MotivoEstado,
            e.Motivo,
            p.TipoDocumento,
            p.NumeroDocumento                                            AS DocumentoPaciente,
            p.HistoriaClinica,
            (p.ApellidoPaterno + ' ' + ISNULL(p.ApellidoMaterno, '') + ', ' + p.Nombres) AS Paciente,
            p.Sexo,
            DATEDIFF(YEAR, p.FechaNacimiento, e.FechaReferencia)
                - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, p.FechaNacimiento, e.FechaReferencia), p.FechaNacimiento)
                            > CAST(e.FechaReferencia AS DATE)
                       THEN 1 ELSE 0 END                                 AS EdadPaciente,
            (m.Apellidos + ', ' + m.Nombres)                             AS Medico,
            es.Nombre                                                    AS Especialidad,
            /* Diagnósticos concatenados sin cursor: STUFF + FOR XML PATH.
               Se usa esta forma en lugar de STRING_AGG para mantener la
               compatibilidad con SQL Server 2016. */
            ISNULL(STUFF((SELECT ', ' + d.CodigoCie10 + ' ' + d.DescripcionDiagnostico
                          FROM   dbo.AtencionDetalle d
                          WHERE  d.IdAtencion = e.IdAtencion
                          ORDER BY d.Item
                          FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), '') AS Diagnosticos
    FROM    Encuentros           e
    INNER JOIN dbo.Paciente      p  ON p.IdPaciente     = e.IdPaciente
    INNER JOIN dbo.Medico        m  ON m.IdMedico       = e.IdMedico
    INNER JOIN dbo.Especialidad  es ON es.IdEspecialidad = e.IdEspecialidad
    WHERE  (@Estado         IS NULL OR e.EstadoCita     = @Estado)
      AND  (@IdPaciente     IS NULL OR e.IdPaciente     = @IdPaciente)
      AND  (@IdMedico       IS NULL OR e.IdMedico       = @IdMedico)
      AND  (@IdEspecialidad IS NULL OR e.IdEspecialidad = @IdEspecialidad)
      AND  (@Documento      IS NULL OR p.NumeroDocumento LIKE '%' + @Documento + '%')
      AND  (@Codigo IS NULL
            OR EXISTS (SELECT 1 FROM dbo.AtencionDetalle d
                       WHERE d.IdAtencion = e.IdAtencion
                         AND d.CodigoCie10 LIKE @Codigo + '%'))
    ORDER BY e.FechaReferencia DESC, Paciente
    OPTION (RECOMPILE);
END
GO

/* El reporte anterior se conserva para no romper nada que aún lo invoque. */
IF OBJECT_ID('dbo.usp_Reporte_Atenciones', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Reporte_Atenciones;
GO
CREATE PROCEDURE dbo.usp_Reporte_Atenciones
    @FechaDesde DATE,
    @FechaHasta DATE,
    @IdMedico   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    EXEC dbo.usp_Reporte_General
         @FechaDesde     = @FechaDesde,
         @FechaHasta     = @FechaHasta,
         @Estado         = NULL,
         @IdPaciente     = NULL,
         @Documento      = NULL,
         @IdMedico       = @IdMedico,
         @IdEspecialidad = NULL,
         @CodigoCie10    = NULL;
END
GO

PRINT '  [5/5] Procedimientos de reportes creados.';
PRINT '=== Procedimientos HVLH: completado. Ejecute 07. ===';
GO
