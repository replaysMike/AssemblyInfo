using System.Linq;
using System.Text;

namespace AssemblyInfo
{
    public static class AssemblyDataFormatter
    {
        public static string GetAssemblyDataOutput(FormatterOutput formatterOutput, AssemblyData data)
        {
            var d = ",";
            var sb = new StringBuilder();
            var properties = typeof(AssemblyData).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).OrderBy(x => x.Name);
            switch (formatterOutput)
            {
                case FormatterOutput.Parsable:
                    sb.AppendLine(string.Join(d, properties.Select(x => x.Name)));
                    sb.AppendLine(string.Join(d, properties.Select(x => x.GetValue(data))));
                    break;
                case FormatterOutput.Pretty:
                    foreach (var property in properties)
                        sb.AppendLine($"{property.Name}: {property.GetValue(data)}");
                    break;
            }
            return sb.ToString();
        }

        public enum FormatterOutput
        {
            Pretty,
            Parsable
        }
    }
}
