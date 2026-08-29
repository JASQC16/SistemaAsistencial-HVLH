namespace Hospital.Presentacion
{
    /// <summary>
    /// Par valor–texto para los combos que no se alimentan de una entidad del dominio
    /// (estados, tipos de documento, opciones "Todos"). Evita tener que interpretar el
    /// texto visible para saber qué eligió el usuario, que es frágil en cuanto alguien
    /// cambia una etiqueta.
    /// </summary>
    public class ElementoLista
    {
        public ElementoLista(string valor, string texto)
        {
            Valor = valor;
            Texto = texto;
        }

        /// <summary>Valor que viaja a la capa de negocio. null representa "sin filtro".</summary>
        public string Valor { get; private set; }

        public string Texto { get; private set; }

        public override string ToString()
        {
            return Texto;
        }
    }
}
