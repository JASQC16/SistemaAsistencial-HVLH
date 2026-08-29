/* =============================================================================
   Hospital Nacional Víctor Larco Herrera - HVLH
   Script 07: CARGA INICIAL DEL CATÁLOGO CIE-10 (MINSA - Perú) Y DATOS DE AGENDA

   ---------------------------------------------------------------------------
   ORIGEN DE LOS DATOS

   Fuente oficial: Ministerio de Salud del Perú - REUNIS (Repositorio Único
   Nacional de Información en Salud), sección de catálogos y tablas maestras
   del sistema HIS-MINSA:

       https://www.minsa.gob.pe/reunis/   ->  "CIE-10 (MINSA - Excel)"
                                              "USO DE CÓDIGOS CIE-10 (RM 447-2024)"
                                              "CESE DE USO DE CÓDIGOS CIE-10 (RM 447-2024)"

   Espejo en datos abiertos del Estado peruano (dataset "Tablas Maestras
   HISMINSA", que incluye la tabla de diagnósticos del HIS):

       https://datosabiertos.gob.pe/

   ---------------------------------------------------------------------------
   QUÉ CARGA ESTE SCRIPT

   Una carga inicial en español con los diagnósticos de uso más frecuente en un
   hospital de salud mental: el capítulo V completo a nivel de categoría
   (F00-F99), las subcategorías de las familias más utilizadas, y un conjunto de
   códigos de morbilidad general y de factores que influyen en el estado de
   salud (capítulo XXI).

   Es una carga de arranque para que el sistema funcione desde el primer día sin
   depender de Internet. El catálogo completo del MINSA (unos 15 000 códigos) se
   incorpora ejecutando la importación descrita en el README: se vuelca el
   archivo oficial en dbo.CatalogoCie10_Staging y se ejecuta
   dbo.usp_Cie10_ImportarDesdeStaging, que consolida con MERGE.

   El script es idempotente: vuelve a poblar el staging y a consolidar, sin
   duplicar filas ni tocar los diagnósticos ya registrados en atenciones.

   Requisito previo: 01, 02, 03, 04, 05 y 06.
   ============================================================================= */

USE HospitalAtencionesDB;
GO

SET NOCOUNT ON;
GO

PRINT '=== Catálogo CIE-10 MINSA: inicio ===';
GO

TRUNCATE TABLE dbo.CatalogoCie10_Staging;
GO

