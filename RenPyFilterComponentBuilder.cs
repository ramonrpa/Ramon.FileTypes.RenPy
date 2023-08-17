using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Ramon.FileTypes.RenPy
{
    [FileTypeComponentBuilderAttribute(Id = "ComponentBuilderExtension_Id",
                                   Name = "ComponentBuilderExtension_Name",
                                   Description = "ComponentBuilderExtension_Description")]
    internal class RenPyFilterComponentBuilder : IFileTypeComponentBuilder
    {
        public IFileTypeManager FileTypeManager { get; set; }
        public IFileTypeDefinition FileTypeDefinition { get; set; }

        public IFileTypeInformation BuildFileTypeInformation(string name)
        {
            var info = this.FileTypeManager.BuildFileTypeInformation();

            info.FileTypeDefinitionId = new FileTypeDefinitionId("Ren'Py Translation File 1.0");
            info.FileTypeName = new LocalizableString("Ren'Py Translation Files");
            info.FileTypeDocumentName = new LocalizableString("Ren'Py Translation File");
            info.FileTypeDocumentsName = new LocalizableString("Ren'Py Translations Files");
            info.Description = new LocalizableString(PluginResources.ComponentBuilderExtension_Description);
            info.FileDialogWildcardExpression = "*.rpy";
            info.DefaultFileExtension = "rpy";
            info.Icon = new IconDescriptor(PluginResources.Plugin_Icon);
            info.Enabled = true;

            return info;
        }

        public INativeFileSniffer BuildFileSniffer(string name)
        {
            return new RenPySniffer();
        }

        public IFileExtractor BuildFileExtractor(string name)
        {
            var parser = new RenPyParser();
            var extractor = this.FileTypeManager.BuildFileExtractor(parser, this);
            return extractor;
        }

        public IAbstractGenerator BuildAbstractGenerator(string name)
        {
            return null;
        }

        public IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name)
        {
            return null;
        }

        public IBilingualDocumentGenerator BuildBilingualGenerator(string name)
        {
            return null;
        }

        public IFileGenerator BuildFileGenerator(string name)
        {
            return this.FileTypeManager.BuildFileGenerator(new RenPyWriter());
        }

        public IAbstractPreviewApplication BuildPreviewApplication(string name)
        {
            return null;
        }

        public IAbstractPreviewControl BuildPreviewControl(string name)
        {
            return null;
        }

        public IPreviewSetsFactory BuildPreviewSetsFactory(string name)
        {
            return null;
        }

        public IQuickTagsFactory BuildQuickTagsFactory(string name)
        {
            return null;
        }

        public IVerifierCollection BuildVerifierCollection(string name)
        {
            return null;
        }
    }
}
