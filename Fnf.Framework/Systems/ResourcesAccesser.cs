using Fnf.Framework.Properties;

namespace Fnf.Framework
{
    public static class ResourcesAccesser
    {
        public static T GetResource<T>(string name)
        {
            return (T)Resources.ResourceManager.GetObject(name, Resources.Culture);
        }
    }
}