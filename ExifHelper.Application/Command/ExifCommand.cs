using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExifHelper.Application.Command
{
    /*
     Example on how to load (needs to change to something better):
        var request = new ImageCopyCommandRequest();
        var response = new ImageCopyCommandResponse();

        var command = _exifCommands(response, request);
        response = (ImageCopyCommandResponse)(command.PerformCommand(request)).Result;
     */

    public abstract class ExifCommand<Tout, Tin> : IExifCommand where Tin : IExifCommandRequest where Tout : IExifCommandResponse
    {
        async Task<object> IExifCommand.PerformCommandAsync(object command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (command is not Tin)
                throw new InvalidOperationException("Object is not a command.");

            return await PerformCommandAsync((Tin)command);
        }

        object IExifCommand.PerformCommand(object command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (command is not Tin)
                throw new InvalidOperationException("Object is not a command.");

            return PerformCommand((Tin)command);
        }

        public abstract Task<Tout> PerformCommandAsync(Tin command);
        public abstract Tout PerformCommand(Tin command);
    }
}
