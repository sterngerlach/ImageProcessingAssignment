
// ImageLib
// ImageMosaicing.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathNet.Numerics.LinearAlgebra;

namespace ImageLib
{
    public static class ImageMosaicing
    {
        /// <summary>
        /// 結果の画像から元の画像への射影変換行列を計算
        /// </summary>
        /// <param name="sourcePosList">元の画像の対応点のリスト</param>
        /// <param name="destPosList">結果の画像の対応点のリスト</param>
        /// <returns>結果の画像の3次元配列</returns>
        public static Matrix<double> DestToSourceProjectionMatrix(
            List<Point> sourcePosList, List<Point> destPosList)
        {
            Debug.Assert(sourcePosList.Count() == destPosList.Count());
            Debug.Assert(sourcePosList.Count() >= 4);

            // 対応点から射影変換を計算
            Matrix<double> matrixA = Matrix<double>.Build.Dense(2 * sourcePosList.Count(), 8);

            for (int row = 0; row < sourcePosList.Count(); ++row) {
                matrixA[row * 2, 0] = destPosList[row].X;
                matrixA[row * 2, 1] = destPosList[row].Y;
                matrixA[row * 2, 2] = 1.0;
                matrixA[row * 2, 6] = -destPosList[row].X * sourcePosList[row].X;
                matrixA[row * 2, 7] = -sourcePosList[row].X * destPosList[row].Y;

                matrixA[row * 2 + 1, 3] = destPosList[row].X;
                matrixA[row * 2 + 1, 4] = destPosList[row].Y;
                matrixA[row * 2 + 1, 5] = 1.0;
                matrixA[row * 2 + 1, 6] = -destPosList[row].X * sourcePosList[row].Y;
                matrixA[row * 2 + 1, 7] = -destPosList[row].Y * sourcePosList[row].Y;
            }

            Vector<double> vectorB = Vector<double>.Build.Dense(2 * sourcePosList.Count());

            for (int row = 0; row < sourcePosList.Count(); ++row) {
                vectorB[row * 2] = sourcePosList[row].X;
                vectorB[row * 2 + 1] = sourcePosList[row].Y;
            }

            Vector<double> vectorProj =
                (matrixA.Transpose() * matrixA).Inverse() * (matrixA.Transpose() * vectorB);
            Matrix<double> matrixProj = Matrix<double>.Build.Dense(3, 3,
                (i, j) => (i * 3 + j == 8) ? 1.0 : vectorProj[i * 3 + j]);

            return matrixProj;
        }

        /// <summary>
        /// 画像の射影変換
        /// </summary>
        /// <param name="sourceImage">元の画像の2次元配列</param>
        /// <param name="sourcePosList">元の画像の対応点のリスト</param>
        /// <param name="destPosList">変換後の画像の対応点のリスト</param>
        /// <returns>結果の画像の3次元配列</returns>
        public static byte[,,] ApplyProjectiveTransformation(
            byte[,,] sourceImage, List<Point> sourcePosList, List<Point> destPosList)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            int numOfChannels = sourceImage.GetLength(2);
            
            // 結果画像から元の画像への射影変換を計算
            Matrix<double> matrixProj = ImageMosaicing.DestToSourceProjectionMatrix(
                sourcePosList, destPosList);
            
            int destWidth = destPosList.Select(pos => pos.X).Max();
            int destHeight = destPosList.Select(pos => pos.Y).Max();
            byte[,,] destImage = new byte[destWidth, destHeight, numOfChannels];

            // 結果の画像を生成
            Vector<double> destPos = Vector<double>.Build.Dense(3);
            Vector<double> sourcePos = Vector<double>.Build.Dense(3);
            int sourcePosX = 0;
            int sourcePosY = 0;

