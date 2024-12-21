using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using VMS.TPS.Common.VolumeModel;
using System.Windows.Forms;

namespace TBIDyn
{
    public class Paciente
    {
        //ExtraerDatos
        public string ID { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }

        public string Study_UID { get; set; }
        public string FOR_UID { get; set; }
        public string Serie_UID { get; set; }
        public string StructureSet_UID { get; set; }

        public double Dosis { get; set; } //dosis en Gy

        //ExtraerAnatomia
        public double Vol_body { get; set; }
        public double Vol_lungs { get; set; }
        public double Diam_origen { get; set; }
        public double z_cabeza { get; set; }
        public double z_lung_sup { get; set; }
        public double z_lung_inf { get; set; }
        public double z_rodilla { get; set; }
        public double z_pies { get; set; }

        public double med_1 { get; set; }
        public double sd_1 { get; set; }
        public double perc80_1 { get; set; }
        public double perc20_1 { get; set; }

        public double med_2 { get; set; }
        public double sd_2 { get; set; }
        public double perc80_2 { get; set; }
        public double perc20_2 { get; set; }

        public double med_3 { get; set; }
        public double sd_3 { get; set; }
        public double perc80_3 { get; set; }
        public double perc20_3 { get; set; }

        public double med_4 { get; set; }
        public double sd_4 { get; set; }
        public double perc80_4 { get; set; }
        public double perc20_4 { get; set; }

        //LlenarPredicciones o ExtraerFeatures
        public double gantry_pies { get; set; }
        public double gantry_rodilla { get; set; }
        public double gantry_lung_inf { get; set; }
        public double gantry_lung_sup { get; set; }
        public double gantry_cabeza { get; set; }

        public double long_arco_1 { get; set; }
        public double long_arco_2 { get; set; }
        public double long_arco_3 { get; set; }
        public double long_arco_4 { get; set; }

        public double um_por_gray_1 { get; set; }
        public double um_por_gray_2 { get; set; }
        public double um_por_gray_3 { get; set; }
        public double um_por_gray_4 { get; set; }

        public double um_por_gray_grado_1 { get; set; }
        public double um_por_gray_grado_2 { get; set; }
        public double um_por_gray_grado_3 { get; set; }
        public double um_por_gray_grado_4 { get; set; }


        public void AsignarMetricas(int indice, MetricasRegion metricaRegion)
        {
            GetType().GetProperty($"med_{indice + 1}").SetValue(this, metricaRegion.media);
            GetType().GetProperty($"sd_{indice + 1}").SetValue(this, metricaRegion.sd);
            GetType().GetProperty($"perc80_{indice + 1}").SetValue(this, metricaRegion.perc80);
            GetType().GetProperty($"perc20_{indice + 1}").SetValue(this, metricaRegion.perc20);
        }

        public void AsignarParametrosArco(int indice, Arco arco)
        {
            GetType().GetProperty($"long_arco_{indice + 1}").SetValue(this, arco.long_arco);
            GetType().GetProperty($"ums_por_gray_{indice + 1}").SetValue(this, arco.um_por_gray);
            GetType().GetProperty($"ums_por_gray_grado_{indice + 1}").SetValue(this, arco.ums_por_gray_grado);
        }

        public void PredecirParametros(StructureSet ss, Patient paciente, double dosisGy, double zRodilla, Dictionary<string,Modelo> modelos)
        {
            ExtraerDatos(ss, paciente, dosisGy);
            ExtraerAnatomia(ss, paciente, zRodilla);
            LlenarPredicciones(modelos);
        }
        public void PredecirParametros(Patient paciente, Course curso, Dictionary<string, Modelo> modelos)
        {
            ExtraerDatos(paciente, curso);
            ExtraerAnatomia(paciente, curso);
            LlenarPredicciones(modelos);
        }

        public void ExtraerFeatures(Patient paciente, Course curso)
        {
            ExtraerDatos(paciente, curso);
            ExtraerAnatomia(paciente, curso);
            ExtraerFeatures(curso);
        }

        public void ExtraerDatos(StructureSet ss, Patient paciente, double dosisGy)
        {
            ID = paciente.Id;
            Apellido = paciente.LastName;
            Nombre = paciente.FirstName;
            //var plan = curso.PlanSetups.First(p => p.Id.Contains("TBI Ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
            StructureSet_UID = ss.UID;
            Serie_UID = ss.Image.Series.UID;
            Study_UID = ss.Image.Series.Study.UID;
            FOR_UID = ss.Image.FOR;
            Dosis = dosisGy;
        }

