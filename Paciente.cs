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
        public double Diam_en_origen { get; set; }
        public double z_cabeza { get; set; }
        public double z_lung_sup { get; set; }
        public double z_lung_inf { get; set; }
        public double z_rodilla { get; set; }
        public double z_pies { get; set; }

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
            Dosis = plan.TotalPrescribedDose.Dose;
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
            Diam_en_origen = Form1.DiamZOrigin(plan);
            var diametros = Form1.Diametros50Central(plan, "BODY").Where(d => !double.IsNaN(d.Item1));
            var pulmones = Form1.InicioFinLungs(plan);
            z_cabeza = diametros.Last().Item2;
            z_pies = diametros.First().Item2;
            z_lung_inf = pulmones.Item1 - userOrigin.z;
            z_lung_sup = pulmones.Item2 - userOrigin.z;
            z_rodilla = -Form1.ZRodilla(plan);
        }

        public void LlenarPredicciones()
        {
            string jsonPathUMs = @"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\trained_models_ums.json";
            string jsonPathGantrys = @"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\trained_models_gantrys.json";
            var models_um = JsonConvert.DeserializeObject<Dictionary<string, Modelo>>(File.ReadAllText(jsonPathUMs));
            var models_gantry = JsonConvert.DeserializeObject<Dictionary<string, Modelo>>(File.ReadAllText(jsonPathGantrys));
            var modelo_gantry_pies = models_gantry.First(m => m.Key == "gantry_inicio_1").Value;
            gantry_pies = modelo_gantry_pies.Predecir(modelo_gantry_pies.ObtenerFeatures(this));
            var modelo_gantry_rodilla = models_gantry.First(m => m.Key == "gantry_fin_1").Value;
            gantry_rodilla = modelo_gantry_rodilla.Predecir(modelo_gantry_rodilla.ObtenerFeatures(this));
            var modelo_gantry_lung_inf = models_gantry.First(m => m.Key == "gantry_fin_2").Value;
            gantry_lung_inf = modelo_gantry_lung_inf.Predecir(modelo_gantry_lung_inf.ObtenerFeatures(this));
            var modelo_gantry_lung_sup = models_gantry.First(m => m.Key == "gantry_fin_3").Value;
            gantry_lung_sup = modelo_gantry_lung_sup.Predecir(modelo_gantry_lung_sup.ObtenerFeatures(this));
            var modelo_gantry_cabeza = models_gantry.First(m => m.Key == "gantry_fin_4").Value;
            gantry_cabeza = modelo_gantry_cabeza.Predecir(modelo_gantry_cabeza.ObtenerFeatures(this));


        }
    }
}
