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
