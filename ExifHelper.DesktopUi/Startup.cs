using Autofac;
using ExifHelper.Application.Configuration.Model;
using ExifHelper.Ioc;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;

namespace ExifHelper.DesktopUi
{
    public class StartUp
    {
        private static string? _configurationJsonFilePath;

        [STAThread]
        static void Main(string[] args)
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!Directory.Exists(currentDirectory)) 
                throw new InvalidOperationException("Cannot find current directory");

            _configurationJsonFilePath = Path.Combine(currentDirectory, "appSettings.json");
            string configurationJson = File.ReadAllText(_configurationJsonFilePath);
            var configuration = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(configurationJson) ?? new Configuration();

            var builder = new ContainerBuilder();

            builder.RegisterModule(new ConfigurationSetup(configuration));
            builder.RegisterModule(new DefaultModule());

            builder.RegisterType<MainWindow>().AsSelf();

            builder.RegisterAssemblyTypes(typeof(IExifPage).Assembly);

            builder.Register<Func<Type, IExifPage>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return (pageType) =>
                {
                    return (IExifPage)context.Resolve(pageType);
                };
            });

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var main = scope.Resolve<MainWindow>();
                main.ShowDialog();
            }

            configurationJson = Newtonsoft.Json.JsonConvert.SerializeObject(configuration);
            File.WriteAllText(_configurationJsonFilePath, configurationJson);
        }
    }
}
