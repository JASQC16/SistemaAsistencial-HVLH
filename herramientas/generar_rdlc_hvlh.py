"""
Genera Reportes/rptGeneralHVLH.rdlc.

El RDLC se escribe con un guion en lugar de a mano porque es XML muy repetitivo:
cada celda del tablix son doce lineas practicamente identicas. Generarlo garantiza
que anchos, estilos y bordes sean consistentes y que anadir una columna sea cambiar
una linea de la lista COLUMNAS.
"""
import base64
import os

RAIZ = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
LOGO = os.path.join(RAIZ, "src", "Hospital.Presentacion", "Recursos", "logo-hvlh-reporte.png")
SALIDA = os.path.join(RAIZ, "src", "Hospital.Presentacion", "Reportes", "rptGeneralHVLH.rdlc")

NS = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition"
NS_RD = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"

# Paleta institucional HVLH
AZUL = "#209FE2"
AZUL_OSCURO = "#125C85"
GRIS = "#D6DBD5"
GRIS_SUAVE = "#F1F3F0"
NARANJA = "#FF9D42"
ROJO = "#E32619"
TEXTO = "#2E3A3F"
TEXTO_SUAVE = "#75837F"
BLANCO = "#FEFEFE"

ANCHO_UTIL = 10.8   # pulgadas disponibles en A4 apaisado con margenes de 0.4"

# (campo, titulo, ancho, alineacion, expresion)
COLUMNAS = [
    ("FechaReferencia", "Fecha",        0.85, "Center", "=Format(Fields!FechaReferencia.Value, \"dd/MM/yyyy\")"),
    ("NumeroCita",      "N. cita",      0.95, "Left",   "=IIf(IsNothing(Fields!NumeroCita.Value), \"Sin cita\", Fields!NumeroCita.Value)"),
    ("NumeroAtencion",  "N. atencion",  0.95, "Left",   "=IIf(IsNothing(Fields!NumeroAtencion.Value), \"-\", Fields!NumeroAtencion.Value)"),
    ("DocumentoPaciente", "Documento",  0.80, "Left",   "=Fields!TipoDocumento.Value & \" \" & Fields!DocumentoPaciente.Value"),
    ("HistoriaClinica", "H. clinica",   0.75, "Left",   "=Fields!HistoriaClinica.Value"),
    ("Paciente",        "Paciente",     1.85, "Left",   "=Fields!Paciente.Value"),
    ("EdadPaciente",    "Edad",         0.40, "Center", "=Fields!EdadPaciente.Value"),
    ("Sexo",            "Sexo",         0.35, "Center", "=Fields!Sexo.Value"),
    ("Medico",          "Profesional",  1.30, "Left",   "=Fields!Medico.Value"),
    ("Especialidad",    "Servicio",     0.90, "Left",   "=Fields!Especialidad.Value"),
    ("EstadoDescripcion", "Estado",     0.80, "Center", "=Fields!EstadoDescripcion.Value"),
    ("Diagnosticos",    "Diagnosticos CIE-10", 0.90, "Left", "=Fields!Diagnosticos.Value"),
]

CAMPOS = [
    "Origen", "NumeroCita", "FechaCita", "NumeroAtencion", "FechaAtencion",
    "FechaReferencia", "EstadoCita", "EstadoDescripcion", "MotivoEstado", "Motivo",
    "TipoDocumento", "DocumentoPaciente", "HistoriaClinica", "Paciente", "Sexo",
    "EdadPaciente", "Medico", "Especialidad", "Diagnosticos",
]

PARAMETROS = [
    ("pInstitucion",   "Hospital Nacional Victor Larco Herrera - HVLH"),
    ("pTituloReporte", "REPORTE DE PACIENTES Y ATENCIONES"),
    ("pFechaDesde",    "01/01/2026"),
    ("pFechaHasta",    "31/12/2026"),
    ("pFiltro",        "Estado: Todos"),
    ("pGenerado",      "01/01/2026 00:00"),
    ("pUsuario",       "Sistema"),
]

# Totales del pie de cabecera: (titulo, expresion, color)
TOTALES = [
    ("TOTAL REGISTROS", "=Count(Fields!EstadoCita.Value)", AZUL_OSCURO),
    ("ATENDIDOS",       "=Sum(IIf(Fields!EstadoCita.Value = \"ATENDIDO\", 1, 0))", AZUL),
    ("NO ATENDIDOS",    "=Sum(IIf(Fields!EstadoCita.Value = \"NO_ATENDIDO\", 1, 0))", NARANJA),
    ("CITADOS",         "=Sum(IIf(Fields!EstadoCita.Value = \"CITADO\", 1, 0))", TEXTO_SUAVE),
    ("NO ACUDIERON",    "=Sum(IIf(Fields!EstadoCita.Value = \"NO_ACUDIO\", 1, 0))", ROJO),
    ("CANCELADOS",      "=Sum(IIf(Fields!EstadoCita.Value = \"CANCELADO\", 1, 0))", TEXTO_SUAVE),
]


