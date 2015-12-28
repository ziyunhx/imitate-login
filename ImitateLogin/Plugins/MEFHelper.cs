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
		IEnumerable<Lazy<IMEFOperation, IMEFOperationData>> operations;

		private CompositionContainer _container;

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

				if (string.IsNullOrEmpty (path))
					catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(Environment.CurrentDirectory, "Extensions/MEF")));
				else
					catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(Environment.CurrentDirectory, path)));

				//Create the CompositionContainer with the parts in the catalog
				_container = new CompositionContainer(catalog);

				//Fill the imports of this object
				this._container.ComposeParts(this);
			}
			catch (CompositionException compositionException)
			{
				ILog logger = LogManager.GetLogger(typeof(MEFHelper));
				logger.Error(compositionException.ToString());
			}
		}

		/// <summary>
		/// Operation the specified loginSite, imageUrl and image.
		/// </summary>
		/// <param name="loginSite">Login site.</param>
		/// <param name="imageUrl">Image URL.</param>
		/// <param name="image">Image.</param>
		public string Operation(LoginSite loginSite, string imageUrl = "", Image image = null)
		{
			try
			{
				foreach (Lazy<IMEFOperation, IMEFOperationData> i in operations)
				{
					if (i.Metadata.loginSite == loginSite)
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