/* -----------------------------------------------------------------------------
   Capítulo V: Trastornos mentales y del comportamiento (F00-F99)
   Categorías de tres caracteres.
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.CatalogoCie10_Staging (CodigoCie10, Descripcion, Estado) VALUES
('F00', N'Demencia en la enfermedad de Alzheimer', 'V'),
('F01', N'Demencia vascular', 'V'),
('F02', N'Demencia en otras enfermedades clasificadas en otra parte', 'V'),
('F03', N'Demencia, no especificada', 'V'),
('F04', N'Síndrome amnésico orgánico, no inducido por alcohol o por otras sustancias psicoactivas', 'V'),
('F05', N'Delirio, no inducido por alcohol o por otras sustancias psicoactivas', 'V'),
('F06', N'Otros trastornos mentales debidos a lesión y disfunción cerebral y a enfermedad física', 'V'),
('F07', N'Trastornos de la personalidad y del comportamiento debidos a enfermedad, lesión y disfunción cerebral', 'V'),
('F09', N'Trastorno mental orgánico o sintomático, no especificado', 'V'),
('F10', N'Trastornos mentales y del comportamiento debidos al uso de alcohol', 'V'),
('F11', N'Trastornos mentales y del comportamiento debidos al uso de opiáceos', 'V'),
('F12', N'Trastornos mentales y del comportamiento debidos al uso de cannabinoides', 'V'),
('F13', N'Trastornos mentales y del comportamiento debidos al uso de sedantes o hipnóticos', 'V'),
('F14', N'Trastornos mentales y del comportamiento debidos al uso de cocaína', 'V'),
('F15', N'Trastornos mentales y del comportamiento debidos al uso de otros estimulantes, incluida la cafeína', 'V'),
('F16', N'Trastornos mentales y del comportamiento debidos al uso de alucinógenos', 'V'),
('F17', N'Trastornos mentales y del comportamiento debidos al uso de tabaco', 'V'),
('F18', N'Trastornos mentales y del comportamiento debidos al uso de disolventes volátiles', 'V'),
('F19', N'Trastornos mentales y del comportamiento debidos al uso de múltiples drogas y al uso de otras sustancias psicoactivas', 'V'),
('F20', N'Esquizofrenia', 'V'),
('F21', N'Trastorno esquizotípico', 'V'),
('F22', N'Trastornos delirantes persistentes', 'V'),
('F23', N'Trastornos psicóticos agudos y transitorios', 'V'),
('F24', N'Trastorno delirante inducido', 'V'),
('F25', N'Trastornos esquizoafectivos', 'V'),
('F28', N'Otros trastornos psicóticos de origen no orgánico', 'V'),
('F29', N'Psicosis de origen no orgánico, no especificada', 'V'),
('F30', N'Episodio maníaco', 'V'),
('F31', N'Trastorno afectivo bipolar', 'V'),
('F32', N'Episodio depresivo', 'V'),
('F33', N'Trastorno depresivo recurrente', 'V'),
('F34', N'Trastornos del humor [afectivos] persistentes', 'V'),
('F38', N'Otros trastornos del humor [afectivos]', 'V'),
('F39', N'Trastorno del humor [afectivo], no especificado', 'V'),
('F40', N'Trastornos fóbicos de ansiedad', 'V'),
('F41', N'Otros trastornos de ansiedad', 'V'),
('F42', N'Trastorno obsesivo-compulsivo', 'V'),
('F43', N'Reacción al estrés grave y trastornos de adaptación', 'V'),
('F44', N'Trastornos disociativos [de conversión]', 'V'),
('F45', N'Trastornos somatomorfos', 'V'),
('F48', N'Otros trastornos neuróticos', 'V'),
('F50', N'Trastornos de la ingestión de alimentos', 'V'),
('F51', N'Trastornos no orgánicos del sueño', 'V'),
('F52', N'Disfunción sexual no ocasionada por trastorno ni por enfermedad orgánicos', 'V'),
('F53', N'Trastornos mentales y del comportamiento asociados con el puerperio, no clasificados en otra parte', 'V'),
('F54', N'Factores psicológicos y del comportamiento asociados con trastornos o enfermedades clasificados en otra parte', 'V'),
('F55', N'Abuso de sustancias que no producen dependencia', 'V'),
('F59', N'Síndromes del comportamiento asociados con alteraciones fisiológicas y factores físicos, no especificados', 'V'),
('F60', N'Trastornos específicos de la personalidad', 'V'),
('F61', N'Trastornos mixtos y otros trastornos de la personalidad', 'V'),
('F62', N'Cambios perdurables de la personalidad, no atribuibles a lesión o enfermedad cerebral', 'V'),
('F63', N'Trastornos de los hábitos y de los impulsos', 'V'),
('F64', N'Trastornos de la identidad de género', 'V'),
('F65', N'Trastornos de la preferencia sexual', 'V'),
('F66', N'Trastornos psicológicos y del comportamiento asociados con el desarrollo y con la orientación sexual', 'V'),
('F68', N'Otros trastornos de la personalidad y del comportamiento en adultos', 'V'),
('F69', N'Trastorno de la personalidad y del comportamiento en adultos, no especificado', 'V'),
('F70', N'Retraso mental leve', 'V'),
('F71', N'Retraso mental moderado', 'V'),
('F72', N'Retraso mental grave', 'V'),
('F73', N'Retraso mental profundo', 'V'),
('F78', N'Otros tipos de retraso mental', 'V'),
('F79', N'Retraso mental, no especificado', 'V'),
('F80', N'Trastornos específicos del desarrollo del habla y del lenguaje', 'V'),
('F81', N'Trastornos específicos del desarrollo de las habilidades escolares', 'V'),
('F82', N'Trastorno específico del desarrollo de la función motriz', 'V'),
('F83', N'Trastornos específicos mixtos del desarrollo', 'V'),
('F84', N'Trastornos generalizados del desarrollo', 'V'),
('F88', N'Otros trastornos del desarrollo psicológico', 'V'),
('F89', N'Trastorno del desarrollo psicológico, no especificado', 'V'),
('F90', N'Trastornos hipercinéticos', 'V'),
('F91', N'Trastornos de la conducta', 'V'),
('F92', N'Trastornos mixtos de la conducta y de las emociones', 'V'),
('F93', N'Trastornos emocionales de comienzo específico en la niñez', 'V'),
('F94', N'Trastornos del comportamiento social de comienzo específico en la niñez y en la adolescencia', 'V'),
('F95', N'Trastornos por tics', 'V'),
('F98', N'Otros trastornos emocionales y del comportamiento que aparecen habitualmente en la niñez y en la adolescencia', 'V'),
('F99', N'Trastorno mental, no especificado', 'V');
GO

/* -----------------------------------------------------------------------------
   Subcategorías de cuatro caracteres de las familias más utilizadas.
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.CatalogoCie10_Staging (CodigoCie10, Descripcion, Estado) VALUES
-- Demencias
('F000', N'Demencia en la enfermedad de Alzheimer, de comienzo temprano', 'V'),
('F001', N'Demencia en la enfermedad de Alzheimer, de comienzo tardío', 'V'),
('F002', N'Demencia en la enfermedad de Alzheimer, atípica o de tipo mixto', 'V'),
('F009', N'Demencia en la enfermedad de Alzheimer, no especificada', 'V'),
('F010', N'Demencia vascular de comienzo agudo', 'V'),
('F011', N'Demencia vascular por infartos múltiples', 'V'),
('F012', N'Demencia vascular subcortical', 'V'),
('F013', N'Demencia vascular mixta, cortical y subcortical', 'V'),
('F019', N'Demencia vascular, no especificada', 'V'),
('F051', N'Delirio superpuesto a demencia', 'V'),
('F060', N'Alucinosis orgánica', 'V'),
('F062', N'Trastorno delirante [esquizofreniforme] orgánico', 'V'),
('F063', N'Trastornos del humor [afectivos] orgánicos', 'V'),
('F064', N'Trastorno de ansiedad orgánico', 'V'),
('F067', N'Trastorno cognoscitivo leve', 'V'),
-- Alcohol y sustancias
('F100', N'Trastornos mentales y del comportamiento debidos al uso de alcohol: intoxicación aguda', 'V'),
('F101', N'Trastornos mentales y del comportamiento debidos al uso de alcohol: uso nocivo', 'V'),
('F102', N'Trastornos mentales y del comportamiento debidos al uso de alcohol: síndrome de dependencia', 'V'),
('F103', N'Trastornos mentales y del comportamiento debidos al uso de alcohol: estado de abstinencia', 'V'),
('F104', N'Trastornos mentales y del comportamiento debidos al uso de alcohol: estado de abstinencia con delirio', 'V'),
('F105', N'Trastornos mentales y del comportamiento debidos al uso de alcohol: trastorno psicótico', 'V'),
('F106', N'Trastornos mentales y del comportamiento debidos al uso de alcohol: síndrome amnésico', 'V'),
('F107', N'Trastornos mentales y del comportamiento debidos al uso de alcohol: trastorno psicótico residual y de comienzo tardío', 'V'),
('F109', N'Trastornos mentales y del comportamiento debidos al uso de alcohol: trastorno mental y del comportamiento, no especificado', 'V'),
('F112', N'Trastornos mentales y del comportamiento debidos al uso de opiáceos: síndrome de dependencia', 'V'),
('F122', N'Trastornos mentales y del comportamiento debidos al uso de cannabinoides: síndrome de dependencia', 'V'),
('F132', N'Trastornos mentales y del comportamiento debidos al uso de sedantes o hipnóticos: síndrome de dependencia', 'V'),
('F142', N'Trastornos mentales y del comportamiento debidos al uso de cocaína: síndrome de dependencia', 'V'),
('F172', N'Trastornos mentales y del comportamiento debidos al uso de tabaco: síndrome de dependencia', 'V'),
('F192', N'Trastornos mentales y del comportamiento debidos al uso de múltiples drogas: síndrome de dependencia', 'V'),
('F195', N'Trastornos mentales y del comportamiento debidos al uso de múltiples drogas: trastorno psicótico', 'V'),
-- Esquizofrenia y psicosis
('F200', N'Esquizofrenia paranoide', 'V'),
('F201', N'Esquizofrenia hebefrénica', 'V'),
('F202', N'Esquizofrenia catatónica', 'V'),
('F203', N'Esquizofrenia indiferenciada', 'V'),
('F204', N'Depresión postesquizofrénica', 'V'),
('F205', N'Esquizofrenia residual', 'V'),
('F206', N'Esquizofrenia simple', 'V'),
('F208', N'Otras esquizofrenias', 'V'),
('F209', N'Esquizofrenia, no especificada', 'V'),
('F220', N'Trastorno delirante', 'V'),
('F228', N'Otros trastornos delirantes persistentes', 'V'),
('F230', N'Trastorno psicótico agudo polimorfo, sin síntomas de esquizofrenia', 'V'),
('F231', N'Trastorno psicótico agudo polimorfo, con síntomas de esquizofrenia', 'V'),
('F232', N'Trastorno psicótico agudo de tipo esquizofrénico', 'V'),
('F239', N'Trastorno psicótico agudo y transitorio, no especificado', 'V'),
('F250', N'Trastorno esquizoafectivo de tipo maníaco', 'V'),
('F251', N'Trastorno esquizoafectivo de tipo depresivo', 'V'),
('F252', N'Trastorno esquizoafectivo de tipo mixto', 'V'),
('F259', N'Trastorno esquizoafectivo, no especificado', 'V'),
-- Trastornos del humor
('F300', N'Hipomanía', 'V'),
('F301', N'Manía sin síntomas psicóticos', 'V'),
('F302', N'Manía con síntomas psicóticos', 'V'),
('F310', N'Trastorno afectivo bipolar, episodio hipomaníaco presente', 'V'),
('F311', N'Trastorno afectivo bipolar, episodio maníaco presente sin síntomas psicóticos', 'V'),
('F312', N'Trastorno afectivo bipolar, episodio maníaco presente con síntomas psicóticos', 'V'),
('F313', N'Trastorno afectivo bipolar, episodio depresivo presente leve o moderado', 'V'),
('F314', N'Trastorno afectivo bipolar, episodio depresivo grave presente sin síntomas psicóticos', 'V'),
('F315', N'Trastorno afectivo bipolar, episodio depresivo grave presente con síntomas psicóticos', 'V'),
('F316', N'Trastorno afectivo bipolar, episodio mixto presente', 'V'),
('F317', N'Trastorno afectivo bipolar, actualmente en remisión', 'V'),
('F319', N'Trastorno afectivo bipolar, no especificado', 'V'),
('F320', N'Episodio depresivo leve', 'V'),
('F321', N'Episodio depresivo moderado', 'V'),
('F322', N'Episodio depresivo grave sin síntomas psicóticos', 'V'),
('F323', N'Episodio depresivo grave con síntomas psicóticos', 'V'),
('F328', N'Otros episodios depresivos', 'V'),
('F329', N'Episodio depresivo, no especificado', 'V'),
('F330', N'Trastorno depresivo recurrente, episodio leve presente', 'V'),
('F331', N'Trastorno depresivo recurrente, episodio moderado presente', 'V'),
('F332', N'Trastorno depresivo recurrente, episodio depresivo grave presente sin síntomas psicóticos', 'V'),
('F333', N'Trastorno depresivo recurrente, episodio depresivo grave presente con síntomas psicóticos', 'V'),
('F334', N'Trastorno depresivo recurrente, actualmente en remisión', 'V'),
('F339', N'Trastorno depresivo recurrente, no especificado', 'V'),
('F340', N'Ciclotimia', 'V'),
('F341', N'Distimia', 'V'),
-- Neuróticos y relacionados con el estrés
('F400', N'Agorafobia', 'V'),
('F401', N'Fobias sociales', 'V'),
('F402', N'Fobias específicas (aisladas)', 'V'),
('F410', N'Trastorno de pánico [ansiedad paroxística episódica]', 'V'),
('F411', N'Trastorno de ansiedad generalizada', 'V'),
('F412', N'Trastorno mixto de ansiedad y depresión', 'V'),
('F418', N'Otros trastornos de ansiedad especificados', 'V'),
('F419', N'Trastorno de ansiedad, no especificado', 'V'),
('F420', N'Predominio de pensamientos o rumiaciones obsesivas', 'V'),
('F421', N'Predominio de actos compulsivos [rituales obsesivos]', 'V'),
('F422', N'Pensamientos y actos obsesivos mixtos', 'V'),
('F430', N'Reacción al estrés agudo', 'V'),
('F431', N'Trastorno de estrés postraumático', 'V'),
('F432', N'Trastornos de adaptación', 'V'),
('F440', N'Amnesia disociativa', 'V'),
('F444', N'Trastornos disociativos del movimiento', 'V'),
('F445', N'Convulsiones disociativas', 'V'),
('F450', N'Trastorno de somatización', 'V'),
('F452', N'Trastorno hipocondríaco', 'V'),
('F453', N'Disfunción autonómica somatomorfa', 'V'),
('F454', N'Trastorno de dolor persistente somatomorfo', 'V'),
('F480', N'Neurastenia', 'V'),
-- Conducta alimentaria, sueño y personalidad
('F500', N'Anorexia nerviosa', 'V'),
('F502', N'Bulimia nerviosa', 'V'),
('F510', N'Insomnio no orgánico', 'V'),
('F514', N'Terrores del sueño [terrores nocturnos]', 'V'),
('F600', N'Trastorno paranoide de la personalidad', 'V'),
('F601', N'Trastorno esquizoide de la personalidad', 'V'),
('F602', N'Trastorno asocial de la personalidad', 'V'),
('F603', N'Trastorno de la personalidad emocionalmente inestable', 'V'),
('F604', N'Trastorno histriónico de la personalidad', 'V'),
('F605', N'Trastorno anancástico de la personalidad', 'V'),
('F606', N'Trastorno de la personalidad ansiosa (evasiva, elusiva)', 'V'),
('F607', N'Trastorno de la personalidad dependiente', 'V'),
('F630', N'Ludopatía', 'V'),
-- Desarrollo, niñez y adolescencia
('F840', N'Autismo en la niñez', 'V'),
('F841', N'Autismo atípico', 'V'),
('F842', N'Síndrome de Rett', 'V'),
('F845', N'Síndrome de Asperger', 'V'),
('F900', N'Perturbación de la actividad y de la atención', 'V'),
('F901', N'Trastorno hipercinético de la conducta', 'V'),
('F909', N'Trastorno hipercinético, no especificado', 'V'),
('F911', N'Trastorno de la conducta insociable', 'V'),
('F912', N'Trastorno de la conducta sociable', 'V'),
('F930', N'Trastorno de ansiedad de separación en la niñez', 'V'),
('F950', N'Trastorno por tic transitorio', 'V'),
('F980', N'Enuresis no orgánica', 'V');
GO

/* -----------------------------------------------------------------------------
   Morbilidad general de uso frecuente en consulta externa y capítulo XXI
   (factores que influyen en el estado de salud y contacto con los servicios).
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.CatalogoCie10_Staging (CodigoCie10, Descripcion, Estado) VALUES
('A09', N'Diarrea y gastroenteritis de presunto origen infeccioso', 'V'),
('B349', N'Infección viral, no especificada', 'V'),
('D509', N'Anemia por deficiencia de hierro, sin otra especificación', 'V'),
('E039', N'Hipotiroidismo, no especificado', 'V'),
('E119', N'Diabetes mellitus tipo 2, sin mención de complicación', 'V'),
('E440', N'Desnutrición proteicocalórica moderada', 'V'),
('E660', N'Obesidad debida a exceso de calorías', 'V'),
('E785', N'Hiperlipidemia, no especificada', 'V'),
('G20', N'Enfermedad de Parkinson', 'V'),
('G252', N'Otras formas especificadas de temblor', 'V'),
('G251', N'Temblor inducido por drogas', 'V'),
('G40', N'Epilepsia', 'V'),
('G409', N'Epilepsia, tipo no especificado', 'V'),
('G43', N'Migraña', 'V'),
('G47', N'Trastornos del sueño', 'V'),
('G210', N'Síndrome neuroléptico maligno', 'V'),
('G211', N'Otro parkinsonismo secundario inducido por drogas', 'V'),
('G240', N'Distonía inducida por drogas', 'V'),
('I10', N'Hipertensión esencial (primaria)', 'V'),
('I679', N'Enfermedad cerebrovascular, no especificada', 'V'),
('J00', N'Rinofaringitis aguda [resfriado común]', 'V'),
('J029', N'Faringitis aguda, no especificada', 'V'),
('J189', N'Neumonía, no especificada', 'V'),
('K021', N'Caries de la dentina', 'V'),
('K297', N'Gastritis, no especificada', 'V'),
('K590', N'Constipación', 'V'),
('M545', N'Lumbago no especificado', 'V'),
('M796', N'Dolor en miembro', 'V'),
('N390', N'Infección de vías urinarias, sitio no especificado', 'V'),
('R069', N'Anormalidad respiratoria, no especificada', 'V'),
('R101', N'Dolor abdominal localizado en parte superior', 'V'),
('R42', N'Mareo y desvanecimiento', 'V'),
('R44', N'Otros síntomas y signos generales que involucran las sensaciones y percepciones', 'V'),
('R450', N'Nerviosismo', 'V'),
('R451', N'Inquietud y agitación', 'V'),
('R452', N'Infelicidad', 'V'),
('R454', N'Irritabilidad y enojo', 'V'),
('R458', N'Otros síntomas y signos que involucran el estado emocional', 'V'),
('R460', N'Muy bajo nivel de higiene personal', 'V'),
('R468', N'Otros síntomas y signos que involucran la apariencia y el comportamiento', 'V'),
('R51', N'Cefalea', 'V'),
('R531', N'Debilidad', 'V'),
('R55', N'Síncope y colapso', 'V'),
('Z000', N'Examen médico general', 'V'),
('Z004', N'Examen psiquiátrico general, no clasificado en otra parte', 'V'),
('Z032', N'Observación por sospecha de trastorno mental y del comportamiento', 'V'),
('Z098', N'Examen de seguimiento consecutivo a otro tratamiento por otras afecciones', 'V'),
('Z133', N'Examen de pesquisa especial para trastornos mentales y del comportamiento', 'V'),
('Z502', N'Rehabilitación del alcohólico', 'V'),
('Z503', N'Rehabilitación del drogadicto', 'V'),
('Z504', N'Psicoterapia, no clasificada en otra parte', 'V'),
('Z515', N'Atención paliativa', 'V'),
('Z546', N'Convalecencia consecutiva a otro tratamiento', 'V'),
('Z634', N'Desaparición o muerte de un miembro de la familia', 'V'),
('Z638', N'Otros problemas especificados en la relación entre esposos o pareja', 'V'),
('Z650', N'Problemas relacionados con condena en procesos civiles y penales sin prisión', 'V'),
('Z713', N'Consulta para instrucción y vigilancia de la dieta', 'V'),
('Z718', N'Otras consultas especificadas', 'V'),
('Z719', N'Consulta, no especificada', 'V'),
('Z760', N'Consulta para repetición de receta', 'V'),
('Z818', N'Historia familiar de otros trastornos mentales y del comportamiento', 'V'),
('Z864', N'Historia personal de abuso de sustancias psicoactivas', 'V'),
('Z915', N'Historia personal de lesión autoinfligida intencionalmente', 'V');
GO

/* -----------------------------------------------------------------------------
   Consolidación: se ejecuta el mismo procedimiento de importación que se usará
   con el archivo oficial completo del MINSA. Así el camino de carga inicial y
   el de actualización periódica son exactamente el mismo código.
   ----------------------------------------------------------------------------- */
