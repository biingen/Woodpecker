using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace OPTT
{
    public class CompareSimilarity
    {

        #region -- 圖片比對演算法 --
        // 內存法
        public static Bitmap RGB2Gray(Bitmap srcBitmap)
        {
            Rectangle rect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);
            System.Drawing.Imaging.BitmapData bmpdata = srcBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpdata.Scan0;

            int bytes = srcBitmap.Width * srcBitmap.Height * 3;
            byte[] rgbvalues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbvalues, 0, bytes);

            double colortemp = 0;
            for (int i = 0; i < rgbvalues.Length; i += 3)
            {
                colortemp = rgbvalues[i + 2] * 0.299 + rgbvalues[i + 1] * 0.587 + rgbvalues[i] * 0.114;
                rgbvalues[i] = rgbvalues[i + 1] = rgbvalues[i + 2] = (byte)colortemp;
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbvalues, 0, ptr, bytes);

            srcBitmap.UnlockBits(bmpdata);
            return (srcBitmap);
        }

        // Sobel法 
        private Bitmap SobelEdgeDetect(Bitmap original)
        {
            Bitmap b = original;
            Bitmap bb = original;
            int width = b.Width;
            int height = b.Height;
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };

            int[,] allPixR = new int[width, height];
            int[,] allPixG = new int[width, height];
            int[,] allPixB = new int[width, height];

            int limit = 128 * 128;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    allPixR[i, j] = b.GetPixel(i, j).R;
                    allPixG[i, j] = b.GetPixel(i, j).G;
                    allPixB[i, j] = b.GetPixel(i, j).B;
                }
            }

            int new_rx = 0, new_ry = 0;
            int new_gx = 0, new_gy = 0;
            int new_bx = 0, new_by = 0;
            int rc, gc, bc;
            for (int i = 1; i < b.Width - 1; i++)
            {
                for (int j = 1; j < b.Height - 1; j++)
                {

                    new_rx = 0;
                    new_ry = 0;
                    new_gx = 0;
                    new_gy = 0;
                    new_bx = 0;
                    new_by = 0;
                    rc = 0;
                    gc = 0;
                    bc = 0;

                    for (int wi = -1; wi < 2; wi++)
                    {
                        for (int hw = -1; hw < 2; hw++)
                        {
                            rc = allPixR[i + hw, j + wi];
                            new_rx += gx[wi + 1, hw + 1] * rc;
                            new_ry += gy[wi + 1, hw + 1] * rc;

                            gc = allPixG[i + hw, j + wi];
                            new_gx += gx[wi + 1, hw + 1] * gc;
                            new_gy += gy[wi + 1, hw + 1] * gc;

                            bc = allPixB[i + hw, j + wi];
                            new_bx += gx[wi + 1, hw + 1] * bc;
                            new_by += gy[wi + 1, hw + 1] * bc;
                        }
                    }
                    if (new_rx * new_rx + new_ry * new_ry > limit || new_gx * new_gx + new_gy * new_gy > limit || new_bx * new_bx + new_by * new_by > limit)
                        bb.SetPixel(i, j, Color.Black);

                    //bb.SetPixel (i, j, Color.FromArgb(allPixR[i,j],allPixG[i,j],allPixB[i,j]));
                    else
                        bb.SetPixel(i, j, Color.Transparent);
                }
            }
            return bb;
        }

        public static bool ImageCompareString(Bitmap firstImage, Bitmap secondImage)
        {
            MemoryStream ms = new MemoryStream();
            firstImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String firstBitmap = Convert.ToBase64String(ms.ToArray());
            ms.Position = 0;
            secondImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String(ms.ToArray());
            if (firstBitmap.Equals(secondBitmap))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// 圖片內容比較1
        /// Refer: http://www.programmer-club.com.tw/ShowSameTitleN/csharp/9880.html
        public float Similarity(System.Drawing.Bitmap img1, System.Drawing.Bitmap img2)
        {
            int rc, bc, gc;
            float cc = 0, hc = 0;

            for (int i = 0; i < img1.Size.Width; i++)
            {
                for (int j = 0; j < img1.Size.Height; j++)
                {
                    System.Drawing.Color c1 = img1.GetPixel(i, j);
                    System.Drawing.Color c2 = img2.GetPixel(i, j);

                    rc = Math.Abs(c1.R - c2.R);
                    bc = Math.Abs(c1.B - c2.B);
                    gc = Math.Abs(c1.G - c2.G);
                    cc = (float)(rc + bc + gc);

                    float f1 = (float)(255 * 3 * img1.Size.Width * img1.Size.Height);
                    hc += cc / f1;
                }
            }
            hc = hc * 100;
            return hc;
        }

        // GetHisogram 取long
        public long[] GetHistogram(System.Drawing.Bitmap picture)
        {
            long[] myHistogram = new long[256];

            for (int i = 0; i < picture.Size.Width; i++)
                for (int j = 0; j < picture.Size.Height; j++)
                {
                    System.Drawing.Color c = picture.GetPixel(i, j);

                    long Temp = 0;
                    Temp += c.R;
                    Temp += c.G;
                    Temp += c.B;

                    Temp = (int)Temp / 3;
                    myHistogram[Temp]++;
                }

            return myHistogram;
        }

        // GetHisogram 取int
        public int[] GetHisogram(Bitmap img)
        {
            BitmapData data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] histogram = new int[256];
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int remain = data.Stride - data.Width * 3;
                for (int i = 0; i < histogram.Length; i++)
                    histogram[i] = 0;
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        int mean = ptr[0] + ptr[1] + ptr[2];
                        mean /= 3;
                        histogram[mean]++;
                        ptr += 3;
                    }
                    ptr += remain;
                }
            }
            img.UnlockBits(data);
            return histogram;
        }

        //計算相減後的絕對值
        private float GetAbs(int firstNum, int secondNum)
        {
            float abs = Math.Abs((float)firstNum - (float)secondNum);
            float result = Math.Max(firstNum, secondNum);
            if (result == 0)
                result = 1;
            return abs / result;
        }

        //最終計算結果
        public float GetResult(int[] firstNum, int[] scondNum)
        {
            if (firstNum.Length != scondNum.Length)
            {
                return 0;
            }
            else
            {
                float result = 0;
                int j = firstNum.Length;
                for (int i = 0; i < j; i++)
                {
                    result += 1 - GetAbs(firstNum[i], scondNum[i]);
                }
                return result / j;
            }
        }

        /// <summary>
        /// 判断图形里是否存在另外一个图形 并返回所在位置
        /// </summary>
        /// <param name=”p_SourceBitmap”>原始图形</param>
        /// <param name=”p_PartBitmap”>小图形</param>
        /// <param name=”p_Float”>溶差</param>
        /// <returns>坐标</returns>
        public Point GetImageContains(Bitmap p_SourceBitmap, Bitmap p_PartBitmap, int p_Float)
        {
            int _SourceWidth = p_SourceBitmap.Width;
            int _SourceHeight = p_SourceBitmap.Height;
            int _PartWidth = p_PartBitmap.Width;
            int _PartHeight = p_PartBitmap.Height;
            Bitmap _SourceBitmap = new Bitmap(_SourceWidth, _SourceHeight);
            Graphics _Graphics = Graphics.FromImage(_SourceBitmap);
            _Graphics.DrawImage(p_SourceBitmap, new Rectangle(0, 0, _SourceWidth, _SourceHeight));
            _Graphics.Dispose();
            BitmapData _SourceData = _SourceBitmap.LockBits(new Rectangle(0, 0, _SourceWidth, _SourceHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] _SourceByte = new byte[_SourceData.Stride * _SourceHeight];
            Marshal.Copy(_SourceData.Scan0, _SourceByte, 0, _SourceByte.Length);  //复制出p_SourceBitmap的相素信息
            _SourceBitmap.UnlockBits(_SourceData);
            Bitmap _PartBitmap = new Bitmap(_PartWidth, _PartHeight);
            _Graphics = Graphics.FromImage(_PartBitmap);
            _Graphics.DrawImage(p_PartBitmap, new Rectangle(0, 0, _PartWidth, _PartHeight));
            _Graphics.Dispose();
            BitmapData _PartData = _PartBitmap.LockBits(new Rectangle(0, 0, _PartWidth, _PartHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] _PartByte = new byte[_PartData.Stride * _PartHeight];
            Marshal.Copy(_PartData.Scan0, _PartByte, 0, _PartByte.Length);   //复制出p_PartBitmap的相素信息
            _PartBitmap.UnlockBits(_PartData);
            for (int i = 0; i != _SourceHeight; i++)
            {
                if (_SourceHeight - i < _PartHeight) return new Point(-1, -1);  //如果 剩余的高 比需要比较的高 还要小 就直接返回
                int _PointX = -1;    //临时存放坐标 需要包正找到的是在一个X点上
                bool _SacnOver = true;   //是否都比配的上
                for (int z = 0; z != _PartHeight - 1; z++)       //循环目标进行比较
                {
                    int _TrueX = GetImageContains(_SourceByte, _PartByte, (i + z) * _SourceData.Stride, z * _PartData.Stride, _SourceWidth, _PartWidth, p_Float);
                    if (_TrueX == -1)   //如果没找到
                    {
                        _PointX = -1;    //设置坐标为没找到
                        _SacnOver = false;   //设置不进行返回
                        break;
                    }
                    else
                    {
                        if (z == 0) _PointX = _TrueX;
                        if (_PointX != _TrueX)   //如果找到了 也的保证坐标和上一行的坐标一样 否则也返回
                        {
                            _PointX = -1;//设置坐标为没找到
                            _SacnOver = false;  //设置不进行返回
                            break;
                        }
                    }
                }
                if (_SacnOver) return new Point(_PointX, i);
            }
            return new Point(-1, -1);
        }

        /// <summary>
        /// 判断图形里是否存在另外一个图形 所在行的索引
        /// </summary>
        /// <param name=”p_Source”>原始图形数据</param>
        /// <param name=”p_Part”>小图形数据</param>
        /// <param name=”p_SourceIndex”>开始位置</param>
        /// <param name=”p_SourceWidth”>原始图形宽</param>
        /// <param name=”p_PartWidth”>小图宽</param>
        /// <param name=”p_Float”>溶差</param>
        /// <returns>所在行的索引 如果找不到返回-1</returns>
        private int GetImageContains(byte[] p_Source, byte[] p_Part, int p_SourceIndex, int p_PartIndex, int p_SourceWidth, int p_PartWidth, int p_Float)
        {
            int _PartIndex = p_PartIndex;//
            int _PartRVA = _PartIndex;//p_PartX轴起点
            int _SourceIndex = p_SourceIndex;//p_SourceX轴起点
            for (int i = 0; i < p_SourceWidth; i++)
            {
                if (p_SourceWidth - i < p_PartWidth) return -1;
                Color _CurrentlyColor = Color.FromArgb((int)p_Source[_SourceIndex + 3], (int)p_Source[_SourceIndex + 2], (int)p_Source[_SourceIndex + 1], (int)p_Source[_SourceIndex]);
                Color _CompareColoe = Color.FromArgb((int)p_Part[_PartRVA + 3], (int)p_Part[_PartRVA + 2], (int)p_Part[_PartRVA + 1], (int)p_Part[_PartRVA]);
                _SourceIndex += 4;//成功，p_SourceX轴加4
                bool _ScanColor = ScanColor(_CurrentlyColor, _CompareColoe, p_Float);
                if (_ScanColor)
                {
                    _PartRVA += 4;//成功，p_PartX轴加4
                    int _SourceRVA = _SourceIndex;
                    bool _Equals = true;
                    for (int z = 0; z != p_PartWidth - 1; z++)
                    {
                        _CurrentlyColor = Color.FromArgb((int)p_Source[_SourceRVA + 3], (int)p_Source[_SourceRVA + 2], (int)p_Source[_SourceRVA + 1], (int)p_Source[_SourceRVA]);
                        _CompareColoe = Color.FromArgb((int)p_Part[_PartRVA + 3], (int)p_Part[_PartRVA + 2], (int)p_Part[_PartRVA + 1], (int)p_Part[_PartRVA]);
                        if (!ScanColor(_CurrentlyColor, _CompareColoe, p_Float))
                        {
                            _PartRVA = _PartIndex;//失败，重置p_PartX轴开始
                            _Equals = false;
                            break;
                        }
                        _PartRVA += 4;//成功，p_PartX轴加4
                        _SourceRVA += 4;//成功，p_SourceX轴加4
                    }
                    if (_Equals) return i;
                }
                else
                {
                    _PartRVA = _PartIndex;//失败，重置p_PartX轴开始
                }
            }
            return -1;
        }

        /// <summary>
        /// 检查色彩(可以根据这个更改比较方式
        /// </summary>
        /// <param name=”p_CurrentlyColor”>当前色彩</param>
        /// <param name=”p_CompareColor”>比较色彩</param>
        /// <param name=”p_Float”>溶差</param>
        /// <returns></returns>
        private bool ScanColor(Color p_CurrentlyColor, Color p_CompareColor, int p_Float)
        {
            int _R = p_CurrentlyColor.R;
            int _G = p_CurrentlyColor.G;
            int _B = p_CurrentlyColor.B;
            return (_R <= p_CompareColor.R + p_Float && _R >= p_CompareColor.R - p_Float) && (_G <= p_CompareColor.G + p_Float && _G >= p_CompareColor.G - p_Float) && (_B <= p_CompareColor.B + p_Float && _B >= p_CompareColor.B - p_Float);
        }

        /// <summary>
        /// 图像二值化1：取图片的平均灰度作为阈值，低于该值的全都为0，高于该值的全都为255
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp1(Bitmap bmp)
        {
            int average = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    average += color.B;
                }
            }
            average = (int)average / (bmp.Width * bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    int value = 255 - color.B;
                    Color newColor = value > average ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

        /// <summary>
        /// 图像二值化2
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp2(Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
            for (int y = 0; y < h; y++)
            {
                byte[] scan = new byte[(w + 7) / 8];
                for (int x = 0; x < w; x++)
                {
                    Color c = img.GetPixel(x, y);
                    if (c.GetBrightness() >= 0.5) scan[x / 8] |= (byte)(0x80 >> (x % 8));
                }
                Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
            }
            return bmp;
        }

        /// <summary>
        /// 圖片內容比較2-1
        /// Refer: http://fecbob.pixnet.net/blog/post/38125033-c%23-%E5%9C%96%E7%89%87%E5%85%A7%E5%AE%B9%E6%AF%94%E8%BC%83
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public struct RGBdata
        {
            public int r;
            public int g;
            public int b;

            public int GetLargest()
            {
                if (r > b)
                {
                    if (r > g)
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    return 3;
                }
            }
        }

        /// <summary>
        /// 圖片內容比較2-2
        /// Refer: http://fecbob.pixnet.net/blog/post/38125033-c%23-%E5%9C%96%E7%89%87%E5%85%A7%E5%AE%B9%E6%AF%94%E8%BC%83
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private RGBdata ProcessBitmap(Bitmap a)
        {
            BitmapData bmpData = a.LockBits(new Rectangle(0, 0, a.Width, a.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            RGBdata data = new RGBdata();

            unsafe
            {
                byte* p = (byte*)(void*)ptr;
                int offset = bmpData.Stride - a.Width * 3;
                int width = a.Width * 3;
                for (int y = 0; y < a.Height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        data.r += p[0];             //gets red values
                        data.g += p[1];             //gets green values
                        data.b += p[2];             //gets blue values
                        ++p;
                    }
                    p += offset;
                }
            }
            a.UnlockBits(bmpData);
            return data;
        }

        /// <summary>
        /// 圖片內容比較2-3
        /// Refer: http://fecbob.pixnet.net/blog/post/38125033-c%23-%E5%9C%96%E7%89%87%E5%85%A7%E5%AE%B9%E6%AF%94%E8%BC%83
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public double GetSimilarity(Bitmap a, Bitmap b)
        {
            RGBdata dataA = ProcessBitmap(a);
            RGBdata dataB = ProcessBitmap(b);
            double result = 0;
            int averageA = 0;
            int averageB = 0;
            int maxA = 0;
            int maxB = 0;
            maxA = ((a.Width * 3) * a.Height);
            maxB = ((b.Width * 3) * b.Height);

            switch (dataA.GetLargest())            //Find dominant color to compare
            {
                case 1:
                    {
                        averageA = Math.Abs(dataA.r / maxA);
                        averageB = Math.Abs(dataB.r / maxB);
                        result = (averageA - averageB) / 2;
                        break;
                    }
                case 2:
                    {
                        averageA = Math.Abs(dataA.g / maxA);
                        averageB = Math.Abs(dataB.g / maxB);
                        result = (averageA - averageB) / 2;
                        break;
                    }
                case 3:
                    {
                        averageA = Math.Abs(dataA.b / maxA);
                        averageB = Math.Abs(dataB.b / maxB);
                        result = (averageA - averageB) / 2;
                        break;
                    }
            }

            result = Math.Abs((result + 100) / 100);
            if (result > 1.0)
            {
                result -= 1.0;
            }

            return result;
        }
        #endregion

    }
}
