using SMRDominio.ClassePessoa;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SMRDominio.ClasseRecompensa
{
    [Table("recompensa")]
    public class Recompensa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int id_empresa { get; set; }

        public string? titulo { get; set; }
        public string? descricao { get; set; }
        public int pontos_necessarios { get; set; }
        public bool Ativo { get; set; }

        [JsonIgnore]
        [ForeignKey("id_empresa")] 
        public Empresa? Empresa { get; set; }
    }
}
