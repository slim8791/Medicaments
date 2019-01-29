using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Medicaments.Models
{
    public class Medicament
    {
        public int MedicamentId { get; set; }
        public string CodeProduit { get; set; }
        public string Fournisseur { get; set; }
        public string DCI1 { get; set; }
        public string DCI2 { get; set; }
        public string AMM { get; set; }
        public string dateamm { get; set; }
        public string Designation { get; set; }
        public string Prix { get; set; }
        public string Nom { get; set; }
    }
}