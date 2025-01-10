public class Utente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Hash della password
    public DateTime DataRegistrazione { get; set; } = DateTime.Now;
    public int Ruolo { get; set; } // "Administrator:3", "Utente:2",


    // Relazione con Metodi di Pagamento
    public List<MetodoPagamento> MetodiPagamento { get; set; } = new();

    // Relazione con Indirizzi di Spedizione
    public List<IndirizzoSpedizione> IndirizziSpedizione { get; set; } = new();

    // Relazione con Vendite
    public List<Vendita> Vendite { get; set; } = new();
}