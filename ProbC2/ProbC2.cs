
// ProbC2.cs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageLib;

namespace ProbC2
{
    public class ProbC2
    {
        public static void Main(string[] args)
        {
            Console.Write("Input base image file name: ");
            string fileName = Console.ReadLine();

            byte[,,] sourceImage = Utility.LoadColorImage(fileName);

            List<byte[,,]> otherImages = new List<byte[,,]>();
            List<List<Point>> baseImagePosList = new List<List<Point>>();
            List<List<Point>> otherImagePosList = new List<List<Point>>();
            List<List<Point>> obstaclePosList = new List<List<Point>>();

            Console.Write("Input the obstacle area in base image: ");
            int[] obstacleArea = Console.ReadLine().Split(' ')
                .Select(str => int.Parse(str)).ToArray();
            obstaclePosList.Add(new List<Point>() {
                new Point(obstacleArea[0], obstacleArea[1]),
                new Point(obstacleArea[2], obstacleArea[3]),
                new Point(obstacleArea[4], obstacleArea[5]),
                new Point(obstacleArea[6], obstacleArea[7]) });

            Console.Write("Input the number of other images: ");
            int numOfOtherImages = int.Parse(Console.ReadLine());
            
            for (int i = 0; i < numOfOtherImages; ++i) {
                Console.Write($"Input image {i} file name: ");
                string otherFileName = Console.ReadLine();

                otherImages.Add(Utility.LoadColorImage(otherFileName));

                Console.Write($"Input the number of corresponding points between base image and image {i}: ");
                int numOfPoints = int.Parse(Console.ReadLine());

                List<Point> baseImagePos = new List<Point>();
                List<Point> otherImagePos = new List<Point>();
                List<Point> obstaclePos = new List<Point>();

                for (int j = 0; j < numOfPoints; ++j) {
                    Console.Write($"Input the source point {j}: ");
                    int[] sourcePos = Console.ReadLine().Split(' ')
                        .Select(str => int.Parse(str)).ToArray();
                    baseImagePos.Add(new Point(sourcePos[0], sourcePos[1]));

                    Console.Write($"Input the dest point {j}: ");
                    int[] destPos = Console.ReadLine().Split(' ')
                        .Select(str => int.Parse(str)).ToArray();
                    otherImagePos.Add(new Point(destPos[0], destPos[1]));
                }
                
                Console.Write($"Input the obstacle area in image {i}: ");
                obstacleArea = Console.ReadLine().Split(' ')
                    .Select(str => int.Parse(str)).ToArray();
                obstaclePos.Add(new Point(obstacleArea[0], obstacleArea[1]));
                obstaclePos.Add(new Point(obstacleArea[2], obstacleArea[3]));
                obstaclePos.Add(new Point(obstacleArea[4], obstacleArea[5]));
                obstaclePos.Add(new Point(obstacleArea[6], obstacleArea[7]));

                baseImagePosList.Add(baseImagePos);
                otherImagePosList.Add(otherImagePos);
                obstaclePosList.Add(obstaclePos);
            }

            ProbC2.ProbC2AndSave(fileName, sourceImage, otherImages,
                baseImagePosList, otherImagePosList, obstaclePosList);

            Console.ReadKey();
        }

        public static string NewFilePath(string sourceFilePath, string operationName)
        {
            string newFileName = Path.GetFileNameWithoutExtension(sourceFilePath)
                + $"-{operationName}" + Path.GetExtension(sourceFilePath);

            return Path.Combine(Path.GetDirectoryName(sourceFilePath), newFileName);
        }

        public static void ProbC2AndSave(
            string sourceFilePath, byte[,,] sourceImage, List<byte[,,]> otherImages,
            List<List<Point>> baseImagePosList, List<List<Point>> otherImagePosList,
            List<List<Point>> obstaclePosList)
        {
            byte[,,] resultImage = ImageMosaicing.RemoveObstacleFromImage(
                sourceImage, otherImages,
                baseImagePosList, otherImagePosList, obstaclePosList);
            string newFilePath = ProbC2.NewFilePath(sourceFilePath, $"ProbC2");
            Utility.SaveColorImage(resultImage, newFilePath);

            Console.WriteLine($"ProbC2 done and saved to \'{newFilePath}\'");
        }
    }
}
