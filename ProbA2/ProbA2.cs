
// ProbA2.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageLib;

namespace ProbA2
{
    public class ProbA2
    {
        public static void Main(string[] args)
        {
            Console.Write("Input image file name: ");
            string fileName = Console.ReadLine();

            byte[,] noiseImage = Utility.LoadGrayscaleImage(fileName);
            int imageWidth = noiseImage.GetLength(0);
            int imageHeight = noiseImage.GetLength(1);

            /*
            byte[,] noiseImage = ProbA2.AddShotNoiseAndSave(
                fileName, sourceImage, (int)(imageWidth * imageHeight * 0.05), 255.0);
            byte[,] noiseImage = ProbA2.AddRandomNoiseAndSave(fileName, sourceImage, 16.0);
            */

            ProbA2.ApplyAveragingFilterAndSave(fileName, noiseImage, 5);
            ProbA2.ApplyMedianFilterAndSave(fileName, noiseImage, 5);

            byte[,] resultImage = ProbA2.ApplyBilateralFilterAndSave(
                fileName, noiseImage, 5, 5.0, 5.0, 1);
            resultImage = ProbA2.ApplyBilateralFilterAndSave(
                fileName, resultImage, 5, 5.0, 5.0, 2);
            resultImage = ProbA2.ApplyBilateralFilterAndSave(
                fileName, resultImage, 5, 5.0, 5.0, 3);

            resultImage = ProbA2.ApplyBilateralFilterAndSave(
                fileName, noiseImage, 5, 10.0, 10.0, 1);
            resultImage = ProbA2.ApplyBilateralFilterAndSave(
                fileName, resultImage, 5, 10.0, 10.0, 2);
            resultImage = ProbA2.ApplyBilateralFilterAndSave(
                fileName, resultImage, 5, 10.0, 10.0, 3);

            resultImage = ProbA2.ApplyBilateralFilterAndSave(
                fileName, noiseImage, 5, 15.0, 15.0, 1);
            resultImage = ProbA2.ApplyBilateralFilterAndSave(
                fileName, resultImage, 5, 15.0, 15.0, 2);
            resultImage = ProbA2.ApplyBilateralFilterAndSave(
                fileName, resultImage, 5, 15.0, 15.0, 3);

            Console.ReadKey();
        }

        public static string NewFilePath(string sourceFilePath, string operationName)
        {
            string newFileName = Path.GetFileNameWithoutExtension(sourceFilePath)
                + $"-{operationName}" + Path.GetExtension(sourceFilePath);

            return Path.Combine(Path.GetDirectoryName(sourceFilePath), newFileName);
        }

        public static byte[,] AddRandomNoiseAndSave(
            string sourceFilePath, byte[,] sourceImage, double noiseLevel)
        {
            byte[,] resultImage = Utility.AddRandomNoise(sourceImage, new Random(), noiseLevel);
            string newFilePath = ProbA2.NewFilePath(
                sourceFilePath, $"RandomNoise{noiseLevel}");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Random noise (level: {noiseLevel}) added " +
                              $"and saved to \'{newFilePath}\'");

            return resultImage;
        }

        public static byte[,] AddShotNoiseAndSave(
            string sourceFilePath, byte[,] sourceImage, int noiseNum, double noiseLevel)
        {
            byte[,] resultImage = Utility.AddShotNoise(
                sourceImage, new Random(), noiseNum, noiseLevel);
            string newFilePath = ProbA2.NewFilePath(
                sourceFilePath, $"ShotNoise{noiseLevel}");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Shot noise (count: {noiseNum}, level: {noiseLevel}) added " +
                              $"and saved to \'{newFilePath}\'");

            return resultImage;
        }

        public static void ApplyAveragingFilterAndSave(
            string sourceFilePath, byte[,] sourceImage, int filterSize)
        {
            byte[,] resultImage = ImageFiltering.ApplyAveragingFilter(sourceImage, filterSize);
            string newFilePath = ProbA2.NewFilePath(sourceFilePath, $"AveragingFilter{filterSize}");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Averaging filter (size: {filterSize}) " +
                              $"applied and saved to \'{newFilePath}\'");
        }

        public static void ApplyMedianFilterAndSave(
            string sourceFilePath, byte[,] sourceImage, int filterSize)
        {
            byte[,] resultImage = ImageFiltering.ApplyMedianFilter(sourceImage, filterSize);
            string newFilePath = ProbA2.NewFilePath(sourceFilePath, $"MedianFilter{filterSize}");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Median filter (size: {filterSize}) " +
                              $"applied and saved to \'{newFilePath}\'");
        }

        public static byte[,] ApplyBilateralFilterAndSave(
            string sourceFilePath, byte[,] sourceImage, int filterSize,
            double sigma1, double sigma2, int times)
        {
            byte[,] resultImage = ImageFiltering.ApplyBilateralFilter(
                sourceImage, filterSize, sigma1, sigma2);
            string newFilePath = ProbA2.NewFilePath(
                sourceFilePath, $"BilateralFilter{filterSize}-{sigma1}-{sigma2}-{times}");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Bilateral filter (size: {filterSize}) " +
                              $"applied and saved to \'{newFilePath}\'");

            return resultImage;
        }
    }
}
