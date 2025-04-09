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
    public class Arco
    {
        public string nombre;
        public double gantry_inicio;
        public double gantry_fin;
        public double long_arco;
        public double um_por_gray;
        public double asim_um_por_gray;
        public double ums_por_gray_grado;
        public double weight_por_norm;
        public double normalizacion;
        public Arco(string _nombre, double _gantry_i, double _gantry_f, double _um_por_gray, double _weight_por_norm, double _normalizacion)
        {
            nombre = _nombre;
            gantry_inicio = _gantry_i;
            gantry_fin = _gantry_f;
            long_arco = Paciente.LongitudArco(_gantry_i, _gantry_f);
            um_por_gray = _um_por_gray;
            ums_por_gray_grado = _um_por_gray / long_arco;
            weight_por_norm = _weight_por_norm;
            normalizacion = _normalizacion;
        }

        public Arco(PlanSetup plan, string nombre)
        {
            List<Beam> arcos = plan.Beams.Where(b => b.Id.ToLower().Contains(nombre.ToLower())).ToList();
            if (arcos == null || arcos.Count == 0)
            {
                return;
            }
            this.nombre = nombre;
            if (arcos.First().GantryDirection == GantryDirection.Clockwise)
            {
                gantry_inicio = arcos.First().ControlPoints.First().GantryAngle;
                gantry_fin = arcos.First().ControlPoints.Last().GantryAngle;
            }
            else
            {
                gantry_inicio = arcos.First().ControlPoints.Last().GantryAngle;
                gantry_fin = arcos.First().ControlPoints.First().GantryAngle;
            }
            /*double primerGantry = arcos.First().ControlPoints.First().GantryAngle;
            double ultimoGantry = arcos.First().ControlPoints.Last().GantryAngle;
            gantry_inicio = Math.Min(primerGantry, ultimoGantry);
            gantry_fin = Math.Max(primerGantry, ultimoGantry);*/
            long_arco = LongArco();
            foreach (var arco in arcos)
            {
                if ((arco.ControlPoints.First().GantryAngle == gantry_inicio || arco.ControlPoints.First().GantryAngle == gantry_fin) && (arco.ControlPoints.Last().GantryAngle == gantry_inicio || arco.ControlPoints.Last().GantryAngle == gantry_fin))
                {
                    um_por_gray += arco.Meterset.Value;
                    weight_por_norm += arco.WeightFactor / plan.PlanNormalizationValue * 40; //normalizo todos a normalizacion 40
                }
            }
            um_por_gray = um_por_gray / (plan.UniqueFractionation.PrescribedDosePerFraction.Dose / 100);
            ums_por_gray_grado = um_por_gray / long_arco;
            normalizacion = plan.PlanNormalizationValue;
        }
        public override string ToString()
        {
            return nombre + "-" + gantry_inicio.ToString() + "-" + gantry_fin.ToString() + "-" + um_por_gray.ToString();
        }

        public double LongArco()
        {
            /*double inicio = gantry_inicio;
            double fin = gantry_fin;
            if (gantry_inicio > 180 )
            {
                inicio = 360 - gantry_inicio;
            }
            if (gantry_fin>180)
            {
                fin = 360 - gantry_fin;
            }
            return inicio + fin;*/
            if ((gantry_inicio > 180 && gantry_fin > 180) || (gantry_inicio < 180 && gantry_fin < 180))
            {
                return Math.Abs(gantry_fin - gantry_inicio);
            }
            else
            {
                return 360 - Math.Max(gantry_fin, gantry_inicio) + Math.Min(gantry_fin, gantry_inicio);
            }
        }

        public static List<Arco> extraerArcos(PlanSetup planAnt, PlanSetup planPost)
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
            //unifico ant y post. Después en algún momento puedo analizar por separado. Promedio las UM y los gantrys
            List<Arco> arcosUnificados = new List<Arco>();
            for (int i = 0; i < 4; i++)
            {
                Arco arco_ant = arcos[i];
                Arco arco_post = arcos[i + 4];
                if (arco_post.nombre == null)
                {
                    arco_post.gantry_fin = arco_ant.gantry_fin;
                    arco_post.gantry_inicio = arco_ant.gantry_inicio;
                    arco_post.long_arco = arco_ant.long_arco;
                    arco_post.um_por_gray = arco_ant.um_por_gray;
                    arco_post.ums_por_gray_grado = arco_ant.ums_por_gray_grado;
                    arco_post.weight_por_norm = arco_ant.weight_por_norm;
                }
                else if (arco_ant.nombre == null)
                {

                    arco_ant.gantry_fin = arco_post.gantry_fin;
                    arco_ant.gantry_inicio = arco_post.gantry_inicio;
                    arco_ant.long_arco = arco_post.long_arco;
                    arco_ant.um_por_gray = arco_post.um_por_gray;
                    arco_ant.ums_por_gray_grado = arco_post.ums_por_gray_grado;
                    arco_ant.weight_por_norm = arco_post.weight_por_norm;

                }
                Arco arco = new Arco((i + 1).ToString(), (arco_ant.gantry_inicio + arco_post.gantry_inicio) / 2, (arco_ant.gantry_fin + arco_post.gantry_fin) / 2, (arco_ant.um_por_gray + arco_post.um_por_gray) / 2, (arco_ant.weight_por_norm + arco_post.weight_por_norm) / 2, arco_ant.normalizacion);
                arcosUnificados.Add(arco);
            }

            return arcosUnificados;
        }
    }
}
