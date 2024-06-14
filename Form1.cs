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
            /*var paciente = app.OpenPatientById("1-107087-0");
            var curso = paciente.Courses.First(c => c.Id.Contains("C0"));
            var planAnt = curso.PlanSetups.First(p => p.Id == "TBI Ant");
            var planPost = curso.PlanSetups.First(p => p.Id == "TBI Post");
            var arcos = Arco.extraerArcos(planAnt, planPost);
            //var export = Perfiles(plan);
            var pulmones = InicioFinLungs(planAnt);
            var diams = Diametros50Central(planAnt, "BODY");
            var perfiles = Perfiles50Central(planAnt, "BODY");
            File.WriteAllLines(@"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\coords.txt", perfiles.ToArray());*/
            /*var exportL = Perfiles50Central(plan, "Lungs");
            File.WriteAllLines(@"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\coordsL.txt", exportL.ToArray());*/
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<string> salidas = new List<string>();
            var fid = File.ReadAllLines(@"\\ariamevadb-svr\va_data$\PlanHelper\Busquedas\TBIs.txt");
            foreach (var linea in fid.Skip(1))
            {
                var lineaSplit = linea.Split(';');
                var paciente = app.OpenPatientById(lineaSplit[0]);
                var curso = paciente.Courses.First(c => c.Id == lineaSplit[3]);
                salidas.Add(ExtraerFeatures(curso));
                app.ClosePatient();
            }
            File.WriteAllLines(@"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\salida.txt", salidas);
            
            var elap = sw.Elapsed;
            DcmTBIDin();
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
            return new Tuple<double, double>(inicio, fin);


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

        public static List<double> Diametros50Central(Ecl.PlanSetup plan, string estructura)
        {
            var body = plan.StructureSet.Structures.First(s => s.Id == estructura);
            //var ss = plan.StructureSet.Structures;
            var cortes = plan.StructureSet.Image.Series.Images.Count() - 1;
            VVector userOrgin = plan.StructureSet.Image.UserOrigin;
            var limitesPulmon = InicioFinLungs(plan);
            //List<VVector[][]> lista = new List<VVector[][]>();
            List<Tuple<double, double>> diametros = new List<Tuple<double, double>>();
            for (int i = 0; i < cortes; i++)
            {
                var corte = body.GetContoursOnImagePlane(i);
                if (corte.Length > 0)
                {
                    VVector[] curva = corte.OrderBy(c => c.Length).Last();
                    if (HayBodyEnXOrigin(curva, userOrgin))
                    {
                        Tuple<double, double> promedios = PromediosCurva(curva);
                        //export.Add(curva.First().z.ToString() + ";" + promedios.Item1.ToString() + ";" + promedios.Item2.ToString());
                        diametros.Add(new Tuple<double,double>(promedios.Item1 - promedios.Item2, curva.First().z));
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
                        diametros.Add(new Tuple<double, double>(promediosSup.Average()- promediosInf.Average(), curva.First().z));
                    }
                }
            }
            double zInf = diametros.OrderBy(d => d.Item2).First().Item2;
            double diam1 = diametros.Where(d => d.Item2 > limitesPulmon.Item2).Average(d=>d.Item1);
            double diam2 = diametros.Where(d => d.Item2 < limitesPulmon.Item2 && d.Item2> limitesPulmon.Item1).Average(d => d.Item1);
            double diam3 = diametros.Where(d => d.Item2 <limitesPulmon.Item1 && d.Item2>userOrgin.z).Average(d => d.Item1);
            double ultimaMitad = (userOrgin.z - zInf) / 2;
            double diam4 = diametros.Where(d => d.Item2 < userOrgin.z && d.Item2> userOrgin.z-ultimaMitad).Average(d => d.Item1);
            double diam5 = diametros.Where(d => d.Item2 < userOrgin.z - ultimaMitad).Average(d => d.Item1);
            List<double> diams = new List<double>();
            diams.Add(diam1);
            diams.Add(diam2);
            diams.Add(diam3);
            diams.Add(diam4);
            diams.Add(diam5);
            return diams;
        }

        public static string ExtraerFeatures(Ecl.Course curso)
        {
            if (curso.PlanSetups.Any(p=>p.Id =="TBI Ant") && curso.PlanSetups.Any(p => p.Id == "TBI Post"))
            {
                Ecl.PlanSetup planAnt = curso.PlanSetups.First(p => p.Id.Contains("TBI Ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
                Ecl.PlanSetup planPost = curso.PlanSetups.First(p => p.Id.Contains("TBI Post") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
                if (planAnt.StructureSet.Structures.Any(s => s.Id == "Lungs"))
                {
                    return "";
                }
                var diametros = Diametros50Central(planAnt, "BODY");
                List<Arco> Arcos = Arco.extraerArcos(planAnt, planPost);
                string output = "";
                foreach (double diametro in diametros)
                {
                    output += Math.Round(diametro, 3).ToString() + ";";
                }
                foreach (Arco ar in Arcos)
                {
                    output+= ar.Nombre + ";"  + ar.GantryInicio.ToString() + ";" + ar.GantryFin.ToString() + ";" + Math.Round(ar.UMporGy, 2).ToString() +";";
                }
                return output;
            }
            else
            {
                return "";
            }
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
                if (arcos==null || arcos.Count==0)
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
                UMporGy = UMporGy / (plan.UniqueFractionation.PrescribedDosePerFraction.Dose/100);
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
                    arcos.Add(new Arco(planPost,nombrePost));
                }
                return arcos;
            }
        }
    }
}