        public void ExtraerDatos(Patient paciente, Course curso)
        {
            var plan = curso.PlanSetups.First(p => p.Id.Contains("TBI Ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
            StructureSet ss = plan.StructureSet;
            double Dosis = plan.TotalPrescribedDose.Dose / 100;
            ExtraerDatos(ss, paciente, Dosis);
        }

        public void ExtraerAnatomia(StructureSet ss, Patient paciente, double zRodilla)
        {
            VVector userOrigin = ss.Image.UserOrigin;
            Vol_body = ss.Structures.First(s => s.Id == "BODY").Volume;
            if (ss.Structures.Any(s => s.Id.ToLower() == "lungs" || s.Id.ToLower() == "pulmones"))
            {
                Vol_lungs = ss.Structures.First(s => s.Id.ToLower() == "lungs" || s.Id.ToLower() == "pulmones").Volume;
            }
            Diam_origen = Extracciones.DiamZOrigin(ss);
            var diametros = Extracciones.Diametros50Central(ss).Where(d => !double.IsNaN(d.Item1));
            var pulmones = Extracciones.InicioFinLungs(ss);
            z_cabeza = diametros.Last().Item2;
            z_pies = diametros.First().Item2;
            z_lung_inf = pulmones.Item1 - userOrigin.z;
            z_lung_sup = pulmones.Item2 - userOrigin.z;
            z_rodilla = zRodilla - userOrigin.z;

            List<List<double>> diametrosZonas = new List<List<double>>
            {
                diametros.Where(d => d.Item2 < z_rodilla).Select(d => d.Item1).ToList(),
                diametros.Where(d => d.Item2 > z_rodilla && d.Item2 < z_lung_inf).Select(d => d.Item1).ToList(),
                diametros.Where(d => d.Item2 > z_lung_inf && d.Item2 < z_lung_sup).Select(d => d.Item1).ToList(),
                diametros.Where(d => d.Item2 > z_lung_sup).Select(d => d.Item1).ToList()
            };

            for (int i = 0; i < 4; i++)
            {
                AsignarMetricas(i, Extracciones.MetricasDeLista(diametrosZonas[i]));
            }
        }

        public void ExtraerAnatomia(Patient paciente, Course curso)
        {
            var plan = curso.PlanSetups.First(p => p.Id.Contains("TBI Ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
            var ss = plan.StructureSet;
            ExtraerAnatomia(ss, paciente, Extracciones.ZRodilla(plan));
        }

        public void LlenarPredicciones(Dictionary<string,Modelo> modelos)
        {
            gantry_pies = PredecirValor(modelos, "gantry_inicio_1");
            gantry_rodilla = PredecirValor(modelos, "gantry_fin_1", "gantry_inicio_2");
            gantry_lung_inf = PredecirValor(modelos, "gantry_fin_2", "gantry_inicio_3");
            gantry_lung_sup = PredecirValor(modelos, "gantry_fin_3", "gantry_inicio_4");
            gantry_cabeza = PredecirValor(modelos, "gantry_fin_4");
            
            
            long_arco_1 = LongitudArco(gantry_pies, gantry_rodilla);
            long_arco_2 = LongitudArco(gantry_rodilla, gantry_lung_inf);
            long_arco_3 = LongitudArco(gantry_lung_inf, gantry_lung_sup);
            long_arco_4 = LongitudArco(gantry_lung_sup, gantry_cabeza);
            um_por_gray_grado_3 = PredecirValor(modelos, "ums_por_gray_grado_3");
            um_por_gray_3 = um_por_gray_grado_3 * long_arco_3;
            um_por_gray_grado_4 = PredecirValor(modelos, "ums_por_gray_grado_4");
            um_por_gray_4 = um_por_gray_grado_4 * long_arco_4;
            um_por_gray_grado_2 = PredecirValor(modelos, "ums_por_gray_grado_2");
            um_por_gray_2 = um_por_gray_grado_2 * long_arco_2;
            um_por_gray_grado_1 = PredecirValor(modelos, "ums_por_gray_grado_1");
            um_por_gray_1 = um_por_gray_grado_1 * long_arco_1;
        }

        public void ExtraerFeatures(Course curso)
        {
            if (curso.PlanSetups.Any(p => p.Id == "TBI Ant") && curso.PlanSetups.Any(p => p.Id == "TBI Post"))
            {
                PlanSetup planAnt = curso.PlanSetups.First(p => p.Id.Contains("TBI Ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
                VVector userOrigin = planAnt.StructureSet.Image.UserOrigin;
                PlanSetup planPost = curso.PlanSetups.First(p => p.Id.Contains("TBI Post") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
                List<Arco> arcos = Arco.extraerArcos(planAnt, planPost);
                for (int i = 0; i < 4; i++)
                {
                    AsignarParametrosArco(i, arcos[i]);
                }
                gantry_pies = arcos[0].gantry_inicio;
                gantry_rodilla = arcos[0].gantry_fin;
                gantry_lung_inf = arcos[1].gantry_fin;
                gantry_lung_sup = arcos[2].gantry_fin;
                gantry_cabeza = arcos[3].gantry_fin;

            }
        }

        public double PredecirValor(Dictionary<string, Modelo> modelos, string sModelo1, string sModelo2 = null)
        {
            double valor = double.NaN;
            var modelo1 = modelos.First(m => m.Key == sModelo1).Value;
            valor = modelo1.Predecir(modelo1.ObtenerFeatures(this));
            if (sModelo2 != null)
            {
                var modelo2 = modelos.First(m => m.Key == sModelo2).Value;
                valor = (valor + modelo2.Predecir(modelo2.ObtenerFeatures(this))) / 2;
            }
            return Math.Round(valor, 0);
        }

        public static double LongitudArco(double gantry_inicio, double gantry_fin)
        {
            if ((gantry_inicio > 180 && gantry_fin > 180) || (gantry_inicio < 180 && gantry_fin < 180))
            {
                return Math.Abs(gantry_fin - gantry_inicio);
            }
            else
            {
                return 360 - Math.Max(gantry_fin, gantry_inicio) + Math.Min(gantry_fin, gantry_inicio);
            }
        }



    }
}
