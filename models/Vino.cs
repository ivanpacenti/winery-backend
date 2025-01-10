public class Vino
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty; // Es: "Rosso", "Bianco"
    public decimal Prezzo { get; set; }
    public int Scorte { get; set; }
    public DateTime DataAggiunta { get; set; } = DateTime.Now;
    public string ImmagineUrl { get; set; } = string.Empty; // URL dell'immagine


    // Relazione con Vendite
    public List<Vendita> Vendite { get; set; } = new();
}