EXEC dbo.usp_Cie10_ImportarDesdeStaging
     @VersionCatalogo = '2024.1-INICIAL',
     @Fuente          = 'MINSA - REUNIS / Tablas maestras HIS (carga inicial reducida)',
     @Usuario         = 'Script 07';
GO

/* -----------------------------------------------------------------------------
   Clasificación por capítulo y grupo, en una sola sentencia basada en rangos.
   No se recorre fila a fila: es una actualización por conjuntos.
   ----------------------------------------------------------------------------- */
UPDATE dbo.CatalogoCie10
SET Capitulo = CASE
        WHEN LEFT(CodigoCie10, 1) IN ('A','B')                       THEN 'I'
        WHEN LEFT(CodigoCie10, 1) = 'C'                              THEN 'II'
        WHEN LEFT(CodigoCie10, 1) = 'D' AND Categoria <= 'D48'       THEN 'II'
        WHEN LEFT(CodigoCie10, 1) = 'D'                              THEN 'III'
        WHEN LEFT(CodigoCie10, 1) = 'E'                              THEN 'IV'
        WHEN LEFT(CodigoCie10, 1) = 'F'                              THEN 'V'
        WHEN LEFT(CodigoCie10, 1) = 'G'                              THEN 'VI'
        WHEN LEFT(CodigoCie10, 1) = 'H' AND Categoria <= 'H59'       THEN 'VII'
        WHEN LEFT(CodigoCie10, 1) = 'H'                              THEN 'VIII'
        WHEN LEFT(CodigoCie10, 1) = 'I'                              THEN 'IX'
        WHEN LEFT(CodigoCie10, 1) = 'J'                              THEN 'X'
        WHEN LEFT(CodigoCie10, 1) = 'K'                              THEN 'XI'
        WHEN LEFT(CodigoCie10, 1) = 'L'                              THEN 'XII'
        WHEN LEFT(CodigoCie10, 1) = 'M'                              THEN 'XIII'
        WHEN LEFT(CodigoCie10, 1) = 'N'                              THEN 'XIV'
        WHEN LEFT(CodigoCie10, 1) = 'O'                              THEN 'XV'
        WHEN LEFT(CodigoCie10, 1) = 'P'                              THEN 'XVI'
        WHEN LEFT(CodigoCie10, 1) = 'Q'                              THEN 'XVII'
        WHEN LEFT(CodigoCie10, 1) = 'R'                              THEN 'XVIII'
        WHEN LEFT(CodigoCie10, 1) IN ('S','T')                       THEN 'XIX'
        WHEN LEFT(CodigoCie10, 1) IN ('V','W','X','Y')               THEN 'XX'
        WHEN LEFT(CodigoCie10, 1) = 'Z'                              THEN 'XXI'
        ELSE NULL END
