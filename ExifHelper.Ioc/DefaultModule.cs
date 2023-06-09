using Autofac;
using ExifHelper.Application.Command;
using System.Reflection;

namespace ExifHelper.Ioc
{
    public class DefaultModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IExifCommand).Assembly).AsClosedTypesOf(typeof(ExifCommand<,>));

            builder.Register<Func<object, object, IExifCommand>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return (commandResponse, commandRequest) =>
                {
                    var commandType = typeof(ExifCommand<,>).MakeGenericType(commandResponse.GetType(), commandRequest.GetType());
                    return (IExifCommand)context.Resolve(commandType);
                };
            });
        }
    }
}