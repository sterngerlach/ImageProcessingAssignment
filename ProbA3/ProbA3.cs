
// ProbA3.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageLib;

namespace ProbA3
{
    public class ProbA3
    {
        public static void Main(string[] args)
        {
            Console.Write("Input image file name: ");
            string fileName = Console.ReadLine();

            byte[,] sourceImage = Utility.LoadGrayscaleImage(fileName);
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);

            ProbA3.BayerDitheringAndSave(fileName, sourceImage);
            ProbA3.SpiralDitheringAndSave(fileName, sourceImage);
            ProbA3.HalftoneDitheringAndSave(fileName, sourceImage);
            ProbA3.RandomDitheringAndSave(fileName, sourceImage);
            ProbA3.ErrorDiffusionDitheringDefaultAndSave(fileName, sourceImage);
            ProbA3.FloydSteinbergDitheringAndSave(fileName, sourceImage);
            ProbA3.JarvisJudiceNinkeDitheringAndSave(fileName, sourceImage);
            ProbA3.StuckiDitheringAndSave(fileName, sourceImage);
            ProbA3.BurkesDitheringAndSave(fileName, sourceImage);
            ProbA3.SierraDitheringAndSave(fileName, sourceImage);
            ProbA3.TwoRowSierraDitheringAndSave(fileName, sourceImage);
            ProbA3.SierraLiteDitheringAndSave(fileName, sourceImage);

            Console.ReadKey();
        }

        public static string NewFilePath(string sourceFilePath, string operationName)
        {
            string newFileName = Path.GetFileNameWithoutExtension(sourceFilePath)
                + $"-{operationName}" + Path.GetExtension(sourceFilePath);

            return Path.Combine(Path.GetDirectoryName(sourceFilePath), newFileName);
        }

        public static void BayerDitheringAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.BayerDithering(sourceImage);
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "BayerDithering");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Bayer dithering done and saved to \'{newFilePath}\'");
        }

        public static void SpiralDitheringAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.SpiralDithering(sourceImage);
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "SpiralDithering");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Spiral dithering done and saved to \'{newFilePath}\'");
        }

        public static void HalftoneDitheringAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.HalftoneDithering(sourceImage);
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "HalftoneDithering");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Halftone dithering done and saved to \'{newFilePath}\'");
        }

        public static void RandomDitheringAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.RandomDithering(sourceImage, new Random());
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "RandomDithering");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Random dithering done and saved to \'{newFilePath}\'");
        }

        public static void ErrorDiffusionDitheringDefaultAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.ErrorDiffusionDitheringDefault(sourceImage);
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "ErrorDiffusionDitheringDefault");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Error diffusion dithering done and saved to \'{newFilePath}\'");
        }

        public static void FloydSteinbergDitheringAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.FloydSteinbergDithering(sourceImage);
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "FloydSteinbergDithering");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Floyd-Steinberg dithering done and saved to \'{newFilePath}\'");
        }

        public static void JarvisJudiceNinkeDitheringAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.JarvisJudiceNinkeDithering(sourceImage);
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "JarvisJudiceNinkeDithering");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Jarvis, Judice & Ninke dithering done and saved to \'{newFilePath}\'");
        }

        public static void StuckiDitheringAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.StuckiDithering(sourceImage);
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "StuckiDithering");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Stucki dithering done and saved to \'{newFilePath}\'");
        }

        public static void BurkesDitheringAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.BurkesDithering(sourceImage);
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "BurkesDithering");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Burkes dithering done and saved to \'{newFilePath}\'");
        }

        public static void SierraDitheringAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.SierraDithering(sourceImage);
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "SierraDithering");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Sierra dithering done and saved to \'{newFilePath}\'");
        }

        public static void TwoRowSierraDitheringAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.TwoRowSierraDithering(sourceImage);
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "TwoRowSierraDithering");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Two-row Sierra dithering done and saved to \'{newFilePath}\'");
        }

        public static void SierraLiteDitheringAndSave(string sourceFilePath, byte[,] sourceImage)
        {
            byte[,] resultImage = ImageHalftoning.SierraLiteDithering(sourceImage);
            string newFilePath = ProbA3.NewFilePath(sourceFilePath, "SierraLiteDithering");
            Utility.SaveGrayscaleImage(resultImage, newFilePath);

            Console.WriteLine($"Sierra lite dithering done and saved to \'{newFilePath}\'");
        }
    }
}
