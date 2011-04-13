using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace N2.Web.Parsing
{
	public class Parser
	{
		public Parser(params AnalyzerBase[] analyzers)
		{
			Tokenizer = new Tokenizer();
			Analyzers = analyzers;
		}

		public Tokenizer Tokenizer { get; set; }
		public IEnumerable<AnalyzerBase> Analyzers { get; set; }

		public IEnumerable<Component> Parse(string text)
		{
			return Parse(Tokenizer.Tokenize(new StringReader(text)));
		}

		public IEnumerable<Component> Parse(IEnumerable<Token> inputTokens)
		{
			var tokens = inputTokens.ToList();
			int indexAtEndOfLast = 0;
			for (int i = 0; i < tokens.Count; i++)
			{
				foreach (var analyzer in Analyzers)
				{
					Component b = analyzer.GetComponent(tokens, i);
					if (b == null)
						continue;

					if (indexAtEndOfLast < i)
						yield return CreateTextBlock(tokens, indexAtEndOfLast, i);

					yield return b;

					i += b.Tokens.Count;
					indexAtEndOfLast = i;
					--i;

					break;
				}
			}

			if (indexAtEndOfLast < tokens.Count)
				yield return CreateTextBlock(tokens, indexAtEndOfLast, tokens.Count);
		}

		private static Component CreateTextBlock(IList<Token> list, int indexAtEndOfLast, int currentIndex)
		{
			var blockTokens = list.Skip(indexAtEndOfLast).Take(currentIndex - indexAtEndOfLast).ToList();
			var b = new Component { Command = TextCommand, Tokens = blockTokens, Data = blockTokens.Select(t => t.Fragment).StringJoin() };
			return b;
		}

		public const string TextCommand = "Text";
	}
}