            for (int x = 0; x < destWidth; ++x) {
                for (int y = 0; y < destHeight; ++y) {
                    destPos[0] = x;
                    destPos[1] = y;
                    destPos[2] = 1.0;
                    
                    // 元の画像の対応点を計算
                    sourcePos = matrixProj * destPos;
                    
                    // 出力画像の画素値を決定
                    sourcePosX = (int)Math.Floor(sourcePos[0] / sourcePos[2] + 0.5);
                    sourcePosY = (int)Math.Floor(sourcePos[1] / sourcePos[2] + 0.5);

                    if (sourcePosX < 0 || sourcePosX >= imageWidth ||
                        sourcePosY < 0 || sourcePosY >= imageHeight) {
                        destImage[x, y, 0] = 255;
                        destImage[x, y, 1] = 255;
                        destImage[x, y, 2] = 255;
                    } else {
                        destImage[x, y, 0] = sourceImage[sourcePosX, sourcePosY, 0];
                        destImage[x, y, 1] = sourceImage[sourcePosX, sourcePosY, 1];
                        destImage[x, y, 2] = sourceImage[sourcePosX, sourcePosY, 2];
                    }
                }
            }

            return destImage;
        }

        /// <summary>
        /// 画像の射影変換
        /// </summary>
        /// <param name="sourceImage">元の画像の2次元配列</param>
        /// <param name="matrixProj">結果の画像から元の画像への射影変換行列</param>
        /// <param name="diffX">結果の画像上で, 元の画像の原点に対応する位置の横方向の座標</param>
        /// <param name="diffY">結果の画像上で, 元の画像の原点に対応する位置の縦方向の座標</param>
        /// <returns>結果の画像の3次元配列</returns>
        public static byte[,,] ApplyProjectiveTransformation2(
            byte[,,] sourceImage, Matrix<double> matrixProj,
            out int diffX, out int diffY)
        {
            int imageWidth = sourceImage.GetLength(0);
            int imageHeight = sourceImage.GetLength(1);
            int numOfChannels = sourceImage.GetLength(2);

            // 結果の画像の大きさを調べる
            Vector<double> topLeftDestPos = Vector<double>.Build.Dense(3);
            Vector<double> topRightDestPos = Vector<double>.Build.Dense(3);
            Vector<double> bottomLeftDestPos = Vector<double>.Build.Dense(3);
            Vector<double> bottomRightDestPos = Vector<double>.Build.Dense(3);

            topLeftDestPos[0] = 0;
            topLeftDestPos[1] = 0;
            topLeftDestPos[2] = 1.0;

            topRightDestPos[0] = imageWidth - 1;
            topRightDestPos[1] = 0;
            topRightDestPos[2] = 1.0;

            bottomLeftDestPos[0] = 0;
            bottomLeftDestPos[1] = imageHeight - 1;
            bottomLeftDestPos[2] = 1.0;

            bottomRightDestPos[0] = imageWidth - 1;
            bottomRightDestPos[1] = imageHeight - 1;
            bottomRightDestPos[2] = 1.0;

            topLeftDestPos = matrixProj.Inverse() * topLeftDestPos;
            topRightDestPos = matrixProj.Inverse() * topRightDestPos;
            bottomLeftDestPos = matrixProj.Inverse() * bottomLeftDestPos;
            bottomRightDestPos = matrixProj.Inverse() * bottomRightDestPos;

            topLeftDestPos = topLeftDestPos.Divide(topLeftDestPos[2]);
            topRightDestPos = topRightDestPos.Divide(topRightDestPos[2]);
            bottomLeftDestPos = bottomLeftDestPos.Divide(bottomLeftDestPos[2]);
            bottomRightDestPos = bottomRightDestPos.Divide(bottomRightDestPos[2]);

            int leftX = (int)Math.Min(topLeftDestPos[0], bottomLeftDestPos[0]);
            int rightX = (int)Math.Max(topRightDestPos[0], bottomRightDestPos[0]);
            int topY = (int)Math.Min(topLeftDestPos[1], topRightDestPos[1]);
            int bottomY = (int)Math.Max(bottomLeftDestPos[1], bottomRightDestPos[1]);

            diffX = -leftX;
            diffY = -topY;

            int destWidth = rightX - leftX + 1;
            int destHeight = bottomY - topY + 1;
            byte[,,] destImage = new byte[destWidth, destHeight, numOfChannels];
            
            // 結果の画像を生成
            Vector<double> destPos = Vector<double>.Build.Dense(3);
            Vector<double> sourcePos = Vector<double>.Build.Dense(3);
            int sourcePosX = 0;
            int sourcePosY = 0;

            for (int x = 0; x < destWidth; ++x) {
                for (int y = 0; y < destHeight; ++y) {
                    destPos[0] = x + leftX;
                    destPos[1] = y + topY;
                    destPos[2] = 1.0;

                    // 元の画像の対応点を計算
                    sourcePos = matrixProj * destPos;

                    // 出力画像の画素値を決定
                    sourcePosX = (int)Math.Floor(sourcePos[0] / sourcePos[2] + 0.5);
                    sourcePosY = (int)Math.Floor(sourcePos[1] / sourcePos[2] + 0.5);

                    if (sourcePosX < 0 || sourcePosX >= imageWidth ||
                        sourcePosY < 0 || sourcePosY >= imageHeight) {
                        destImage[x, y, 0] = 255;
                        destImage[x, y, 1] = 255;
                        destImage[x, y, 2] = 255;
                    } else {
                        destImage[x, y, 0] = sourceImage[sourcePosX, sourcePosY, 0];
                        destImage[x, y, 1] = sourceImage[sourcePosX, sourcePosY, 1];
                        destImage[x, y, 2] = sourceImage[sourcePosX, sourcePosY, 2];
                    }
                }
            }

            return destImage;
        }

