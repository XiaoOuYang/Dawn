using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Application.Extensions
{
    /// <summary>
    /// 记录
    /// </summary>
    internal class RecordWriter : TextWriter
    {
        public RecordWriter(TextWriter innerWriter, StringBuilder recorder)
        {
            this.m_innerWriter = innerWriter;
            this.m_recorder = recorder;
        }

        private TextWriter m_innerWriter;
        private StringBuilder m_recorder;

        public override Encoding Encoding
        {
            get { return this.m_innerWriter.Encoding; }
        }

        public override void Write(char value)
        {
            this.m_innerWriter.Write(value);
            m_recorder.Append(value);
        }

        public override void Write(string value)
        {
            if (value != null)
            {
                this.m_innerWriter.Write(value);
                m_recorder.Append(value);

            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            this.m_innerWriter.Write(buffer, index, count);

            m_recorder.Append(buffer, index, count);
        }
      
    }
}
