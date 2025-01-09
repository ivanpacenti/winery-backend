public class MetodoPagamento
{
    public int Id { get; set; }
    public int UtenteId { get; set; } // Foreign Key
    public string Tipo { get; set; } = string.Empty; // Es: "Carta di Credito", "PayPal"
    public string Dettagli { get; set; } = string.Empty; // Es: ultime 4 cifre
    public DateTime DataAggiunta { get; set; } = DateTime.Now;

    // Relazione con Utente
    public Utente Utente { get; set; } = null!;
}