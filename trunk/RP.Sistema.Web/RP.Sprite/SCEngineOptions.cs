using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RP.Sprite.Shell
{
	public class SCEngineOptions
	{

		private string _sourceDirectory;
		private string _destinationDirectoryImage;
	    private string _destinationVirtualPathImage;
        private string _destinationDirectoryCSS;
        private string _fileNameImage = Guid.NewGuid().ToString();
        private string _fileNameCSS = Guid.NewGuid().ToString();
	    private int _padding = 10;

		/// <summary>
		/// Directory to get the images from
		/// </summary>
		public string SourceDirectory
		{
			get 
			{
				if (string.IsNullOrEmpty(_sourceDirectory))
				{
					_sourceDirectory = Environment.CurrentDirectory;
				}
				return _sourceDirectory;
			}
			set { _sourceDirectory = value; }
		}

		/// <summary>
		/// Directory where to put the process result (sprite file)
		/// </summary>
		public string DestinationDirectoryImage
		{
			get 
			{
				if (string.IsNullOrEmpty(_destinationDirectoryImage))
				{
					_destinationDirectoryImage = SourceDirectory;
				}
				return _destinationDirectoryImage; }
			set { _destinationDirectoryImage = value; }
		}

        /// <summary>
        /// Directory where to put the process result (sprite file)
        /// </summary>
        public string DestinationVirtualPathImage
        {
            get { return _destinationVirtualPathImage; }
            set { _destinationVirtualPathImage = value; }
        }

        /// <summary>
        /// Directory where to put the process result (css file)
        /// </summary>
        public string DestinationDirectoryCSS
        {
            get
            {
                if (string.IsNullOrEmpty(_destinationDirectoryCSS))
                {
                    _destinationDirectoryCSS = SourceDirectory;
                }
                return _destinationDirectoryCSS;
            }
            set { _destinationDirectoryCSS = value; }
        }

        public int Padding
	    {
	        get { return _padding; }
            set { _padding = value; }
	    }

	    /// <summary>
		/// prefix used before every css class
		/// </summary>
		public string CssClassPrefix { get; set; }

		/// <summary>
		/// specify packing complexity level (1 to 3)
		/// </summary>
		public int BinPackingLevel { get; set; }

		/// <summary>
		/// The name of the files being generated.
		/// Three files are being generated: the sprite, the css and a demo html.
		/// All of them will have the same name and different file extensions.
		/// By default this is a GUID.
		/// </summary>
		public string FileNameImage 
		{
			get
			{
				return _fileNameImage;
			}
			set
			{
				_fileNameImage = value;
			}
		}

        /// <summary>
        /// The name of the files being generated.
        /// Three files are being generated: the sprite, the css and a demo html.
        /// All of them will have the same name and different file extensions.
        /// By default this is a GUID.
        /// </summary>
        public string FileNameCSS
        {
            get
            {
                return _fileNameCSS;
            }
            set
            {
                _fileNameCSS = value;
            }
        }

		/// <summary>
		/// Gets or sets whether the process creates destination directory if doesn't exist or not
		/// </summary>
		public bool CreateDirectory { get; set; }

		/// <summary>
		/// Gets or set whether the process should override existing files or not
		/// </summary>
		public bool Override { get; set; }

	}
}
