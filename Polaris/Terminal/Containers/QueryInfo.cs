//  This file is part of Polaris-Terminal - A developer console for Unity.
//  https://github.com/dynamiquel/Polaris-Options
//  Copyright (c) 2020 dynamiquel

//  MIT License
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:

//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.

//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.

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

        /// <summary>
        /// Creates a QueryInfo from a string which includes a command ID and parameters.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static QueryInfo FromString(string query)
        {
            var queryInfo = new QueryInfo(string.Empty);
            
            // Removes the excess spacing.
            query = query.Trim();

            var inputParams = new List<string>();
            var charList = query.ToCharArray();
            var readingParam = false;
            var param = string.Empty;

            for (var i = 0; i < query.Length; i++)
            {
                // Start the grouping
                if (charList[i] == '(' && !readingParam)
                {
                    readingParam = true;
                }
                else if (charList[i] != ')' && readingParam && charList[i] != '(')
                {
                    param += charList[i].ToString();
                }
                else if (charList[i] != ' ' && !readingParam)
                {
                    param += charList[i].ToString();
                }
                else if (charList[i] == ' ' && !readingParam)
                {
                    inputParams.Add(param);
                    param = "";
                }
                // End the grouping
                else if (charList[i] == ')' && readingParam)
                {
                    readingParam = false;
                }
                
                if (i == query.Length - 1 && !string.IsNullOrEmpty(param))
                {
                    inputParams.Add(param);
                    param = "";
                }
            }
            
            for (var i = 0; i < inputParams.Count; i++)
            {
                // Removes the excess spacing.
                inputParams[i] = inputParams[i].Trim();
            }

            if (inputParams.Count > 0)
            {
                queryInfo.Command = inputParams[0];
                inputParams.RemoveAt(0);

                if (inputParams.Count > 0)
                    queryInfo.AddParameters(inputParams);
            }

            return queryInfo;
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