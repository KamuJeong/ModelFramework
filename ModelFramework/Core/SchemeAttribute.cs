using System;

namespace Kamu.ModelFramework
{
    [AttributeUsage( AttributeTargets.Class )]
    public class SchemeAttribute : Attribute
    {
        public string Scheme {get;}
        public SchemeAttribute(string scheme) => Scheme = scheme;
    }
}