        /// <summary>
        /// 点が多角形の内側にあるかどうかを判定
        /// </summary>
        /// <param name="posX">点の横方向の座標</param>
        /// <param name="posY">点の縦方向の座標</param>
        /// <param name="posList">多角形の座標のリスト</param>
        /// <returns>点が多角形の内側にあるかどうか</returns>
        public static bool IsPointInPolygon(int posX, int posY, List<Point> posList)
        {
            double minX = posList.Select(pos => pos.X).Min();
            double maxX = posList.Select(pos => pos.X).Max();
            double minY = posList.Select(pos => pos.Y).Min();
            double maxY = posList.Select(pos => pos.Y).Max();

            if (posX < minX || posX > maxX ||
                posY < minY || posY > maxY)
                return false;
            
            bool isInside = false;

            for (int i = 0, j = posList.Count - 1; i < posList.Count; j = i++)
                if ((posList[i].Y > posY) != (posList[j].Y > posY) &&
                    posX < (posList[j].X - posList[i].X) * (posY - posList[i].Y) /
                    (posList[j].Y - posList[i].Y) + posList[i].X)
                    isInside = !isInside;

            return isInside;
        }

        /// <summary>
        /// 画像から障害物を除去
        /// </summary>
        /// <param name="baseImage">基準画像の3次元配列</param>
        /// <param name="otherImages">障害物を除去するために使用する補助画像</param>
        /// <param name="baseImagePosList">補助画像ごとに対応点をまとめたリスト</param>
        /// <param name="otherImagePosList">補助画像ごとに対応点をまとめたリスト</param>
        /// <param name="obstaclePosList">基準画像と補助画像における障害物の座標をまとめたリスト</param>
        /// <returns>結果の画像の3次元配列</returns>
        public static byte[,,] RemoveObstacleFromImage(
            byte[,,] baseImage, List<byte[,,]> otherImages,
            List<List<Point>> baseImagePosList, List<List<Point>> otherImagePosList,
            List<List<Point>> obstaclePosList)
        {
            int imageWidth = baseImage.GetLength(0);
            int imageHeight = baseImage.GetLength(1);
            int numOfChannels = baseImage.GetLength(2);
            byte[,,] destImage = new byte[imageWidth, imageHeight, numOfChannels];
            int i;

            Debug.Assert(otherImages.Count() == baseImagePosList.Count());
            Debug.Assert(baseImagePosList.Count() == otherImagePosList.Count());

            // 他の入力画像を基準画像に変換
            for (i = 0; i < otherImages.Count(); ++i) {
                // 他の入力画像から基準画像への射影変換を計算
                Matrix<double> matrixProj = ImageMosaicing.DestToSourceProjectionMatrix(
                    otherImagePosList[i], baseImagePosList[i]);
                // 他の入力画像から基準画像への射影変換を実行
                otherImages[i] = ImageMosaicing.ApplyProjectiveTransformation(
                    otherImages[i], otherImagePosList[i], baseImagePosList[i]);

                // 障害物の領域を表す座標も射影行列で変換
                Vector<double> sourcePos = Vector<double>.Build.Dense(3);
                Vector<double> destPos = Vector<double>.Build.Dense(3);

                for (int j = 0; j < obstaclePosList[i + 1].Count(); ++j) {
                    sourcePos[0] = obstaclePosList[i + 1][j].X;
                    sourcePos[1] = obstaclePosList[i + 1][j].Y;
                    sourcePos[2] = 1.0;

                    destPos = matrixProj.Inverse() * sourcePos;

                    obstaclePosList[i + 1][j] = new Point(
                        (int)Math.Floor(destPos[0] / destPos[2] + 0.5),
                        (int)Math.Floor(destPos[1] / destPos[2] + 0.5));
                }
            }
            
            for (int x = 0; x < imageWidth; ++x) {
                for (int y = 0; y < imageHeight; ++y) {
                    // 基準画像の障害物領域であるとき, 他の画像で埋める
                    if (ImageMosaicing.IsPointInPolygon(x, y, obstaclePosList[0])) {
                        // 他の画像の障害物領域でなければ採用
                        for (i = 0; i < otherImages.Count(); ++i)
                            if (!ImageMosaicing.IsPointInPolygon(x, y, obstaclePosList[i + 1]))
                                break;
                    
                        if (i != otherImages.Count()) {
                            // 他の画像で障害物領域ではなかったので採用
                            destImage[x, y, 0] = otherImages[i][x, y, 0];
                            destImage[x, y, 1] = otherImages[i][x, y, 1];
                            destImage[x, y, 2] = otherImages[i][x, y, 2];
                        } else {
                            // 他の画像で障害物領域でないものが見つからなかった
                            destImage[x, y, 0] = 0;
                            destImage[x, y, 1] = 0;
                            destImage[x, y, 2] = 0;
                        }
                    } else {
                        // 基準画像の障害物領域ではない
                        destImage[x, y, 0] = baseImage[x, y, 0];
                        destImage[x, y, 1] = baseImage[x, y, 1];
                        destImage[x, y, 2] = baseImage[x, y, 2];
                    }
                }
            }
            
            return destImage;
        }

