using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using static Ramon.FileTypes.RenPy.RenPyFile.Unit;

namespace Ramon.FileTypes.RenPy
{
    internal class RenPyFile
    {
        private const string LANGUAGUE_PATTERN = @"^translate [\w]+ ([\w]+_[\d\w]+|strings):$";
        private const string SOURCE_PATTERN = @"# ([\w]*)[\s]?""(.+)""|old ""(.+)""";
        private const string TARGET_PATTERN = @"""(.*)""|new ""(.*)""";

        private Dictionary<string, Unit> _units = new Dictionary<string, Unit>();
        private string _filePath;

        public RenPyFile(string filePath)
        {
            _filePath = filePath;

            GetUnits();
        }

        private void GetUnits()
        {
            IEnumerable<string> lines = File.ReadLines(_filePath);

            Unit currentUnit = new Unit();

            bool inStrings = false;
            int lineNumber = 0;

            foreach (string line in lines)
            {
                Match match = Regex.Match(line, LANGUAGUE_PATTERN);
                if (match.Success)
                {
                    string id = match.Groups[1].Value;
                    if (id == "strings")
                    {
                        inStrings = true;
                    }
                    else
                    {
                        currentUnit = new Unit
                        {
                            id = id
                        };
                    }
                }

                match = Regex.Match(line, SOURCE_PATTERN);
                if (match.Success)
                {
                    if (inStrings)
                    {
                        string hash = CreateMD5(line);
                        currentUnit = new Unit
                        {
                            id = $"strings_{hash.Substring(0, 8)}"
                        };
                    }
                    currentUnit._source = new Segment
                    {
                        line = lineNumber,
                        content = line,
                        patternIndex = inStrings ? 3 : 2
                    };
                }
                else if (currentUnit.source != "")
                {
                    match = Regex.Match(line, TARGET_PATTERN);
                    if (match.Success)
                    {
                        currentUnit._target = new Segment
                        {
                            line = lineNumber,
                            content = line,
                            patternIndex = inStrings ? 2 : 1
                        };
                        _units.Add(currentUnit.id, currentUnit);
                    }
                }
                lineNumber++;
            }
        }

        public void Save(string filePath)
        {
            string[] lines = File.ReadAllLines(_filePath);

            foreach (Unit unit in _units.Values)
            {
                lines[unit._source.line] = unit._source.content;
                lines[unit._target.line] = unit._target.content;
            }

            File.WriteAllLines(filePath, lines);
        }

        private string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        public static bool IsRenPyFile(string filePath)
        {
            IEnumerable<string> lines = File.ReadLines(filePath);

            var translateLine = lines.Where(line =>
            {
                Match match = Regex.Match(line, LANGUAGUE_PATTERN);
                return match.Success;
            }).FirstOrDefault();

            return translateLine != null;
        }

        public Dictionary<string, Unit> Units
        {
            get
            {
                return _units;
            }
        }

        internal class Unit
        {
            public string id { get; set; } = "";
            public string tag
            {
                get { return _source?.GetTextFromPattern(SOURCE_PATTERN, 1); }
            }
            public string source
            {
                get { return _source?.GetTextFromPattern(SOURCE_PATTERN, _source.patternIndex); }
                set
                {
                    Regex regex = new Regex(@"""(.*)""");
                    _source.content = regex.Replace(_source.content, $"\"{value}\""); ;
                }
            }
            public string target
            {
                get { return _target?.GetTextFromPattern(TARGET_PATTERN, _target.patternIndex); }
                set
                {
                    Regex regex = new Regex(@"""(.*)""");
                    _target.content = regex.Replace(_target.content, $"\"{value}\"");
                }
            }
            public Segment _source { get; set; }
            public Segment _target { get; set; }

            internal class Segment
            {
                public int line { get; set; }
                public string content { get; set; }
                public int patternIndex { get; set; }
                public string GetTextFromPattern(string pattern, int index)
                {
                    Match match = Regex.Match(content, pattern);
                    return match.Success ? match.Groups[index].Value : "";
                }
            }
        }
    }
}
