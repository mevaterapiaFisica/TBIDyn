using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
//using VMS.TPS.Common.VolumeModel;


namespace TBIDyn
{
    public static class Extracciones
    {
        #region info de StructureSet
        public static double WED(VVector punto1, VVector punto2, Image ct, List<Hu2Densidad.PuntoCurva> CurvaHU)
        {
            int longitud = Convert.ToInt32(Math.Abs(punto1.y - punto2.y)) + 1; //cantidad de puntos son mm+1 o sea que tengo tantos segmentos como mm
            if (longitud == 2)
            {
                return 1;
            }
            double[] lineaCT = new double[longitud];

            ct.GetImageProfile(punto1, punto2, lineaCT);
            return Hu2Densidad.CalcularWEDLinea(lineaCT, CurvaHU);
        }

        private static Tuple<double, double,double> diametros50Curva(VVector[] curva, VVector userOrigin, Image ct, List<Hu2Densidad.PuntoCurva> CurvaHU)
        {
            double xmin = curva.OrderBy(c => c.x).First().x;
            double xmax = curva.OrderBy(c => c.x).Last().x;
            double longitud = Math.Abs(xmax - xmin);
            double centroMenos50 = xmin + longitud / 4;
            double centroMas50 = xmin + 3 * longitud / 4;

            VVector[] curvaR = curva.Where(c => c.x > centroMenos50 && c.x < centroMas50).Select(c => new VVector(Math.Round(c.x, 2), Math.Round(c.y, 2), c.z)).ToArray();

            /*if (curvaR.Count() > 50)
            {

            }*/

            var agrupados = curvaR.GroupBy(c => c.x).Distinct().ToList();
            List<double> listDiams = new List<double>();
            List<double> listAsim = new List<double>();
            foreach (var agrupado in agrupados)
            {
                if (agrupado.Count() == 2)
                {
                    listDiams.Add(WED(agrupado.ElementAt(0), agrupado.ElementAt(1), ct, CurvaHU));
                    listAsim.Add((agrupado.Select(a => a.y).Average() - userOrigin.y));
                }
            }
            if (listDiams.Count > 0)
            {
                return new Tuple<double, double,double>(Math.Round(listDiams.Average(), 3), Math.Round(curva.First().z - userOrigin.z, 1), Math.Round(listDiams.Average(), 3));
            }
            return new Tuple<double, double, double>(double.NaN, double.NaN,double.NaN);
        }

        public static List<Tuple<double, double,double>> Diametros50Central(StructureSet ss)
        {
            var body = ss.Structures.First(s => s.Id.ToUpper() == "BODY");
            var cortes = ss.Image.Series.Images.Count() - 1;
            VVector userOrigin = ss.Image.UserOrigin;
            var limitesPulmon = InicioFinLungs(ss);
            var CurvaHU = Hu2Densidad.CurvaHU();
            List<Tuple<double, double,double>> diametros50 = new List<Tuple<double, double,double>>();

            for (int i = 0; i < cortes; i++)
            {
                var corte = body.GetContoursOnImagePlane(i);
                if (corte.Length > 0)
                {
                    VVector[] curva = corte.OrderBy(c => c.Length).Last();
                    if (HayBodyEnXOrigin(curva, userOrigin))
                    {
                        diametros50.Add(diametros50Curva(curva, userOrigin, ss.Image, CurvaHU));
                    }
                    else
                    {
                        List<double> diamsCorte = new List<double>();
                        List<double> asimCorte = new List<double>();
                        for (int j = 0; j < corte.Length; j++)
                        {
                            var curvaN = corte[j];
                            var diam = diametros50Curva(curvaN, userOrigin, ss.Image, CurvaHU);
                            diamsCorte.Add(diam.Item1);
                            asimCorte.Add(diam.Item3);
                        }
                        diametros50.Add(new Tuple<double, double, double>(Math.Round(diamsCorte.Average(), 3), Math.Round(curva.First().z - userOrigin.z, 0),Math.Round(asimCorte.Average(),3)));
                    }
                }
            }
            return diametros50;
        }

        public static Tuple<double, double> InicioFinLungs(StructureSet ss)
        {
            if (!ss.Structures.Any(s => s.Id.ToLower().Contains("lung") || s.Id.ToLower().Contains("pulmon")))
            {
                return null;
            }
            var lungs = ss.Structures.First(s => s.Id.ToLower().Contains("lung") || s.Id.ToLower().Contains("pulmon"));
            var cortes = ss.Image.Series.Images.Count() - 1;
            double inicio = double.NaN;
            double fin = double.NaN;

            for (int i = 0; i < cortes; i++)
            {
                var corte = lungs.GetContoursOnImagePlane(i);
                if (corte.Length > 0)
                {
                    if (double.IsNaN(inicio))
                    {
                        inicio = corte.First().First().z;
                    }
                    if (double.IsNaN(fin) || corte.First().First().z > fin)
                    {
                        fin = corte.First().First().z;
                    }
                }
            }
            return new Tuple<double, double>(inicio, fin);
        }



