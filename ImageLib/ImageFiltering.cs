
// ImageLib
// ImageFiltering.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLib
{
    public static class ImageFiltering
    {
        /// <summary>
        /// 画像にフィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="filter">フィルタの2次元配列</param>
        /// <returns>フィルタ適用後の画像の2次元配列</returns>
        public static byte[,] ApplyFilter(byte[,] sourceImage, double[,] filter)
        {
            Debug.Assert(filter.GetLength(0) == filter.GetLength(1));
            Debug.Assert(filter.GetLength(0) % 2 == 1);

            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            int filterSize = filter.GetLength(0);
            double pixelValue;
            byte[,] destImage = new byte[imageWidth, imageHeight];

            for (int x = 0; x < imageWidth; ++x) {
                for (int y = 0; y < imageHeight; ++y) {
                    pixelValue = 0.0;

                    for (int m = -filterSize / 2; m <= filterSize / 2; ++m)
                        for (int n = -filterSize / 2; n <= filterSize / 2; ++n)
                            pixelValue += Utility.GetPixel(sourceImage, x + m, y + n) *
                                filter[m + filterSize / 2, n + filterSize / 2];

                    destImage[x, y] = (byte)Utility.Clamp(pixelValue, byte.MinValue, byte.MaxValue);
                }
            }

            return destImage;
        }

        /// <summary>
        /// 画像にフィルタを適用(画素値に負の値も含まれる)
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="filter">フィルタの2次元配列</param>
        /// <returns>フィルタ適用後の画像の2次元配列</returns>
        public static double[,] ApplyFilterAllowNegativeValues(byte[,] sourceImage, double[,] filter)
        {
            Debug.Assert(filter.GetLength(0) == filter.GetLength(1));
            Debug.Assert(filter.GetLength(0) % 2 == 1);

            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            int filterSize = filter.GetLength(0);
            double[,] destImage = new double[imageWidth, imageHeight];

            for (int x = 0; x < imageWidth; ++x) {
                for (int y = 0; y < imageHeight; ++y) {
                    for (int m = -filterSize / 2; m <= filterSize / 2; ++m) {
                        for (int n = -filterSize / 2; n <= filterSize / 2; ++n) {
                            destImage[x, y] += Utility.GetPixel(sourceImage, x + m, y + n) *
                                filter[m + filterSize / 2, n + filterSize / 2];
                        }
                    }
                }
            }

            return destImage;
        }

        /// <summary>
        /// フィルタを各要素の合計値が1となるように正規化
        /// </summary>
        /// <param name="filter">フィルタの2次元配列</param>
        /// <returns>正規化されたフィルタの2次元配列</returns>
        public static void NormalizeFilter(double[,] filter)
        {
            Debug.Assert(filter.GetLength(0) == filter.GetLength(1));
            Debug.Assert(filter.GetLength(0) % 2 == 1);

            double filterSum = filter.Cast<double>().Sum();

            for (int m = 0; m < filter.GetLength(0); ++m)
                for (int n = 0; n < filter.GetLength(1); ++n)
                    filter[m, n] /= filterSum;
        }

        /// <summary>
        /// 画像に平均化フィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="filterSize">フィルタのサイズ</param>
        /// <returns>フィルタ適用後の画像の2次元配列</returns>
        public static byte[,] ApplyAveragingFilter(byte[,] sourceImage, int filterSize)
        {
            Debug.Assert(filterSize % 2 == 1);

            double[,] filter = new double[filterSize, filterSize];

            for (int m = 0; m < filterSize; ++m)
                for (int n = 0; n < filterSize; ++n)
                    filter[m, n] = 1.0 / (filterSize * filterSize);

            return ImageFiltering.ApplyFilter(sourceImage, filter);
        }

        /// <summary>
        /// 画像にガウシアンフィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="filterSize">フィルタのサイズ</param>
        /// <param name="standardDeviation">ガウス分布の標準偏差</param>
        /// <returns>フィルタ適用後の画像の2次元配列</returns>
        public static byte[,] ApplyGaussianFilter(byte[,] sourceImage, int filterSize, double standardDeviation)
        {
            Debug.Assert(filterSize % 2 == 1);

            double[,] filter = new double[filterSize, filterSize];

            // ガウシアンフィルタを作成
            for (int m = 0; m < filterSize; ++m) {
                for (int n = 0; n < filterSize; ++n) {
                    double r = Math.Pow(m - (filterSize / 2), 2.0) + Math.Pow(n - (filterSize / 2), 2.0);
                    filter[m, n] = Math.Exp(-r / (2.0 * Math.Pow(standardDeviation, 2.0)));
                }
            }

            // フィルタの各要素の合計値が1となるように正規化
            ImageFiltering.NormalizeFilter(filter);

            return ImageFiltering.ApplyFilter(sourceImage, filter);
        }

        /// <summary>
        /// 画像に横方向の微分フィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>フィルタ適用後の画像の2次元配列</returns>
        public static double[,] ApplyHorizontalDerivativeFilter(byte[,] sourceImage)
        {
            double[,] filter = new double[3, 3] {
                {  0.0, 0.0, 0.0 },
                { -0.5, 0.0, 0.5 },
                {  0.0, 0.0, 0.0 }
            };

            return ImageFiltering.ApplyFilterAllowNegativeValues(sourceImage, filter);
        }

        /// <summary>
        /// 画像に縦方向の微分フィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>フィルタ適用後の画像の2次元配列</returns>
        public static double[,] ApplyVerticalDerivativeFilter(byte[,] sourceImage)
        {
            double[,] filter = new double[3, 3] {
                { 0.0,  0.5, 0.0 },
                { 0.0,  0.0, 0.0 },
                { 0.0, -0.5, 0.0 }
            };

            return ImageFiltering.ApplyFilterAllowNegativeValues(sourceImage, filter);
        }

        /// <summary>
        /// 画像に微分フィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="multiplyConstant">画像が見やすくなるように適当な定数倍を掛けるかどうか</param>
        /// <returns>エッジ強度画像の2次元配列</returns>
        public static byte[,] ApplyDerivativeFilter(byte[,] sourceImage, bool multiplyConstant)
        {
            byte[,] destImage = ImageFiltering.CreateGradientImage(
                ImageFiltering.ApplyHorizontalDerivativeFilter(sourceImage),
                ImageFiltering.ApplyVerticalDerivativeFilter(sourceImage));

            if (!multiplyConstant)
                return destImage;

            byte maxPixelValue = destImage.Cast<byte>().Max();
            int imageWidth = destImage.GetLength(0);
            int imageHeight = destImage.GetLength(1);

            // 見やすくなるように適当な定数倍を掛ける
            for (int x = 0; x < imageWidth; ++x)
                for (int y = 0; y < imageHeight; ++y)
                    destImage[x, y] = (byte)(destImage[x, y] * 255.0 / maxPixelValue);

            return destImage;
        }

        /// <summary>
        /// 画像に横方向のプリューウィットフィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>フィルタ適用後の画像の2次元配列</returns>
        public static double[,] ApplyHorizontalPrewittFilter(byte[,] sourceImage)
        {
            double[,] filter = new double[3, 3] {
                { -1.0 / 6.0, 0.0, 1.0 / 6.0 },
                { -1.0 / 6.0, 0.0, 1.0 / 6.0 },
                { -1.0 / 6.0, 0.0, 1.0 / 6.0 }
            };

            return ImageFiltering.ApplyFilterAllowNegativeValues(sourceImage, filter);
        }

        /// <summary>
        /// 画像に縦方向のプリューウィットフィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>フィルタ適用後の画像の2次元配列</returns>
        public static double[,] ApplyVerticalPrewittFilter(byte[,] sourceImage)
        {
            double[,] filter = new double[3, 3] {
                {  1.0 / 6.0,  1.0 / 6.0,  1.0 / 6.0 },
                {        0.0,        0.0,        0.0 },
                { -1.0 / 6.0, -1.0 / 6.0, -1.0 / 6.0 }
            };

            return ImageFiltering.ApplyFilterAllowNegativeValues(sourceImage, filter);
        }

        /// <summary>
        /// 画像にプリューウィットフィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="multiplyConstant">画像が見やすくなるように適当な定数倍を掛けるかどうか</param>
        /// <returns>エッジ強度画像の2次元配列</returns>
        public static byte[,] ApplyPrewittFilter(byte[,] sourceImage, bool multiplyConstant)
        {
            byte[,] destImage = ImageFiltering.CreateGradientImage(
                ImageFiltering.ApplyHorizontalPrewittFilter(sourceImage),
                ImageFiltering.ApplyVerticalPrewittFilter(sourceImage));

            if (!multiplyConstant)
                return destImage;

            byte maxPixelValue = destImage.Cast<byte>().Max();
            int imageWidth = destImage.GetLength(0);
            int imageHeight = destImage.GetLength(1);

            // 見やすくなるように適当な定数倍を掛ける
            for (int x = 0; x < imageWidth; ++x)
                for (int y = 0; y < imageHeight; ++y)
                    destImage[x, y] = (byte)(destImage[x, y] * 255.0 / maxPixelValue);

            return destImage;
        }

        /// <summary>
        /// 画像に横方向のソーベルフィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>フィルタ適用後の画像の2次元配列</returns>
        public static double[,] ApplyHorizontalSobelFilter(byte[,] sourceImage)
        {
            double[,] filter = new double[3, 3] {
                { -1.0 / 8.0, 0.0, 1.0 / 8.0 },
                { -2.0 / 8.0, 0.0, 2.0 / 8.0 },
                { -1.0 / 8.0, 0.0, 1.0 / 8.0 }
            };

            return ImageFiltering.ApplyFilterAllowNegativeValues(sourceImage, filter);
        }

        /// <summary>
        /// 画像に縦方向のソーベルフィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <returns>フィルタ適用後の画像の2次元配列</returns>
        public static double[,] ApplyVerticalSobelFilter(byte[,] sourceImage)
        {
            double[,] filter = new double[3, 3] {
                {  1.0 / 8.0,  2.0 / 8.0,  1.0 / 8.0 },
                {        0.0,        0.0,        0.0 },
                { -1.0 / 8.0, -2.0 / 8.0, -1.0 / 8.0 }
            };

            return ImageFiltering.ApplyFilterAllowNegativeValues(sourceImage, filter);
        }

        /// <summary>
        /// 画像にソーベルフィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="multiplyConstant">画像が見やすくなるように適当な定数倍を掛けるかどうか</param>
        /// <returns>エッジ強度画像の2次元配列</returns>
        public static byte[,] ApplySobelFilter(byte[,] sourceImage, bool multiplyConstant)
        {
            byte[,] destImage = ImageFiltering.CreateGradientImage(
                ImageFiltering.ApplyHorizontalSobelFilter(sourceImage),
                ImageFiltering.ApplyVerticalSobelFilter(sourceImage));

            if (!multiplyConstant)
                return destImage;

            byte maxPixelValue = destImage.Cast<byte>().Max();
            int imageWidth = destImage.GetLength(0);
            int imageHeight = destImage.GetLength(1);

            // 見やすくなるように適当な定数倍を掛ける
            for (int x = 0; x < imageWidth; ++x)
                for (int y = 0; y < imageHeight; ++y)
                    destImage[x, y] = (byte)(destImage[x, y] * 255.0 / maxPixelValue);

            return destImage;
        }

        /// <summary>
        /// エッジ強度画像を作成
        /// </summary>
        /// <param name="horizontalDerivative">横方向の微分画像</param>
        /// <param name="verticalDerivative">縦方向の微分画像</param>
        /// <returns>エッジ強度画像の2次元配列</returns>
        public static byte[,] CreateGradientImage(double[,] horizontalDerivative, double[,] verticalDerivative)
        {
            Debug.Assert(horizontalDerivative.Length == verticalDerivative.Length);
            Debug.Assert(horizontalDerivative.GetLength(0) == verticalDerivative.GetLength(0));

            int imageWidth = horizontalDerivative.GetLength(0);
            int imageHeight = verticalDerivative.GetLength(1);
            double pixelValue;
            byte[,] destImage = new byte[imageWidth, imageHeight];

            for (int x = 0; x < imageWidth; ++x) {
                for (int y = 0; y < imageHeight; ++y) {
                    pixelValue = Math.Sqrt(Math.Pow(horizontalDerivative[x, y], 2.0) +
                        Math.Pow(verticalDerivative[x, y], 2.0));
                    destImage[x, y] = (byte)Utility.Clamp(pixelValue, byte.MinValue, byte.MaxValue);
                }
            }

            return destImage;
        }

        /// <summary>
        /// 画像にLoG(Laplacian of Gaussian)フィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="filterSize">フィルタのサイズ</param>
        /// <param name="standardDeviation">ガウス分布の標準偏差</param>
        /// <returns>フィルタ適用後の画像の2次元配列</returns>
        public static double[,] ApplyLoGFilter(byte[,] sourceImage, int filterSize, double standardDeviation)
        {
            Debug.Assert(filterSize % 2 == 1);

            double[,] filter = new double[filterSize, filterSize];

            for (int m = 0; m < filterSize; ++m) {
                for (int n = 0; n < filterSize; ++n) {
                    double r = Math.Pow(m - (filterSize / 2), 2.0) + Math.Pow(n - (filterSize / 2), 2.0);
                    filter[m, n] = Math.Exp(-r / (2.0 * Math.Pow(standardDeviation, 2.0))) *
                        (r - 2.0 * Math.Pow(standardDeviation, 2.0)) /
                        (2.0 * Math.PI * Math.Pow(standardDeviation, 6.0));
                }
            }

            return ImageFiltering.ApplyFilterAllowNegativeValues(sourceImage, filter);
        }

        /// <summary>
        /// LoGフィルタを用いたゼロ交差法によりエッジを検出
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="filterSize">フィルタのサイズ</param>
        /// <param name="standardDeviation">ガウス分布の標準偏差</param>
        /// <returns>検出されたエッジを示す白黒画像</returns>
        public static byte[,] DetectEdges(byte[,] sourceImage, int filterSize, double standardDeviation)
        {
            double[,] zeroCrossingImage = ImageFiltering.ApplyLoGFilter(
                sourceImage, filterSize, standardDeviation);
            int imageWidth = zeroCrossingImage.GetLength(0);
            int imageHeight = zeroCrossingImage.GetLength(1);
            byte[,] destImage = new byte[imageWidth, imageHeight];

            for (int x = 0; x < imageWidth - 1; ++x) {
                for (int y = 0; y < imageHeight - 1; ++y) {
                    if (zeroCrossingImage[x, y] * zeroCrossingImage[x + 1, y] < 0.0 ||
                        zeroCrossingImage[x, y] * zeroCrossingImage[x, y + 1] < 0.0)
                        destImage[x, y] = 255;
                    else
                        destImage[x, y] = 0;
                }
            }

            return destImage;
        }

        /// <summary>
        /// 画像に鮮鋭化フィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="filterSize">フィルタのサイズ</param>
        /// <param name="k">鮮鋭化フィルタのパラメータ</param>
        /// <returns>フィルタを適用した画像の2次元配列</returns>
        public static byte[,] ApplySharpeningFilter(byte[,] sourceImage, int filterSize, double k)
        {
            Debug.Assert(filterSize % 2 == 1);

            double[,] filter = new double[filterSize, filterSize];

            for (int m = 0; m < filterSize; ++m) {
                for (int n = 0; n < filterSize; ++n) {
                    if (m == filterSize / 2 && n == filterSize / 2) {
                        filter[m, n] = 1.0 + k * ((filterSize * filterSize) - 1) /
                            (filterSize * filterSize);
                    } else {
                        filter[m, n] = -k / (filterSize * filterSize);
                    }
                }
            }

            return ImageFiltering.ApplyFilter(sourceImage, filter);
        }

        /// <summary>
        /// 画像にメディアンフィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="filterSize">フィルタのサイズ</param>
        /// <returns>フィルタを適用した画像の2次元配列</returns>
        public static byte[,] ApplyMedianFilter(byte[,] sourceImage, int filterSize)
        {
            Debug.Assert(filterSize % 2 == 1);

            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            byte[,] pixelValues = new byte[filterSize, filterSize];
            byte[,] destImage = new byte[imageWidth, imageHeight];

            for (int x = 0; x < imageWidth; ++x) {
                for (int y = 0; y < imageHeight; ++y) {
                    for (int m = -filterSize / 2; m <= filterSize / 2; ++m) {
                        for (int n = -filterSize / 2; n <= filterSize / 2; ++n) {
                            pixelValues[m + filterSize / 2, n + filterSize / 2] =
                                Utility.GetPixel(sourceImage, x + m, y + n);
                        }
                    }

                    destImage[x, y] = pixelValues.Cast<byte>().Median();
                }
            }

            return destImage;
        }

        /// <summary>
        /// バイラテラルフィルタの算出
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="filterSize">フィルタのサイズ</param>
        /// <param name="x">注目画素の横方向の座標</param>
        /// <param name="y">注目画素の縦方向の座標</param>
        /// <param name="sigma1">ガウシアンフィルタを制御するパラメータ</param>
        /// <param name="sigma2">注目画素との画素値の差を制御するパラメータ</param>
        /// <returns>指定された座標に対するフィルタの2次元配列</returns>
        public static double[,] CalculateBilateralFilter(
            byte[,] sourceImage, int filterSize,
            int x, int y, double sigma1, double sigma2)
        {
            Debug.Assert(filterSize % 2 == 1);
            Debug.Assert(x >= 0 && x < sourceImage.GetLength(0));
            Debug.Assert(y >= 0 && y < sourceImage.GetLength(1));

            double[,] filter = new double[filterSize, filterSize];

            for (int m = -filterSize / 2; m <= filterSize / 2; ++m) {
                for (int n = -filterSize / 2; n <= filterSize / 2; ++n) {
                    filter[m + filterSize / 2, n + filterSize / 2] =
                        Math.Exp(-(Math.Pow(m, 2.0) + Math.Pow(n, 2.0))
                            / (2.0 * Math.Pow(sigma1, 2.0))) *
                        Math.Exp(-Math.Pow(Utility.GetPixel(sourceImage, x, y) -
                            Utility.GetPixel(sourceImage, x + m, y + n), 2.0)
                            / (2.0 * Math.Pow(sigma2, 2.0)));
                }
            }

            ImageFiltering.NormalizeFilter(filter);

            return filter;
        }

        /// <summary>
        /// 画像にバイラテラルフィルタを適用
        /// </summary>
        /// <param name="sourceImage">画像の2次元配列</param>
        /// <param name="filterSize">フィルタのサイズ</param>
        /// <param name="sigma1">ガウシアンフィルタを制御するパラメータ</param>
        /// <param name="sigma2">注目画素との画素値の差を制御するパラメータ</param>
        /// <returns>フィルタを適用した画像の2次元配列</returns>
        public static byte[,] ApplyBilateralFilter(
            byte[,] sourceImage, int filterSize, double sigma1, double sigma2)
        {
            Debug.Assert(filterSize % 2 == 1);

            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            byte[,] destImage = new byte[imageWidth, imageHeight];
            double[,] filter;
            double pixelValue;

            for (int x = 0; x < imageWidth; ++x) {
                for (int y = 0; y < imageHeight; ++y) {
                    filter = ImageFiltering.CalculateBilateralFilter(
                        sourceImage, filterSize, x, y, sigma1, sigma2);
                    pixelValue = 0.0;

                    for (int m = -filterSize / 2; m <= filterSize / 2; ++m)
                        for (int n = -filterSize / 2; n <= filterSize / 2; ++n)
                            pixelValue += Utility.GetPixel(sourceImage, x + m, y + n) *
                                filter[m + filterSize / 2, n + filterSize / 2];

                    destImage[x, y] = (byte)Utility.Clamp(pixelValue, byte.MinValue, byte.MaxValue);
                }
            }

            return destImage;
        }
    }
}
