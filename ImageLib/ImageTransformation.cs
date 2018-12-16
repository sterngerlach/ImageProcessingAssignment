
// ImageLib
// ImageGrayLevelTransformation.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLib
{
    public static class ImageTransformation
    {
        /// <summary>
        /// ガンマ補正
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] GammaCorrection(byte[,] sourceImage, double gammaValue)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            byte[,] destImage = new byte[imageWidth, imageHeight];

            byte[] conversionTable = Enumerable.Range(0, 256)
                .Select(pixelValue => 255.0 * Math.Pow(pixelValue / 255.0, 1.0 / gammaValue))
                .Select(pixelValue => (byte)Utility.Clamp(pixelValue, byte.MinValue, byte.MaxValue))
                .ToArray();

            for (int x = 0; x < imageWidth; ++x)
                for (int y = 0; y < imageHeight; ++y)
                    destImage[x, y] = conversionTable[sourceImage[x, y]];

            return destImage;
        }

        /// <summary>
        /// 画像のヒストグラムの作成
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>画像のヒストグラムの配列</returns>
        public static int[] Histogram(byte[,] sourceImage)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            int[] resultArray = new int[256];

            for (int x = 0; x < imageWidth; ++x)
                for (int y = 0; y < imageHeight; ++y)
                    resultArray[sourceImage[x, y]]++;

            return resultArray;
        }

        /// <summary>
        /// ヒストグラムの画像の作成
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="histImageHeight">ヒストグラムの画像の高さ</param>
        /// <returns>ヒストグラムの画像</returns>
        public static byte[,] HistogramImage(byte[,] sourceImage, int histImageHeight)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            int histImageWidth = 512;
            int[] imageHist = ImageTransformation.Histogram(sourceImage);
            int histMax = imageHist.Max();
            byte[,] destImage = new byte[histImageWidth, histImageHeight];

            imageHist = imageHist
                .Select(histValue => histValue * histImageHeight / histMax)
                .ToArray();

            for (int x = 0; x < histImageWidth; ++x) {
                for (int y = 0; y < histImageHeight; ++y) {
                    if (y < histImageHeight - imageHist[x / 2])
                        destImage[x, y] = 255;
                    else
                        destImage[x, y] = 0;
                }
            }

            return destImage;
        }

        /// <summary>
        /// ヒストグラムの平坦化
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>結果の画像の2次元配列</returns>
        public static byte[,] HistogramEqualization(byte[,] sourceImage)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            int imageSize = imageWidth * imageHeight;
            byte[,] destImage = new byte[imageWidth, imageHeight];
            int[] imageHist = ImageTransformation.Histogram(sourceImage);
            
            for (int i = 1; i < imageHist.Length; ++i)
                imageHist[i] = imageHist[i] + imageHist[i - 1];

            byte[] conversionTable = imageHist
                .Select(accumValue => accumValue * 255.0 / imageSize)
                .Select(accumValue => (byte)Utility.Clamp(accumValue, byte.MinValue, byte.MaxValue))
                .ToArray();

            for (int x = 0; x < imageWidth; ++x)
                for (int y = 0; y < imageHeight; ++y)
                    destImage[x, y] = conversionTable[sourceImage[x, y]];

            return destImage;
        }
    }
}
