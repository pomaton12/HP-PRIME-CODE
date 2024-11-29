using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HP_PRIME_CODE.ClasesAdicionales
{
    public class BlockAnalyzer
    {
        private readonly List<string> _keywords;
        private readonly Dictionary<string, string> _matchingKeywords;

        public BlockAnalyzer()
        {
            _keywords = new List<string> { "BEGIN", "IF", "IFERR", "FOR", "WHILE", "REPEAT", "CASE" };
            _matchingKeywords = new Dictionary<string, string>
            {
                { "BEGIN", "END" },
                { "IF", "END" },
                { "IFERR", "END" },
                { "FOR", "END" },
                { "WHILE", "END" },
                { "REPEAT", "UNTIL" },
                { "CASE", "END" }
            };
        }

        public (int depth, string currentBlock)? GetBlockState(List<string> lines, int currentLineIndex)
        {
            int depth = 0;
            string currentBlock = null;
            Stack<string> openBlocks = new Stack<string>();

            for (int i = 0; i <= currentLineIndex; i++)
            {
                var line = lines[i].Trim();

                // Detectar palabras clave de apertura
                foreach (var keyword in _keywords)
                {
                    if (Regex.IsMatch(line, $@"^\s*{keyword}\b", RegexOptions.IgnoreCase))
                    {
                        // Verificar si la línea contiene tanto IF como END en la misma línea
                        if (Regex.IsMatch(line, $@"^\s*IF\b.*END\b", RegexOptions.IgnoreCase))
                        {
                            // Omitir el procesamiento de este bloque de una sola línea
                            continue;
                        }

                        openBlocks.Push(keyword);
                        depth++;
                        currentBlock = keyword;
                        break;
                    }
                }

                // Detectar palabras clave de cierre
                if (openBlocks.Count > 0 && _matchingKeywords.TryGetValue(openBlocks.Peek(), out var closingKeyword))
                {
                    // Verificar si la línea contiene tanto CASE como END en la misma línea
                    if (Regex.IsMatch(line, $@"^\s*CASE\b.*END\b", RegexOptions.IgnoreCase))
                    {
                        // Omitir el procesamiento de este bloque de una sola línea
                        continue;
                    }

                    if (Regex.IsMatch(line, $@"^\s*{closingKeyword}\b", RegexOptions.IgnoreCase))
                    {
                        openBlocks.Pop();
                        depth--;
                        currentBlock = openBlocks.Count > 0 ? openBlocks.Peek() : null;
                    }
                }
            }

            return depth > 0 ? (depth, currentBlock) : null;
        }
    }
}