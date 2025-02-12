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
using FellowOakDicom.IO;
using FellowOakDicom;

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

        public double DosisTotal { get; set; } //dosis en Gy
        public double DosisFraccion { get; set; } //dosis en Gy
        public int NumFraciones { get; set; }

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

        public double normalizacion { get; set; }
        public double weight_por_norm_1 { get; set; }
        public double weight_por_norm_2 { get; set; }
        public double weight_por_norm_3 { get; set; }
        public double weight_por_norm_4 { get; set; }
        public double um_por_gray_1 { get; set; }
        public double um_por_gray_2 { get; set; }
        public double um_por_gray_3 { get; set; }
        public double um_por_gray_4 { get; set; }

        public double um_por_gray_grado_1 { get; set; }
        public double um_por_gray_grado_2 { get; set; }
        public double um_por_gray_grado_3 { get; set; }
        public double um_por_gray_grado_4 { get; set; }


        public string ToStringRegion1()
        {
            return med_1.ToString() + "," + sd_1.ToString() + "," + perc20_1.ToString() + "," + perc80_1.ToString();
        }

        public string ToStringRegion2()
        {
            return med_2.ToString() + "," + sd_2.ToString() + "," + perc20_2.ToString() + "," + perc80_2.ToString();
        }
        public string ToStringRegion3()
        {
            return med_3.ToString() + "," + sd_3.ToString() + "," + perc20_3.ToString() + "," + perc80_3.ToString();
        }
        public string ToStringRegion4()
        {
            return med_4.ToString() + "," + sd_4.ToString() + "," + perc20_4.ToString() + "," + perc80_4.ToString();
        }


        public string ToStringGantry()
        {
            return ID + ";" + z_pies.ToString() + ";" + z_rodilla.ToString() + ";" + z_lung_inf.ToString() + ";" + z_lung_sup.ToString() + ";" + z_cabeza.ToString() + ";" +
                gantry_pies.ToString() + ";" + gantry_rodilla.ToString() + ";" + gantry_lung_inf.ToString() + ";" + gantry_lung_sup.ToString() + ";" + gantry_cabeza.ToString() + ";";
        }

        public static string GantryCSVHeader()
        {
            return "ID;z_pies;z_rodilla;z_lung_inf;z_lung_sup;z_cabeza;gantry_pies;gantry_rodilla;gantry_lung_inf;gantry_lung_sup;gantry_cabeza";
        }
        public static string UMCSVHeader()
        {
            return "ID;Vol_body;Vol_lungs;med_1;sd_1;perc20_1;perc80_1;med_2;sd_2;perc20_2;perc80_2;med_3;sd_3;perc20_3;perc80_3;med_4;sd_4;perc20_4;perc80_4;" +
                "um_por_gray_1;um_por_gray_grado_1;weight_por_norm_1;um_por_gray_2;um_por_gray_grado_2;weight_por_norm_2;um_por_gray_3;um_por_gray_grado_3;weight_por_norm_3;um_por_gray_4;um_por_gray_grado_4;weight_por_norm_4;normalizacion";
        }

        public string ToStringUMs()
        {
            return ID + ";" + Vol_body.ToString() + ";" + Vol_lungs.ToString() + ";" + med_1.ToString() + ";" + sd_1.ToString() + ";" + perc20_1.ToString() + ";" + perc80_1.ToString() + ";" +
                med_2.ToString() + ";" + sd_2.ToString() + ";" + perc20_2.ToString() + ";" + perc80_2.ToString() + ";" +
                med_3.ToString() + ";" + sd_3.ToString() + ";" + perc20_3.ToString() + ";" + perc80_3.ToString() + ";" +
                med_4.ToString() + ";" + sd_4.ToString() + ";" + perc20_4.ToString() + ";" + perc80_4.ToString() + ";" +
                um_por_gray_1.ToString() + ";" + um_por_gray_grado_1.ToString() + ";" + weight_por_norm_1.ToString() + ";" +
                um_por_gray_2.ToString() + ";" + um_por_gray_grado_2.ToString() + ";" + weight_por_norm_2.ToString() + ";" +
                um_por_gray_3.ToString() + ";" + um_por_gray_grado_3.ToString() + ";" + weight_por_norm_3.ToString() + ";" +
                um_por_gray_4.ToString() + ";" + um_por_gray_grado_4.ToString() + ";" + weight_por_norm_4.ToString() + ";" +
                normalizacion.ToString();
        }

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
            GetType().GetProperty($"um_por_gray_{indice + 1}").SetValue(this, arco.um_por_gray);
            GetType().GetProperty($"um_por_gray_grado_{indice + 1}").SetValue(this, arco.ums_por_gray_grado);
            GetType().GetProperty($"weight_por_norm_{indice + 1}").SetValue(this, arco.weight_por_norm);
        }

        public void PredecirPaciente(StructureSet ss, Patient paciente, double dosisGy, double zRodilla, Dictionary<string, Modelo> modelos, Course curso, int numFx)
        {
            ExtraerDatos(ss, paciente, dosisGy, curso, numFx);
            ExtraerAnatomia(ss, paciente, zRodilla);
            LlenarPredicciones(modelos);
        }
        public void PredecirPaciente(Patient paciente, Course curso, Dictionary<string, Modelo> modelos, int numFx)
        {
            ExtraerDatos(paciente, curso, numFx);
            ExtraerAnatomia(paciente, curso);
            LlenarPredicciones(modelos);
        }

        public void ExtraerPaciente(Patient paciente, Course curso, int numFx)
        {
            ExtraerDatos(paciente, curso, numFx);
            ExtraerAnatomia(paciente, curso);
            ExtraerFeatures(curso);
        }

        public void ExtraerDatos(StructureSet ss, Patient paciente, double dosisGy, Course curso, int numFx) //despues sacar curso y obtener Serie Instance UID de context
        {
            var plan = curso.PlanSetups.First(p => p.Id.ToLower().Contains("tbi ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
            ID = paciente.Id;
            Apellido = paciente.LastName;
            Nombre = paciente.FirstName;
            //var plan = curso.PlanSetups.First(p => p.Id.Contains("TBI Ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
            StructureSet_UID = ss.UID;
            Serie_UID = plan.SeriesUID;
            //Serie_UID = ss.Image.Series.UID;
            Study_UID = ss.Image.Series.Study.UID;
            FOR_UID = ss.Image.FOR;
            DosisFraccion = dosisGy;
            NumFraciones = numFx;

        }

        public void ExtraerDatos(Patient paciente, Course curso, int numFx)
        {
            var plan = curso.PlanSetups.First(p => p.Id.ToLower().Contains("tbi ant"));// && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
            StructureSet ss = plan.StructureSet;
            double Dosis = plan.TotalPrescribedDose.Dose / 100;
            ExtraerDatos(ss, paciente, Dosis, curso, numFx);
        }

        public void ExtraerAnatomia(StructureSet ss, Patient paciente, double zRodilla)
        {
            VVector userOrigin = ss.Image.UserOrigin;
            if (!ss.Structures.Any(s => s.Id.ToUpper() == "BODY"))
            {
                return;
            }
            Vol_body = ss.Structures.First(s => s.Id.ToUpper() == "BODY").Volume;
            if (ss.Structures.Any(s => s.Id.ToLower() == "lungs" || s.Id.ToLower() == "pulmones" || s.Id.ToLower() == "pulmon"))
            {
                Vol_lungs = ss.Structures.First(s => s.Id.ToLower() == "lungs" || s.Id.ToLower() == "pulmones" || s.Id.ToLower() == "pulmon").Volume;
            }
            Diam_origen = Extracciones.DiamZOrigin(ss);
            var diametros = Extracciones.Diametros50Central(ss).Where(d => !double.IsNaN(d.Item1));
            var pulmones = Extracciones.InicioFinLungs(ss);
            z_cabeza = diametros.Last().Item2;
            z_pies = diametros.First().Item2;
            z_lung_inf = pulmones.Item1 - userOrigin.z;
            z_lung_sup = pulmones.Item2 - userOrigin.z;
            z_rodilla = -zRodilla;// - userOrigin.z;

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
            var plan = curso.PlanSetups.First(p => p.Id.ToLower().Contains("tbi ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
            var ss = plan.StructureSet;
            ExtraerAnatomia(ss, paciente, Extracciones.ZRodilla(plan));
        }

        public void LlenarPredicciones(Dictionary<string, Modelo> modelos)
        {
            gantry_pies = PredecirValor(modelos, "gantry_inicio_1");
            gantry_rodilla = PredecirValor(modelos, "gantry_fin_1", "gantry_inicio_2");
            gantry_lung_inf = PredecirValor(modelos, "gantry_fin_2", "gantry_inicio_3");
            gantry_lung_sup = PredecirValor(modelos, "gantry_fin_3", "gantry_inicio_4");
            //gantry_rodilla = PredecirValor(modelos, "gantry_inicio_2");
            //gantry_lung_inf = PredecirValor(modelos, "gantry_inicio_3");
            //gantry_lung_sup = PredecirValor(modelos, "gantry_inicio_4");
            gantry_cabeza = PredecirValor(modelos, "gantry_fin_4");


            long_arco_1 = LongitudArco(gantry_pies, gantry_rodilla);
            long_arco_2 = LongitudArco(gantry_rodilla, gantry_lung_inf);
            long_arco_3 = LongitudArco(gantry_lung_inf, gantry_lung_sup);
            long_arco_4 = LongitudArco(gantry_lung_sup, gantry_cabeza);
            /*um_por_gray_grado_3 = PredecirValor(modelos, "ums_por_gray_grado_3");
            um_por_gray_3 = um_por_gray_grado_3 * long_arco_3;
            um_por_gray_grado_4 = PredecirValor(modelos, "ums_por_gray_grado_4");
            um_por_gray_4 = um_por_gray_grado_4 * long_arco_4;
            um_por_gray_grado_2 = PredecirValor(modelos, "ums_por_gray_grado_2");
            um_por_gray_2 = um_por_gray_grado_2 * long_arco_2;
            um_por_gray_grado_1 = PredecirValor(modelos, "ums_por_gray_grado_1");
            um_por_gray_1 = um_por_gray_grado_1 * long_arco_1;*/
            um_por_gray_3 = PredecirValor(modelos, "um_por_gray_3");
            um_por_gray_2 = PredecirValor(modelos, "um_por_gray_2");
            um_por_gray_4 = PredecirValor(modelos, "um_por_gray_4");
            um_por_gray_1 = PredecirValor(modelos, "um_por_gray_1");

            weight_por_norm_3 = PredecirValor(modelos, "weight_por_norm_3");
            weight_por_norm_2 = PredecirValor(modelos, "weight_por_norm_2");
            weight_por_norm_4 = PredecirValor(modelos, "weight_por_norm_4");
            weight_por_norm_1 = PredecirValor(modelos, "weight_por_norm_1");
        }

        public void ExtraerFeatures(Course curso)
        {
            if (curso.PlanSetups.Any(p => p.Id.ToLower().Contains("tbi ant")) && curso.PlanSetups.Any(p => p.Id.ToLower().Contains("tbi post")))
            {
                PlanSetup planAnt = curso.PlanSetups.First(p => p.Id.ToLower().Contains("tbi ant") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
                VVector userOrigin = planAnt.StructureSet.Image.UserOrigin;
                PlanSetup planPost = curso.PlanSetups.First(p => p.Id.ToLower().Contains("tbi post") && p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved);
                List<Arco> arcos = Arco.extraerArcos(planAnt, planPost);
                normalizacion = planAnt.PlanNormalizationValue;
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
            return Math.Round(valor, 3);
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

        public void EscribirDCM_Ant()
        {
            DicomFile dicomFile = DicomFile.Open(@"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\TBI Ant.dcm");
            DicomDataset dataset = dicomFile.Dataset;
            dataset.AddOrUpdate(DicomTag.SOPInstanceUID, dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID) + new Random().Next().ToString());
            dataset.AddOrUpdate(DicomTag.PatientName, this.Apellido.ToUpper() + "^" + this.Nombre.ToUpper());
            dataset.AddOrUpdate(DicomTag.PatientID, this.ID);
            dataset.AddOrUpdate(DicomTag.StudyInstanceUID, this.Study_UID);
            dataset.AddOrUpdate(DicomTag.SeriesInstanceUID, this.Serie_UID);
            dataset.AddOrUpdate(DicomTag.StudyInstanceUID, this.Study_UID);
            dataset.AddOrUpdate(DicomTag.FrameOfReferenceUID, this.FOR_UID);
            dataset.AddOrUpdate(DicomTag.ApprovalStatus, "UNAPPROVED");
            dataset.AddOrUpdate(DicomTag.RTPlanLabel, "TBI Ant_mod");
            //var ExtendedInterface = ;

            //dataset.Remove(DicomTag.Parse("3253,1000"));

            dataset.Remove(new DicomTag(12883, 4096, "Varian Medical Systems VISION 3253"));
            dataset.Remove(new DicomTag(12883, 4097, "Varian Medical Systems VISION 3253"));
            dataset.Remove(new DicomTag(12883, 4098, "Varian Medical Systems VISION 3253"));
            dataset.Remove(new DicomTag(12935, 4096, "Varian Medical Systems VISION 3287"));
            dataset.Remove(new DicomTag(12935, 0016));
            dataset.Remove(new DicomTag(12935, 0017));
            dataset.Remove(new DicomTag(12883, 0016));
            //var dt = new DicomTag(12883, 0016, "Varian Medical Systems VISION 3253");
            //dataset.AddOrUpdate(DicomTag.StructurSetRe, this.Study_UID);
            DicomSequence structureSetReference = dataset.GetSequence(DicomTag.ReferencedStructureSetSequence);
            structureSetReference.First().AddOrUpdate(DicomTag.ReferencedSOPInstanceUID, this.StructureSet_UID);
            DicomSequence doseReferenceSequence = dataset.GetSequence(DicomTag.DoseReferenceSequence);
            doseReferenceSequence.First().AddOrUpdate(DicomTag.DeliveryMaximumDose, this.DosisFraccion);
            doseReferenceSequence.First().AddOrUpdate(DicomTag.OrganAtRiskMaximumDose, this.DosisFraccion);
            doseReferenceSequence.First().Remove(DicomTag.DoseReferenceUID);
            doseReferenceSequence.First().AddOrUpdate(DicomVR.LO, new DicomTag(12903, 4096, "Varian Medical Systems VISION 3267"), "BODY1");
            DicomSequence beamSequence = dataset.GetSequence(DicomTag.BeamSequence);
            DicomSequence fractionGroupSequence = dataset.GetSequence(DicomTag.FractionGroupSequence);
            fractionGroupSequence.First().AddOrUpdate(DicomTag.NumberOfFractionsPlanned, this.NumFraciones);
            fractionGroupSequence.First().AddOrUpdate(DicomTag.NumberOfBeams, TotalDeArcos());

            DicomSequence referencedBeamSequence = fractionGroupSequence.First().GetSequence(DicomTag.ReferencedBeamSequence);

            DicomSequence patientSetupSequence = dataset.GetSequence(DicomTag.PatientSetupSequence);
            DicomDataset patientSetupModelo = patientSetupSequence.First().Clone();
            patientSetupSequence.Items.Clear();
            DicomDataset beamModelo = beamSequence.First().Clone();

            beamSequence.Items.Clear();

            DicomDataset referencedBeamModelo = referencedBeamSequence.First().Clone();
            referencedBeamSequence.Items.Clear();

            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < NumArcos(i) + 1; j++)
                {
                    beamSequence.Items.Add(ArcoADDCM(i, j, beamModelo, false)); //para Anterior;
                    referencedBeamSequence.Items.Add(ReferenceBeam(i, j, false, referencedBeamModelo));
                    patientSetupSequence.Items.Add(PatientSetup(i, j, false, patientSetupModelo));
                }
            }
            dicomFile.Save(@"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\Import\TBI Ant_mod.dcm");
        }

        public int SubArcoNumero(int arco, int subarco)
        {
            int contador = 0;
            for (int i = 1; i < arco; i++)
            {
                contador += NumArcos(i);
            }
            return contador + subarco;
        }

        public int TotalDeArcos()
        {
            int contador = 0;
            for (int i = 1; i < 5; i++)
            {
                contador += NumArcos(i);
            }
            return contador;
        }

        public int NumArcos(int arco)
        {
            if (arco == 3)
            {
                double velocidad = LongitudArco(this.gantry_lung_inf, this.gantry_lung_sup) / (um_por_gray_3 / 4 * DosisFraccion);
                return Convert.ToInt32(Math.Ceiling(0.3 / velocidad));
            }
            else
            {
                double um_por_gray_grado = 0;
                if (arco == 1)
                {
                    um_por_gray_grado = this.um_por_gray_grado_1;
                }
                else if (arco == 2)
                {
                    um_por_gray_grado = this.um_por_gray_grado_2;
                }
                else
                {
                    um_por_gray_grado = this.um_por_gray_grado_4;
                }
                return Convert.ToInt32(Math.Ceiling(um_por_gray_grado / 4 * DosisFraccion / 20));
            }
        }

        public DicomDataset ArcoADDCM(int arco, int subarco, DicomDataset beamModelo, bool esPos)
        {
            double gantry_inicio;
            double gantry_fin;
            double UM_total;
            string prefijo = "ant";
            if (esPos)
            {
                prefijo = "pos";
            }
            string nombreCampo = prefijo + arco.ToString() + "." + subarco.ToString();
            double dose_rate = 300;
            if (arco == 1)
            {
                gantry_inicio = gantry_pies;
                gantry_fin = gantry_rodilla;
                UM_total = um_por_gray_1 * DosisFraccion;
            }
            else if (arco == 2)
            {
                gantry_inicio = gantry_rodilla;
                gantry_fin = gantry_lung_inf;
                UM_total = um_por_gray_2 * DosisFraccion;
            }
            else if (arco == 3)
            {
                gantry_inicio = gantry_lung_inf;
                gantry_fin = gantry_lung_sup;
                UM_total = um_por_gray_3 * DosisFraccion;
                dose_rate = 100;
            }
            else
            {
                gantry_inicio = gantry_lung_sup;
                gantry_fin = gantry_cabeza;
                UM_total = um_por_gray_4 * DosisFraccion;
            }
            DicomDataset nuevoArco = beamModelo.Clone();
            nuevoArco.AddOrUpdate(DicomTag.BeamNumber, SubArcoNumero(arco, subarco));
            nuevoArco.AddOrUpdate(DicomTag.BeamName, nombreCampo);
            var primerControlPoint = nuevoArco.GetSequence(DicomTag.ControlPointSequence).First();
            var segundoControlPoint = nuevoArco.GetSequence(DicomTag.ControlPointSequence).Last();
            primerControlPoint.AddOrUpdate(DicomTag.DoseRateSet, dose_rate);
            if (subarco % 2 != 0) //es inpar
            {
                primerControlPoint.AddOrUpdate(DicomTag.GantryAngle, gantry_inicio);
                primerControlPoint.AddOrUpdate(DicomTag.GantryRotationDirection, "CW");
                segundoControlPoint.AddOrUpdate(DicomTag.GantryAngle, gantry_fin);
            }
            else
            {
                primerControlPoint.AddOrUpdate(DicomTag.GantryAngle, gantry_fin);
                primerControlPoint.AddOrUpdate(DicomTag.GantryRotationDirection, "CC");
                segundoControlPoint.AddOrUpdate(DicomTag.GantryAngle, gantry_inicio);
            }
            //FALTA AGREGAR ISOCENTRO!!!!!!!!!!!!!!!
            nuevoArco.Remove(DicomTag.ReferencedReferenceImageSequence);
            return nuevoArco;
        }

        public DicomDataset PatientSetup(int arco, int subarco, bool esPost, DicomDataset modelo)
        {
            string posicion = "HFS";
            if (esPost)
            {
                posicion = "HFP";
            }
            DicomDataset nuevoPatientSetup = modelo.Clone();
            nuevoPatientSetup.AddOrUpdate(DicomTag.PatientPosition, posicion);
            nuevoPatientSetup.AddOrUpdate(DicomTag.PatientSetupNumber, SubArcoNumero(arco, subarco));
            return nuevoPatientSetup;
        }

        public DicomDataset ReferenceBeam(int arco, int subarco, bool esPost, DicomDataset modelo)
        {
            DicomDataset nuevoReferenceBeam = modelo.Clone();
            nuevoReferenceBeam.AddOrUpdate(DicomTag.BeamMeterset, UMArco(arco) / NumArcos(arco));
            nuevoReferenceBeam.AddOrUpdate(DicomTag.ReferencedBeamNumber, SubArcoNumero(arco, subarco));
            return nuevoReferenceBeam;
        }

        public double UMArco(int arco)
        {
            double um = 0;
            if (arco == 1)
            {
                um = um_por_gray_1 * DosisFraccion;
            }
            else if (arco == 2)
            {
                um = um_por_gray_2 * DosisFraccion;
            }
            else if (arco == 3)
            {
                um = um_por_gray_3 * DosisFraccion;
            }
            else
            {
                um = um_por_gray_4 * DosisFraccion;
            }
            return um;
        }
        #region CSVs

        public static string EtiquetasUMArco3()
        {
            return "ID,Vol_body,Vol_lungs,Diam_origen,med_3,sd_3,perc80_3,perc20_3,long_arco_3,um_por_gray_3,ums_por_gray_grado_3";
        }

        public static string EtiquetasUMArco2()
        {
            return "ID,Vol_body,Vol_lungs,Diam_origen,med_2,sd_2,perc80_2,perc20_2,med_3,sd_3,perc80_3,perc20_3,um_por_gray_3,long_arco_2,um_por_gray_2,ums_por_gray_grado_2";
        }
        public static string EtiquetasUMArco4()
        {
            return "ID,Vol_body,Vol_lungs,Diam_origen,med_4,sd_4,perc80_4,perc20_4,med_3,sd_3,perc80_3,perc20_3,um_por_gray_3,z_cabeza,um_por_gray_4,ums_por_gray_grado_4";
        }
        public static string EtiquetasUMArco1()
        {
            return "ID,Vol_body,Vol_lungs,Diam_origen,med_1,sd_1,perc80_1,perc20_1,med_2,sd_2,perc80_2,perc20_2,um_por_gray_2,z_pies,um_por_gray_1,ums_por_gray_grado_1";
        }

        public static string EtiquetasWeightArco3()
        {
            return "ID,Vol_body,Vol_lungs,Diam_origen,med_3,sd_3,perc80_3,perc20_3,long_arco_3,weight_por_norm_3";
        }

        public static string EtiquetasWeightArco2()
        {
            return "ID,Vol_body,Vol_lungs,Diam_origen,med_2,sd_2,perc80_2,perc20_2,med_3,sd_3,perc80_3,perc20_3,weight_por_norm_3,long_arco_2,weight_por_norm_2";
        }
        public static string EtiquetasWeightArco4()
        {
            return "ID,Vol_body,Vol_lungs,Diam_origen,med_4,sd_4,perc80_4,perc20_4,med_3,sd_3,perc80_3,perc20_3,weight_por_norm_3,z_cabeza,weight_por_norm_4";
        }
        public static string EtiquetasWeightArco1()
        {
            return "ID,Vol_body,Vol_lungs,Diam_origen,med_1,sd_1,perc80_1,perc20_1,med_2,sd_2,perc80_2,perc20_2,weight_por_norm_2,z_pies,weight_por_norm_1";
        }

        public string ToStringUMArco3()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString() + "," + Diam_origen.ToString() + "," + ToStringRegion3() + "," + long_arco_3.ToString() + "," + um_por_gray_3.ToString() + "," + um_por_gray_grado_3.ToString();
        }
        public string ToStringUMArco2()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString() + "," + Diam_origen.ToString() + "," + ToStringRegion2() + "," + ToStringRegion3() + "," + um_por_gray_3.ToString() + "," + long_arco_2.ToString() + "," + um_por_gray_2.ToString() + "," + um_por_gray_grado_2.ToString();
        }
        public string ToStringUMArco4()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString() + "," + Diam_origen.ToString() + "," + ToStringRegion4() + "," + ToStringRegion3() + "," + um_por_gray_3.ToString() + "," + z_cabeza.ToString() + "," + um_por_gray_4.ToString() + "," + um_por_gray_grado_4.ToString();
        }
        public string ToStringUMArco1()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString() + "," + Diam_origen.ToString() + "," + ToStringRegion1() + "," + ToStringRegion2() + "," + um_por_gray_2.ToString() + "," + z_pies.ToString() + "," + um_por_gray_1.ToString() + "," + um_por_gray_grado_1.ToString();
        }

        public string ToStringWeightArco3()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString() + "," + Diam_origen.ToString() + "," + ToStringRegion3() + "," + long_arco_3.ToString() + "," + weight_por_norm_3.ToString();
        }
        public string ToStringWeightArco2()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString() + "," + Diam_origen.ToString() + "," + ToStringRegion2() + "," + ToStringRegion3() + "," + weight_por_norm_3.ToString() + "," + long_arco_2.ToString() + "," + weight_por_norm_2.ToString();
        }
        public string ToStringWeightArco4()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString() + "," + Diam_origen.ToString() + "," + ToStringRegion4() + "," + ToStringRegion3() + "," + weight_por_norm_3.ToString() + "," + z_cabeza.ToString() + "," + weight_por_norm_4.ToString();
        }
        public string ToStringWeightArco1()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString() + "," + Diam_origen.ToString() + "," + ToStringRegion1() + "," + ToStringRegion2() + "," + weight_por_norm_2.ToString() + "," + z_pies.ToString() + "," + weight_por_norm_1.ToString();
        }
        public string ToStringGantryArco1()
        {
            return ID + "," + Diam_origen.ToString() + "," + z_pies.ToString() + "," + z_rodilla.ToString() + "," + gantry_pies.ToString() + "," + gantry_rodilla.ToString();
        }

        public string ToStringGantryArco2()
        {
            return ID + "," + Diam_origen.ToString() + "," + z_rodilla.ToString() + "," + z_lung_inf.ToString() + "," + gantry_rodilla.ToString() + "," + gantry_lung_inf.ToString();
        }

        public string ToStringGantryArco3()
        {
            return ID + "," + Diam_origen.ToString() + "," + z_lung_inf.ToString() + "," + z_lung_sup.ToString() + "," + gantry_lung_inf.ToString() + "," + gantry_lung_sup.ToString();
        }

        public string ToStringGantryArco4()
        {
            return ID + "," + Diam_origen.ToString() + "," + z_lung_sup.ToString() + "," + z_cabeza.ToString() + "," + gantry_lung_sup.ToString() + "," + gantry_cabeza.ToString();
        }

        public static string EtiquetaGantryArco1()
        {
            return "ID,Diam_origen,z_pies,z_rodilla,gantry_inicio_1,gantry_fin_1";
        }
        public static string EtiquetaGantryArco2()
        {
            return "ID,Diam_origen,z_rodilla,z_lung_inf,gantry_inicio_2,gantry_fin_2";
        }
        public static string EtiquetaGantryArco3()
        {
            return "ID,Diam_origen,z_lung_inf,z_lung_sup,gantry_inicio_3,gantry_fin_3";
        }
        public static string EtiquetaGantryArco4()
        {
            return "ID,Diam_origen,z_lung_sup,z_cabeza,gantry_inicio_4,gantry_fin_4";
        }

        public static void EscribirCSVs(List<Paciente> lista_pacientes)
        {
            List<string> UM_arco1 = new List<string>();
            UM_arco1.Add(EtiquetasUMArco1());
            List<string> Gantry_arco1 = new List<string>();
            Gantry_arco1.Add(EtiquetaGantryArco1());
            List<string> Weight_arco1 = new List<string>();
            Weight_arco1.Add(EtiquetasWeightArco1());


            List<string> UM_arco2 = new List<string>();
            UM_arco2.Add(EtiquetasUMArco2());
            List<string> Gantry_arco2 = new List<string>();
            Gantry_arco2.Add(EtiquetaGantryArco2());
            List<string> Weight_arco2 = new List<string>();
            Weight_arco2.Add(EtiquetasWeightArco2());

            List<string> UM_arco3 = new List<string>();
            UM_arco3.Add(EtiquetasUMArco3());
            List<string> Gantry_arco3 = new List<string>();
            Gantry_arco3.Add(EtiquetaGantryArco3());
            List<string> Weight_arco3 = new List<string>();
            Weight_arco3.Add(EtiquetasWeightArco3());

            List<string> UM_arco4 = new List<string>();
            UM_arco4.Add(EtiquetasUMArco4());
            List<string> Gantry_arco4 = new List<string>();
            Gantry_arco4.Add(EtiquetaGantryArco4());
            List<string> Weight_arco4 = new List<string>();
            Weight_arco4.Add(EtiquetasWeightArco4());

            foreach (Paciente p in lista_pacientes)

            {
                UM_arco1.Add(p.ToStringUMArco1());
                Gantry_arco1.Add(p.ToStringGantryArco1());
                Weight_arco1.Add(p.ToStringWeightArco1());

                UM_arco2.Add(p.ToStringUMArco2());
                Gantry_arco2.Add(p.ToStringGantryArco2());
                Weight_arco2.Add(p.ToStringWeightArco2());

                UM_arco3.Add(p.ToStringUMArco3());
                Gantry_arco3.Add(p.ToStringGantryArco3());
                Weight_arco3.Add(p.ToStringWeightArco3());

                UM_arco4.Add(p.ToStringUMArco4());
                Gantry_arco4.Add(p.ToStringGantryArco4());
                Weight_arco4.Add(p.ToStringWeightArco4());
            }
            string path = @"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\";
            File.WriteAllLines(path + "UM_arco1.csv", UM_arco1.ToArray());
            File.WriteAllLines(path + "UM_arco2.csv", UM_arco2.ToArray());
            File.WriteAllLines(path + "UM_arco3.csv", UM_arco3.ToArray());
            File.WriteAllLines(path + "UM_arco4.csv", UM_arco4.ToArray());
            File.WriteAllLines(path + "Gantry_arco1.csv", Gantry_arco1.ToArray());
            File.WriteAllLines(path + "Gantry_arco2.csv", Gantry_arco2.ToArray());
            File.WriteAllLines(path + "Gantry_arco3.csv", Gantry_arco3.ToArray());
            File.WriteAllLines(path + "Gantry_arco4.csv", Gantry_arco4.ToArray());
            File.WriteAllLines(path + "Weight_arco1.csv", Weight_arco1.ToArray());
            File.WriteAllLines(path + "Weight_arco2.csv", Weight_arco2.ToArray());
            File.WriteAllLines(path + "Weight_arco3.csv", Weight_arco3.ToArray());
            File.WriteAllLines(path + "Weight_arco4.csv", Weight_arco4.ToArray());
        }

        #endregion



    }
    public class Diferencia
    {
        public double gantry_pies { get; set; }
        public double gantry_rodilla { get; set; }
        public double gantry_lung_inf { get; set; }
        public double gantry_lung_sup { get; set; }
        public double gantry_cabeza { get; set; }

        public double weight_por_norm_1 { get; set; }
        public double weight_por_norm_2 { get; set; }
        public double weight_por_norm_3 { get; set; }
        public double weight_por_norm_4 { get; set; }
        public double um_por_gray_1 { get; set; }
        public double um_por_gray_2 { get; set; }
        public double um_por_gray_3 { get; set; }
        public double um_por_gray_4 { get; set; }

        public Diferencia(Paciente planificado, Paciente predicho)
        {
            gantry_pies = Math.Round(planificado.gantry_pies - predicho.gantry_pies,1);
            gantry_rodilla = Math.Round(planificado.gantry_rodilla - predicho.gantry_rodilla,1);
            gantry_lung_inf = Math.Round(planificado.gantry_lung_inf - predicho.gantry_lung_inf, 1);
            gantry_lung_sup = Math.Round(planificado.gantry_lung_sup - predicho.gantry_lung_sup, 1);
            gantry_cabeza = Math.Round(planificado.gantry_cabeza - predicho.gantry_cabeza, 1);
            weight_por_norm_1 = Math.Round(planificado.weight_por_norm_1 - predicho.weight_por_norm_1, 1);
            weight_por_norm_2 = Math.Round(planificado.weight_por_norm_2 - predicho.weight_por_norm_2, 1);
            weight_por_norm_3 = Math.Round(planificado.weight_por_norm_3 - predicho.weight_por_norm_3, 1);
            weight_por_norm_4 = Math.Round(planificado.weight_por_norm_4 - predicho.weight_por_norm_4, 1);
            um_por_gray_1 = Math.Round(planificado.um_por_gray_1 - predicho.um_por_gray_1, 1);
            um_por_gray_2 = Math.Round(planificado.um_por_gray_2 - predicho.um_por_gray_2, 1);
            um_por_gray_3 = Math.Round(planificado.um_por_gray_3 - predicho.um_por_gray_3, 1);
            um_por_gray_4 = Math.Round(planificado.um_por_gray_4 - predicho.um_por_gray_4, 1);
        }

        public override string ToString()
        {
            var valores = this.GetType()
                  .GetProperties()
                  .Where(p => p.PropertyType == typeof(double))
                  .Select(p => p.GetValue(this)?.ToString())
                  .ToArray();

            return string.Join(";", valores);
        }

    }
}
