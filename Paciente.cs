using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ecl = VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using VMS.TPS.Common.VolumeModel;

namespace TBIDyn
{
    public class Paciente
    {
        public string ID { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }

        public string Study_UID { get; set; }
        public string FOR_UID { get; set; }
        public string Serie_UID { get; set; }
        public string StructureSet_UID { get; set; }

        public double Dosis { get; set; } //dosis en Gy

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

        public double um_por_gray_1 { get; set; }
        public double um_por_gray_2 { get; set; }
        public double um_por_gray_3 { get; set; }
        public double um_por_gray_4 { get; set; }
        public double long_arco_1 { get; set; }
        public double long_arco_2 { get; set; }
        public double long_arco_3 { get; set; }
        public double long_arco_4 { get; set; }

        public double arco1_ums { get; set; }
        public double arco2_ums { get; set; }
        public double arco3_ums { get; set; }
        public double arco4_ums { get; set; }
        public double gantry_pies { get; set; }
        public double gantry_rodilla { get; set; }
        public double gantry_lung_inf { get; set; }
        public double gantry_lung_sup { get; set; }
        public double gantry_cabeza { get; set; }


        public void LlenarPaciente(Ecl.Patient paciente, Ecl.Course curso)
        {
            ID = paciente.Id;
            Apellido = paciente.LastName;
            Nombre = paciente.FirstName;
            var plan = curso.PlanSetups.First(p => p.Id.Contains("TBI Ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
            StructureSet_UID = plan.StructureSet.UID;
            Serie_UID= plan.StructureSet.Image.Series.UID;
            Study_UID = plan.StructureSet.Image.Series.Study.UID;
            FOR_UID = plan.StructureSet.Image.FOR;
            Dosis = plan.TotalPrescribedDose.Dose/100;
        }

        public void LlenarAnatomia(Ecl.Patient paciente, Ecl.Course curso)
        {
            var plan = curso.PlanSetups.First(p => p.Id.Contains("TBI Ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
            VVector userOrigin = plan.StructureSet.Image.UserOrigin;
            Vol_body = plan.StructureSet.Structures.First(s => s.Id == "BODY").Volume;
            if (plan.StructureSet.Structures.Any(s => s.Id == "Lungs"))
            {
                Vol_lungs = plan.StructureSet.Structures.First(s => s.Id == "Lungs").Volume;
            }
            Diam_origen = Form1.DiamZOrigin(plan);
            var diametros = Form1.Diametros50Central(plan, "BODY").Where(d => !double.IsNaN(d.Item1));
            var pulmones = Form1.InicioFinLungs(plan);
            z_cabeza = diametros.Last().Item2;
            z_pies = diametros.First().Item2;
            z_lung_inf = pulmones.Item1 - userOrigin.z;
            z_lung_sup = pulmones.Item2 - userOrigin.z;
            z_rodilla = -Form1.ZRodilla(plan);

            var diametrosZona1 = diametros.Where(d => d.Item2 < z_rodilla).Select(d => d.Item1).ToList();
            var diametrosZona2 = diametros.Where(d => d.Item2 > z_rodilla && d.Item2 < z_lung_inf).Select(d => d.Item1).ToList();
            var diametrosZona3 = diametros.Where(d => d.Item2 > z_lung_inf && d.Item2 < z_lung_sup).Select(d => d.Item1).ToList();
            var diametrosZona4 = diametros.Where(d => d.Item2 > z_lung_sup).Select(d => d.Item1).ToList();
            
            var metrica1 = Form1.MetricasDeLista(diametrosZona1);
            var metrica2 =Form1.MetricasDeLista(diametrosZona2);
            var metrica3 =Form1.MetricasDeLista(diametrosZona3);
            var metrica4 = Form1.MetricasDeLista(diametrosZona4);

            med_1 = metrica1.media; sd_1 = metrica1.sd; perc20_1 = metrica1.perc20; perc80_1 = metrica1.perc80;
            med_2 = metrica2.media; sd_2 = metrica2.sd; perc20_2 = metrica2.perc20; perc80_2 = metrica2.perc80;
            med_3 = metrica3.media; sd_3 = metrica3.sd; perc20_3 = metrica3.perc20; perc80_3 = metrica3.perc80;
            med_4 = metrica4.media; sd_4 = metrica4.sd; perc20_4 = metrica4.perc20; perc80_4 = metrica4.perc80;
        }

        public void LlenarPredicciones()
        {
            string jsonPathUMs = @"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\trained_models_ums.json";
            string jsonPathGantrys = @"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\trained_models_gantrys.json";
            var models_um = JsonConvert.DeserializeObject<Dictionary<string, Modelo>>(File.ReadAllText(jsonPathUMs));
            var models_gantry = JsonConvert.DeserializeObject<Dictionary<string, Modelo>>(File.ReadAllText(jsonPathGantrys));
            gantry_pies = PredecirValor(models_gantry, "gantry_inicio_1");
            gantry_rodilla = PredecirValor(models_gantry, "gantry_fin_1", "gantry_inicio_2");
            gantry_lung_inf = PredecirValor(models_gantry, "gantry_fin_2", "gantry_inicio_3");
            gantry_lung_sup = PredecirValor(models_gantry, "gantry_fin_3", "gantry_inicio_4");
            gantry_cabeza = PredecirValor(models_gantry, "gantry_fin_4");
            long_arco_1 = LongitudArco(gantry_pies, gantry_rodilla);
            long_arco_2 = LongitudArco(gantry_rodilla, gantry_lung_inf);
            long_arco_3 = LongitudArco(gantry_lung_inf, gantry_lung_sup);
            long_arco_4 = LongitudArco(gantry_lung_sup, gantry_cabeza);
            
            double ums_por_gray_grado_3 = PredecirValor(models_um, "ums_por_gray_grado_3");
            um_por_gray_3 = ums_por_gray_grado_3 * long_arco_3;
            double ums_por_gray_grado_4 = PredecirValor(models_um, "ums_por_gray_grado_4");
            um_por_gray_4 = ums_por_gray_grado_4 * long_arco_4;
            double ums_por_gray_grado_2 = PredecirValor(models_um, "ums_por_gray_grado_2");
            um_por_gray_2 = ums_por_gray_grado_2 * long_arco_2;
            double ums_por_gray_grado_1 = PredecirValor(models_um, "ums_por_gray_grado_1");
            um_por_gray_1 = ums_por_gray_grado_1 * long_arco_1;
            arco1_ums = um_por_gray_1 * Dosis;
            arco2_ums = um_por_gray_2 * Dosis;
            arco3_ums = um_por_gray_3 * Dosis;
            arco4_ums = um_por_gray_4 * Dosis;
            /*var modelo_gantry_pies = models_gantry.First(m => m.Key == "gantry_inicio_1").Value;
            gantry_pies = Math.Round(modelo_gantry_pies.Predecir(modelo_gantry_pies.ObtenerFeatures(this)),0);
            var modelo_gantry_rodilla1 = models_gantry.First(m => m.Key == "gantry_fin_1").Value;
            var modelo_gantry_rodilla2 = models_gantry.First(m => m.Key == "gantry_inicio_2").Value;
            gantry_rodilla = Math.Round(modelo_gantry_rodilla.Predecir(modelo_gantry_rodilla.ObtenerFeatures(this)),0);
            var modelo_gantry_lung_inf = models_gantry.First(m => m.Key == "gantry_fin_2").Value;
            gantry_lung_inf = Math.Round(modelo_gantry_lung_inf.Predecir(modelo_gantry_lung_inf.ObtenerFeatures(this)),0);
            var modelo_gantry_lung_sup = models_gantry.First(m => m.Key == "gantry_fin_3").Value;
            gantry_lung_sup = Math.Round(modelo_gantry_lung_sup.Predecir(modelo_gantry_lung_sup.ObtenerFeatures(this)),0);
            var modelo_gantry_cabeza = models_gantry.First(m => m.Key == "gantry_fin_4").Value;
            gantry_cabeza = Math.Round(modelo_gantry_cabeza.Predecir(modelo_gantry_cabeza.ObtenerFeatures(this)),0);*/

        }

        public double PredecirValor(Dictionary<string,Modelo> modelos, string sModelo1, string sModelo2=null)
        {
            double valor = double.NaN;
            var modelo1 = modelos.First(m => m.Key == sModelo1).Value;
            valor = modelo1.Predecir(modelo1.ObtenerFeatures(this));
            if (sModelo2!=null)
            {
                var modelo2 = modelos.First(m => m.Key == sModelo2).Value;
                valor = (valor + modelo2.Predecir(modelo2.ObtenerFeatures(this)))/2;
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
