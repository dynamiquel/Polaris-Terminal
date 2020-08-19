using System.Collections.Generic;
using System.Text;

namespace Polaris.Terminal
{
    public class QueryInfo
    {
        public string Command {get; set;}
        public List<string> Parameters {get; private set;}

        public QueryInfo(string command)
        {
            Command = command;
            Parameters = new List<string>();
        }
        
        public QueryInfo(string command, IEnumerable<string> parameters)
        {
            Command = command;
            Parameters = new List<string>(parameters);
        }

        public void AddParameter(string parameter)
        {
            if (Parameters == null)
                Parameters = new List<string>();
            
            Parameters.Add(parameter);
        }

        public void AddParameters(IEnumerable<string> parameters)
        {
            if (Parameters == null)
                Parameters = new List<string>();
            
            Parameters.AddRange(parameters);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Command);

            if (Parameters.Count > 0)
                sb.Append(" -");

            foreach (var parameter in Parameters)
                sb.Append($" {parameter}");

            return sb.ToString();
        }
    }
}