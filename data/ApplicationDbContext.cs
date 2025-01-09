using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Vino> Vini { get; set; }
    public DbSet<Vendita> Vendite { get; set; }
    public DbSet<Utente> Utenti { get; set; }
    public DbSet<MetodoPagamento> MetodiPagamento { get; set; }
    public DbSet<IndirizzoSpedizione> IndirizziSpedizione { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configura il tipo di colonna per Prezzo
        modelBuilder.Entity<Vino>()
            .Property(v => v.Prezzo)
            .HasPrecision(18, 2); // Precisione: 18 cifre, Scala: 2 decimali
    }
}