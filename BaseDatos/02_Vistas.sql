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
