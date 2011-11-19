using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Parsing;
using System.IO;

namespace N2.Management.Content.Export
{
    public class CsvParser
    {
        char separator;
        char quote = '"';

        public CsvParser(char separator)
        {
            this.separator = separator;
        }

        public IEnumerable<IList<string>> Parse(TextReader reader)
        {
            bool isEscaping = false;
            bool isQuoting = false;
            bool isEndingQuote = false;
            var cell = new StringBuilder();
            var row = new List<string>();

            for (int value = reader.Read(); value > 0; value = reader.Read())
            {
                char c = (char)value;

                if (isEndingQuote)
                {
                    if (c == '\r')
                        continue;

                    isEndingQuote = false;

                    if (c == '\n')
                    {
                        AppendCell(cell, row, trim: false);
                        continue;
                    }

                    if (c == quote)
                    {
                        isQuoting = true;
                        cell.Append(quote);
                        continue;
                    }
                    
                    if (c == separator)
                    {
                        AppendCell(cell, row, trim: false);
                        continue;
                    }
                }

                if(isQuoting)
                {
                    if(isEscaping)
                    {
                        isEscaping = false;
                        if(c == 'n')
                        {
                            cell.Append(Environment.NewLine);
                        }
                        else if(c == quote)
                        {
                            cell.Append("\"");
                        }
                        else
                        {
                            cell.Append('\\');
                            cell.Append(c);
                        }
                        continue;
                    }

                    if(c == quote)
                    {
                        isQuoting = false;
                        isEndingQuote = true;
                        continue;
                    }

                    if(c == '\\')
                    {
                        isEscaping = true;
                        continue;
                    }

                    cell.Append(c);
                    continue;
                }

                if (c == separator)
                {
                    AppendCell(cell, row, trim: true);
                    continue;
                }

                if (c == '\r')
                    continue;

                if (c == '\n')
                {
                    AppendCell(cell, row, trim: true);
                    yield return row;
                    row = new List<string>();
                    continue;
                }

                if (c == quote && cell.Length == 0)
                {
                    isQuoting = true;
                    continue;
                }

                cell.Append(c);
            }

            if (cell.Length > 0)
                AppendCell(cell, row, trim: !isEndingQuote);

            if (row.Count > 0)
                yield return row;
        }

        private static void AppendCell(StringBuilder cell, List<string> row, bool trim)
        {
            if (trim)
                row.Add(cell.ToString().Trim());
            else
                row.Add(cell.ToString());
            cell.Length = 0;
        }
    }
}
