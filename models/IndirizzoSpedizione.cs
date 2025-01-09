public class IndirizzoSpedizione
{
    public int Id { get; set; }
    public int UtenteId { get; set; } // Foreign Key
    public string Via { get; set; } = string.Empty;
    public string Citta { get; set; } = string.Empty;
    public string CAP { get; set; } = string.Empty;
    public string Provincia { get; set; } = string.Empty;
    public string Paese { get; set; } = "Italia"; // Valore predefinito
    public DateTime DataAggiunta { get; set; } = DateTime.Now;

    // Relazione con Utente
    public Utente Utente { get; set; } = null!;
}