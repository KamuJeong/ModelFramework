using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kamu.ModelFramework
{
    public static class ModelProviderFactory
    {
        private static Dictionary<string, Func<Uri, ModelContainer, ModelProvider>> _providerFactory = new Dictionary<string, Func<Uri, ModelContainer, ModelProvider>>();

        /// <summary>
        /// Factory function registration for a provider with a corresponding scheme
        /// </summary>
        /// <param name="scheme"> URI's scheme for provider ex) "pump", "chromatogram" etc. </param>
        /// <param name="factory"> factory function to instantiate a provider </param>
        public static void Register(string scheme, Func<Uri, ModelContainer, ModelProvider> factory) => _providerFactory.Add(scheme, factory);
        
        internal static ModelProvider Create(Uri uri, ModelContainer container) 
        {
            if(_providerFactory.TryGetValue(uri.Scheme, out var factory))
            {
                return factory(uri, container);      
            }

            foreach(var type  in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(ModelProvider))))
            {
                var counter = type.GetProperty("Scheme", BindingFlags.Static | BindingFlags.Public);
                if(counter != null)
                    counter.GetValue(null);
            }

            if(_providerFactory.TryGetValue(uri.Scheme, out factory))
            {
                return factory(uri, container);      
            }
            throw new ArgumentException($"\'{uri.Scheme}\' is not supported.");
        }

        public static IEnumerable<string> Schemes => _providerFactory.Keys;
    }
}