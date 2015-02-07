using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace RP.Util.Class
{
    public class Imagem
    {
        public static System.Drawing.Image CropAndResizeImage(Image image, int ImgWidth, int ImgHeight)
        {
            Image ImageResized, ImageCroped;

            // redimensiona a imagem para o tamanho necessario
            ImageResized = ResizeImage(image, ImgWidth, ImgHeight);

            // Armazena a imagem
            ImageCroped = ImageResized;

            // se a largura ou altura da imagem for maior que o definido
            //if (ImageResized.Width > ImgWidth || ImageResized.Height > ImgHeight)
            //{
            //    // corta a imagem tendo o centro da imagem como base
            //    ImageCroped = CropImage(ImageResized, ImgWidth, ImgHeight, ((ImageResized.Width - ImgWidth) / 2), ((ImageResized.Height - ImgHeight) / 2));
            //}

            return ImageCroped;
        }

        /*public static Image AppendBorder(Image original, int borderWidth)
        {
            var borderColor = Color.White;

            var newSize = new Size(
                original.Width + borderWidth * 2,
                original.Height + borderWidth * 2);

            var img = new Bitmap(newSize.Width, newSize.Height);
            var g = Graphics.FromImage(img);

            g.Clear(borderColor);
            g.DrawImage(original, new Point(borderWidth, borderWidth));
            g.Dispose();

            return img;
        }*/

        //Image resizing
        public static System.Drawing.Image ResizeImage(Image Image, int maxWidth, int maxHeight)
        {
            //return FixedSize(Image, maxWidth, maxHeight, true);
            int width = Image.Width;
            int height = Image.Height;
            if (width > maxWidth || height > maxHeight)
            {
                //The flips are in here to prevent any embedded image thumbnails -- usually from cameras
                //from displaying as the thumbnail image later, in other words, we want a clean
                //resize, not a grainy one.
                Image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipX);
                Image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipX);

                float ratio = 0;
                if (width > height)
                {
                    ratio = (float)width / (float)height;
                    width = maxWidth;
                    height = Convert.ToInt32(Math.Round((float)width / ratio));
                }
                else
                {
                    ratio = (float)height / (float)width;
                    height = maxHeight;
                    width = Convert.ToInt32(Math.Round((float)height / ratio));
                }

                //Rectangle destRect = new Rectangle(0, 0, maxWidth, maxHeight);
                //// Draw image to screen.
                //e.Graphics.DrawImage(newImage, destRect);

                //return the resized image
                return Image.GetThumbnailImage(width, height, null, IntPtr.Zero);
            }
            //return the original resized image
            return Image;
        }

        public static Image FixedSize(Image imgPhoto, int Width, int Height, bool needToFill)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (!needToFill)
            {
                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                }
                else
                {
                    nPercent = nPercentW;
                }
            }
            else
            {
                if (nPercentH > nPercentW)
                {
                    nPercent = nPercentH;
                    destX = (int)Math.Round((Width -
                        (sourceWidth * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentW;
                    destY = (int)Math.Round((Height -
                        (sourceHeight * nPercent)) / 2);
                }
            }

            if (nPercent > 1)
                nPercent = 1;

            int destWidth = (int)Math.Round(sourceWidth * nPercent);
            int destHeight = (int)Math.Round(sourceHeight * nPercent);

            System.Drawing.Bitmap bmPhoto = new System.Drawing.Bitmap(
                destWidth <= Width ? destWidth : Width,
                destHeight < Height ? destHeight : Height,
                              PixelFormat.Format32bppRgb);
            //bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
            //                 imgPhoto.VerticalResolution);

            System.Drawing.Graphics grPhoto = System.Drawing.Graphics.FromImage(bmPhoto);
            grPhoto.Clear(System.Drawing.Color.White);
            grPhoto.InterpolationMode = InterpolationMode.Default;
            //InterpolationMode.HighQualityBicubic;
            grPhoto.CompositingQuality = CompositingQuality.Default;
            grPhoto.SmoothingMode = SmoothingMode.Default;

            grPhoto.DrawImage(imgPhoto,
                new System.Drawing.Rectangle(destX, destY, destWidth, destHeight),
                new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                System.Drawing.GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        //Overload for crop that default starts top left of the image.
        public static System.Drawing.Image CropImage(Image Image, int Width, int Height)
        {
            return CropImage(Image, Width, Height, 0, 0);
        }

        //The crop image sub
        public static System.Drawing.Image CropImage(Image Image, int Width, int Height, int StartAtX, int StartAtY)
        {
            Image outimage;
            System.IO.MemoryStream mm = null;
            try
            {
                //check the image height against our desired image height
                if (Image.Height < Height)
                {
                    Height = Image.Height;
                }

                if (Image.Width < Width)
                {
                    Width = Image.Width;
                }

                //create a bitmap window for cropping
                Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                bmPhoto.SetResolution(72, 72);

                //create a new graphics object from our image and set properties
                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;

                //now do the crop
                grPhoto.DrawImage(Image, new Rectangle(0, 0, Width, Height), StartAtX, StartAtY, Width, Height, GraphicsUnit.Pixel);

                // Save out to memory and get an image from it to send back out the method.
                using (mm = new System.IO.MemoryStream())
                {
                    bmPhoto.Save(mm, System.Drawing.Imaging.ImageFormat.Jpeg);
                    Image.Dispose();
                    bmPhoto.Dispose();
                    grPhoto.Dispose();
                    outimage = Image.FromStream(mm);
                }

                return outimage;
            }
            catch (System.Exception ex)
            {
                throw new RP.Util.Exception.RPException("Error cropping image, the error was: " + ex.Message);
            }
        }

        //Hard resize attempts to resize as close as it can to the desired size and then crops the excess
        public static System.Drawing.Image HardResizeImage(Image Image, int Width, int Height)
        {
            //// RESIZE PERCENT
            //int sourceWidth = imgToResize.Width;
            //int sourceHeight = imgToResize.Height;

            //float nPercent = 0;
            //float nPercentW = 0;
            //float nPercentH = 0;

            //nPercentW = ((float)size.Width / (float)sourceWidth);
            //nPercentH = ((float)size.Height / (float)sourceHeight);

            //if (nPercentH < nPercentW)
            //    nPercent = nPercentH;
            //else
            //    nPercent = nPercentW;

            //int destWidth = (int)(sourceWidth * nPercent);
            //int destHeight = (int)(sourceHeight * nPercent);


            int width = Image.Width;
            int height = Image.Height;
            Image resized = null;
            if (Width > Height)
            {
                resized = ResizeImage(Image, Width, Width);
            }
            else
            {
                resized = ResizeImage(Image, Height, Height);
            }
            Image output = CropImage(resized, Height, Width);
            //return the original resized image
            return output;
        }

        public static bool IsImage(string ext)
        {
            return ext.ToLower() == ".gif" || ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp";
        }
    }
}
