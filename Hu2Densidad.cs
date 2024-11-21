using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBIDyn
{
    public static class Hu2Densidad
    {
        //public static List<PuntoCurva> Curva = new List<PuntoCurva>();


        public static List<PuntoCurva> CurvaHU()
        {
            List<PuntoCurva> Curva = new List<PuntoCurva>();

            Curva.Add(new PuntoCurva(-1050, 0));
            Curva.Add(new PuntoCurva(-1000, 0));
            Curva.Add(new PuntoCurva(-792.51, 0.207));
            Curva.Add(new PuntoCurva(-70.94, 0.949));
            Curva.Add(new PuntoCurva(-12.91, 1));
            Curva.Add(new PuntoCurva(42.19, 1.042));
            Curva.Add(new PuntoCurva(768.96, 1.506));
            Curva.Add(new PuntoCurva(3000, 2.93));

            return Curva;
        }


        public class PuntoCurva
        {
            public double HU { get; set; }
            public double DensidadRel { get; set; }

            public PuntoCurva(double _HU, double _densidadRel)
            {
                HU = _HU;
                DensidadRel = _densidadRel;
            }
        }

        public static double ConvertirAHU(double HU, List<PuntoCurva> Curva)
        {
            if (HU > 3000)
            {
                return 2.93;
            }

            else if (Curva.Any(p => p.HU == HU))
            {
                return Curva.First(p => p.HU == HU).DensidadRel;
            }
            else
            {
                PuntoCurva puntoAnt = Curva.Last(p => p.HU < HU);
                PuntoCurva puntoPost = Curva.First(p => p.HU > HU);
                return (puntoPost.DensidadRel - puntoAnt.DensidadRel) / (puntoPost.HU - puntoAnt.HU) * (HU - puntoAnt.HU) + puntoAnt.DensidadRel;
            }
        }
        public static double CalcularWEDsegumento(double valor1, double valor2, List<PuntoCurva> Curva) //los puntos están espaciados 1mm aprox así que promedio valor CT entre dos puntos, convierto a densidad relativa y lo tengo en mm
        {
            return ConvertirAHU((valor1 + valor2) / 2, Curva);
        }

        public static double CalcularWEDLinea(double[] linea, List<PuntoCurva> Curva) //los puntos están espaciados 1mm aprox así que promedio valor CT entre dos puntos, convierto a densidad relativa y lo tengo en mm
        {
            double WED = 0;
            for (int i = 0; i < linea.Length - 1; i++)
            {
                WED += CalcularWEDsegumento(linea[i], linea[i + 1], Curva);
            }
            return WED;
        }
    }
}
