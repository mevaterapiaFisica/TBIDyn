using System;
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
    }
}

