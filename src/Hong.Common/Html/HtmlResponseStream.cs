using System.IO;
using System.Text;

namespace Hong.Common.Html
{
    public class HtmlResponseStream : Stream
    {
        Stream _body = null;
        StringBuilder _html = new StringBuilder();
        UrlInfo _urlInfo = null;

        public HtmlResponseStream(Stream body, UrlInfo urlInfo)
        {
            _body = body;
            _urlInfo = urlInfo;
        }

        public override bool CanRead
        {
            get
            {
                return _body.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _body.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _body.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _body.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _body.Position;
            }
            set
            {
                _body.Position = value;
            }
        }

        public override void Flush()
        {
            _body.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _body.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _body.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _body.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _body.Write(buffer, offset, count);

            _html.Append(Encoding.UTF8.GetString(buffer, offset, count));
        }

        public void Save()
        {
            if (_urlInfo != null)
            {
                _urlInfo.Content = _html.ToString();
            }
        }
    }
}
