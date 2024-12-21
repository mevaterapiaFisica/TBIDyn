﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBIDyn
{
    public class Modelo
    {
        public List<string> Features { get; set; }
        public List<double> Coefficients { get; set; }
        public double Intercept { get; set; }


        public static Dictionary<string,Modelo> InicializarModelos()
        {
            string jsonPathUMs = @"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\trained_models_ums.json";
            string jsonPathGantrys = @"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\trained_models_gantrys.json";
            var modelos = JsonConvert.DeserializeObject<Dictionary<string, Modelo>>(File.ReadAllText(jsonPathUMs));
            var models_gantry = JsonConvert.DeserializeObject<Dictionary<string, Modelo>>(File.ReadAllText(jsonPathGantrys));
            foreach (var modelo_g in models_gantry)
            {
                if (!modelos.ContainsKey(modelo_g.Key))
                {
                    modelos.Add(modelo_g.Key, modelo_g.Value);
                }
            }
            return modelos;
        }
        public double Predecir(Dictionary<string, double> inputFeatures)
        {
            double prediction = this.Intercept;

            for (int i = 0; i < this.Features.Count; i++)
            {
                string featureName = this.Features[i];
                if (inputFeatures.ContainsKey(featureName))
                {
                    prediction += this.Coefficients[i] * inputFeatures[featureName];
                }
                else
                {
                    Console.WriteLine($"Feature '{featureName}' no encontrada en la entrada.");
                }
            }

            return prediction;
        }
        public Dictionary<string, double> ObtenerFeatures(Paciente paciente)
        {
            var features = new Dictionary<string, double>();

            // Obtener todas las propiedades públicas de la clase
            var propiedades = typeof(Paciente).GetProperties();

            foreach (var nombreFeature in this.Features)
            {
                // Buscar la propiedad correspondiente
                var propiedad = propiedades.FirstOrDefault(p => p.Name == nombreFeature);

                if (propiedad != null && propiedad.PropertyType == typeof(double))
                {
                    // Obtener el valor de la propiedad
                    var valor = (double)propiedad.GetValue(paciente);
                    features[nombreFeature] = valor;
                }
                else
                {
                    throw new ArgumentException($"La propiedad '{nombreFeature}' no existe o no es de tipo double.");
                }
            }

            return features;
        }
    }
}

