using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public class ProfileBuilder : IProfileBuilder
    {
        private readonly string _machineName;
        private readonly PluginGraph _pluginGraph;
        private readonly ProfileManager _profileManager;
        private string _lastProfile;
        private bool _useMachineOverrides;


        public ProfileBuilder(PluginGraph pluginGraph, string machineName)
        {
            _pluginGraph = pluginGraph;
            _profileManager = pluginGraph.ProfileManager;
            _machineName = machineName;
        }


        public ProfileBuilder(PluginGraph pluginGraph)
            : this(pluginGraph, GetMachineName())
        {
        }

        #region IProfileBuilder Members

        public void AddProfile(string profileName)
        {
            _lastProfile = profileName;
        }

        public void OverrideProfile(TypePath typePath, string instanceKey)
        {
            // TODO:  what if the Type cannot be found?

            ReferencedInstance instance = new ReferencedInstance(instanceKey);
            _profileManager.SetDefault(_lastProfile, typePath.FindType(), instance);
        }

        public void AddMachine(string machineName, string profileName)
        {
            _useMachineOverrides = machineName == _machineName;

            if (_useMachineOverrides)
            {
                _profileManager.DefaultMachineProfileName = profileName;
            }
        }

        public void OverrideMachine(TypePath typePath, string instanceKey)
        {
            if (!_useMachineOverrides)
            {
                return;
            }

            // TODO:  what if the Type cannot be found?
            ReferencedInstance instance = new ReferencedInstance(instanceKey);
            _profileManager.SetMachineDefault(typePath.FindType(), instance);
        }

        public void SetDefaultProfileName(string profileName)
        {
            _profileManager.DefaultProfileName = profileName;
        }

        #endregion

        public static string GetMachineName()
        {
            string machineName = string.Empty;
            try
            {
                machineName = Environment.MachineName.ToUpper();
            }
            finally
            {
            }

            return machineName;
        }
    }
}