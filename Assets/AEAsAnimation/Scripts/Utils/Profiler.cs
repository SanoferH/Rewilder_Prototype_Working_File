using System.Collections.Generic;

namespace AEAsAnimation
{
    public class Profiler
    {
        private List<string> _tags = new List<string>();
        private List<long> _millseconds = new List<long>();

        private long _startMillSeconds = 0;

        public Profiler Start()
        {
            _startMillSeconds = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            _tags.Clear();
            _millseconds.Clear();
            return this;
        }

        public void Check(string tag = "")
        {
            _tags.Add(tag);
            _millseconds.Add(System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startMillSeconds);
        }

        public string Show(int digit = 0)
        {
            var result = "";
            for (var i = 0; i < _millseconds.Count; i++)
            {
                var tag = _tags[i];
                var millsec = digit <= 0 ? _millseconds[i] : _millseconds[i] % (System.Math.Pow(10, digit));
                if (tag != "") result += tag + ":";
                result += millsec.ToString() + " ms\n";
            }

            return result;
        }

        public override string ToString()
        {
            return Show();
        }
    }
}