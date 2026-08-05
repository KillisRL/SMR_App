using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMRDominio.ClassePessoa
{
    [Table("cliente_importado")]
    public class ClienteImportado
    {
        [Key]
        [Column("id_cliente_importado")]
        public int IdClienteImportado { get; set; }

        [Column("id_empresa")]
        public int IdEmpresa { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("documento")]
        public string Documento { get; set; }

        [Column("data_importacao")]
        public DateTime DataImportacao { get; set; } = DateTime.Now;
    }
}