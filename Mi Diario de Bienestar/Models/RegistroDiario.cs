using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mi_Diario_de_Bienestar.Models
{
        public class RegistroDiario
        {

            public int Id { get; set; }
            public DateTime Fecha { get; set; }

            public string Descripcion { get; set; } = string.Empty;

            public int ActividadFisica { get; set; }

            public int Energia { get; set; }

            public string FechaIso => Fecha.ToString("yyyy-MM-dd");
        }
    }
