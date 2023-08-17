using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Text;

namespace Ramon.FileTypes.RenPy
{
    internal class RenPyTextExtractor : IMarkupDataVisitor
    {
        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            VisitChildren(commentMarker);
        }

        internal StringBuilder PlainText
        {
            get;
            set;
        }

        public string GetPlainText(ISegment segment)
        {
            PlainText = new StringBuilder("");
            VisitChildren(segment);
            return PlainText.ToString();
        }

        private void VisitChildren(IAbstractMarkupDataContainer container)
        {
            foreach (var item in container)
            {
                item.AcceptVisitor(this);
            }
        }

        public void VisitText(IText text)
        {
            PlainText.Append(text.Properties.Text);
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            VisitChildren(tagPair);
        }

        public void VisitSegment(ISegment segment)
        {
            VisitChildren(segment);
        }

        public void VisitLocationMarker(ILocationMarker location)
        {

        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {

        }

        public void VisitOtherMarker(IOtherMarker marker)
        {

        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            PlainText.Append(tag.TagProperties.TagContent);
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {

        }
    }
}
