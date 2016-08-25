using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using log4net;
using System.Drawing;

namespace ImitateLogin
{
	/// <summary>
	/// MEF helper.
	/// </summary>
    public class MEFHelper
    {
		[ImportMany]
		IEnumerable<Lazy<IMEFOperation, ILoginSiteData>> operations;

		private CompositionContainer _container;
        ILog logger = LogManager.GetLogger(typeof(MEFHelper));

        /// <summary>
        /// Initializes a new instance of the <see cref="ImitateLogin.MEFHelper"/> class.
        /// </summary>
        /// <param name="path">Path.</param>
        public MEFHelper(string path = "")
		{
			try
			{
				//An aggregate catalog that combines multiple catalogs
				var catalog = new AggregateCatalog();
				//Adds all the parts found in the same assembly as the Program class
				catalog.Catalogs.Add(new AssemblyCatalog(typeof(MEFHelper).Assembly));

				if(string.IsNullOrEmpty(path))
					path = "Extensions";

                if (Directory.Exists(path))
                    catalog.Catalogs.Add(new DirectoryCatalog(path));
                else if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)))
                    catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)));
                else
                    logger.Warn("No MEF extensions path has configured.");

                //Create the CompositionContainer with the parts in the catalog
                _container = new CompositionContainer(catalog);

				//Fill the imports of this object
				this._container.ComposeParts(this);
			}
			catch (CompositionException compositionException)
			{
				logger.Error(compositionException.ToString());
			}
		}

		/// <summary>
		/// Operation the specified loginSite, imageUrl and image.
		/// </summary>
		/// <param name="loginSite">Login site.</param>
		/// <param name="imageUrl">Image URL.</param>
		/// <param name="image">Image.</param>
		public string Operation(string loginSite, string imageUrl = "", Image image = null)
		{
			try
			{
				foreach (Lazy<IMEFOperation, ILoginSiteData> i in operations)
				{
					if (i.Metadata.loginSite.ToLower() == loginSite.ToLower())
						return i.Value.Operate(imageUrl, image);
				}
				ILog logger = LogManager.GetLogger(typeof(MEFHelper));
				logger.Error("Operation Not Found!");
			}
			catch(Exception ex) 
			{
				ILog logger = LogManager.GetLogger(typeof(MEFHelper));
				logger.Error("Operation Not Found!" +Environment.NewLine + ex.ToString());
			}
			return "";
		}
    }
}