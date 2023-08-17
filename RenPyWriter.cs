using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Ramon.FileTypes.RenPy
{
    internal class RenPyWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
    {
        private IPersistentFileConversionProperties _originalFileProperties;
        private INativeOutputFileProperties _nativeFileProperties;
        private RenPyFile _targetFile = null;
        private RenPyTextExtractor _textExtractor = null;

        public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
        {
            _originalFileProperties = fileProperties;
        }

        public void SetOutputProperties(INativeOutputFileProperties properties)
        {
            _nativeFileProperties = properties;
        }

        public void SetFileProperties(IFileProperties fileInfo)
        {
            _targetFile = new RenPyFile(_originalFileProperties.OriginalFilePath);
        }

        public void Initialize(IDocumentProperties documentInfo)
        {
            _textExtractor = new RenPyTextExtractor();
        }

        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            string unitId = paragraphUnit.Properties.Contexts.Contexts[1].GetMetaData("UnitID");

            RenPyFile.Unit unit;
            if (_targetFile.Units.TryGetValue(unitId, out unit))
            {
                foreach (ISegmentPair segmentPair in paragraphUnit.SegmentPairs)
                {
                    unit.source = _textExtractor.GetPlainText(segmentPair.Source);
                    unit.target = _textExtractor.GetPlainText(segmentPair.Target);
                }
            }

        }
        public void FileComplete()
        {
            _targetFile.Save(_nativeFileProperties.OutputFilePath);
            _targetFile = null;
        }

        public void Complete()
        {

        }

        public void Dispose()
        {
        }
    }
}
