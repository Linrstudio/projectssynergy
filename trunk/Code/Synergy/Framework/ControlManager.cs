using System;

namespace Synergy
{
    public class ControlManager
    {
        static PluginManager pluginmanager = new PluginManager("Controls");
        static bool initialized = false;
        public static void Initialize()
        {
            if (!initialized)
            {
                pluginmanager.OnLoad += new PluginManager.OnLoadHandler(pluginmanager_OnLoad);
                pluginmanager.Load();
                initialized = true;
            }
        }

        static void pluginmanager_OnLoad(Plugin _LoadedPlugin)
        {

        }

        public static Control CreateControl(string _Name)
        {
            object o = pluginmanager.CreateInstance(_Name);
            try
            {
                return (Control)o;
            }
            catch { }
            return null;
        }
    }
}