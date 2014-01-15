using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Parsing;
using System.IO;
using N2.Engine;

namespace N2.Management.Content.Export
{
    [Service]
    public class CsvParser
    {
        const char quote = '"';

        public CsvParser()
        {
        }
        
        public virtual IEnumerable<CsvRow> Parse(char separator, TextReader reader)
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
                    yield return new CsvRow(row, separator);
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
                yield return new CsvRow(row, separator);
        }

        private static void AppendCell(StringBuilder cell, List<string> row, bool trim)
        {
            if (trim)
                row.Add(cell.ToString().Trim());
            else
                row.Add(cell.ToString());
            cell.Length = 0;
        }

        public virtual char GuessBestSeparator(Func<TextReader> readerFactory, params char[] possibleSeparators)
        {
            var scores = possibleSeparators.ToDictionary(c => c, c => 0);
            for (int i = 0; i < possibleSeparators.Length; i++)
            {
                var c = possibleSeparators[i];
                using (var reader = readerFactory())
                {
                    var passes = 5;
                    int previousCount = 0;
                    foreach(var row in Parse(c, reader))
                    {
                        if (--passes < 0)
                            break;

                        if (previousCount == 0)
                        {
                            if (row.Columns.Count > 1)
                                scores[c] = 1;
                        }
                        else
                        {
                            if (row.Columns.Count == previousCount)
                                scores[c] = scores[c] + 1;
                            else if (row.Columns.Count > previousCount)
                                scores[c] = scores[c] - 1;
                        }

                        previousCount = row.Columns.Count;
                    }
                }
            }
            return scores.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).First();
        }
    }
}
