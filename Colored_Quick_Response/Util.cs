using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colored_Quick_Response
{
    class Util
    {
        private CryptoQrUtilities.QrUtilitiesWrapper w;
        EuclideanColorFiltering filter;
        OtsuThreshold bw;
        ExtractChannel ecR;
        ExtractChannel ecG;
        ExtractChannel ecB;
        public Util()
        {
            w = new CryptoQrUtilities.QrUtilitiesWrapper();
            filter = new EuclideanColorFiltering();
            filter.CenterColor = new AForge.Imaging.RGB(Color.Black);
            filter.Radius = 0;
            bw = new OtsuThreshold();
            ecR = new ExtractChannel(RGB.R);
            ecG = new ExtractChannel(RGB.G);
            ecB = new ExtractChannel(RGB.B);
        }

        public Bitmap CreateQR(string Data, int Width = 300, int Height = 300)
        {
            return w.GetQrEncoding(Data, Width, Height);
        }
        public Bitmap CreatecQR(string Data, int Width = 300, int Height = 300)
        {
            for (int i = 0; i < Data.Length % 3; i++)
                Data += " ";
            Bitmap res = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int blocksize = Data.Length / 3;
            res = new Add(CreaterQR(Data, 0, blocksize, Width, Height)).Apply(res);
            res = new Add(CreategQR(Data, blocksize, blocksize, Width, Height)).Apply(res);
            res = new Add(CreatebQR(Data, blocksize * 2, blocksize, Width, Height)).Apply(res);
            return res;
        }
        public List<Bitmap> CreateAll(string Data, int Width = 300, int Height = 300)
        {
            for (int i = 0; i < Data.Length % 3; i++)
                Data += " ";
            Bitmap res = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int blocksize = Data.Length / 3;
            List<Bitmap> all = new List<Bitmap>(4);
            all.Add(CreaterQR(Data, 0, blocksize, Width, Height));
            all.Add(CreategQR(Data, blocksize, blocksize, Width, Height));
            all.Add(CreatebQR(Data, blocksize*2, blocksize, Width, Height));
            res = new Add(all[0]).Apply(res);
            res = new Add(all[1]).Apply(res);
            res = new Add(all[2]).Apply(res);
            all.Add(res);
            return all;
        }
        public Bitmap CreaterQR(string Data, int Index, int BlockSize, int Width = 300, int Height = 300)
        {
            Bitmap tmp = new Bitmap(Width, Height);
            filter.FillColor = new AForge.Imaging.RGB(255, 0, 0);
            tmp = w.GetQrEncoding(Data.Substring(Index, BlockSize), Width, Height);
            filter.ApplyInPlace(tmp);
            return tmp;
        }
        public Bitmap CreategQR(string Data, int Index, int BlockSize, int Width = 300, int Height = 300)
        {
            Bitmap tmp = new Bitmap(Width, Height);
            filter.FillColor = new AForge.Imaging.RGB(0, 255, 0);
            tmp = w.GetQrEncoding(Data.Substring(Index, BlockSize), Width, Height);
            filter.ApplyInPlace(tmp);
            return tmp;
        }
        public Bitmap CreatebQR(string Data, int Index, int BlockSize, int Width = 300, int Height = 300)
        {
            Bitmap tmp = new Bitmap(Width, Height);
            filter.FillColor = new AForge.Imaging.RGB(0, 0, 255);
            tmp = w.GetQrEncoding(Data.Substring(Index, BlockSize), Width, Height);
            filter.ApplyInPlace(tmp);
            return tmp;
        }
        public string ReadcQR(Bitmap bmp)
        {
            string R = ReadrQR(bmp);
            string G = ReadgQR(bmp);
            string B = ReadbQR(bmp);
            if (!string.IsNullOrEmpty(R) && !string.IsNullOrEmpty(G) && !string.IsNullOrEmpty(B))
                return R + G + B;
            else return "";
        }
        public Bitmap c2rQR(Bitmap bmp)
        {
            Bitmap tmp = ecR.Apply(bmp);
            tmp = bw.Apply(tmp);
            return tmp;
        }
        public Bitmap c2gQR(Bitmap bmp)
        {
            Bitmap tmp = ecG.Apply(bmp);
            tmp = bw.Apply(tmp);
            return tmp;
        }
        public Bitmap c2bQR(Bitmap bmp)
        {
            Bitmap tmp = ecB.Apply(bmp);
            tmp = bw.Apply(tmp);
            return tmp;
        }
        public List<Bitmap> c2QR(Bitmap bmp)
        {
            List<Bitmap> res = new List<Bitmap>(3);
            Bitmap tmp = ecR.Apply(bmp);
            tmp = bw.Apply(tmp);
            res.Add(tmp);
            tmp = ecG.Apply(bmp);
            tmp = bw.Apply(tmp);
            res.Add(tmp);
            tmp = ecB.Apply(bmp);
            tmp = bw.Apply(tmp);
            res.Add(tmp);
            return res;
        }
        public string ReadrQR(Bitmap bmp)
        {
            string res = "";
            Bitmap tmp = ecR.Apply(bmp);
            tmp = bw.Apply(tmp);
            res += w.GetQrDecoding(tmp);
            return res;
        }
        public string ReadgQR(Bitmap bmp)
        {
            string res = "";
            Bitmap tmp = ecG.Apply(bmp);
            tmp = bw.Apply(tmp);
            res += w.GetQrDecoding(tmp);
            return res;
        }
        public string ReadbQR(Bitmap bmp)
        {
            string res = "";
            Bitmap tmp = ecB.Apply(bmp);
            tmp = bw.Apply(tmp);
            res += w.GetQrDecoding(tmp);
            return res;
        }

        public string ReadQR(Bitmap bmp)
        {
            string data = "";
            data += w.GetQrDecoding(bmp);
            return data;
        }
    }
}
