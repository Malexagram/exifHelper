using Autofac;
using ExifHelper.Application.Configuration;
using ExifHelper.Application.Configuration.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExifHelper.Ioc
{
    public class ConfigurationSetup : Module
    {
        private Configuration _configuration;

        public ConfigurationSetup(Configuration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new ConfigurationManager(_configuration)).As<IConfigurationManager>();
        }
    }
}
