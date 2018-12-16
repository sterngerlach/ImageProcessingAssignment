
// ImageLib
// ImgaeHalftoning.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLib
{
    public static class ImageHalftoning
    {
        /// <summary>
        /// ベイヤー型(ドット分散型)(サイズ4)
        /// </summary>
        private static readonly byte[,] BayerMatrix4x4 = new byte[4, 4] {
            {  0,  8,  2, 10 },
            { 12,  4, 14,  6 },
            {  3, 11,  1,  9 },
            { 15,  7, 13,  5 }
        };

        /// <summary>
        /// 渦巻き型(サイズ4)
        /// </summary>
        private static readonly byte[,] SpiralMatrix4x4 = new byte[4, 4] {
            {  9,  8,  7,  6 },
            { 10,  1,  0,  5 },
            { 11,  2,  3,  4 },
            { 12, 13, 14, 15 }
        };

        /// <summary>
        /// 網点型(ドット集中型)(サイズ4)
        /// </summary>
        private static readonly byte[,] HalftoneMatrix4x4 = new byte[4, 4] {
            {  0,  2, 14, 12 },
            {  8, 10,  5,  7 },
            { 15, 13,  1,  3 },
            {  4,  6,  9, 11 }
        };

        /// <summary>
        /// 組織的ディザ法(ベイヤー型)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] BayerDithering(byte[,] sourceImage)
            => ImageHalftoning.OrderedDithering(sourceImage, ImageHalftoning.BayerMatrix4x4);

        /// <summary>
        /// 組織的ディザ法(渦巻き型)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] SpiralDithering(byte[,] sourceImage)
            => ImageHalftoning.OrderedDithering(sourceImage, ImageHalftoning.SpiralMatrix4x4);

        /// <summary>
        /// 組織的ディザ法(網点型)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] HalftoneDithering(byte[,] sourceImage)
            => ImageHalftoning.OrderedDithering(sourceImage, ImageHalftoning.HalftoneMatrix4x4);

        /// <summary>
        /// 組織的ディザ法
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="ditherPattern">ディザパターンの行列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] OrderedDithering(byte[,] sourceImage, byte[,] ditherPattern)
        {
            Debug.Assert(ditherPattern.GetLength(0) == 4);
            Debug.Assert(ditherPattern.GetLength(1) == 4);

            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            byte[,] destImage = new byte[imageWidth, imageHeight];
            byte patternValue;

            for (int x = 0; x < imageWidth; ++x) {
                for (int y = 0; y < imageHeight; ++y) {
                    patternValue = ditherPattern[x % 4, y % 4];

                    if (sourceImage[x, y] >= patternValue * 16 + 8)
                        destImage[x, y] = 255;
                    else
                        destImage[x, y] = 0;
                }
            }

            return destImage;
        }

        /// <summary>
        /// ランダムディザ法
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="random">擬似乱数生成器</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] RandomDithering(byte[,] sourceImage, Random random)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            byte[,] destImage = new byte[imageWidth, imageHeight];
            byte randomValue;

            for (int x = 0; x < imageWidth; ++x) {
                for (int y = 0; y < imageHeight; ++y) {
                    randomValue = (byte)random.Next(0, byte.MaxValue + 1);

                    if (sourceImage[x, y] >= randomValue)
                        destImage[x, y] = 255;
                    else
                        destImage[x, y] = 0;
                }
            }

            return destImage;
        }

        /// <summary>
        /// 誤差拡散のパターン(教科書の数値)
        /// </summary>
        private static readonly int[,] ErrorDiffusionPatternDefault = new int[2, 3] {
            { 0, 0, 5 },
            { 3, 5, 3 }
        };

        /// <summary>
        /// Floyd-Steinberg型の誤差拡散のパターン
        /// </summary>
        private static readonly int[,] FloydSteinbergDiffusionPattern = new int[2, 3] {
            { 0, 0, 7 },
            { 1, 3, 5 }
        };

        /// <summary>
        /// Jarvis, Judice & Ninke型の誤差拡散のパターン
        /// </summary>
        private static readonly int[,] JarvisJudiceNinkeDiffusionPattern = new int[3, 5] {
            { 0, 0, 0, 7, 5 },
            { 3, 5, 7, 5, 3 },
            { 1, 3, 5, 3, 1 }
        };

        /// <summary>
        /// Stucki型の誤差拡散のパターン
        /// </summary>
        private static readonly int[,] StuckiDiffusionPattern = new int[3, 5] {
            { 0, 0, 0, 8, 4 },
            { 2, 4, 8, 4, 2 },
            { 1, 2, 4, 2, 1 }
        };

        /// <summary>
        /// Burkes型の誤差拡散のパターン
        /// </summary>
        private static readonly int[,] BurkesDiffusionPattern = new int[2, 5] {
            { 0, 0, 0, 8, 4 },
            { 2, 4, 8, 4, 2 }
        };

        /// <summary>
        /// Sierra型の誤差拡散のパターン
        /// </summary>
        private static readonly int[,] SierraDiffusionPattern = new int[3, 5] {
            { 0, 0, 0, 5, 3 },
            { 2, 4, 5, 4, 2 },
            { 0, 2, 3, 2, 0 }
        };

        /// <summary>
        /// Two-Row Sierra型の誤差拡散のパターン
        /// </summary>
        private static readonly int[,] TwoRowSierraDiffusionPattern = new int[2, 5] {
            { 0, 0, 0, 4, 3 },
            { 1, 2, 3, 2, 1 }
        };

        /// <summary>
        /// Sierra Lite型の誤差拡散のパターン
        /// </summary>
        private static readonly int[,] SierraLiteDiffusionPattern = new int[2, 3] {
            { 0, 0, 2 },
            { 1, 1, 0 }
        };

        /// <summary>
        /// 誤差拡散法(教科書の数値)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の2次元配列</returns>
        public static byte[,] ErrorDiffusionDitheringDefault(byte[,] sourceImage)
            => ImageHalftoning.ErrorDiffusionDithering(sourceImage, ImageHalftoning.ErrorDiffusionPatternDefault, 1);

        /// <summary>
        /// 誤差拡散法(Floyd-Steinberg型)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] FloydSteinbergDithering(byte[,] sourceImage)
            => ImageHalftoning.ErrorDiffusionDithering(sourceImage, ImageHalftoning.FloydSteinbergDiffusionPattern, 1);

        /// <summary>
        /// 誤差拡散法(Jarvis, Judice & Ninke型)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] JarvisJudiceNinkeDithering(byte[,] sourceImage)
            => ImageHalftoning.ErrorDiffusionDithering(sourceImage, ImageHalftoning.JarvisJudiceNinkeDiffusionPattern, 2);

        /// <summary>
        /// 誤差拡散法(Stucki型)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] StuckiDithering(byte[,] sourceImage)
            => ImageHalftoning.ErrorDiffusionDithering(sourceImage, ImageHalftoning.StuckiDiffusionPattern, 2);

        /// <summary>
        /// 誤差拡散法(Burkes型)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] BurkesDithering(byte[,] sourceImage)
            => ImageHalftoning.ErrorDiffusionDithering(sourceImage, ImageHalftoning.BurkesDiffusionPattern, 2);

        /// <summary>
        /// 誤差拡散法(Sierra型)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] SierraDithering(byte[,] sourceImage)
            => ImageHalftoning.ErrorDiffusionDithering(sourceImage, ImageHalftoning.SierraDiffusionPattern, 2);

        /// <summary>
        /// 誤差拡散法(Two-Row Sierra型)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] TwoRowSierraDithering(byte[,] sourceImage)
            => ImageHalftoning.ErrorDiffusionDithering(sourceImage, ImageHalftoning.TwoRowSierraDiffusionPattern, 2);

        /// <summary>
        /// 誤差拡散法(Sierra Lite型)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] SierraLiteDithering(byte[,] sourceImage)
            => ImageHalftoning.ErrorDiffusionDithering(sourceImage, ImageHalftoning.SierraLiteDiffusionPattern, 1);

        /// <summary>
        /// 誤差拡散法
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="errorDiffusionPattern">誤差拡散のパターンの2次元配列</param>
        /// <param name="centerPositionX">誤差拡散のパターンにおける中心画素の位置</param>
        /// <returns>結果の2次元配列</returns>
        public static byte[,] ErrorDiffusionDithering(
            byte[,] sourceImage, int[,] errorDiffusionPattern, int centerPositionX)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            int patternWidth = errorDiffusionPattern.GetLength(1);
            int patternHeight = errorDiffusionPattern.GetLength(0);
            int patternValueSum = errorDiffusionPattern.Cast<int>().Sum();

            byte[,] destImage = new byte[imageWidth, imageHeight];
            int[,] errorDiffusion = new int[patternHeight, imageWidth + patternWidth - 1];
            int pixelValue;
            int errorValue;

            for (int y = 0; y < imageHeight; ++y) {
                for (int x = 0; x < imageWidth; ++x) {
                    pixelValue = sourceImage[x, y] + errorDiffusion[0, x + centerPositionX] / patternValueSum;

                    if (pixelValue >= 128)
                        destImage[x, y] = 255;
                    else
                        destImage[x, y] = 0;

                    errorValue = pixelValue - destImage[x, y];

                    for (int n = 0; n < patternHeight; ++n)
                        for (int m = 0; m < patternWidth; ++m)
                            errorDiffusion[n, x + m] += errorValue * errorDiffusionPattern[n, m];
                }

                for (int m = 0; m < imageWidth + patternWidth - 1; ++m) {
                    for (int n = 0; n < patternHeight - 1; ++n)
                        errorDiffusion[n, m] = errorDiffusion[n + 1, m];

                    errorDiffusion[patternHeight - 1, m] = 0;
                }
            }

            return destImage;
        }
    }
}
