
// ImageLib
// ImageAreaDivision.cs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLib
{
    public static class ImageAreaDivision
    {
        /// <summary>
        /// 領域統合法
        /// </summary>
        /// <param name="sourceImage">画像の3次元配列</param>
        /// <param name="averageDiffThreshold">隣接する画素集合の平均値の差の閾値</param>
        /// <returns>結果の画像の3次元配列</returns>
        public static byte[,,] RegionUnificationMethod(byte[,,] sourceImage, double averageDiffThreshold)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            int numOfChannels = sourceImage.GetLength(2);
            byte[,,] destImage = new byte[imageWidth, imageHeight, numOfChannels];
            int[,] labelMap = new int[imageWidth, imageHeight];
            Stack<Point> posStack = new Stack<Point>();
            int numOfAreas = 0;
            int currentNumOfAreas = 0;

            // 同じ画素値をもつ隣接する画素の集合を取得
            while (true) {
                // 未分類の画素を探索
                Point? unlabeledPos = ImageAreaDivision.GetUnlabeledPixelLocation(labelMap);

                // 未分類の画素が無ければ領域の分割が終了
                if (!unlabeledPos.HasValue)
                    break;

                // 未分類の画素をスタックにプッシュ
                posStack.Push(unlabeledPos.Value);

                // 未分類の画素にラベルを付加
                labelMap[unlabeledPos.Value.X, unlabeledPos.Value.Y] = ++numOfAreas;

                // 分割された領域の個数を更新
                ++currentNumOfAreas;

                while (posStack.Count() > 0) {
                    Point currentPos = posStack.Pop();
                    Color currentColor = Color.FromArgb(
                        sourceImage[currentPos.X, currentPos.Y, 0],
                        sourceImage[currentPos.X, currentPos.Y, 1],
                        sourceImage[currentPos.X, currentPos.Y, 2]);
                    
                    // 注目画素の1つ左の画素を調べ, 同じ画素値であれば注目画素と同じラベルを付ける
                    if (currentPos.X - 1 >= 0 &&
                        labelMap[currentPos.X - 1, currentPos.Y] == 0 &&
                        sourceImage[currentPos.X - 1, currentPos.Y, 0] == currentColor.R &&
                        sourceImage[currentPos.X - 1, currentPos.Y, 1] == currentColor.G &&
                        sourceImage[currentPos.X - 1, currentPos.Y, 2] == currentColor.B) {
                        if (!posStack.Contains(new Point(currentPos.X - 1, currentPos.Y))) {
                            labelMap[currentPos.X - 1, currentPos.Y] = labelMap[currentPos.X, currentPos.Y];
                            // 新しくラベルの付いた画素を注目画素として処理を実行
                            posStack.Push(new Point(currentPos.X - 1, currentPos.Y));
                        }
                    }

                    // 注目画素の1つ右の画素を調べ, 同じ画素値であれば注目画素と同じラベルを付ける
                    if (currentPos.X + 1 < imageWidth &&
                        labelMap[currentPos.X + 1, currentPos.Y] == 0 &&
                        sourceImage[currentPos.X + 1, currentPos.Y, 0] == currentColor.R &&
                        sourceImage[currentPos.X + 1, currentPos.Y, 1] == currentColor.G &&
                        sourceImage[currentPos.X + 1, currentPos.Y, 2] == currentColor.B) {
                        if (!posStack.Contains(new Point(currentPos.X + 1, currentPos.Y))) {
                            labelMap[currentPos.X + 1, currentPos.Y] = labelMap[currentPos.X, currentPos.Y];
                            // 新しくラベルの付いた画素を注目画素として処理を実行
                            posStack.Push(new Point(currentPos.X + 1, currentPos.Y));
                        }
                    }

                    // 注目画素の1つ上の画素を調べ, 同じ画素値であれば注目画素と同じラベルを付ける
                    if (currentPos.Y - 1 >= 0 &&
                        labelMap[currentPos.X, currentPos.Y - 1] == 0 &&
                        sourceImage[currentPos.X, currentPos.Y - 1, 0] == currentColor.R &&
                        sourceImage[currentPos.X, currentPos.Y - 1, 1] == currentColor.G &&
                        sourceImage[currentPos.X, currentPos.Y - 1, 2] == currentColor.B) {
                        if (!posStack.Contains(new Point(currentPos.X, currentPos.Y - 1))) {
                            labelMap[currentPos.X, currentPos.Y - 1] = labelMap[currentPos.X, currentPos.Y];
                            // 新しくラベルの付いた画素を注目画素として処理を実行
                            posStack.Push(new Point(currentPos.X, currentPos.Y - 1));
                        }
                    }

                    // 注目画素の1つ下の画素を調べ, 同じ画素値であれば注目画素と同じラベルを付ける
                    if (currentPos.Y + 1 < imageHeight &&
                        labelMap[currentPos.X, currentPos.Y + 1] == 0 &&
                        sourceImage[currentPos.X, currentPos.Y + 1, 0] == currentColor.R &&
                        sourceImage[currentPos.X, currentPos.Y + 1, 1] == currentColor.G &&
                        sourceImage[currentPos.X, currentPos.Y + 1, 2] == currentColor.B) {
                        if (!posStack.Contains(new Point(currentPos.X, currentPos.Y + 1))) {
                            labelMap[currentPos.X, currentPos.Y + 1] = labelMap[currentPos.X, currentPos.Y];
                            // 新しくラベルの付いた画素を注目画素として処理を実行
                            posStack.Push(new Point(currentPos.X, currentPos.Y + 1));
                        }
                    }
                }
            }

            // 同じラベルに属する画素数
            int[] numOfPixels = new int[numOfAreas];

            // 同じラベルに属する画素の画素値の平均
            double[,] averageColors = new double[numOfAreas, numOfChannels];

            for (int x = 0; x < imageWidth; ++x) {
                for (int y = 0; y < imageHeight; ++y) {
                    // 同じラベルに属する画素数を更新
                    numOfPixels[labelMap[x, y] - 1]++;

                    // 同じラベルに属する画素の画素値の合計を更新
                    averageColors[labelMap[x, y] - 1, 0] += sourceImage[x, y, 0];
                    averageColors[labelMap[x, y] - 1, 1] += sourceImage[x, y, 1];
                    averageColors[labelMap[x, y] - 1, 2] += sourceImage[x, y, 2];
                }
            }

            // 同じラベルに属する画素の画素値の平均を計算
            for (int i = 0; i < numOfAreas; ++i) {
                // ラベルに属する画素が存在しなければスキップ
                if (numOfPixels[i] == 0)
                    continue;

                averageColors[i, 0] /= numOfPixels[i];
                averageColors[i, 1] /= numOfPixels[i];
                averageColors[i, 2] /= numOfPixels[i];
            }

            // 領域の統合処理
            while (true) {
                // 平均値の差が最小となる集合の組を取得
                int minPairLabel0 = 0;
                int minPairLabel1 = 0;
                double averageDiff = 0.0;
                double averageDiffMin = double.MaxValue;

                // 隣接する画素集合の平均値の差を計算
                for (int x = 0; x < imageWidth; ++x) {
                    for (int y = 0; y < imageHeight; ++y) {
                        for (int dx = -1; dx <= 0; ++dx) {
                            for (int dy = -1; dy <= 0; ++dy) {
                                if (dx == 0 && dy == 0)
                                    continue;
                                if (dx != 0 && dy != 0)
                                    continue;
                                if (x + dx < 0 || x + dx >= imageWidth)
                                    continue;
                                if (y + dy < 0 || y + dy >= imageHeight)
                                    continue;
                                if (labelMap[x, y] == labelMap[x + dx, y + dy])
                                    continue;

                                // 集合の平均値の差を計算
                                averageDiff = Math.Sqrt(
                                    Math.Pow(averageColors[labelMap[x, y] - 1, 0] - 
                                        averageColors[labelMap[x + dx, y + dy] - 1, 0], 2.0) +
                                    Math.Pow(averageColors[labelMap[x, y] - 1, 1] -
                                        averageColors[labelMap[x + dx, y + dy] - 1, 1], 2.0) +
                                    Math.Pow(averageColors[labelMap[x, y] - 1, 2] -
                                        averageColors[labelMap[x + dx, y + dy] - 1, 2], 2.0));

                                // 最小となる集合の組を更新
                                if (averageDiff < averageDiffMin) {
                                    minPairLabel0 = labelMap[x, y];
                                    minPairLabel1 = labelMap[x + dx, y + dy];
                                    averageDiffMin = averageDiff;
                                }
                            }
                        }
                    }
                }
                
                // 集合の平均値の差が閾値を上回れば終了
                if (averageDiffMin >= averageDiffThreshold) {
                    // 分割された領域の画素値の平均を, 出力画素の画素値に設定
                    for (int x = 0; x < imageWidth; ++x) {
                        for (int y = 0; y < imageHeight; ++y) {
                            destImage[x, y, 0] = (byte)Utility.Clamp(
                                averageColors[labelMap[x, y] - 1, 0], byte.MinValue, byte.MaxValue);
                            destImage[x, y, 1] = (byte)Utility.Clamp(
                                averageColors[labelMap[x, y] - 1, 1], byte.MinValue, byte.MaxValue);
                            destImage[x, y, 2] = (byte)Utility.Clamp(
                                averageColors[labelMap[x, y] - 1, 2], byte.MinValue, byte.MaxValue);
                        }
                    }
                    break;
                }

                // 平均値の差が最小となる集合を統合
                for (int x = 0; x < imageWidth; ++x)
                    for (int y = 0; y < imageHeight; ++y)
                        if (labelMap[x, y] == minPairLabel1)
                            labelMap[x, y] = minPairLabel0;

                // 同じラベルに属する画素値の平均を更新
                averageColors[minPairLabel0 - 1, 0] =
                    (numOfPixels[minPairLabel0 - 1] * averageColors[minPairLabel0 - 1, 0] +
                     numOfPixels[minPairLabel1 - 1] * averageColors[minPairLabel1 - 1, 0]) /
                    (numOfPixels[minPairLabel0 - 1] + numOfPixels[minPairLabel1 - 1]);
                averageColors[minPairLabel0 - 1, 1] =
                    (numOfPixels[minPairLabel0 - 1] * averageColors[minPairLabel0 - 1, 1] +
                     numOfPixels[minPairLabel1 - 1] * averageColors[minPairLabel1 - 1, 1]) /
                    (numOfPixels[minPairLabel0 - 1] + numOfPixels[minPairLabel1 - 1]);
                averageColors[minPairLabel0 - 1, 2] =
                    (numOfPixels[minPairLabel0 - 1] * averageColors[minPairLabel0 - 1, 2] +
                     numOfPixels[minPairLabel1 - 1] * averageColors[minPairLabel1 - 1, 2]) /
                    (numOfPixels[minPairLabel0 - 1] + numOfPixels[minPairLabel1 - 1]);
                averageColors[minPairLabel1 - 1, 0] = 0.0;
                averageColors[minPairLabel1 - 1, 1] = 0.0;
                averageColors[minPairLabel1 - 1, 2] = 0.0;

                // 同じラベルに属する画素数を更新
                numOfPixels[minPairLabel0 - 1] += numOfPixels[minPairLabel1 - 1];

                // 分割された領域の個数を更新
                --currentNumOfAreas;

                // 領域の個数が1つになった時点で終了
                if (currentNumOfAreas <= 1) {
                    // 分割された領域の画素値の平均を, 出力画素の画素値に設定
                    for (int x = 0; x < imageWidth; ++x) {
                        for (int y = 0; y < imageHeight; ++y) {
                            destImage[x, y, 0] = (byte)Utility.Clamp(
                                averageColors[labelMap[x, y] - 1, 0], byte.MinValue, byte.MaxValue);
                            destImage[x, y, 1] = (byte)Utility.Clamp(
                                averageColors[labelMap[x, y] - 1, 1], byte.MinValue, byte.MaxValue);
                            destImage[x, y, 2] = (byte)Utility.Clamp(
                                averageColors[labelMap[x, y] - 1, 2], byte.MinValue, byte.MaxValue);
                        }
                    }
                    break;
                }
            }

            return destImage;
        }

        /// <summary>
        /// ラベルが付いていない画素の位置を取得
        /// </summary>
        /// <param name="labelMap">ラベルの2次元配列</param>
        /// <returns>ラベルが付いていない画素の位置</returns>
        private static Point? GetUnlabeledPixelLocation(int[,] labelMap)
        {
            int imageWidth = labelMap.GetLength(0);
            int imageHeight = labelMap.GetLength(1);

            for (int x = 0; x < imageWidth; ++x)
                for (int y = 0; y < imageHeight; ++y)
                    if (labelMap[x, y] == 0)
                        return new Point(x, y);

            return null;
        }

        /// <summary>
        /// K-Means法
        /// </summary>
        /// <param name="sourceImage">画像の3次元配列</param>
        /// <param name="numOfClasses">分割するクラスタの個数</param>
        /// <param name="random">擬似乱数生成器</param>
        /// <returns>結果の画像の3次元配列</returns>
        public static byte[,,] KMeansMethod(
            byte[,,] sourceImage, int numOfClasses, double weightOfPos, Random random)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            int numOfChannels = sourceImage.GetLength(2);
            byte[,,] destImage = new byte[imageWidth, imageHeight, numOfChannels];
            int[,] clusterMap = new int[imageWidth, imageHeight];
            int[,] newClusterMap = new int[imageWidth, imageHeight];
            int[] clusterNumOfPixels = new int[numOfClasses];
            double[,] clusterCentroid = new double[numOfClasses, numOfChannels + 2];

            // 各画素が属するクラスタを初期化
            for (int x = 0; x < imageWidth; ++x)
                for (int y = 0; y < imageHeight; ++y)
                    clusterMap[x, y] = -1;
            
            // 各クラスタの画素値の平均を決定
            for (int i = 0; i < numOfClasses; ++i) {
                int x = random.Next(0, imageWidth);
                int y = random.Next(0, imageHeight);
                clusterCentroid[i, 0] = sourceImage[x, y, 0];
                clusterCentroid[i, 1] = sourceImage[x, y, 1];
                clusterCentroid[i, 2] = sourceImage[x, y, 2];
                clusterCentroid[i, 3] = x;
                clusterCentroid[i, 4] = y;
                clusterMap[x, y] = i;
            }
            
            do {
                int newCluster;
                double minDist;
                double distToCentroid;
                
                int x = 0;
                int y = 0;
                int i = 0;

                // 各画素が属する新たなクラスタを決定
                for (x = 0; x < imageWidth; ++x) {
                    for (y = 0; y < imageHeight; ++y) {
                        newCluster = -1;
                        minDist = double.MaxValue;
                        distToCentroid = 0.0;

                        for (i = 0; i < numOfClasses; ++i) {
                            distToCentroid = Math.Sqrt(
                                Math.Pow(sourceImage[x, y, 0] - clusterCentroid[i, 0], 2.0) +
                                Math.Pow(sourceImage[x, y, 1] - clusterCentroid[i, 1], 2.0) +
                                Math.Pow(sourceImage[x, y, 2] - clusterCentroid[i, 2], 2.0) +
                                Math.Pow(weightOfPos * x - clusterCentroid[i, 3], 2.0) +
                                Math.Pow(weightOfPos * y - clusterCentroid[i, 4], 2.0));

                            if (distToCentroid < minDist) {
                                minDist = distToCentroid;
                                newCluster = i;
                            }
                        }
                        
                        newClusterMap[x, y] = newCluster;
                    }
                }

                // 各画素が属するクラスタが変化しなければ終了
                for (x = 0; x < imageWidth; ++x)
                    for (y = 0; y < imageHeight; ++y)
                        if (clusterMap[x, y] != newClusterMap[x, y])
                            break;

                if (x == imageWidth && y == imageHeight) {
                    // クラスタの画素値の平均を, 出力画素の画素値に設定
                    for (x = 0; x < imageWidth; ++x) {
                        for (y = 0; y < imageHeight; ++y) {
                            destImage[x, y, 0] = (byte)Utility.Clamp(
                                clusterCentroid[clusterMap[x, y], 0], byte.MinValue, byte.MaxValue);
                            destImage[x, y, 1] = (byte)Utility.Clamp(
                                clusterCentroid[clusterMap[x, y], 1], byte.MinValue, byte.MaxValue);
                            destImage[x, y, 2] = (byte)Utility.Clamp(
                                clusterCentroid[clusterMap[x, y], 2], byte.MinValue, byte.MaxValue);
                        }
                    }
                    break;
                }

                // 各画素が属するクラスタを更新
                for (x = 0; x < imageWidth; ++x)
                    for (y = 0; y < imageHeight; ++y)
                        clusterMap[x, y] = newClusterMap[x, y];

                newClusterMap = new int[imageWidth, imageHeight];
                
                for (i = 0; i < numOfClasses; ++i) {
                    clusterCentroid[i, 0] = 0.0;
                    clusterCentroid[i, 1] = 0.0;
                    clusterCentroid[i, 2] = 0.0;
                    clusterCentroid[i, 3] = 0.0;
                    clusterCentroid[i, 4] = 0.0;
                    clusterNumOfPixels[i] = 0;
                }

                // 各クラスタの画素値の平均を計算
                for (x = 0; x < imageWidth; ++x) {
                    for (y = 0; y < imageHeight; ++y) {
                        clusterNumOfPixels[clusterMap[x, y]]++;
                        clusterCentroid[clusterMap[x, y], 0] += sourceImage[x, y, 0];
                        clusterCentroid[clusterMap[x, y], 1] += sourceImage[x, y, 1];
                        clusterCentroid[clusterMap[x, y], 2] += sourceImage[x, y, 2];
                        clusterCentroid[clusterMap[x, y], 3] += weightOfPos * x;
                        clusterCentroid[clusterMap[x, y], 4] += weightOfPos * y;
                    }
                }

                for (i = 0; i < numOfClasses; ++i) {
                    clusterCentroid[i, 0] /= clusterNumOfPixels[i];
                    clusterCentroid[i, 1] /= clusterNumOfPixels[i];
                    clusterCentroid[i, 2] /= clusterNumOfPixels[i];
                    clusterCentroid[i, 3] /= clusterNumOfPixels[i];
                    clusterCentroid[i, 4] /= clusterNumOfPixels[i];
                }
            } while (true);
            
            return destImage;
        }
    }
}