WHERE Capitulo IS NULL;
GO

UPDATE dbo.CatalogoCie10
SET CapituloNombre = CASE Capitulo
        WHEN 'I'     THEN N'Ciertas enfermedades infecciosas y parasitarias'
        WHEN 'II'    THEN N'Neoplasias'
        WHEN 'III'   THEN N'Enfermedades de la sangre y de los órganos hematopoyéticos'
        WHEN 'IV'    THEN N'Enfermedades endocrinas, nutricionales y metabólicas'
        WHEN 'V'     THEN N'Trastornos mentales y del comportamiento'
        WHEN 'VI'    THEN N'Enfermedades del sistema nervioso'
        WHEN 'VII'   THEN N'Enfermedades del ojo y sus anexos'
        WHEN 'VIII'  THEN N'Enfermedades del oído y de la apófisis mastoides'
        WHEN 'IX'    THEN N'Enfermedades del sistema circulatorio'
        WHEN 'X'     THEN N'Enfermedades del sistema respiratorio'
        WHEN 'XI'    THEN N'Enfermedades del sistema digestivo'
        WHEN 'XII'   THEN N'Enfermedades de la piel y del tejido subcutáneo'
        WHEN 'XIII'  THEN N'Enfermedades del sistema osteomuscular y del tejido conjuntivo'
        WHEN 'XIV'   THEN N'Enfermedades del sistema genitourinario'
        WHEN 'XV'    THEN N'Embarazo, parto y puerperio'
        WHEN 'XVI'   THEN N'Ciertas afecciones originadas en el periodo perinatal'
        WHEN 'XVII'  THEN N'Malformaciones congénitas y anomalías cromosómicas'
        WHEN 'XVIII' THEN N'Síntomas, signos y hallazgos anormales no clasificados en otra parte'
        WHEN 'XIX'   THEN N'Traumatismos, envenenamientos y otras consecuencias de causas externas'
        WHEN 'XX'    THEN N'Causas externas de morbilidad y de mortalidad'
        WHEN 'XXI'   THEN N'Factores que influyen en el estado de salud y contacto con los servicios de salud'
        ELSE NULL END