        /// <summary>
        /// 基準画像と別の画像を結合
        /// </summary>
        /// <param name="baseImage">基準画像の3次元配列</param>
        /// <param name="otherImages">画像の3次元配列のリスト</param>
        /// <param name="baseImagePosList">画像ごとに対応点をまとめたリスト</param>
        /// <param name="otherImagePosList">画像ごとに対応点をまとめたリスト</param>
        /// <returns>結果の画像の3次元配列</returns>
        public static byte[,,] CombineImages(
            byte[,,] baseImage, List<byte[,,]> otherImages,
            List<List<Point>> baseImagePosList, List<List<Point>> otherImagePosList)
        {
            List<Point> diffList = new List<Point>();
            int imageWidth = baseImage.GetLength(0);
            int imageHeight = baseImage.GetLength(1);
            int numOfChannels = baseImage.GetLength(2);
            int baseImageWidth = baseImage.GetLength(0);
            int baseImageHeight = baseImage.GetLength(1);
            int baseImageTopLeftX = 0;
            int baseImageTopLeftY = 0;

            Debug.Assert(otherImages.Count() == baseImagePosList.Count());
            Debug.Assert(baseImagePosList.Count() == otherImagePosList.Count());
            
            // 他の入力画像を基準画像に変換
            for (int i = 0; i < otherImages.Count(); ++i) {
                // 他の入力画像から基準画像への射影変換を計算
                Matrix<double> matrixProj = ImageMosaicing.DestToSourceProjectionMatrix(
                    otherImagePosList[i], baseImagePosList[i]);
                
                // 他の入力画像から基準画像への射影変換を実行
                otherImages[i] = ImageMosaicing.ApplyProjectiveTransformation2(
                    otherImages[i], matrixProj, out int diffX, out int diffY);

                // 基準画像の原点に対応する, 補助画像上の点の座標を保存
                diffList.Add(new Point(diffX, diffY));

                // 基準画像の原点に対応する, 結果の画像の座標を更新
                if (diffX > 0)
                    baseImageTopLeftX = Math.Max(baseImageTopLeftX, diffX);
                if (diffY > 0)
                    baseImageTopLeftY = Math.Max(baseImageTopLeftY, diffY);

                // 結果の画像の大きさを更新
                if (diffX < 0)
                    imageWidth = Math.Max(baseImageWidth, otherImages[i].GetLength(0) - diffX);
                if (diffY < 0)
                    imageHeight = Math.Max(baseImageHeight, otherImages[i].GetLength(1) - diffY);
            }

            // 結果の画像の大きさを更新
            imageWidth += baseImageTopLeftX;
            imageHeight += baseImageTopLeftY;
            
            // 結果の画像を白で埋める
            byte[,,] destImage = new byte[imageWidth, imageHeight, numOfChannels];
            
            for (int x = 0; x < imageWidth; ++x) {
                for (int y = 0; y < imageHeight; ++y) {
                    destImage[x, y, 0] = 255;
                    destImage[x, y, 1] = 255;
                    destImage[x, y, 2] = 255;
                }
            }
            
            int actualX;
            int actualY;

            // 結果の画像に補助画像を埋める
            for (int i = 0; i < otherImages.Count(); ++i) {
                for (int x = 0; x < otherImages[i].GetLength(0); ++x) {
                    for (int y = 0; y < otherImages[i].GetLength(1); ++y) {
                        // 結果の画像における座標を計算
                        actualX = baseImageTopLeftX - diffList[i].X + x;
                        actualY = baseImageTopLeftY - diffList[i].Y + y;

                        if (actualX < 0 || actualX >= imageWidth ||
                            actualY < 0 || actualY >= imageHeight)
                            continue;

                        destImage[actualX, actualY, 0] = otherImages[i][x, y, 0];
                        destImage[actualX, actualY, 1] = otherImages[i][x, y, 1];
                        destImage[actualX, actualY, 2] = otherImages[i][x, y, 2];
                    }
                }
            }

            // 結果の画像に基準画像を埋める
            for (int x = 0; x < baseImage.GetLength(0); ++x) {
                for (int y = 0; y < baseImage.GetLength(1); ++y) {
                    // 結果の画像における座標を計算
                    actualX = baseImageTopLeftX + x;
                    actualY = baseImageTopLeftY + y;

                    if (actualX < 0 || actualX >= imageWidth ||
                            actualY < 0 || actualY >= imageHeight)
                        continue;

                    destImage[actualX, actualY, 0] = baseImage[x, y, 0];
                    destImage[actualX, actualY, 1] = baseImage[x, y, 1];
                    destImage[actualX, actualY, 2] = baseImage[x, y, 2];
                }
            }

            return destImage;
        }
    }
}
