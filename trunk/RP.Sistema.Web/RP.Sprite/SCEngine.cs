using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using RP.Sprite.Tools.BinPacking;

namespace RP.Sprite.Shell
{
	public class SCEngine
	{
		SCEngineOptions _options = null;
		private SCEngineOptions Options { get { return _options; } }

		public void GenerateSprite(SCEngineOptions options)
		{
			_options = options;

			//var ImageBlocks = GetGroupedImagesBySize();
			List<SpriteImageItem> imageBlocks;
			Size spriteSize;
			GetImagesInfo(out imageBlocks, out spriteSize, options.Padding);

			Bitmap sprite = new Bitmap(spriteSize.Width, spriteSize.Height);
			StringBuilder sbCSS = new StringBuilder();
			//StringBuilder sbHTML = new StringBuilder();

			Graphics g = Graphics.FromImage(sprite);

			g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            var classNames = "." + options.CssClassPrefix + "sprite";
            
            for (int i = 0; i < imageBlocks.Count; i++)
			{
				// add image to sprite
				var img = new Bitmap(imageBlocks[i].Image);
				var pd = img.PhysicalDimension;
				var location = new Point(imageBlocks[i].Position.X, imageBlocks[i].Position.Y);

				img.SetResolution(sprite.HorizontalResolution, sprite.VerticalResolution);

				g.DrawImage(img, location);

				/*	== Draws a rectangle around the image. Usefull for debugging == */
				// g.DrawRectangle(new Pen(new SolidBrush(Color.Red), 1), new Rectangle(location, img.Size));

				// create css class
				string className = options.CssClassPrefix + Path.GetFileNameWithoutExtension(imageBlocks[i].Name).Replace(" ", "-");
				sbCSS.AppendLine(
					string.Format(
						".{0} {{ background-position: {1}px {2}px; width: {3}px; height: {4}px; }}",
						className,
						(-imageBlocks[i].Position.X).ToString(),
						(-imageBlocks[i].Position.Y - sprite.Height).ToString(),
						imageBlocks[i].Image.Width.ToString(),
						imageBlocks[i].Image.Height.ToString()
						)
					);

                classNames += ", ." + className;
				// create sample html page
				//sbHTML.AppendLine("<div style=\"float:left;\" class=\"spriteImage " + className + "\"></div>");
			}

			// save files
			//SaveFiles(sprite, sbCSS, sbHTML);
            SaveFiles(sprite, sbCSS, classNames, options.DestinationVirtualPathImage);
        }

		/// <summary>
		/// Save everything to disk
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="sbCSS"></param>
		/// <param name="sbHTML"></param>
		//private void SaveFiles(Bitmap sprite, StringBuilder sbCSS, StringBuilder sbHTML)
		private void SaveFiles(Bitmap sprite, StringBuilder sbCSS, string classNames, string virtualPathImage)
		{
			if (!Directory.Exists(Options.DestinationDirectoryImage))
			{
				if (Options.CreateDirectory)
				{
					Directory.CreateDirectory(Options.DestinationDirectoryImage);
				}
			}

            if (!Directory.Exists(Options.DestinationDirectoryCSS))
            {
                if (Options.CreateDirectory)
                {
                    Directory.CreateDirectory(Options.DestinationDirectoryCSS);
                }
            }
	
			FileMode fileMode = Options.Override ? FileMode.Create : FileMode.CreateNew;
			string spriteFileName = Path.Combine(Options.DestinationDirectoryImage, Options.FileNameImage + ".png");
			string cssFileName = Path.Combine(Options.DestinationDirectoryCSS, Options.FileNameCSS + ".css");
            //string demoHTMLFileName = Path.Combine(Options.DestinationDirectoryCSS, Options.FileNameCSS + ".html");
            //if (!Options.Override && (File.Exists(spriteFileName) || File.Exists(cssFileName) || File.Exists(demoHTMLFileName)))
            if (!Options.Override && (File.Exists(spriteFileName) || File.Exists(cssFileName)))
			{
				throw new Exception("Arquivos já existem.");
			}

			// write image file
			Console.WriteLine("Writing image file");
			FileStream fsSprite = new FileStream(spriteFileName, fileMode);
			sprite.Save(fsSprite, System.Drawing.Imaging.ImageFormat.Png);
			fsSprite.Flush();
			fsSprite.Close();

			// write CSS file
			Console.WriteLine("Writing CSS file");
			sbCSS.Insert(0, classNames + " { background-image: url(" + virtualPathImage + Options.FileNameImage + ".png) } ");
			FileStream fsCSS = new FileStream(cssFileName, fileMode);
			StreamWriter swCSS = new StreamWriter(fsCSS);
			swCSS.Write(sbCSS.ToString());
			swCSS.Flush();
			fsCSS.Close();

            //// write HTML file
            //Console.WriteLine("Writing demo HTML file");
            //sbHTML.Insert(0, "<head><LINK REL=StyleSheet HREF=\"" + Options.FileNameCSS + ".css\" TYPE=\"text/css\"></head><body>");
            //sbHTML.Append("</body>");
            //FileStream fsHTML = new FileStream(demoHTMLFileName, fileMode);
            //StreamWriter swHTML = new StreamWriter(fsHTML);
            //swHTML.Write(sbHTML.ToString());
            //swHTML.Flush();
            //fsHTML.Close();
		}


