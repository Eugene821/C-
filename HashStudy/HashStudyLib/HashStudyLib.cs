namespace HashStudyLib
{
    public class HashStudyLib
    {
        public static IEnumerable<string> ReadLogSection(string filePath, DateTime startTime, DateTime endTime)
        {
            foreach (var line in File.ReadLines(filePath))
            {
                //회사꺼는 23개 0, 23
                if (DateTime.TryParse(line.Substring(0, 19), out DateTime logTime))
                {
                    if(logTime >=startTime && logTime < endTime)
                    {
                        yield return line;
                    }
                }

            }
        }

    }
}
