using System;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using static Ramon.FileTypes.RenPy.RenPyFile;

namespace Ramon.FileTypes.RenPy
{
    internal class RenPyParser : AbstractBilingualFileTypeComponent, IBilingualParser, INativeContentCycleAware
    {
        IPersistentFileConversionProperties _fileConversionProperties;
        RenPyFile _renPyFile = null;

        public IDocumentProperties DocumentProperties
        {
            get;
            set;
        }

        public IBilingualContentHandler Output
        {
            get;
            set;
        }

        public event EventHandler<ProgressEventArgs> Progress;

        public void SetFileProperties(IFileProperties properties)
        {
            _fileConversionProperties = properties.FileConversionProperties;

            Output.Initialize(DocumentProperties);

            IFileProperties fileInfo = ItemFactory.CreateFileProperties();
            fileInfo.FileConversionProperties = _fileConversionProperties;
            Output.SetFileProperties(fileInfo);
        }

        public void StartOfInput()
        {
            OnProgress(0);
            _renPyFile = new RenPyFile(_fileConversionProperties.OriginalFilePath);
        }

        public void EndOfInput()
        {
            Output.FileComplete();
            Output.Complete();

            OnProgress(100);
            _renPyFile = null;
        }

        public bool ParseNext()
        {
            int totalSegmentsCount = _renPyFile.Units.Count;
            int currentSegmentsCount = 0;
            foreach (Unit item in _renPyFile.Units.Values)
            {
                Output.ProcessParagraphUnit(CreateParagraphUnit(item));

                currentSegmentsCount++;
                OnProgress(Convert.ToByte(Math.Round(100 * ((decimal)currentSegmentsCount / totalSegmentsCount), 0)));
            }
            return false;
        }

        private IParagraphUnit CreateParagraphUnit(Unit item)
        {
            IParagraphUnit paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);

            ISegmentPairProperties segmentPairProperties = ItemFactory.CreateSegmentPairProperties();
            ISegment srcSegment = CreateSegment(item.tag, item.source, segmentPairProperties);
            paragraphUnit.Source.Add(srcSegment);

            ISegment trgSegment = CreateSegment(item.tag, item.target, segmentPairProperties);
            paragraphUnit.Target.Add(trgSegment);

            paragraphUnit.Properties.Contexts = CreateContext(item.id);

            return paragraphUnit;
        }

        private ISegment CreateSegment(string tag, string segText, ISegmentPairProperties pair)
        {
            ISegment segment = ItemFactory.CreateSegment(pair);

            if (tag != null && tag != "")
            {
                segment.Add(CreateTagPair(segText, tag));
            }
            else
            {
                segment.Add(CreateText(segText));
            }

            return segment;
        }

        private IText CreateText(string segText)
        {
            ITextProperties textProperties = PropertiesFactory.CreateTextProperties(segText);
            IText textContent = ItemFactory.CreateText(textProperties);

            return textContent;
        }

        private IContextProperties CreateContext(string unitId)
        {
            IContextProperties contextProperties = PropertiesFactory.CreateContextProperties();
            IContextInfo contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
            contextInfo.Purpose = ContextPurpose.Information;

            IContextInfo contextId = PropertiesFactory.CreateContextInfo("UnitId");
            contextId.SetMetaData("UnitID", unitId);

            contextProperties.Contexts.Add(contextInfo);
            contextProperties.Contexts.Add(contextId);

            return contextProperties;
        }

        private ITagPair CreateTagPair(string text, string tag)
        {
            IStartTagProperties startTag = PropertiesFactory.CreateStartTagProperties(tag);

            startTag.DisplayText = tag;
            startTag.CanHide = true;
            IEndTagProperties endTag = PropertiesFactory.CreateEndTagProperties(tag);
            endTag.DisplayText = tag;
            endTag.CanHide = true;

            ITagPair tagPair = ItemFactory.CreateTagPair(startTag, endTag);

            var textElement = ItemFactory.CreateText(PropertiesFactory.CreateTextProperties(text));
            tagPair.Add(textElement);

            return tagPair;
        }

        public void Dispose()
        {
            _renPyFile = null;
        }

        protected virtual void OnProgress(byte percent)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressEventArgs(percent));
            }
        }
    }
}
