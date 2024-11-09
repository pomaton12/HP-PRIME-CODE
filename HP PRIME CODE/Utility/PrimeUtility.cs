using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public static class Refactoring
{

    private static CodeBlock[] _codeBlocks;

    /// <summary>
    /// Formats the code indentation
    /// </summary>
    /// <param name="lines">Code lines to be changed</param>
    /// <param name="indentation">Indentation to add</param>
    /// <returns>Null if everything was closed, or the last opened block that prevented the code indentation</returns>
    public static CodeBlock FormatLines(ref List<string> lines, String indentation)
    {
        // Find code blocks
        if (_codeBlocks == null)
            _codeBlocks = new[]
            {
                    new CodeBlock("BEGIN"), new CodeBlock("CASE"), new CodeBlock("IFERR"), new CodeBlock("IF"),
                    new CodeBlock("FOR"), new CodeBlock("WHILE"), new CodeBlock("REPEAT", "UNTIL")
                };

        var opened = new Stack<CodeBlock>();
        var openedBlocks = 0;

        for (var l = 0; l < lines.Count; l++)
        {
            var line = lines[l].Trim(new[] { '\t', ' ', '\n', '\r' });

            var lineStart = 0;
            int tmpLine;
            bool repeat;

            do
            {
                repeat = false;

                if (line.Length == 0)
                    break;

                foreach (var block in _codeBlocks)
                {
                    tmpLine = block.MatchesOpen(line.Substring(Math.Min(line.Length - 1, lineStart)));

                    if (tmpLine > 0)
                    {
                        lineStart = tmpLine;
                        block.Line = l;
                        opened.Push(block);
                        repeat = true;
                    }
                }
            } while (repeat);

            do
            {
                repeat = false;

                if (line.Length == 0)
                    break;

                if (opened.Count > 0)
                {
                    tmpLine = opened.Peek().MatchesClose(line.Substring(Math.Min(line.Length - 1, lineStart)));

                    if (tmpLine > 0)
                    {
                        lineStart = tmpLine;
                        opened.Pop();
                        openedBlocks = opened.Count;
                        repeat = true;
                    }
                }
            } while (repeat);

            var tmp = "";
            for (var i = 0; i < openedBlocks; i++)
                tmp += indentation;

            openedBlocks = opened.Count;
            lines[l] = tmp + line;
        }

        return opened.Count > 0 ? opened.Pop() : null;
    }



}


public class CodeBlock
{
    private readonly Regex _blockOpen, _blockClose;
    private static Regex _regexComments;

    /// <summary>
    /// Initializes a block of code
    /// </summary>
    /// <param name="blockOpen">Text or regular expression</param>
    /// <param name="blockClose">Text or regular expression</param>
    public CodeBlock(string blockOpen, string blockClose = @"\sEND[\s;]")
    {
        const string s = @"\s"; // Space or line ending
        const string o = @"[\(\s]"; // Space, line ending or open parenthesis
        _blockOpen = new Regex(blockOpen.Contains('\\') ? blockOpen : (s + blockOpen + o), RegexOptions.IgnoreCase);
        _blockClose = new Regex(blockClose.Contains('\\') ? blockClose : (s + blockClose + o), RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Current line
    /// </summary>
    public int Line { get; set; }

    internal int MatchesOpen(string p)
    {
        return Match(p, _blockOpen);
    }

    internal int MatchesClose(string p)
    {
        return Match(p, _blockClose);
    }

    private static int Match(string p, Regex regexMatch)
    {
        if (_regexComments == null)
            _regexComments = new Regex(@"""[^""""\\]*(?:\\.[^""""\\]*)*""");

        // Remove comments and strings
        var m = regexMatch.Match(" " + (_regexComments.Replace(p, match => new String(' ',
            match.Length)) + "//").Split(new[] { "//" }, 2, StringSplitOptions.None)[0] + " ");

        if (!m.Success)
            return 0;

        return m.Index + m.Value.Length - 1;
    }
}

