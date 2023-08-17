using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Ramon.FileTypes.RenPy
{
    internal class RenPySniffer : INativeFileSniffer
    {
        public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
        {
            SniffInfo info = new SniffInfo();

            info.IsSupported = RenPyFile.IsRenPyFile(nativeFilePath);

            return info;
        }
    }
}
