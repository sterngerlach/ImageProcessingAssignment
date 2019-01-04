
// ProbE.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageLib;

namespace ProbE
{
    public class ProbE
    {
        public static void Main(string[] args)
        {
            Console.Write("Input image file name: ");
            string fileName = Console.ReadLine();

            byte[,,] sourceImage = Utility.LoadColorImage(fileName);

            Console.Write("Input the threshold: ");
            double diffThreshold = double.Parse(Console.ReadLine());
            ProbE.RegionUnificationMethodAndSave(fileName, sourceImage, diffThreshold);
            
            Console.Write("Input the number of the clusters: ");
            int numOfClasses = int.Parse(Console.ReadLine());
            Console.Write("Weight of position: ");
            double weightOfPos = double.Parse(Console.ReadLine());
            ProbE.KMeansMethodAndSave(fileName, sourceImage, numOfClasses, weightOfPos);
        
            Console.ReadKey();
        }

        public static string NewFilePath(string sourceFilePath, string operationName)
        {
            string newFileName = Path.GetFileNameWithoutExtension(sourceFilePath)
                + $"-{operationName}" + Path.GetExtension(sourceFilePath);

            return Path.Combine(Path.GetDirectoryName(sourceFilePath), newFileName);
        }

        public static void RegionUnificationMethodAndSave(
            string sourceFilePath, byte[,,] sourceImage, double averageDiffThreshold)
        {
            byte[,,] resultImage = ImageAreaDivision.RegionUnificationMethod(
                sourceImage, averageDiffThreshold);
            string newFilePath = ProbE.NewFilePath(
                sourceFilePath, $"RegionUnificationMethod-{averageDiffThreshold}");
            Utility.SaveColorImage(resultImage, newFilePath);

            Console.WriteLine($"Region unification method done and saved to \'{newFilePath}\'");
        }

        public static void KMeansMethodAndSave(
            string sourceFilePath, byte[,,] sourceImage, int numOfClasses, double weightOfPos)
        {
            byte[,,] resultImage = ImageAreaDivision.KMeansMethod(
                sourceImage, numOfClasses, weightOfPos, new Random());
            string newFilePath = ProbE.NewFilePath(
                sourceFilePath, $"KMeansMethod-{numOfClasses}-{weightOfPos}");
            Utility.SaveColorImage(resultImage, newFilePath);

            Console.WriteLine($"K-means method done and saved to \'{newFilePath}\'");
        }
    }
}
