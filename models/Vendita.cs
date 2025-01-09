public class Vendita
{
    public int Id { get; set; }
    public int VinoId { get; set; } // Foreign Key
    public int UtenteId { get; set; } // Foreign Key
    public int Quantita { get; set; }
    public DateTime DataVendita { get; set; } = DateTime.Now;

    // Relazioni
    public Vino Vino { get; set; } = null!;
    public Utente Utente { get; set; } = null!;
}