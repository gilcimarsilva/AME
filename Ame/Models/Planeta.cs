using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ame.Models
{
    public class Planeta
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string clima { get; set; }
        public string terreno { get; set; }
        public int qtdApareceuEmFilme { get; set; }
    }
}