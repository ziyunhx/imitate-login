using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImitateLogin
{
    public static class MEFHelper
    {
        public static T ComposePartsSelf<T>(this T obj) where T : class
        {
            var catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ILogin).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(Environment.CurrentDirectory, "Extensions/MEF")));
            //catalog.Catalogs.Add(new DirectoryCatalog("addin"));

            var _container = new CompositionContainer(catalog);

            _container.ComposeParts(obj);

            return obj;
        }
    }
}