def escapar(texto):
    return (texto.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;"))


def textbox(nombre, valor, top, left, ancho, alto,
            tamano="9pt", peso="Default", color=TEXTO, alineacion="Left",
            fondo=None, alineacion_vertical="Middle", familia="Segoe UI",
            zindex=0, padding="2pt"):
    """Cuadro de texto suelto del cuerpo del reporte."""
    fondo_xml = "          <BackgroundColor>%s</BackgroundColor>\n" % fondo if fondo else ""
    return """      <Textbox Name="{nombre}">
        <CanGrow>true</CanGrow>
        <KeepTogether>true</KeepTogether>
        <Paragraphs>
          <Paragraph>
            <TextRuns>
              <TextRun>
                <Value>{valor}</Value>
                <Style>
                  <FontFamily>{familia}</FontFamily>
                  <FontSize>{tamano}</FontSize>
                  <FontWeight>{peso}</FontWeight>
                  <Color>{color}</Color>
                </Style>
              </TextRun>
            </TextRuns>
            <Style>
              <TextAlign>{alineacion}</TextAlign>
            </Style>
          </Paragraph>
        </Paragraphs>
        <Top>{top}in</Top>
        <Left>{left}in</Left>
        <Height>{alto}in</Height>
        <Width>{ancho}in</Width>
        <ZIndex>{zindex}</ZIndex>
        <Style>
{fondo}          <VerticalAlign>{valineacion}</VerticalAlign>
          <PaddingLeft>{padding}</PaddingLeft>
          <PaddingRight>{padding}</PaddingRight>
          <PaddingTop>2pt</PaddingTop>
          <PaddingBottom>2pt</PaddingBottom>
        </Style>
      </Textbox>
""".format(nombre=nombre, valor=escapar(valor), top=top, left=left, ancho=ancho, alto=alto,
           tamano=tamano, peso=peso, color=color, alineacion=alineacion, familia=familia,
           fondo=fondo_xml, valineacion=alineacion_vertical, zindex=zindex, padding=padding)


def celda(nombre, valor, tamano="8pt", peso="Default", color=TEXTO,
          alineacion="Left", fondo=None, borde=GRIS):
    """Cuadro de texto dentro de una celda del tablix."""
    fondo_xml = "            <BackgroundColor>%s</BackgroundColor>\n" % fondo if fondo else ""
    return """        <TablixCell>
          <CellContents>
            <Textbox Name="{nombre}">
              <CanGrow>true</CanGrow>
              <KeepTogether>true</KeepTogether>
              <Paragraphs>
                <Paragraph>
                  <TextRuns>
                    <TextRun>
                      <Value>{valor}</Value>
                      <Style>
                        <FontFamily>Segoe UI</FontFamily>
                        <FontSize>{tamano}</FontSize>
                        <FontWeight>{peso}</FontWeight>
                        <Color>{color}</Color>
                      </Style>
                    </TextRun>
                  </TextRuns>
                  <Style>
                    <TextAlign>{alineacion}</TextAlign>
                  </Style>
                </Paragraph>
              </Paragraphs>
              <Style>
{fondo}                <Border>
                  <Color>{borde}</Color>
                  <Style>Solid</Style>
                  <Width>0.5pt</Width>
                </Border>
                <VerticalAlign>Middle</VerticalAlign>
                <PaddingLeft>3pt</PaddingLeft>
                <PaddingRight>3pt</PaddingRight>
                <PaddingTop>2pt</PaddingTop>
                <PaddingBottom>2pt</PaddingBottom>
              </Style>
            </Textbox>
          </CellContents>
        </TablixCell>
""".format(nombre=nombre, valor=escapar(valor), tamano=tamano, peso=peso, color=color,
           alineacion=alineacion, fondo=fondo_xml, borde=borde)


def construir():
    with open(LOGO, "rb") as archivo:
        logo_base64 = base64.b64encode(archivo.read()).decode("ascii")

    partes = []
    partes.append('<?xml version="1.0" encoding="utf-8"?>\n')
    partes.append('<Report xmlns="%s" xmlns:rd="%s">\n' % (NS, NS_RD))
    partes.append("  <AutoRefresh>0</AutoRefresh>\n")

    # ---- Origen de datos --------------------------------------------------
    partes.append("""  <DataSources>
    <DataSource Name="dsHospital">
      <ConnectionProperties>
        <DataProvider>System.Data.DataSet</DataProvider>
        <ConnectString>/* Local Connection */</ConnectString>
      </ConnectionProperties>
      <rd:DataSourceID>a1b2c3d4-0001-4000-8000-000000000001</rd:DataSourceID>
    </DataSource>
  </DataSources>
""")

    campos_xml = "".join(
        '      <Field Name="{c}">\n        <DataField>{c}</DataField>\n'
        '        <rd:TypeName>System.String</rd:TypeName>\n      </Field>\n'.format(c=c)
        for c in CAMPOS)

    partes.append("""  <DataSets>
    <DataSet Name="dsReporte">
      <Fields>
{campos}      </Fields>
      <Query>
        <DataSourceName>dsHospital</DataSourceName>
        <CommandText />
        <rd:UseGenericDesigner>true</rd:UseGenericDesigner>
      </Query>
    </DataSet>
  </DataSets>
""".format(campos=campos_xml))

    # ---- Cuerpo -----------------------------------------------------------
    cuerpo = []

    # Logo institucional
    cuerpo.append("""      <Image Name="imgLogo">
        <Source>Embedded</Source>
        <Value>LogoHVLH</Value>
        <Sizing>FitProportional</Sizing>
        <Top>0in</Top>
        <Left>0in</Left>
        <Height>0.78in</Height>
        <Width>0.78in</Width>
        <ZIndex>1</ZIndex>
        <Style />
      </Image>
""")

    cuerpo.append(textbox("tbInstitucion", "=Parameters!pInstitucion.Value",
                          top=0.02, left=0.88, ancho=7.2, alto=0.30,
                          tamano="14pt", peso="Bold", color=AZUL_OSCURO, zindex=2))
    cuerpo.append(textbox("tbTitulo", "=Parameters!pTituloReporte.Value",
                          top=0.32, left=0.88, ancho=7.2, alto=0.26,
                          tamano="11pt", peso="Bold", color=TEXTO, zindex=3))
    cuerpo.append(textbox(
        "tbPeriodo",
        '=" Periodo consultado:  " & Parameters!pFechaDesde.Value & "   al   " & Parameters!pFechaHasta.Value',
        top=0.58, left=0.88, ancho=7.2, alto=0.20, tamano="9pt", color=TEXTO, zindex=4))
    cuerpo.append(textbox("tbFiltro", '=" Filtro aplicado:  " & Parameters!pFiltro.Value',
                          top=0.78, left=0.88, ancho=9.9, alto=0.20,
                          tamano="9pt", color=TEXTO_SUAVE, zindex=5))

    cuerpo.append(textbox("tbGenerado", '="Generado el " & Parameters!pGenerado.Value',
                          top=0.10, left=8.2, ancho=2.6, alto=0.20,
                          tamano="8pt", color=TEXTO_SUAVE, alineacion="Right", zindex=6))
    cuerpo.append(textbox("tbUsuario", '="Usuario: " & Parameters!pUsuario.Value',
                          top=0.30, left=8.2, ancho=2.6, alto=0.20,
                          tamano="8pt", color=TEXTO_SUAVE, alineacion="Right", zindex=7))

    # Franja azul institucional
    cuerpo.append("""      <Line Name="lnSeparador">
        <Top>1.02in</Top>
        <Left>0in</Left>
        <Height>0in</Height>
        <Width>%.2fin</Width>
        <ZIndex>8</ZIndex>
        <Style>
          <Border>
            <Color>%s</Color>
            <Style>Solid</Style>
            <Width>2.5pt</Width>
          </Border>
        </Style>
      </Line>
""" % (ANCHO_UTIL, AZUL))

    # Totales
    ancho_total = ANCHO_UTIL / len(TOTALES)
    z = 10
    for indice, (titulo, expresion, color) in enumerate(TOTALES):
        izquierda = round(indice * ancho_total, 3)
        cuerpo.append(textbox("tbTotTitulo%d" % indice, titulo,
                              top=1.10, left=izquierda, ancho=round(ancho_total, 3), alto=0.18,
                              tamano="7.5pt", peso="Bold", color=TEXTO_SUAVE,
                              alineacion="Center", fondo=GRIS_SUAVE, zindex=z))
        z += 1
        cuerpo.append(textbox("tbTotValor%d" % indice, expresion,
                              top=1.28, left=izquierda, ancho=round(ancho_total, 3), alto=0.26,
                              tamano="13pt", peso="Bold", color=color,
                              alineacion="Center", fondo=GRIS_SUAVE, zindex=z))
        z += 1

    # ---- Tablix -----------------------------------------------------------
    columnas_xml = "".join(
        "          <TablixColumn>\n            <Width>%.2fin</Width>\n          </TablixColumn>\n" % ancho
        for _, _, ancho, _, _ in COLUMNAS)

    cabecera_xml = "".join(
        celda("thd" + campo, titulo, tamano="8pt", peso="Bold", color=BLANCO,
              alineacion="Center", fondo=AZUL, borde=AZUL)
        for campo, titulo, _, _, _ in COLUMNAS)

    detalle_xml = "".join(
        celda("tdt" + campo, expresion, tamano="8pt", alineacion=alineacion)
        for campo, _, _, alineacion, expresion in COLUMNAS)

    miembros_columna = "".join("          <TablixMember />\n" for _ in COLUMNAS)

    cuerpo.append("""      <Tablix Name="tblDatos">
        <TablixBody>
          <TablixColumns>
{columnas}          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.30in</Height>
              <TablixCells>
{cabecera}              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.24in</Height>
              <TablixCells>
{detalle}              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
{miembros}          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
              <RepeatOnNewPage>true</RepeatOnNewPage>
            </TablixMember>
            <TablixMember>
              <Group Name="Detalle" />
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>dsReporte</DataSetName>
        <NoRowsMessage>No se encontraron registros para el periodo y los filtros seleccionados.</NoRowsMessage>
        <Top>1.66in</Top>
        <Left>0in</Left>
        <Height>0.54in</Height>
        <Width>{ancho}in</Width>
        <ZIndex>40</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
""".format(columnas=columnas_xml, cabecera=cabecera_xml, detalle=detalle_xml,
           miembros=miembros_columna, ancho=ANCHO_UTIL))

    partes.append("""  <ReportSections>
    <ReportSection>
      <Body>
        <ReportItems>
{cuerpo}        </ReportItems>
        <Height>2.30in</Height>
        <Style />
      </Body>
      <Width>{ancho}in</Width>
      <Page>
        <PageFooter>
          <Height>0.35in</Height>
          <PrintOnFirstPage>true</PrintOnFirstPage>
          <PrintOnLastPage>true</PrintOnLastPage>
          <ReportItems>
{pie}          </ReportItems>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
          </Style>
        </PageFooter>
        <PageHeight>8.27in</PageHeight>
        <PageWidth>11.69in</PageWidth>
        <LeftMargin>0.4in</LeftMargin>
        <RightMargin>0.4in</RightMargin>
        <TopMargin>0.4in</TopMargin>
        <BottomMargin>0.4in</BottomMargin>
        <ColumnSpacing>0.13in</ColumnSpacing>
        <Style />
      </Page>
    </ReportSection>
  </ReportSections>
""".format(cuerpo="".join("  " + linea + "\n" for linea in "".join(cuerpo).split("\n") if linea.strip()),
           ancho=ANCHO_UTIL,
           pie="".join("    " + linea + "\n" for linea in (
               textbox("tbPie",
                       '=Parameters!pInstitucion.Value & "  -  Sistema de atenciones ambulatorias"',
                       top=0.06, left=0, ancho=7.0, alto=0.20,
                       tamano="7.5pt", color=TEXTO_SUAVE) +
               textbox("tbPagina",
                       '="Pagina " & Globals!PageNumber & " de " & Globals!TotalPages',
                       top=0.06, left=7.4, ancho=3.4, alto=0.20,
                       tamano="7.5pt", color=TEXTO_SUAVE, alineacion="Right")
           ).split("\n") if linea.strip())))

    # ---- Parametros -------------------------------------------------------
    parametros_xml = "".join("""    <ReportParameter Name="{nombre}">
      <DataType>String</DataType>
      <DefaultValue>
        <Values>
          <Value>{valor}</Value>
        </Values>
      </DefaultValue>
      <Prompt>{nombre}</Prompt>
    </ReportParameter>
""".format(nombre=nombre, valor=escapar(valor)) for nombre, valor in PARAMETROS)

    partes.append("  <ReportParameters>\n%s  </ReportParameters>\n" % parametros_xml)

    # ---- Imagen incrustada ------------------------------------------------
    partes.append("""  <EmbeddedImages>
    <EmbeddedImage Name="LogoHVLH">
      <MIMEType>image/png</MIMEType>
      <ImageData>%s</ImageData>
    </EmbeddedImage>
  </EmbeddedImages>
""" % logo_base64)

    partes.append("  <rd:ReportUnitType>Inch</rd:ReportUnitType>\n")
    partes.append("  <rd:ReportID>a1b2c3d4-0002-4000-8000-000000000002</rd:ReportID>\n")
    partes.append("</Report>\n")

    return "".join(partes)


if __name__ == "__main__":
    contenido = construir()
    os.makedirs(os.path.dirname(SALIDA), exist_ok=True)
    with open(SALIDA, "w", encoding="utf-8") as archivo:
        archivo.write(contenido)
    print("Generado:", SALIDA, "(%d bytes)" % len(contenido))
