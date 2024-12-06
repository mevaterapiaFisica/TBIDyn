using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBIDyn
{
    public class Paciente
    {
        public string ID { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }

        public string UID_CT { get; set; }
        public string UID_StructureSet { get; set; }


        public double Vol_body { get; set; }
        public double Vol_lungs { get; set; }
        public double Diam_en_origen { get; set; }
        public double z_cabeza { get; set; }
        public double z_lung_sup { get; set; }
        public double z_lung_inf { get; set; }
        public double z_rodilla { get; set; }
        public double z_pies { get; set; }

        public double 
    }
}
