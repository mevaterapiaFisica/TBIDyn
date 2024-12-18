using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using VMS.TPS.Common.VolumeModel;


namespace TBIDyn
{

    
    public static class Extracciones
    {
        #region perfiles
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

        private static Tuple<double, double> diametros50Curva(VVector[] curva, VVector userOrigin, Ecl.Image ct, List<Hu2Densidad.PuntoCurva> CurvaHU)
        {
            double xmin = curva.OrderBy(c => c.x).First().x;
            double xmax = curva.OrderBy(c => c.x).Last().x;
            double longitud = Math.Abs(xmax - xmin);
            double centroMenos50 = xmin + longitud / 4;
            double centroMas50 = xmin + 3 * longitud / 4;


            VVector[] curvaR = curva.Where(c => c.x > centroMenos50 && c.x < centroMas50).Select(c => new VVector(Math.Round(c.x, 2), Math.Round(c.y, 2), c.z)).ToArray();

            var agrupados = curvaR.GroupBy(c => c.x).Distinct().ToList();
            List<double> listDiams = new List<double>();
            foreach (var agrupado in agrupados)
            {
                if (agrupado.Count() == 2)
                {
                    listDiams.Add(WED(agrupado.ElementAt(0), agrupado.ElementAt(1), ct, CurvaHU));
                    //listDiams.Add(Math.Round(Math.Abs(agrupado.ElementAt(0).y - agrupado.ElementAt(1).y)));
                }
            }
            if (listDiams.Count > 0)
            {
                return new Tuple<double, double>(Math.Round(listDiams.Average(), 3), Math.Round(curva.First().z - userOrigin.z, 1));
            }
            return new Tuple<double, double>(double.NaN, double.NaN);
        }

        public static List<Tuple<double, double>> Diametros50Central(Ecl.PlanSetup plan)
        {
            var body = plan.StructureSet.Structures.First(s => s.Id == "BODY");
            //var ss = plan.StructureSet.Structures;
            var cortes = plan.StructureSet.Image.Series.Images.Count() - 1;
            VVector userOrigin = plan.StructureSet.Image.UserOrigin;
            var limitesPulmon = InicioFinLungs(plan);
            var CurvaHU = Hu2Densidad.CurvaHU();
            List<Tuple<double, double>> diametros50 = new List<Tuple<double, double>>();
            
            for (int i = 0; i < cortes; i++)
            {
                var corte = body.GetContoursOnImagePlane(i);
                if (corte.Length > 0)
                {
                    VVector[] curva = corte.OrderBy(c => c.Length).Last();
                    if (HayBodyEnXOrigin(curva, userOrigin))
                    {
                        diametros50.Add(diametros50Curva(curva, userOrigin, plan.StructureSet.Image, CurvaHU));
                    }
                    else
                    {
                        List<double> diamsCorte = new List<double>();
                        for (int j = 0; j < corte.Length; j++)
                        {
                            var curvaN = corte[j];
                            var diam = diametros50Curva(curvaN, userOrigin, plan.StructureSet.Image, CurvaHU);
                            diamsCorte.Add(diam.Item1);
                        }
                        diametros50.Add(new Tuple<double, double>(Math.Round(diamsCorte.Average(), 3), Math.Round(curva.First().z - userOrigin.z, 0)));
                    }
                }
            }
            return diametros50;
        }
    }
