using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Watering2.Models;

namespace Watering2.Configuration
{
    public sealed class ConfigController : INotifyPropertyChanged
    {
        private WateringConfig _wateringConfiguration;
        private System.Configuration.Configuration _configuration;
        private bool _configChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public ConfigController()
        {
            ReadSettings();
            _configChanged = false;
            if (_wateringConfiguration != null)
                _wateringConfiguration.PropertyChanged += WateringConfiguration_PropertyChanged;
        }

        private void WateringConfiguration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _configChanged = true;
        }

        private void ReadSettings()
        {
            _wateringConfiguration = new WateringConfig();
            _wateringConfiguration.SectionInformation.ForceSave = true;

            _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if(_configuration?.Sections == null)
                return;
            if (_configuration.Sections["MyConfiguration"] == null)
            {
                _configuration.Sections.Add("MyConfiguration", _wateringConfiguration);
                _configuration.Save(ConfigurationSaveMode.Full);
            }
            _wateringConfiguration = _configuration.GetSection("MyConfiguration") as WateringConfig;

        }

        public WateringConfig Configuration => _wateringConfiguration;

        public bool ConfigurationChanged => _configChanged;

        public void SaveSettings()
        {
            _configuration.Save(ConfigurationSaveMode.Modified);
            _configChanged = false;
            OnPropertyChanged();
        }


        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
