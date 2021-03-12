using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kamu.ModelFramework
{
    public static class ModelProviderFactory
    {
        private static Dictionary<string, Func<Uri, ModelContainer, ModelProvider>> _providerFactory = new Dictionary<string, Func<Uri, ModelContainer, ModelProvider>>();

        public static IEnumerable<string> Schemes => _providerFactory.Keys;

        public static void Register(Type type)
        {
            if (!type.IsSubclassOf(typeof(ModelProvider)))
                throw new ArgumentNullException($"{type.Name} is not a subclass of ModelProvider");

            var schemeAttribute = type.GetCustomAttribute<SchemeAttribute>(false);

            if (schemeAttribute == null)
                throw new NotImplementedException(nameof(SchemeAttribute));

            Register(type, schemeAttribute.Scheme);
        }

        private static void Register(Type type, string scheme)
        {
            var constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                                                    new Type[] { }, null);
            var uriSetMethod = typeof(ModelProvider).GetProperty("Uri").GetSetMethod(true);
            var modelsSetMethod = typeof(ModelProvider).GetProperty("Models", BindingFlags.Instance | BindingFlags.NonPublic)
                                                        .GetSetMethod(true);
            _providerFactory[scheme] = (u, c) =>
                {
                    var provider = constructorInfo.Invoke(null) as ModelProvider;
                    uriSetMethod.Invoke(provider, new object[] { u });
                    modelsSetMethod.Invoke(provider, new object[] { c });
                    return provider;
                };
        }

        public static void Reset() => _providerFactory.Clear();

        internal static ModelProvider Create(Uri uri, ModelContainer container)
        {
            if (_providerFactory.TryGetValue(uri.Scheme, out var factory))
            {
                return factory(uri, container);
            }

            var schemes = Schemes.ToArray();
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                                                                        .Where(t => t.IsSubclassOf(typeof(ModelProvider))))
            {
                var schemeAttribute = type.GetCustomAttribute<SchemeAttribute>(false);
                if (schemeAttribute != null && !schemes.Contains(schemeAttribute.Scheme))
                {
                    if (_providerFactory.ContainsKey(schemeAttribute.Scheme))
                        throw new ArgumentException($"{schemeAttribute.Scheme} is already registered.");

                    Register(type, schemeAttribute.Scheme);
                }
            }

            if (_providerFactory.TryGetValue(uri.Scheme, out factory))
            {
                return factory(uri, container);
            }

            throw new ArgumentException($"\'{uri.Scheme}\' is not supported.");
        }
    }
}