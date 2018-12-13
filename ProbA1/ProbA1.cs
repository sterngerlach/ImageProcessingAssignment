
// ProbA1.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageLib;

namespace ProbA1
{
    public class ProbA1
    {
        public static void Main(string[] args)
        {
            Console.Write("Input image file name: ");
            string fileName = Console.ReadLine();

            byte[,] sourceImage = Utility.LoadGrayscaleImage(fileName);

            ProbA1.SaveGrayscaleImage(fileName, sourceImage);
            ProbA1.ApplyAveragingFilterAndSave(fileName, sourceImage, 3);
            ProbA1.ApplyAveragingFilterAndSave(fileName, sourceImage, 5);
            ProbA1.ApplyAveragingFilterAndSave(fileName, sourceImage, 9);
            ProbA1.ApplyGaussianFilterAndSave(fileName, sourceImage, 5, 1.5);
            ProbA1.ApplyDerivativeFilterAndSave(fileName, sourceImage);
            ProbA1.DetectEdgesAndSave(fileName, sourceImage, 9, 1.5);
            ProbA1.ApplySharpeningFilterAndSave(fileName, sourceImage, 3, 4);
            ProbA1.ApplySharpeningFilterAndSave(fileName, sourceImage, 3, 9);
            ProbA1.ApplySharpeningFilterAndSave(fileName, sourceImage, 3, 18);
            ProbA1.ApplyPrewittFilterAndSave(fileName, sourceImage);
            ProbA1.ApplySobelFilterAndSave(fileName, sourceImage);

            Console.ReadKey();
        }

        public static string NewFilePath(string sourceFilePath, string operationName)
        {
            string newFileName = Path.GetFileNameWithoutExtension(sourceFilePath)
                + $"-{operationName}" + Path.GetExtension(sourceFilePath);

            return Path.Combine(Path.GetDirectoryName(sourceFilePath), newFileName);
        }

        public static void SaveGrayscaleImage(string sourceFilePath, byte[,] grayscaleImage)
        {
            string newFilePath = ProbA1.NewFilePath(sourceFilePath, "Grayscale");
            Utility.SaveGrayscaleImage(grayscaleImage, newFilePath);

            Console.WriteLine($"Saved grayscale image to \'{newFilePath}\'");
        }

        public static void ApplyAveragingFilterAndSave(
            string sourceFilePath, byte[,] sourceImage, int filterSize)
        {
            byte[,] resultImage = ImageFiltering.ApplyAveragingFilter(sourceImage, filterSize);
            string newFilePath = ProbA1.NewFilePath(sourceFilePath, $"AveragingFilter{filterSize}");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Averaging filter (size: {filterSize}) " +
                              $"applied and saved to \'{newFilePath}\'");
        }

        public static void ApplyGaussianFilterAndSave(
            string sourceFilePath, byte[,] sourceImage, int filterSize, double standardDeviation)
        {
            byte[,] resultImage = ImageFiltering.ApplyGaussianFilter(
                sourceImage, filterSize, standardDeviation);
            string newFilePath = ProbA1.NewFilePath(
                sourceFilePath, $"GaussianFilter{filterSize}-{standardDeviation}");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Gaussian filter (size: {filterSize}, " +
                              $"standard deviation: {standardDeviation}) " +
                              $"applied and saved to \'{newFilePath}\'");
        }

        public static void ApplyDerivativeFilterAndSave(
            string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageFiltering.ApplyDerivativeFilter(sourceImage, true);
            string newFilePath = ProbA1.NewFilePath(sourceFilePath, "DerivativeFilter");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Derivative filter applied and saved to \'{newFilePath}\'");
        }

        public static void ApplyPrewittFilterAndSave(
            string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageFiltering.ApplyPrewittFilter(sourceImage, true);
            string newFilePath = ProbA1.NewFilePath(sourceFilePath, "PrewittFilter");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Prewitt filter applied and saved to \'{newFilePath}\'");
        }

        public static void ApplySobelFilterAndSave(
            string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageFiltering.ApplySobelFilter(sourceImage, true);
            string newFilePath = ProbA1.NewFilePath(sourceFilePath, "SobelFilter");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Sobel filter applied and saved to \'{newFilePath}\'");
        }

        public static void DetectEdgesAndSave(
            string sourceFilePath, byte[,] sourceImage, int filterSize, double standardDeviation)
        {
            byte[,] resultImage = ImageFiltering.DetectEdges(
                sourceImage, filterSize, standardDeviation);
            string newFilePath = ProbA1.NewFilePath(
                sourceFilePath, $"DetectEdges{filterSize}-{standardDeviation}");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Edge detection done and file saved to \'{newFilePath}\'");
        }

        public static void ApplySharpeningFilterAndSave(
            string sourceFilePath, byte[,] sourceImage, int filterSize, double k)
        {
            byte[,] resultImage = ImageFiltering.ApplySharpeningFilter(sourceImage, filterSize, k);
            string newFilePath = ProbA1.NewFilePath(
                sourceFilePath, $"SharpeningFilter{filterSize}-{k}");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Sharpening filter applied and saved to \'{newFilePath}\'");
        }
    }
}
