namespace SMRDominio.ClasseIndicacao
{
    public class Indicacao
    {
        public int Id { get; set; }

        public int Id_Promotor_Indicou { get; set; }

        public int Id_Bonificacao { get; set; }

        public string? Nome_Indicado { get; set; }

        public string? Telefone_Indicado { get; set; }

        public string? Status_Indicacao { get; set; }

        public DateTime Data_Indicacao { get; set; }

        public DateTime? Data_Validacao { get; set; }
    }
}