WHERE CapituloNombre IS NULL;
GO

/* Grupos del capítulo V, que son los que el hospital consulta a diario. */
UPDATE dbo.CatalogoCie10
SET Grupo = CASE
        WHEN Categoria BETWEEN 'F00' AND 'F09' THEN 'F00-F09'
        WHEN Categoria BETWEEN 'F10' AND 'F19' THEN 'F10-F19'
        WHEN Categoria BETWEEN 'F20' AND 'F29' THEN 'F20-F29'
        WHEN Categoria BETWEEN 'F30' AND 'F39' THEN 'F30-F39'
        WHEN Categoria BETWEEN 'F40' AND 'F48' THEN 'F40-F48'
        WHEN Categoria BETWEEN 'F50' AND 'F59' THEN 'F50-F59'
        WHEN Categoria BETWEEN 'F60' AND 'F69' THEN 'F60-F69'
        WHEN Categoria BETWEEN 'F70' AND 'F79' THEN 'F70-F79'
        WHEN Categoria BETWEEN 'F80' AND 'F89' THEN 'F80-F89'
        WHEN Categoria BETWEEN 'F90' AND 'F98' THEN 'F90-F98'
        WHEN Categoria = 'F99'                 THEN 'F99'
        ELSE Grupo END
WHERE Capitulo = 'V' AND Grupo IS NULL;
GO