		/// <summary>
		/// Returns a list with the image files available to be added to the sprite
		/// </summary>
		/// <returns></returns>
		private List<FileInfo> GetFiles()
		{
			DirectoryInfo dir = new DirectoryInfo(Options.SourceDirectory);
			if (!dir.Exists)
			{
				Console.WriteLine("Source files directory not found!");
				Environment.Exit(99);
			}

			List<FileInfo> files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList();
			files = (from f in files
					 where
						f.Name.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) ||
						f.Name.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase) ||
						f.Name.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase) ||
						f.Name.EndsWith(".bmp", StringComparison.InvariantCultureIgnoreCase)
					 orderby f.Name
					 select f).ToList();

			if (files.Count == 0)
			{
				Console.WriteLine("No images found!");
				Environment.Exit(100);
			}

			return files;
		}

		/// <summary>
		/// Calculates the image os the resulting sprite based on the available files.
		/// This method will group images by size and put them in a square.
		/// After that all squares will be fitted inside the final big square.
		/// </summary>
		/// <returns></returns>
		public Dictionary<Size, List<SpriteImageItem>> GetGroupedImagesBySize()
		{
			var ImagesBySize = new Dictionary<Size, List<SpriteImageItem>>();

			foreach (var file in GetFiles())
			{
				var image = Image.FromFile(file.FullName);
				if (!ImagesBySize.ContainsKey(image.Size))
				{
					ImagesBySize.Add(image.Size, new List<SpriteImageItem>());
				}

				ImagesBySize[image.Size].Add(new SpriteImageItem() { Image = image, Name = file.Name });
			}

			return ImagesBySize;
		}

		/// <summary>
		/// Collect information about the images and bin pack them using the chosen algorithm
		/// </summary>
		/// <param name="imagesInfo"></param>
		/// <param name="spriteSize"></param>
		public void GetImagesInfo(out List<SpriteImageItem> imagesInfo, out Size spriteSize, int padding)
		{
			imagesInfo = new List<SpriteImageItem>();

			// calculate initial sprite size to optimize process speed
			//	-> make sure that the widest and the tallest images fit
			//	-> make sure the resulting area is the same as the sum of all images area
			int maxW = 0;
			int maxH = 0;
			//int totalArea = 0;
			foreach (var file in GetFiles())
			{
				var image = Image.FromFile(file.FullName);
				imagesInfo.Add(new SpriteImageItem() { Image = image, Name = file.Name });

				if (image.Width > maxW)
					maxW = image.Width;
				if (image.Height > maxH)
					maxH = image.Height;

				//totalArea = image.Width * image.Height;
			}

			int finalW = 0;
			int finalH = 0;

			//int totalW = (int)Math.Ceiling((decimal)totalArea / 2);
			//int totalH = totalW;

			finalW = maxW;// maxW > totalW ? maxW : totalW;
			finalH = maxH; // maxH > totalH ? maxH : totalH;

			List<SpriteImageItem> unfitImagesInfo = new List<SpriteImageItem>();
			bool finished = false;
			while (!finished)
			{
				// start packing process
				RectanglePacker packer = null;
				switch (this.Options.BinPackingLevel)
				{
					case 1:		// faster and less compless algorithm
						packer = new SimpleRectanglePacker(finalW, finalH);
						break;
					case 2:		// still fast and much better packing algorithm
						packer = new CygonRectanglePacker(finalW, finalH);
						break;
					case 3:		// the best packing algorithm. Addicional processing time might not be noticeable
						packer = new ArevaloRectanglePacker(finalW, finalH);
						break;
					default:	// default
						packer = new CygonRectanglePacker(finalW, finalH);
						break;
				}

				//var usedArea = 0;
				foreach (var iInfo in imagesInfo)
				{
					// Find a place for a rectangle of size 30x20 in the packing area
					Point placement;
                    if (packer.TryPack(iInfo.Image.Width + padding, iInfo.Image.Height + padding, out placement))
					{
						iInfo.Position = placement;
						//usedArea += iInfo.Image.Width * iInfo.Image.Height;
					}
					else
					{
						unfitImagesInfo.Add(iInfo);
					}
				}

				if (unfitImagesInfo.Count == 0)
				{
					finished = true;
				}
				else
				{
					// add the diff between the used area and the remaining images area
					//var extraW = 0;
					//var extraH = 0;
					//foreach (var unfit in unfitImagesInfo)
					//{
					//    extraW += unfit.Image.Width;
					//    extraH += unfit.Image.Height;
					//}

					// after some tests the speed and result are quite the same as adding a few more pixels to the 
					//	sprite final size and run the whole process again
					finalW += 10;
					finalH += 10;

					unfitImagesInfo.Clear();
				}
			}

			spriteSize = new Size(finalW, finalH);
		}



	}
}
