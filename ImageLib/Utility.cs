
// ImageLib
// Utility.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLib
{
    public static class Utility
    {
        /// <summary>
        /// 値を指定した範囲内に収める
        /// </summary>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
            => (val.CompareTo(min) < 0) ? min : (val.CompareTo(max) > 0) ? max : val;

        /// <summary>
        /// 指定された数値が2の階乗かどうか判断する
        /// </summary>
        public static bool IsPowerOfTwo(this int val)
            => (val != 0) && ((val & (val - 1)) == 0);

        /// <summary>
        /// 中央値を求める
        /// </summary>
        public static byte Median(this IEnumerable<byte> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            
            byte[] sortedValues = source.OrderBy(n => n).ToArray();

            if (sortedValues.Length == 0)
                throw new InvalidOperationException();

            return (sortedValues.Length % 2 == 0) ?
                (byte)((sortedValues[sortedValues.Length / 2 - 1] +
                    sortedValues[sortedValues.Length / 2]) / 2.0) :
                sortedValues[sortedValues.Length / 2];
        }

        /// <summary>
        /// グレースケール画像の読み込み
        /// </summary>
        /// <param name="filePath">画像のファイル名</param>
        /// <returns>画像の2次元配列(横方向の座標, 縦方向の座標)</returns>
        public static byte[,] LoadGrayscaleImage(string filePath)
        {
            Bitmap image = new Bitmap(Image.FromFile(filePath));
            byte[,] imageData = new byte[image.Width, image.Height];

            for (int y = 0; y < image.Height; ++y) {
                for (int x = 0; x < image.Width; ++x) {
                    Color pixelColor = image.GetPixel(x, y);
                    imageData[x, y] = (byte)((pixelColor.R + pixelColor.G + pixelColor.B) / 3);
                }
            }

            return imageData;
        }

        /// <summary>
        /// カラー画像の読み込み
        /// </summary>
        /// <param name="filePath">画像のファイル名</param>
        /// <returns>画像の3次元配列(横方向の座標, 縦方向の画像, チャネル)</returns>
        public static byte[,,] LoadColorImage(string filePath)
        {
            Bitmap image = new Bitmap(Image.FromFile(filePath));
            byte[,,] imageData = new byte[image.Width, image.Height, 3];

            for (int y = 0; y < image.Height; ++y) {
                for (int x = 0; x < image.Width; ++x) {
                    Color pixelColor = image.GetPixel(x, y);
                    imageData[x, y, 0] = pixelColor.R;
                    imageData[x, y, 1] = pixelColor.G;
                    imageData[x, y, 2] = pixelColor.B;
                }
            }

            return imageData;
        }

        /// <summary>
        /// グレースケール画像の保存
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="filePath">画像のファイル名</param>
        public static void SaveGrayscaleImage(byte[,] sourceImage, string filePath)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            Bitmap newImage = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);

            for (int y = 0; y < imageHeight; ++y) {
                for (int x = 0; x < imageWidth; ++x) {
                    Color pixelColor = Color.FromArgb(
                        sourceImage[x, y], sourceImage[x, y], sourceImage[x, y]);
                    newImage.SetPixel(x, y, pixelColor);
                }
            }

            newImage.Save(filePath);
        }

        /// <summary>
        /// カラー画像の保存
        /// </summary>
        /// <param name="sourceImage">画像の3次元配列</param>
        /// <param name="filePath">画像のファイル名</param>
        public static void SaveColorImage(byte[,,] sourceImage, string filePath)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            Bitmap newImage = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);

            for (int y = 0; y < imageHeight; ++y) {
                for (int x = 0; x < imageWidth; ++x) {
                    Color pixelColor = Color.FromArgb(
                        sourceImage[x, y, 0], sourceImage[x, y, 1], sourceImage[x, y, 2]);
                    newImage.SetPixel(x, y, pixelColor);
                }
            }

            newImage.Save(filePath);
        }

        /// <summary>
        /// 画像上の指定された位置の画素値を最近傍法により取得
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="x">横方向の座標</param>
        /// <param name="y">縦方向の座標</param>
        /// <returns>指定された座標の画素値</returns>
        public static byte GetPixel(byte[,] sourceImage, int x, int y)
        {
            x = Clamp(x, 0, sourceImage.GetLength(0) - 1);
            y = Clamp(y, 0, sourceImage.GetLength(1) - 1);
            return sourceImage[x, y];
        }

        /// <summary>
        /// 画像上の指定された位置の画素値を設定
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="x">横方向の座標</param>
        /// <param name="y">縦方向の座標</param>
        public static void SetPixel(byte[,] sourceImage, int x, int y, byte newValue)
        {
            if (x < 0 || x >= sourceImage.GetLength(0))
                return;
            if (y < 0 || y >= sourceImage.GetLength(1))
                return;

            sourceImage[x, y] = newValue;
        }

        /// <summary>
        /// 画像にランダムノイズを付加
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="random">擬似乱数生成器</param>
        /// <param name="noiseLevel">ノイズレベルの最大値(0から255まで)</param>
        /// <returns>ノイズが付加された画像の2次元配列</returns>
        public static byte[,] AddRandomNoise(byte[,] sourceImage, Random random, double noiseLevel)
        {
            Debug.Assert(noiseLevel >= 0.0 && noiseLevel <= 255.0);

            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            byte[,] destImage = new byte[imageWidth, imageHeight];

            for (int x = 0; x < imageWidth; ++x) {
                for (int y = 0; y < imageHeight; ++y) {
                    destImage[x, y] = (byte)Clamp(
                        sourceImage[x, y] + (random.NextDouble() - 0.5) * noiseLevel * 2.0,
                        byte.MinValue, byte.MaxValue);
                }
            }

            return destImage;
        }

        /// <summary>
        /// 画像にショットノイズを付加
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="random">擬似乱数生成器</param>
        /// <param name="noiseNum">ショットノイズの個数</param>
        /// <param name="noiseLevel">ノイズレベルの最大値(0から255まで)</param>
        /// <returns>ノイズが付加された画像の2次元配列</returns>
        public static byte[,] AddShotNoise(
            byte[,] sourceImage, Random random, int noiseNum, double noiseLevel)
        {
            Debug.Assert(noiseLevel >= 0.0 && noiseLevel <= 255.0);

            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            byte[,] destImage = sourceImage.Clone() as byte[,];

            for (int i = 0; i < noiseNum; ++i) {
                int x = random.Next(0, imageWidth);
                int y = random.Next(0, imageHeight);
                destImage[x, y] = (byte)Clamp(
                    sourceImage[x, y] + random.NextDouble() * noiseLevel,
                    byte.MinValue, byte.MaxValue);
            }

            return destImage;
        }
    }
}
