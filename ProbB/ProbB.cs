
// ProbB.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageLib;

namespace ProbB
{
    public class ProbB
    {
        public static void Main(string[] args)
        {
            Console.Write("Input image file name: ");
            string fileName = Console.ReadLine();

            byte[,] sourceImage = Utility.LoadGrayscaleImage(fileName);

            ProbB.GammaCorrectionAndSave(fileName, sourceImage, 3.0);
            ProbB.GammaCorrectionAndSave(fileName, sourceImage, 2.0);
            ProbB.GammaCorrectionAndSave(fileName, sourceImage, 1.5);
            ProbB.GammaCorrectionAndSave(fileName, sourceImage, 1.0);
            ProbB.GammaCorrectionAndSave(fileName, sourceImage, 0.66);
            ProbB.GammaCorrectionAndSave(fileName, sourceImage, 0.5);
            ProbB.GammaCorrectionAndSave(fileName, sourceImage, 0.33);

            ProbB.CreateHistogramImageAndSave(fileName, sourceImage, "Before");
            byte[,] resultImage = ProbB.HistogramEqualizationAndSave(fileName, sourceImage);
            ProbB.CreateHistogramImageAndSave(fileName, resultImage, "After");

            Console.ReadKey();
        }

        public static string NewFilePath(string sourceFilePath, string operationName)
        {
            string newFileName = Path.GetFileNameWithoutExtension(sourceFilePath)
                + $"-{operationName}" + Path.GetExtension(sourceFilePath);

            return Path.Combine(Path.GetDirectoryName(sourceFilePath), newFileName);
        }

        public static void GammaCorrectionAndSave(string sourceFilePath, byte[,] sourceImage, double gammaValue)
        {
            byte[,] resultImage = ImageTransformation.GammaCorrection(sourceImage, gammaValue);
            string newFilePath = ProbB.NewFilePath(sourceFilePath, $"GammaCorrection-{gammaValue}");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Gamma correction (gamma: {gammaValue}) done and saved to \'{newFilePath}\'");
        }

        public static void CreateHistogramImageAndSave(string sourceFilePath, byte[,] sourceImage, string imageCaption)
        {
            byte[,] resultImage = ImageTransformation.HistogramImage(sourceImage, 256);
            string newFilePath = ProbB.NewFilePath(sourceFilePath, $"HistogramImage{imageCaption}");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Histogram image created and saved to \'{newFilePath}\'");
        }

        public static byte[,] HistogramEqualizationAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageTransformation.HistogramEqualization(sourceImage);
            string newFilePath = ProbB.NewFilePath(sourceFilePath, $"HistogramEqualization");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Histogram equalization done and saved to \'{newFilePath}\'");

            return resultImage;
        }
    }
}
