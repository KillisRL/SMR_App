using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace SMRInfraestrutura
{
    public  class SMRDBContext : DbContext
    {
        public readonly IConfiguration _configuracao;
    
        public DbSet<SMRDominio.ClassePessoa.Pessoa> Pessoa {  get; set; }
        public DbSet<SMRDominio.ClassePessoa.Promotor> Promotor { get; set; }
        public DbSet<SMRDominio.ClassePessoa.Empresa> Empresa { get; set; }
        public DbSet<SMRDominio.ClassePessoa.ClienteImportado> ClientesImportados { get; set; }
        public DbSet<SMRDominio.ClasseRecompensa.Recompensa> Recompensas { get; set; }
        public DbSet<SMRDominio.ClasseBonificacao.Bonificacao> Bonificacoes { get; set; }
        public DbSet<SMRDominio.ClasseIndicacao.Indicacao> Indicacao { get; set; }
        public DbSet<SMRDominio.ClassePessoa.PromotorPontos> PromotorPontos { get; set; }
        public SMRDBContext(IConfiguration configuracao, DbContextOptions<SMRDBContext> options): base(options)
        {
            _configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

         
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var typeDatabase = _configuracao["TypeDatabase"];
            var connectionString = _configuracao.GetConnectionString(typeDatabase);

            if (typeDatabase == "SqlServer")
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
            else if (typeDatabase == "MariaDb") // Dica: garanta que a string bata com o "MariaDb" do seu appsettings.json (case-sensitive em alguns contextos)
            {
                // Descomentado e configurado para o driver Pomelo (MySQL/MariaDB)
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }
    }
}
