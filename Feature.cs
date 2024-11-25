using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBIDyn
{
    public class Feature
    {
        public string ID { get; set; }
        public double Vol_body { get; set; }
        public double Vol_lungs { get; set; }
        public double Diam_en_origen { get; set; }
        public double z_cabeza { get; set; }
        public double z_lung_sup { get; set; }
        public double z_lung_inf { get; set; }
        //public double z_ref { get; set; }
        public double z_rodilla { get; set; }
        public double z_pies { get; set; }
        public List<MetricasRegion> regiones { get; set; }
        public List<Arco> arcos { get; set; }

        public Feature()
        {
            regiones = new List<MetricasRegion>();
            arcos = new List<Arco>();
        }

        public bool TieneAlgoNulo()
        { 
            return Vol_body==0 || Vol_lungs==0 || Diam_en_origen ==double.NaN || z_rodilla ==double.NaN || arcos.Any(a=>a==null) || regiones.Any(r=>r.media==0 ||r.media==double.NaN);
        }

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
            return "ID,Vol_body,Vol_lungs,Diam_origen,med_4,sd_4,perc80_4,perc20_4,med_3,sd_3,perc80_3,perc20_3,um_por_gray_3,long_arco_4,um_por_gray_4,ums_por_gray_grado_4";
        }
        public static string EtiquetasUMArco1()
        {
            return "ID,Vol_body,Vol_lungs,Diam_origen,med_1,sd_1,perc80_1,perc20_1,med_2,sd_2,perc80_2,perc20_2,um_por_gray_2,long_arco_1,um_por_gray_1,ums_por_gray_grado_1";
        }

        public string ToStringUMArco3()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString()+ "," + Diam_en_origen.ToString() + "," + regiones[2].ToString() + "," + arcos[2].long_arco.ToString() + "," + arcos[2].um_por_gray.ToString() + "," + arcos[2].ums_por_gray_grado.ToString();
        }
        public string ToStringUMArco2()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString() + "," + Diam_en_origen.ToString() + "," + regiones[1].ToString() + "," + regiones[2].ToString() + "," + arcos[2].um_por_gray + "," + arcos[1].long_arco.ToString() + "," + arcos[1].um_por_gray.ToString() + "," + arcos[1].ums_por_gray_grado.ToString();
        }
        public string ToStringUMArco4()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString() + "," + Diam_en_origen.ToString() + "," + regiones[3].ToString() + "," + regiones[2].ToString() + "," + arcos[2].um_por_gray + "," + arcos[3].long_arco.ToString() + "," + arcos[3].um_por_gray.ToString() + "," + arcos[3].ums_por_gray_grado.ToString();
        }
        public string ToStringUMArco1()
        {
            return ID + "," + Vol_body.ToString() + "," + Vol_lungs.ToString() + "," + Diam_en_origen.ToString() + "," + regiones[0].ToString() + "," + regiones[1].ToString() + "," + arcos[1].um_por_gray + "," + arcos[0].long_arco.ToString() + "," + arcos[0].um_por_gray.ToString() + "," + arcos[0].ums_por_gray_grado.ToString();
        }
        public string ToStringGantryArco1()
        {
            return ID + "," + Diam_en_origen.ToString() + "," + z_pies.ToString() + "," + z_rodilla.ToString() + "," + arcos[0].gantry_inicio.ToString() + "," + arcos[0].gantry_fin.ToString();
        }

        public string ToStringGantryArco2()
        {
            return ID + "," + Diam_en_origen.ToString() + "," + z_rodilla.ToString() + "," + z_lung_inf.ToString() + "," + arcos[1].gantry_inicio.ToString() + "," + arcos[1].gantry_fin.ToString();
        }

        public string ToStringGantryArco3()
        {
            return ID + "," + Diam_en_origen.ToString() + "," + z_lung_inf.ToString() + "," + z_lung_sup.ToString() + "," + arcos[2].gantry_inicio.ToString() + "," + arcos[2].gantry_fin.ToString();
        }

        public string ToStringGantryArco4()
        {
            return ID + "," + Diam_en_origen.ToString() + "," + z_lung_sup.ToString() + "," + z_cabeza.ToString() + "," + arcos[3].gantry_inicio.ToString() + "," + arcos[3].gantry_fin.ToString();
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


        public static void EscribirCSVs(List<Feature> lista_features)
        {
            List<string> UM_arco1 = new List<string>();
            UM_arco1.Add(EtiquetasUMArco1());
            List<string> Gantry_arco1 = new List<string>();
            Gantry_arco1.Add(EtiquetaGantryArco1());
            List<string> UM_arco2 = new List<string>();
            UM_arco2.Add(EtiquetasUMArco2());
            List<string> Gantry_arco2 = new List<string>();
            Gantry_arco2.Add(EtiquetaGantryArco2());
            List<string> UM_arco3 = new List<string>();
            UM_arco3.Add(EtiquetasUMArco3());
            List<string> Gantry_arco3 = new List<string>();
            Gantry_arco3.Add(EtiquetaGantryArco3());
            List<string> UM_arco4 = new List<string>();
            UM_arco4.Add(EtiquetasUMArco4());
            List<string> Gantry_arco4 = new List<string>();
            Gantry_arco4.Add(EtiquetaGantryArco4());
            foreach (Feature f in lista_features)
            {
                UM_arco1.Add(f.ToStringUMArco1());
                Gantry_arco1.Add(f.ToStringGantryArco1());

                UM_arco2.Add(f.ToStringUMArco2());
                Gantry_arco2.Add(f.ToStringGantryArco2());

                UM_arco3.Add(f.ToStringUMArco3());
                Gantry_arco3.Add(f.ToStringGantryArco3());

                UM_arco4.Add(f.ToStringUMArco4());
                Gantry_arco4.Add(f.ToStringGantryArco4());
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
        }
       
    }

    public class MetricasRegion
    {
        public double media { get; set; }
        public double sd { get; set; }
        public double perc80 { get; set; }
        public double perc20 { get; set; }

        public override string ToString()
        {
            return media.ToString() + "," + sd.ToString() + "," + perc80.ToString() + "," + perc20.ToString();
        }
    }
}
