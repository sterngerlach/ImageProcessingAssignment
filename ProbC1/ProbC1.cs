
// ProbC1.cs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageLib;


namespace ProbC
{
    public class ProbC1
    {
        public static void Main(string[] args)
        {
            Console.Write("Input image file name: ");
            string fileName = Console.ReadLine();

            byte[,,] sourceImage = Utility.LoadColorImage(fileName);

            List<Point> sourcePosList = new List<Point>();
            List<Point> destPosList = new List<Point>();

            Console.Write("Input the number of corresponding points: ");
            int numOfPoints = int.Parse(Console.ReadLine());

            for (int i = 0; i < numOfPoints; ++i) {
                Console.Write($"Input the source point {i}: ");
                string[] sourcePos = Console.ReadLine().Split(' ');
                sourcePosList.Add(new Point(int.Parse(sourcePos[0]), int.Parse(sourcePos[1])));

                Console.Write($"Input the destination point {i}: ");
                string[] destPos = Console.ReadLine().Split(' ');
                destPosList.Add(new Point(int.Parse(destPos[0]), int.Parse(destPos[1])));
            }

            ProbC1.ProbC1AndSave(fileName, sourceImage, sourcePosList, destPosList);

            Console.ReadKey();
        }

        public static string NewFilePath(string sourceFilePath, string operationName)
        {
            string newFileName = Path.GetFileNameWithoutExtension(sourceFilePath)
                + $"-{operationName}" + Path.GetExtension(sourceFilePath);

            return Path.Combine(Path.GetDirectoryName(sourceFilePath), newFileName);
        }

        public static void ProbC1AndSave(
            string sourceFilePath, byte[,,] sourceImage,
            List<Point> sourcePosList, List<Point> destPosList)
        {
            byte[,,] resultImage = ImageMosaicing.ApplyProjectiveTransformation(
                sourceImage, sourcePosList, destPosList);
            string newFilePath = ProbC1.NewFilePath(sourceFilePath, $"ProbC1");
            Utility.SaveColorImage(resultImage, newFilePath);

            Console.WriteLine($"ProbC1 done and saved to \'{newFilePath}\'");
        }
    }
}
