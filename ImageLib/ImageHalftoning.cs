
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
    }
}