        public static bool HayBodyEnXOrigin(VVector[] curva, VVector userOrigin)
        {
            for (int j = 1; j < curva.Length; j++)
            {
                if ((curva[j].x - userOrigin.x) * (curva[j - 1].x - userOrigin.x) < 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static double DiamZOrigin(StructureSet ss)
        {
            if (!ss.Structures.Any(s => s.Id.ToUpper() == "BODY"))
            {
                return double.NaN;
            }
            var body = ss.Structures.First(s => s.Id.ToUpper() == "BODY");
            var cortes = ss.Image.Series.Images.Count() - 1;
            VVector userOrgin = ss.Image.UserOrigin;
            for (int i = 0; i < cortes; i++)
            {
                var corte = body.GetContoursOnImagePlane(i);
                if (corte.Length > 0)
                {
                    VVector[] curva = corte.OrderBy(c => c.Length).Last();
                    if (Math.Round(curva.First().z, 1) == Math.Round(userOrgin.z, 1))
                    {
                        //double opc1 = Math.Abs(curva.OrderBy(c => c.y).First().y - curva.OrderBy(c => c.y).Last().y); //No es exacto pero es lo más simple
                        //double opc2 = Math.Abs(curva.OrderBy(c => c.y).First().y - userOrgin.y) * 2;
                        //return Math.Abs(curva.OrderBy(c => c.y).First().y - curva.OrderBy(c => c.y).Last().y); //No es exacto pero es lo más simple
                        return Math.Abs(curva.OrderBy(c => c.y).First().y - userOrgin.y) * 2;
                    }
                }
            }
            return double.NaN;
        }

        #endregion

        #region info de Plan
        public static double ZRodilla(PlanSetup plan) 
        {
            if (plan.Beams.Any(b => b.Id.Contains("ant1")))
            {
                double diamZorigin = DiamZOrigin(plan.StructureSet);
                VVector userOrgin = plan.StructureSet.Image.UserOrigin;
                double alpha = 360 - plan.Beams.Where(b => b.Id.Contains("ant1")).First().ControlPoints.Select(c => c.GantryAngle).Max(); // angulo de gantry 
                /*double angGantry = 360 - plan.Beams.Where(b => b.Id.Contains("ant1")).First().ControlPoints.Select(c => c.GantryAngle).Max();
                double angGantryRad = (angGantry - 11.31) * Math.PI / 180; //11.31 es el angulo del hemicampo
                

                double beta = 11.31;//Math.Atan(20 / 100); //angulo hemicampo
                double gamma = alpha - beta;
                double X = Math.Tan(alpha * Math.PI / 180)*448;
                double zrodilla2 = Math.Tan(gamma*Math.PI/180)*(1224-diamZorigin/2)- userOrgin.z;*/


                //version chatgpt
                double d = 1224 - diamZorigin / 2; //Distancia iso camilla - diamZOrigin/2. Distancia iso centro del paciente
                double f_w = 200 * (1 + d / 1000); // 1000 por la distancia iso. Acá reescalo el tamaño de campo
                double zrodilla3 = f_w * Math.Cos(alpha * Math.PI / 180) + d * Math.Sin(alpha*Math.PI / 180)-userOrgin.z;
                string nombre = plan.Course.Patient.LastName + "; " + plan.Course.Patient.FirstName;
                //return -Math.Tan(angGantryRad) * (1224 - diamZorigin / 2);
                return zrodilla3;

            }
            return double.NaN;
        }



        #endregion

        #region metodos auxiliares

        public static MetricasRegion MetricasDeLista(List<double> diametros,List<double> asimetrias)
        {
            MetricasRegion metricas = new MetricasRegion();
            if (diametros.Count == 0)
            {
                metricas.media = double.NaN;
                metricas.perc20 = double.NaN;
                metricas.perc80 = double.NaN;
                metricas.sd = double.NaN;
                metricas.asim = double.NaN;
                return metricas;
            }

            metricas.asim = Math.Round(asimetrias.Average(), 3);
            metricas.media = Math.Round(diametros.Average(), 3);
            metricas.sd = Math.Round(CalcularDesviacionEstandar(diametros, metricas.media), 3);
            metricas.perc20 = Math.Round(CalcularPercentil(diametros, 20), 3);
            metricas.perc80 = Math.Round(CalcularPercentil(diametros, 80), 3);

            if (metricas.media == double.NaN)
            {

            }
            return metricas;
        }


        public static double CalcularDesviacionEstandar(List<double> datos, double media)
        {
            double sumaCuadrados = datos.Sum(x => Math.Pow(x - media, 2));
            return Math.Sqrt(sumaCuadrados / datos.Count);
        }

        public static double CalcularPercentil(List<double> datos, double percentil)
        {
            var datosOrdenados = datos.OrderBy(x => x).ToList();
            int n = datosOrdenados.Count;
            double posicion = (percentil / 100.0) * (n - 1);
            int abajo = (int)Math.Floor(posicion);
            int arriba = (int)Math.Ceiling(posicion);

            if (abajo == arriba)
            {
                return datosOrdenados[abajo];
            }

            double interpolacion = posicion - abajo;
            return datosOrdenados[abajo] + interpolacion * (datosOrdenados[arriba] - datosOrdenados[abajo]);
        }
        #endregion

    }
}