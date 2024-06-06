using System;
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
//using

namespace TBIDyn
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            string planTBI = @"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\TBI Ant.dcm";

            var file = DicomFile.Open(planTBI);
            DicomDataset ds = file.Dataset;
            var instance = ds.GetSingleValue<string>(DicomTag.SOPInstanceUID) + "11";
            ds.AddOrUpdate(DicomTag.SOPInstanceUID, instance);
            var BeamSequence = ds.GetSequence(DicomTag.BeamSequence);
            var Beam1 = BeamSequence.First();
            double[] CumMetWeights = new double[] { 0, 0.25, 0.5, 0.75, 1 };
            double[] CumGantryAngFx = new double[] { 0, 0.1, 0.3, 0.6, 1 };
            var cpSequence = Beam1.GetSequence(DicomTag.ControlPointSequence);

            var GantryInicial = cpSequence.First().GetSingleValue<double>(DicomTag.GantryAngle);
            var GantryFinal = cpSequence.Last().GetSingleValue<double>(DicomTag.GantryAngle);
            string rotationDir = cpSequence.First().GetSingleValue<string>(DicomTag.GantryRotationDirection);

            
            
            for (int i=0;i<3;i++)
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
            }
            foreach (DicomDataset cp in cpSequence)
            {
                int index = cp.GetSingleValue<int>(DicomTag.ControlPointIndex);
                cp.AddOrUpdate(DicomTag.ControlPointIndex, index);
                if (index!=cpSequence.Count()-1)//no es el ultimo
                {
                    cp.AddOrUpdate(DicomTag.GantryRotationDirection, rotationDir);
                }
                cp.AddOrUpdate(DicomTag.CumulativeMetersetWeight, CumMetWeights[index]);
                cp.AddOrUpdate(DicomTag.GantryAngle, GantryInicial + CumGantryAngFx[index] * (GantryFinal-GantryInicial));
                var RDR1 = cp.GetSequence(DicomTag.ReferencedDoseReferenceSequence).First();
                RDR1.AddOrUpdate(DicomTag.CumulativeDoseReferenceCoefficient, CumMetWeights[index]);
                RDR1.AddOrUpdate(DicomTag.ReferencedDoseReferenceNumber, 1);
            }
            Beam1.AddOrUpdate(DicomTag.NumberOfControlPoints, cpSequence.Count());
            file.Save(@"\\fisica0\centro_de_datos2018\101_Cosas de\PABLO\TBI Dyn\out.dcm");
            InitializeComponent();
        }
    }
}
