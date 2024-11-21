using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FellowOakDicom.IO;
using FellowOakDicom;
using Ecl = VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using VMS.TPS.Common.VolumeModel;
//using

namespace TBIDyn
{
    public partial class Form1 : Form
    {
        public static string planTBI = @"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\TBI Ant.dcm";
        public static string planMLC = @"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\TBI_MLC.dcm";



        public Form1()
        {
            Ecl.Application app = Ecl.Application.CreateApplication("paberbuj", "123qwe");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<string> salidas = new List<string>();
            salidas.Add("media_1;desvest_1;perc20_1;perc80_1;media_2;desvest_2;perc20_2;perc80_2;media_3;desvest_3;perc20_3;perc80_3;media_4;desvest_4;perc20_4;perc80_4;Inicio_1;Fin_1;UM/grado_1;Inicio_2;Fin_2;UM/grado_2;Inicio_3;Fin_3;UM/grado_3;Inicio_4;Fin_4;UM/grado_4");
            var fid = File.ReadAllLines(@"\\ariamevadb-svr\va_data$\PlanHelper\Busquedas\Busqueda_21-11-2024_17_36_31");
            foreach (var linea in fid.Skip(1))
            {
                var lineaSplit = linea.Split(';');
                var paciente = app.OpenPatientById(lineaSplit[0]);
                var curso = paciente.Courses.First(c => c.Id == lineaSplit[3]);
                var plan = curso.PlanSetups.First(p => p.Id.Contains("TBI Ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
                ZRodilla(plan);
                salidas.Add(ExtraerFeatures(curso));
                app.ClosePatient();
            }
            File.WriteAllLines(@"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\salida.txt", salidas);

            var elap = sw.Elapsed;
            //DcmTBIDin();
            InitializeComponent();
        }



        public static List<string> Perfiles(Ecl.PlanSetup plan)
        {
            var body = plan.StructureSet.Structures.First(s => s.Id == "BODY");
            var cortes = plan.StructureSet.Image.Series.Images.Count() - 1;
            VVector userOrgin = plan.StructureSet.Image.UserOrigin;
            //List<VVector[][]> lista = new List<VVector[][]>();
            List<string> export = new List<string>();
            for (int i = 0; i < cortes; i++)
            {
                var corte = body.GetContoursOnImagePlane(i);
                //  lista.Add(corte);

                if (corte.Length > 0)
                {
                    VVector[] curva = corte.OrderBy(c => c.Length).Last();
                    //List<VVector> cruces = curva.Where(p => p.x*curva.prev).ToList();
                    string linea = "";
                    List<double> Ys = new List<double>();
                    for (int j = 1; j < curva.Length; j++)
                    {
                        if ((curva[j].x - userOrgin.x) * (curva[j - 1].x - userOrgin.x) < 0)
                        {
                            Ys.Add(curva[j].y);
                            if (linea == "")
                            {
                                linea += curva[j].z.ToString();
                            }
                        }
                    }
                    foreach (var y in Ys.OrderBy(y => y))
                    {
                        linea += ";" + y.ToString();
                    }

                    if (linea != "")
                    {
                        export.Add(linea);
                    }


                    /*if (i==170)
                    {
                        foreach (VVector vector in corte.First())
                        {
                            export.Add(vector.x + ";" + vector.y + ";" + vector.z);
                        }

                    }*/

                }
            }
            return export;
        }

        public static Tuple<double, double> InicioFinLungs(Ecl.PlanSetup plan)
        {
            if (!plan.StructureSet.Structures.Any(s => s.Id == "Lungs"))
            {
                var curso = plan.Course;
                var paciente = curso.Patient;
                var ss = plan.StructureSet.Structures;
                return null;
            }
            var lungs = plan.StructureSet.Structures.First(s => s.Id == "Lungs");
            var cortes = plan.StructureSet.Image.Series.Images.Count() - 1;
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
            if (double.IsNaN(inicio) || double.IsNaN(fin))
            {

            }
            return new Tuple<double, double>(inicio, fin);


        }

        public static double ZRodilla(Ecl.PlanSetup plan) //No funciona óptimo. Igual los planes no paran en rodilla
        {
            if (plan.Beams.Any(b => b.Id.Contains("ant1")))
            {
                double diamZorigin = DiamZOrigin(plan);
                VVector userOrgin = plan.StructureSet.Image.UserOrigin;
                double angGantry = 360 - plan.Beams.Where(b => b.Id.Contains("ant1")).First().ControlPoints.Select(c => c.GantryAngle).Max();
                double angGantryRad = (angGantry - 11.31) * Math.PI / 180; //11.31 es el angulo del hemicampo
                return Math.Tan(angGantryRad) * (1224 - diamZorigin / 2);
            }
            return double.NaN;
        }

        public static double DiamZOrigin(Ecl.PlanSetup plan)
        {
            var body = plan.StructureSet.Structures.First(s => s.Id == "BODY");
            var cortes = plan.StructureSet.Image.Series.Images.Count() - 1;
            VVector userOrgin = plan.StructureSet.Image.UserOrigin;
            for (int i = 0; i < cortes; i++)
            {
                var corte = body.GetContoursOnImagePlane(i);
                if (corte.Length > 0)
                {
                    VVector[] curva = corte.OrderBy(c => c.Length).Last();
                    if (Math.Round(curva.First().z, 2) == Math.Round(userOrgin.z, 2))
                    {

                        return Math.Abs(curva.OrderBy(c => c.y).First().y - curva.OrderBy(c => c.y).Last().y); //No es exacto pero es lo más simple
                    }
                }
            }
            return double.NaN;
        }
        public static List<string> Perfiles50Central(Ecl.PlanSetup plan, string estructura)
        {
            var body = plan.StructureSet.Structures.First(s => s.Id == estructura);
            //var ss = plan.StructureSet.Structures;
            var cortes = plan.StructureSet.Image.Series.Images.Count() - 1;
            VVector userOrgin = plan.StructureSet.Image.UserOrigin;
            //List<VVector[][]> lista = new List<VVector[][]>();
            List<string> export = new List<string>();
            for (int i = 0; i < cortes; i++)
            {
                var corte = body.GetContoursOnImagePlane(i);
                if (corte.Length > 0)
                {
                    VVector[] curva = corte.OrderBy(c => c.Length).Last();
                    if (HayBodyEnXOrigin(curva, userOrgin))
                    {
                        Tuple<double, double> promedios = PromediosCurva(curva);
                        export.Add(curva.First().z.ToString() + ";" + promedios.Item1.ToString() + ";" + promedios.Item2.ToString());
                    }
                    else
                    {
                        List<double> promediosSup = new List<double>();
                        List<double> promediosInf = new List<double>();
                        for (int j = 0; j < corte.Length; j++)
                        {
                            var curvaN = corte[j];
                            Tuple<double, double> promedios = PromediosCurva(curvaN);
                            promediosSup.Add(promedios.Item1);
                            promediosInf.Add(promedios.Item2);
                        }
                        export.Add(curva.First().z.ToString() + ";" + promediosSup.Average().ToString() + ";" + promediosInf.Average().ToString());
                    }
                }
            }
            return export;
        }

        public static List<Tuple<double, double>> Diametros50Central(Ecl.PlanSetup plan, string estructura)
        {
            var body = plan.StructureSet.Structures.First(s => s.Id == estructura);
            //var ss = plan.StructureSet.Structures;
            var cortes = plan.StructureSet.Image.Series.Images.Count() - 1;
            VVector userOrigin = plan.StructureSet.Image.UserOrigin;
            var limitesPulmon = InicioFinLungs(plan);
            var CurvaHU = Hu2Densidad.CurvaHU();
            //List<VVector[][]> lista = new List<VVector[][]>();
            //List<Tuple<double, double>> diametros = new List<Tuple<double, double>>();
            List<Tuple<double, double>> diametros50 = new List<Tuple<double, double>>();
            //List<Tuple<double, double>> diametrosCent = new List<Tuple<double, double>>();
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

        public static double WED(VVector punto1, VVector punto2, Ecl.Image ct, List<Hu2Densidad.PuntoCurva> CurvaHU)
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


        public static string ExtraerFeatures(Ecl.Course curso)
        {
            if (curso.PlanSetups.Any(p => p.Id == "TBI Ant") && curso.PlanSetups.Any(p => p.Id == "TBI Post"))
            {
                Ecl.PlanSetup planAnt = curso.PlanSetups.First(p => p.Id.Contains("TBI Ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
                VVector userOrigin = planAnt.StructureSet.Image.UserOrigin;
                Ecl.PlanSetup planPost = curso.PlanSetups.First(p => p.Id.Contains("TBI Post") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);

                var c = planAnt.Course;
                var pat = c.Patient.Id;
                var ss = planAnt.StructureSet;

                var sss = ss.Structures;
                if (planAnt.StructureSet.Structures.Count() == 0 || !planAnt.StructureSet.Structures.Any(s => s.Id == "Lungs") || planAnt.StructureSet.Structures.First(s => s.Id == "Lungs").Volume == 0)
                {
                    return "";
                }

                var diametros = Diametros50Central(planAnt, "BODY").Where(d => !double.IsNaN(d.Item1));
                var pulmones = InicioFinLungs(planAnt);
                var zCabeza = diametros.Last().Item2;
                var zPies = diametros.First().Item2;
                var zPulmonesInf = pulmones.Item1 - userOrigin.z;
                var zPulmonesSup = pulmones.Item2 - userOrigin.z;
                var zRodilla = -ZRodilla(planAnt);
                var diametrosZona1 = diametros.Where(d => d.Item2 < zRodilla).Select(d => d.Item1).ToList();
                var diametrosZona2 = diametros.Where(d => d.Item2 > zRodilla && d.Item2 < zPulmonesInf).Select(d => d.Item1).ToList();
                var diametrosZona3 = diametros.Where(d => d.Item2 > zPulmonesInf && d.Item2 < zPulmonesSup).Select(d => d.Item1).ToList();
                var diametrosZona4 = diametros.Where(d => d.Item2 > zPulmonesSup).Select(d => d.Item1).ToList();
                List<Arco> Arcos = Arco.extraerArcos(planAnt, planPost);
                string output = "";
                output += MetricasDeLista(diametrosZona1);
                output += MetricasDeLista(diametrosZona2);
                output += MetricasDeLista(diametrosZona3);
                output += MetricasDeLista(diametrosZona4);

                foreach (Arco ar in Arcos)
                {
                    output += ar.GantryInicio.ToString() + ";" + ar.GantryFin.ToString() + ";" + Math.Round(ar.UMporGy, 2).ToString() + ";";
                }
                return output;
            }
            else
            {
                return "";
            }
        }
        public static string MetricasDeLista(List<double> datos)
        {
            if (datos.Count == 0)
            {
                return "0;0;0;0";
            }

            double media = Math.Round(datos.Average(),3);
            double desvest = Math.Round(CalcularDesviacionEstandar(datos, media),3);
            double perc20 = Math.Round(CalcularPercentil(datos, 20),3);
            double perc80 = Math.Round(CalcularPercentil(datos, 80),3);

            return media.ToString() + ";" + desvest.ToString() + ";" + perc20.ToString() + ";" + perc80.ToString() + ";";
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

        public static Tuple<double, double> PromediosCurva(VVector[] curva)
        {
            double xmin = curva.OrderBy(c => c.x).First().x;
            double xmax = curva.OrderBy(c => c.x).Last().x;
            double dist = xmax - xmin;

            VVector[] porcionCentral = curva.Where(c => c.x > xmin && c.x < xmax).ToArray();
            double ymean = porcionCentral.Average(c => c.y);
            VVector[] centroSuperior = porcionCentral.Where(c => c.y > ymean).ToArray();
            VVector[] centroInferior = porcionCentral.Where(c => c.y < ymean).ToArray();
            double promedioSup = centroSuperior.Average(c => c.y);
            double promedioInf = centroInferior.Average(c => c.y);
            return new Tuple<double, double>(promedioSup, promedioInf);
        }
        public static void DcmTBIDin()
        {
            //var file = DicomFile.Open(planTBI);
            var file = DicomFile.Open(planMLC);
            DicomDataset ds = file.Dataset;
            Random rn = new Random();
            var instance = ds.GetSingleValue<string>(DicomTag.SOPInstanceUID) + rn.Next(100).ToString();
            ds.AddOrUpdate(DicomTag.SOPInstanceUID, instance);
            var BeamSequence = ds.GetSequence(DicomTag.BeamSequence);
            //var Beam1 = BeamSequence.First();
            var Beam1 = BeamSequence.Last();
            double[] CumMetWeights = new double[] { 0, 0.25, 0.5, 0.75, 1 };
            double[] CumGantryAngFx = new double[] { 0, 0.1, 0.3, 0.6, 1 };
            var cpSequence = Beam1.GetSequence(DicomTag.ControlPointSequence);

            var GantryInicial = cpSequence.First().GetSingleValue<double>(DicomTag.GantryAngle);
            var GantryFinal = cpSequence.Last().GetSingleValue<double>(DicomTag.GantryAngle);
            string rotationDir = cpSequence.First().GetSingleValue<string>(DicomTag.GantryRotationDirection);

            var MLCsAbiertos = Beam1.GetSequence(DicomTag.ControlPointSequence).First().GetSequence(DicomTag.BeamLimitingDevicePositionSequence).Last().GetValues<double>(DicomTag.LeafJawPositions);
            /*for (int i=0;i<3;i++)
            {
                DicomDataset cp = new DicomDataset();
                cp.AddOrUpdate(DicomTag.ControlPointIndex, i + 2);
                DicomSequence rdr1 = new DicomSequence(DicomTag.ReferencedDoseReferenceSequence);
                rdr1.Items.Add(new DicomDataset());
                cp.Add(rdr1);
                //cp.InternalTransferSyntax
                //cpSequence.Last().CopyTo(cp);
                //cpSequence.Items.Insert(i + 1, cp);
                cpSequence.Items.Add(cp);
            }*/

            foreach (DicomDataset cp in cpSequence)
            {
                int index = cp.GetSingleValue<int>(DicomTag.ControlPointIndex);
                //cp.AddOrUpdate(DicomTag.ControlPointIndex, index);
                if (index != cpSequence.Count() - 1)//no es el ultimo
                {
                    cp.AddOrUpdate(DicomTag.GantryRotationDirection, rotationDir);
                }
                cp.AddOrUpdate(DicomTag.CumulativeMetersetWeight, CumMetWeights[index]);
                cp.AddOrUpdate(DicomTag.GantryAngle, GantryInicial + CumGantryAngFx[index] * (GantryFinal - GantryInicial));
                //cp.GetSequence(DicomTag.BeamLimitingDevicePositionSequence).Last().AddOrUpdate(DicomTag.LeafJawPositions,MLCsAbiertos);
                DicomSequence rdrs = new DicomSequence(DicomTag.ReferencedDoseReferenceSequence);
                rdrs.Items.Add(new DicomDataset());
                cp.Add(rdrs);
                var RDR1 = rdrs.First();
                RDR1.AddOrUpdate(DicomTag.CumulativeDoseReferenceCoefficient, CumMetWeights[index]);
                RDR1.AddOrUpdate(DicomTag.ReferencedDoseReferenceNumber, 1);
            }
            //Beam1.AddOrUpdate(DicomTag.NumberOfControlPoints, cpSequence.Count());
            file.Save(@"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\out.dcm");
        }

        public class Arco
        {
            public string Nombre;
            public double GantryInicio;
            public double GantryFin;
            public double UMporGy;

            public Arco(Ecl.PlanSetup plan, string nombre)
            {
                List<Ecl.Beam> arcos = plan.Beams.Where(b => b.Id.ToLower().Contains(nombre.ToLower())).ToList();
                if (arcos == null || arcos.Count == 0)
                {
                    return;
                }
                Nombre = nombre;
                GantryInicio = arcos.First().ControlPoints.First().GantryAngle;
                GantryFin = arcos.First().ControlPoints.Last().GantryAngle;
                foreach (var arco in arcos)
                {
                    if ((arco.ControlPoints.First().GantryAngle == GantryInicio || arco.ControlPoints.First().GantryAngle == GantryFin) && (arco.ControlPoints.Last().GantryAngle == GantryInicio || arco.ControlPoints.Last().GantryAngle == GantryFin))
                    {
                        UMporGy += arco.Meterset.Value;
                    }
                }
                UMporGy = UMporGy / (plan.UniqueFractionation.PrescribedDosePerFraction.Dose / 100);
            }
            public override string ToString()
            {
                return Nombre + "-" + GantryInicio.ToString() + "-" + GantryFin.ToString() + "-" + UMporGy.ToString();
            }

            public static List<Arco> extraerArcos(Ecl.PlanSetup planAnt, Ecl.PlanSetup planPost)
            {
                List<Arco> arcos = new List<Arco>();
                string[] nombresAnt = new string[] { "ant1", "ant2", "ant3", "ant4" };
                string[] nombresPost = new string[] { "post1", "post2", "post3", "post4" };
                foreach (string nombreAnt in nombresAnt)
                {
                    arcos.Add(new Arco(planAnt, nombreAnt));
                }
                foreach (string nombrePost in nombresPost)
                {
                    arcos.Add(new Arco(planPost, nombrePost));
                }
                return arcos;
            }
        }
    }
}
