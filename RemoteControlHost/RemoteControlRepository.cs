using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteControlHost.Library;

namespace RemoteControlHost
{
    public class RemoteControlRepository
    {
        [ImportMany(typeof(IRemoteControlModule))] 
        private IEnumerable<IRemoteControlModule> _modules;

        public IDictionary<string, IRemoteControlModule> Modules { get; private set; }
        
        private CompositionContainer _container;

        public RemoteControlRepository()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));

            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);

            var modulesDict = _modules.ToDictionary(remoteControlModule => remoteControlModule.ModuleName);
            Modules = new ReadOnlyDictionary<string, IRemoteControlModule>(modulesDict);
        }
    }
}