PRINT '  Catálogo CIE-10 cargado.';
GO

/* =============================================================================
   DATOS DE AGENDA DE EJEMPLO

   Se generan citas en los cinco estados para que el módulo de reportes tenga
   algo que mostrar desde el primer arranque. Solo se insertan si la tabla está
   vacía, de modo que una segunda ejecución no duplique nada.
   ============================================================================= */

IF NOT EXISTS (SELECT 1 FROM dbo.Cita)
BEGIN
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);

    /* Se toman los pacientes y médicos ya existentes, sin depender de que sus
       identificadores sean 1, 2, 3...: se resuelven por documento y colegiatura. */
    DECLARE @Agenda TABLE
    (
        Fila       INT IDENTITY(1,1),
        IdPaciente INT,
        IdMedico   INT,
        FechaCita  DATETIME2(0),
        Motivo     NVARCHAR(300),
        Estado     VARCHAR(12),
        MotivoEstado NVARCHAR(300)
    );

    INSERT INTO @Agenda (IdPaciente, IdMedico, FechaCita, Motivo, Estado, MotivoEstado)
    SELECT p.IdPaciente,
           m.IdMedico,
           DATEADD(HOUR, 8 + (ROW_NUMBER() OVER (ORDER BY p.IdPaciente, m.IdMedico) % 8),
                   CAST(DATEADD(DAY, -1 * ((ROW_NUMBER() OVER (ORDER BY p.IdPaciente, m.IdMedico)) % 20), @Hoy) AS DATETIME2(0))),
           N'Control ambulatorio programado',
           CASE (ROW_NUMBER() OVER (ORDER BY p.IdPaciente, m.IdMedico)) % 5
                WHEN 0 THEN 'CITADO'
                WHEN 1 THEN 'ATENDIDO'
                WHEN 2 THEN 'NO_ATENDIDO'
                WHEN 3 THEN 'NO_ACUDIO'
                ELSE        'CANCELADO' END,
           CASE (ROW_NUMBER() OVER (ORDER BY p.IdPaciente, m.IdMedico)) % 5
                WHEN 2 THEN N'Profesional no disponible por emergencia hospitalaria'
                WHEN 3 THEN N'El paciente no se presentó a la hora programada'
                WHEN 4 THEN N'Cancelada a solicitud del familiar responsable'
                ELSE NULL END
    FROM   dbo.Paciente p
    CROSS  JOIN dbo.Medico m
    WHERE  p.Activo = 1 AND m.Activo = 1;

    DECLARE @IdUsuario INT = (SELECT TOP (1) IdUsuario FROM dbo.Usuario ORDER BY IdUsuario);

    /* El estado ATENDIDO de la semilla se corrige más abajo: solo se deja en
       ATENDIDO aquello que efectivamente tenga una atención asociada. */
    INSERT INTO dbo.Cita
        (NumeroCita, IdPaciente, IdMedico, FechaCita, MotivoCita, Estado, MotivoEstado,
         IdUsuarioRegistro, FechaRegistro)
    SELECT 'CI-' + CAST(YEAR(a.FechaCita) AS VARCHAR(4)) + '-' +
               RIGHT('000000' + CAST(NEXT VALUE FOR dbo.SeqNumeroCita AS VARCHAR(10)), 6),
           a.IdPaciente, a.IdMedico, a.FechaCita, a.Motivo,
           CASE WHEN a.Estado = 'ATENDIDO' THEN 'CITADO' ELSE a.Estado END,
           a.MotivoEstado, @IdUsuario, SYSDATETIME()
    FROM   @Agenda a
    WHERE  a.Fila <= 30;

    /* Se enlazan las atenciones ya registradas con una cita del mismo paciente,
       para que el histórico quede coherente con el nuevo modelo. */
    ;WITH Candidatas AS
    (
        SELECT at.IdAtencion,
               c.IdCita,
               ROW_NUMBER() OVER (PARTITION BY c.IdCita ORDER BY at.IdAtencion) AS OrdenCita,
               ROW_NUMBER() OVER (PARTITION BY at.IdAtencion ORDER BY c.IdCita) AS OrdenAtencion
        FROM   dbo.Atencion at
        INNER JOIN dbo.Cita c
                ON c.IdPaciente = at.IdPaciente
               AND c.IdMedico   = at.IdMedico
               AND c.Estado     = 'CITADO'
        WHERE  at.IdCita IS NULL
          AND  at.Estado <> 'N'
    )
    UPDATE at
    SET    at.IdCita = c.IdCita
    FROM   dbo.Atencion at
    INNER JOIN Candidatas c ON c.IdAtencion = at.IdAtencion
    WHERE  c.OrdenCita = 1 AND c.OrdenAtencion = 1;

    /* Las citas que quedaron con atención pasan a ATENDIDO. */
    UPDATE c
    SET    c.Estado = 'ATENDIDO'
    FROM   dbo.Cita c
    WHERE  EXISTS (SELECT 1 FROM dbo.Atencion a WHERE a.IdCita = c.IdCita AND a.Estado <> 'N');

    PRINT '  Agenda de ejemplo generada.';
END
ELSE
BEGIN
    PRINT '  La tabla dbo.Cita ya contenía datos: no se generó agenda de ejemplo.';
END
GO

/* Los diagnósticos ya registrados se etiquetan con la versión del catálogo. */
UPDATE dbo.AtencionDetalle
SET    VersionCatalogoCie10 = '2024.1-INICIAL'
WHERE  VersionCatalogoCie10 IS NULL;
GO

PRINT '';
PRINT '=== Resumen ===';
GO

SELECT 'Catálogo CIE-10' AS Objeto, COUNT(1) AS Registros FROM dbo.CatalogoCie10
UNION ALL SELECT 'Pacientes',  COUNT(1) FROM dbo.Paciente
UNION ALL SELECT 'Citas',      COUNT(1) FROM dbo.Cita
UNION ALL SELECT 'Atenciones', COUNT(1) FROM dbo.Atencion;
GO

PRINT '=== Catálogo CIE-10 MINSA: completado ===';
GO
