//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from CustomLang.g4 by ANTLR 4.13.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.2")]
[System.CLSCompliant(false)]
public partial class CustomLangLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		T__17=18, T__18=19, T__19=20, T__20=21, T__21=22, T__22=23, T__23=24, 
		T__24=25, T__25=26, T__26=27, T__27=28, T__28=29, T__29=30, T__30=31, 
		T__31=32, T__32=33, T__33=34, T__34=35, T__35=36, T__36=37, T__37=38, 
		T__38=39, T__39=40, T__40=41, EXPORT=42, BEGIN=43, LOCAL=44, CASE=45, 
		DEFAULT=46, IF=47, THEN=48, ELSE=49, IFERR=50, FOR=51, FROM=52, WHILE=53, 
		TO=54, DO=55, DOWNTO=56, STEP=57, PRINT=58, REPEAT=59, UNTIL=60, END=61, 
		BREAK=62, CONTINUE=63, VIEW=64, AND=65, OR=66, XOR=67, NOT=68, ID=69, 
		HEX_NUMBER=70, NUMBER=71, ASSIGN=72, EQUALS=73, SEMI=74, STRING=75, WS=76, 
		COMMENT=77;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"T__9", "T__10", "T__11", "T__12", "T__13", "T__14", "T__15", "T__16", 
		"T__17", "T__18", "T__19", "T__20", "T__21", "T__22", "T__23", "T__24", 
		"T__25", "T__26", "T__27", "T__28", "T__29", "T__30", "T__31", "T__32", 
		"T__33", "T__34", "T__35", "T__36", "T__37", "T__38", "T__39", "T__40", 
		"EXPORT", "BEGIN", "LOCAL", "CASE", "DEFAULT", "IF", "THEN", "ELSE", "IFERR", 
		"FOR", "FROM", "WHILE", "TO", "DO", "DOWNTO", "STEP", "PRINT", "REPEAT", 
		"UNTIL", "END", "BREAK", "CONTINUE", "VIEW", "AND", "OR", "XOR", "NOT", 
		"ID", "HEX_NUMBER", "NUMBER", "ASSIGN", "EQUALS", "SEMI", "STRING", "WS", 
		"COMMENT"
	};


	public CustomLangLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public CustomLangLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "','", "'('", "')'", "'#pragma'", "'mode'", "'separator'", "'integer'", 
		"'.'", "'h32'", "'h64'", "'KILL'", "'kill'", "'RETURN'", "'\\u25B6'", 
		"'^'", "'*'", "'/'", "'+'", "'-'", "'['", "']'", "'{'", "'}'", "'\\u222B'", 
		"'\\u03A3'", "'\\u2202'", "'\\u221A'", "'\\u03A3LIST'", "'\\u03A0LIST'", 
		"'\\u0394LIST'", "'!'", "'B\\u2192R'", "'>'", "'<'", "'\\u2264'", "'\\u2265'", 
		"'=='", "'>='", "'<='", "'\\u2260'", "'<>'", null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, "':='", "'='", "';'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, "EXPORT", "BEGIN", "LOCAL", "CASE", 
		"DEFAULT", "IF", "THEN", "ELSE", "IFERR", "FOR", "FROM", "WHILE", "TO", 
		"DO", "DOWNTO", "STEP", "PRINT", "REPEAT", "UNTIL", "END", "BREAK", "CONTINUE", 
		"VIEW", "AND", "OR", "XOR", "NOT", "ID", "HEX_NUMBER", "NUMBER", "ASSIGN", 
		"EQUALS", "SEMI", "STRING", "WS", "COMMENT"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "CustomLang.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static CustomLangLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,77,637,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,2,24,7,24,2,25,7,25,2,26,7,26,2,27,7,27,2,28,
		7,28,2,29,7,29,2,30,7,30,2,31,7,31,2,32,7,32,2,33,7,33,2,34,7,34,2,35,
		7,35,2,36,7,36,2,37,7,37,2,38,7,38,2,39,7,39,2,40,7,40,2,41,7,41,2,42,
		7,42,2,43,7,43,2,44,7,44,2,45,7,45,2,46,7,46,2,47,7,47,2,48,7,48,2,49,
		7,49,2,50,7,50,2,51,7,51,2,52,7,52,2,53,7,53,2,54,7,54,2,55,7,55,2,56,
		7,56,2,57,7,57,2,58,7,58,2,59,7,59,2,60,7,60,2,61,7,61,2,62,7,62,2,63,
		7,63,2,64,7,64,2,65,7,65,2,66,7,66,2,67,7,67,2,68,7,68,2,69,7,69,2,70,
		7,70,2,71,7,71,2,72,7,72,2,73,7,73,2,74,7,74,2,75,7,75,2,76,7,76,1,0,1,
		0,1,1,1,1,1,2,1,2,1,3,1,3,1,3,1,3,1,3,1,3,1,3,1,3,1,4,1,4,1,4,1,4,1,4,
		1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,5,1,6,1,6,1,6,1,6,1,6,1,6,1,6,1,
		6,1,7,1,7,1,8,1,8,1,8,1,8,1,9,1,9,1,9,1,9,1,10,1,10,1,10,1,10,1,10,1,11,
		1,11,1,11,1,11,1,11,1,12,1,12,1,12,1,12,1,12,1,12,1,12,1,13,1,13,1,14,
		1,14,1,15,1,15,1,16,1,16,1,17,1,17,1,18,1,18,1,19,1,19,1,20,1,20,1,21,
		1,21,1,22,1,22,1,23,1,23,1,24,1,24,1,25,1,25,1,26,1,26,1,27,1,27,1,27,
		1,27,1,27,1,27,1,28,1,28,1,28,1,28,1,28,1,28,1,29,1,29,1,29,1,29,1,29,
		1,29,1,30,1,30,1,31,1,31,1,31,1,31,1,32,1,32,1,33,1,33,1,34,1,34,1,35,
		1,35,1,36,1,36,1,36,1,37,1,37,1,37,1,38,1,38,1,38,1,39,1,39,1,40,1,40,
		1,40,1,41,1,41,1,41,1,41,1,41,1,41,1,41,1,41,1,41,1,41,1,41,1,41,3,41,
		306,8,41,1,42,1,42,1,42,1,42,1,42,1,42,1,42,1,42,1,42,1,42,3,42,318,8,
		42,1,43,1,43,1,43,1,43,1,43,1,43,1,43,1,43,1,43,1,43,3,43,330,8,43,1,44,
		1,44,1,44,1,44,1,44,1,44,1,44,1,44,3,44,340,8,44,1,45,1,45,1,45,1,45,1,
		45,1,45,1,45,1,45,1,45,1,45,1,45,1,45,1,45,1,45,3,45,356,8,45,1,46,1,46,
		1,46,1,46,3,46,362,8,46,1,47,1,47,1,47,1,47,1,47,1,47,1,47,1,47,3,47,372,
		8,47,1,48,1,48,1,48,1,48,1,48,1,48,1,48,1,48,3,48,382,8,48,1,49,1,49,1,
		49,1,49,1,49,1,49,1,49,1,49,1,49,1,49,3,49,394,8,49,1,50,1,50,1,50,1,50,
		1,50,1,50,3,50,402,8,50,1,51,1,51,1,51,1,51,1,51,1,51,1,51,1,51,3,51,412,
		8,51,1,52,1,52,1,52,1,52,1,52,1,52,1,52,1,52,1,52,1,52,3,52,424,8,52,1,
		53,1,53,1,53,1,53,3,53,430,8,53,1,54,1,54,1,54,1,54,3,54,436,8,54,1,55,
		1,55,1,55,1,55,1,55,1,55,1,55,1,55,1,55,1,55,1,55,1,55,3,55,450,8,55,1,
		56,1,56,1,56,1,56,1,56,1,56,1,56,1,56,3,56,460,8,56,1,57,1,57,1,57,1,57,
		1,57,1,57,1,57,1,57,1,57,1,57,3,57,472,8,57,1,58,1,58,1,58,1,58,1,58,1,
		58,1,58,1,58,1,58,1,58,1,58,1,58,3,58,486,8,58,1,59,1,59,1,59,1,59,1,59,
		1,59,1,59,1,59,1,59,1,59,3,59,498,8,59,1,60,1,60,1,60,1,60,1,60,1,60,3,
		60,506,8,60,1,61,1,61,1,61,1,61,1,61,1,61,1,61,1,61,1,61,1,61,3,61,518,
		8,61,1,62,1,62,1,62,1,62,1,62,1,62,1,62,1,62,1,62,1,62,1,62,1,62,1,62,
		1,62,1,62,1,62,3,62,536,8,62,1,63,1,63,1,63,1,63,1,63,1,63,1,63,1,63,3,
		63,546,8,63,1,64,1,64,1,64,1,64,1,64,1,64,3,64,554,8,64,1,65,1,65,1,65,
		1,65,3,65,560,8,65,1,66,1,66,1,66,1,66,1,66,1,66,3,66,568,8,66,1,67,1,
		67,1,67,1,67,1,67,1,67,3,67,576,8,67,1,68,1,68,5,68,580,8,68,10,68,12,
		68,583,9,68,1,69,1,69,4,69,587,8,69,11,69,12,69,588,1,70,4,70,592,8,70,
		11,70,12,70,593,1,70,1,70,4,70,598,8,70,11,70,12,70,599,3,70,602,8,70,
		1,71,1,71,1,71,1,72,1,72,1,73,1,73,1,74,1,74,5,74,613,8,74,10,74,12,74,
		616,9,74,1,74,1,74,1,75,4,75,621,8,75,11,75,12,75,622,1,75,1,75,1,76,1,
		76,1,76,1,76,5,76,631,8,76,10,76,12,76,634,9,76,1,76,1,76,0,0,77,1,1,3,
		2,5,3,7,4,9,5,11,6,13,7,15,8,17,9,19,10,21,11,23,12,25,13,27,14,29,15,
		31,16,33,17,35,18,37,19,39,20,41,21,43,22,45,23,47,24,49,25,51,26,53,27,
		55,28,57,29,59,30,61,31,63,32,65,33,67,34,69,35,71,36,73,37,75,38,77,39,
		79,40,81,41,83,42,85,43,87,44,89,45,91,46,93,47,95,48,97,49,99,50,101,
		51,103,52,105,53,107,54,109,55,111,56,113,57,115,58,117,59,119,60,121,
		61,123,62,125,63,127,64,129,65,131,66,133,67,135,68,137,69,139,70,141,
		71,143,72,145,73,147,74,149,75,151,76,153,77,1,0,7,6,0,65,90,95,95,97,
		122,209,209,241,241,945,969,7,0,48,57,65,90,95,95,97,122,209,209,241,241,
		945,969,3,0,48,57,65,70,97,102,1,0,48,57,3,0,10,10,13,13,34,34,3,0,9,10,
		13,13,32,32,2,0,10,10,13,13,671,0,1,1,0,0,0,0,3,1,0,0,0,0,5,1,0,0,0,0,
		7,1,0,0,0,0,9,1,0,0,0,0,11,1,0,0,0,0,13,1,0,0,0,0,15,1,0,0,0,0,17,1,0,
		0,0,0,19,1,0,0,0,0,21,1,0,0,0,0,23,1,0,0,0,0,25,1,0,0,0,0,27,1,0,0,0,0,
		29,1,0,0,0,0,31,1,0,0,0,0,33,1,0,0,0,0,35,1,0,0,0,0,37,1,0,0,0,0,39,1,
		0,0,0,0,41,1,0,0,0,0,43,1,0,0,0,0,45,1,0,0,0,0,47,1,0,0,0,0,49,1,0,0,0,
		0,51,1,0,0,0,0,53,1,0,0,0,0,55,1,0,0,0,0,57,1,0,0,0,0,59,1,0,0,0,0,61,
		1,0,0,0,0,63,1,0,0,0,0,65,1,0,0,0,0,67,1,0,0,0,0,69,1,0,0,0,0,71,1,0,0,
		0,0,73,1,0,0,0,0,75,1,0,0,0,0,77,1,0,0,0,0,79,1,0,0,0,0,81,1,0,0,0,0,83,
		1,0,0,0,0,85,1,0,0,0,0,87,1,0,0,0,0,89,1,0,0,0,0,91,1,0,0,0,0,93,1,0,0,
		0,0,95,1,0,0,0,0,97,1,0,0,0,0,99,1,0,0,0,0,101,1,0,0,0,0,103,1,0,0,0,0,
		105,1,0,0,0,0,107,1,0,0,0,0,109,1,0,0,0,0,111,1,0,0,0,0,113,1,0,0,0,0,
		115,1,0,0,0,0,117,1,0,0,0,0,119,1,0,0,0,0,121,1,0,0,0,0,123,1,0,0,0,0,
		125,1,0,0,0,0,127,1,0,0,0,0,129,1,0,0,0,0,131,1,0,0,0,0,133,1,0,0,0,0,
		135,1,0,0,0,0,137,1,0,0,0,0,139,1,0,0,0,0,141,1,0,0,0,0,143,1,0,0,0,0,
		145,1,0,0,0,0,147,1,0,0,0,0,149,1,0,0,0,0,151,1,0,0,0,0,153,1,0,0,0,1,
		155,1,0,0,0,3,157,1,0,0,0,5,159,1,0,0,0,7,161,1,0,0,0,9,169,1,0,0,0,11,
		174,1,0,0,0,13,184,1,0,0,0,15,192,1,0,0,0,17,194,1,0,0,0,19,198,1,0,0,
		0,21,202,1,0,0,0,23,207,1,0,0,0,25,212,1,0,0,0,27,219,1,0,0,0,29,221,1,
		0,0,0,31,223,1,0,0,0,33,225,1,0,0,0,35,227,1,0,0,0,37,229,1,0,0,0,39,231,
		1,0,0,0,41,233,1,0,0,0,43,235,1,0,0,0,45,237,1,0,0,0,47,239,1,0,0,0,49,
		241,1,0,0,0,51,243,1,0,0,0,53,245,1,0,0,0,55,247,1,0,0,0,57,253,1,0,0,
		0,59,259,1,0,0,0,61,265,1,0,0,0,63,267,1,0,0,0,65,271,1,0,0,0,67,273,1,
		0,0,0,69,275,1,0,0,0,71,277,1,0,0,0,73,279,1,0,0,0,75,282,1,0,0,0,77,285,
		1,0,0,0,79,288,1,0,0,0,81,290,1,0,0,0,83,305,1,0,0,0,85,317,1,0,0,0,87,
		329,1,0,0,0,89,339,1,0,0,0,91,355,1,0,0,0,93,361,1,0,0,0,95,371,1,0,0,
		0,97,381,1,0,0,0,99,393,1,0,0,0,101,401,1,0,0,0,103,411,1,0,0,0,105,423,
		1,0,0,0,107,429,1,0,0,0,109,435,1,0,0,0,111,449,1,0,0,0,113,459,1,0,0,
		0,115,471,1,0,0,0,117,485,1,0,0,0,119,497,1,0,0,0,121,505,1,0,0,0,123,
		517,1,0,0,0,125,535,1,0,0,0,127,545,1,0,0,0,129,553,1,0,0,0,131,559,1,
		0,0,0,133,567,1,0,0,0,135,575,1,0,0,0,137,577,1,0,0,0,139,584,1,0,0,0,
		141,591,1,0,0,0,143,603,1,0,0,0,145,606,1,0,0,0,147,608,1,0,0,0,149,610,
		1,0,0,0,151,620,1,0,0,0,153,626,1,0,0,0,155,156,5,44,0,0,156,2,1,0,0,0,
		157,158,5,40,0,0,158,4,1,0,0,0,159,160,5,41,0,0,160,6,1,0,0,0,161,162,
		5,35,0,0,162,163,5,112,0,0,163,164,5,114,0,0,164,165,5,97,0,0,165,166,
		5,103,0,0,166,167,5,109,0,0,167,168,5,97,0,0,168,8,1,0,0,0,169,170,5,109,
		0,0,170,171,5,111,0,0,171,172,5,100,0,0,172,173,5,101,0,0,173,10,1,0,0,
		0,174,175,5,115,0,0,175,176,5,101,0,0,176,177,5,112,0,0,177,178,5,97,0,
		0,178,179,5,114,0,0,179,180,5,97,0,0,180,181,5,116,0,0,181,182,5,111,0,
		0,182,183,5,114,0,0,183,12,1,0,0,0,184,185,5,105,0,0,185,186,5,110,0,0,
		186,187,5,116,0,0,187,188,5,101,0,0,188,189,5,103,0,0,189,190,5,101,0,
		0,190,191,5,114,0,0,191,14,1,0,0,0,192,193,5,46,0,0,193,16,1,0,0,0,194,
		195,5,104,0,0,195,196,5,51,0,0,196,197,5,50,0,0,197,18,1,0,0,0,198,199,
		5,104,0,0,199,200,5,54,0,0,200,201,5,52,0,0,201,20,1,0,0,0,202,203,5,75,
		0,0,203,204,5,73,0,0,204,205,5,76,0,0,205,206,5,76,0,0,206,22,1,0,0,0,
		207,208,5,107,0,0,208,209,5,105,0,0,209,210,5,108,0,0,210,211,5,108,0,
		0,211,24,1,0,0,0,212,213,5,82,0,0,213,214,5,69,0,0,214,215,5,84,0,0,215,
		216,5,85,0,0,216,217,5,82,0,0,217,218,5,78,0,0,218,26,1,0,0,0,219,220,
		5,9654,0,0,220,28,1,0,0,0,221,222,5,94,0,0,222,30,1,0,0,0,223,224,5,42,
		0,0,224,32,1,0,0,0,225,226,5,47,0,0,226,34,1,0,0,0,227,228,5,43,0,0,228,
		36,1,0,0,0,229,230,5,45,0,0,230,38,1,0,0,0,231,232,5,91,0,0,232,40,1,0,
		0,0,233,234,5,93,0,0,234,42,1,0,0,0,235,236,5,123,0,0,236,44,1,0,0,0,237,
		238,5,125,0,0,238,46,1,0,0,0,239,240,5,8747,0,0,240,48,1,0,0,0,241,242,
		5,931,0,0,242,50,1,0,0,0,243,244,5,8706,0,0,244,52,1,0,0,0,245,246,5,8730,
		0,0,246,54,1,0,0,0,247,248,5,931,0,0,248,249,5,76,0,0,249,250,5,73,0,0,
		250,251,5,83,0,0,251,252,5,84,0,0,252,56,1,0,0,0,253,254,5,928,0,0,254,
		255,5,76,0,0,255,256,5,73,0,0,256,257,5,83,0,0,257,258,5,84,0,0,258,58,
		1,0,0,0,259,260,5,916,0,0,260,261,5,76,0,0,261,262,5,73,0,0,262,263,5,
		83,0,0,263,264,5,84,0,0,264,60,1,0,0,0,265,266,5,33,0,0,266,62,1,0,0,0,
		267,268,5,66,0,0,268,269,5,8594,0,0,269,270,5,82,0,0,270,64,1,0,0,0,271,
		272,5,62,0,0,272,66,1,0,0,0,273,274,5,60,0,0,274,68,1,0,0,0,275,276,5,
		8804,0,0,276,70,1,0,0,0,277,278,5,8805,0,0,278,72,1,0,0,0,279,280,5,61,
		0,0,280,281,5,61,0,0,281,74,1,0,0,0,282,283,5,62,0,0,283,284,5,61,0,0,
		284,76,1,0,0,0,285,286,5,60,0,0,286,287,5,61,0,0,287,78,1,0,0,0,288,289,
		5,8800,0,0,289,80,1,0,0,0,290,291,5,60,0,0,291,292,5,62,0,0,292,82,1,0,
		0,0,293,294,5,69,0,0,294,295,5,88,0,0,295,296,5,80,0,0,296,297,5,79,0,
		0,297,298,5,82,0,0,298,306,5,84,0,0,299,300,5,101,0,0,300,301,5,120,0,
		0,301,302,5,112,0,0,302,303,5,111,0,0,303,304,5,114,0,0,304,306,5,116,
		0,0,305,293,1,0,0,0,305,299,1,0,0,0,306,84,1,0,0,0,307,308,5,66,0,0,308,
		309,5,69,0,0,309,310,5,71,0,0,310,311,5,73,0,0,311,318,5,78,0,0,312,313,
		5,98,0,0,313,314,5,101,0,0,314,315,5,103,0,0,315,316,5,105,0,0,316,318,
		5,110,0,0,317,307,1,0,0,0,317,312,1,0,0,0,318,86,1,0,0,0,319,320,5,76,
		0,0,320,321,5,79,0,0,321,322,5,67,0,0,322,323,5,65,0,0,323,330,5,76,0,
		0,324,325,5,108,0,0,325,326,5,111,0,0,326,327,5,99,0,0,327,328,5,97,0,
		0,328,330,5,108,0,0,329,319,1,0,0,0,329,324,1,0,0,0,330,88,1,0,0,0,331,
		332,5,67,0,0,332,333,5,65,0,0,333,334,5,83,0,0,334,340,5,69,0,0,335,336,
		5,99,0,0,336,337,5,97,0,0,337,338,5,115,0,0,338,340,5,101,0,0,339,331,
		1,0,0,0,339,335,1,0,0,0,340,90,1,0,0,0,341,342,5,68,0,0,342,343,5,69,0,
		0,343,344,5,70,0,0,344,345,5,65,0,0,345,346,5,85,0,0,346,347,5,76,0,0,
		347,356,5,84,0,0,348,349,5,100,0,0,349,350,5,101,0,0,350,351,5,102,0,0,
		351,352,5,97,0,0,352,353,5,117,0,0,353,354,5,108,0,0,354,356,5,116,0,0,
		355,341,1,0,0,0,355,348,1,0,0,0,356,92,1,0,0,0,357,358,5,73,0,0,358,362,
		5,70,0,0,359,360,5,105,0,0,360,362,5,102,0,0,361,357,1,0,0,0,361,359,1,
		0,0,0,362,94,1,0,0,0,363,364,5,84,0,0,364,365,5,72,0,0,365,366,5,69,0,
		0,366,372,5,78,0,0,367,368,5,116,0,0,368,369,5,104,0,0,369,370,5,101,0,
		0,370,372,5,110,0,0,371,363,1,0,0,0,371,367,1,0,0,0,372,96,1,0,0,0,373,
		374,5,69,0,0,374,375,5,76,0,0,375,376,5,83,0,0,376,382,5,69,0,0,377,378,
		5,101,0,0,378,379,5,108,0,0,379,380,5,115,0,0,380,382,5,101,0,0,381,373,
		1,0,0,0,381,377,1,0,0,0,382,98,1,0,0,0,383,384,5,73,0,0,384,385,5,70,0,
		0,385,386,5,69,0,0,386,387,5,82,0,0,387,394,5,82,0,0,388,389,5,105,0,0,
		389,390,5,102,0,0,390,391,5,101,0,0,391,392,5,114,0,0,392,394,5,114,0,
		0,393,383,1,0,0,0,393,388,1,0,0,0,394,100,1,0,0,0,395,396,5,70,0,0,396,
		397,5,79,0,0,397,402,5,82,0,0,398,399,5,102,0,0,399,400,5,111,0,0,400,
		402,5,114,0,0,401,395,1,0,0,0,401,398,1,0,0,0,402,102,1,0,0,0,403,404,
		5,70,0,0,404,405,5,82,0,0,405,406,5,79,0,0,406,412,5,77,0,0,407,408,5,
		102,0,0,408,409,5,114,0,0,409,410,5,111,0,0,410,412,5,109,0,0,411,403,
		1,0,0,0,411,407,1,0,0,0,412,104,1,0,0,0,413,414,5,87,0,0,414,415,5,72,
		0,0,415,416,5,73,0,0,416,417,5,76,0,0,417,424,5,69,0,0,418,419,5,119,0,
		0,419,420,5,104,0,0,420,421,5,105,0,0,421,422,5,108,0,0,422,424,5,101,
		0,0,423,413,1,0,0,0,423,418,1,0,0,0,424,106,1,0,0,0,425,426,5,84,0,0,426,
		430,5,79,0,0,427,428,5,116,0,0,428,430,5,111,0,0,429,425,1,0,0,0,429,427,
		1,0,0,0,430,108,1,0,0,0,431,432,5,68,0,0,432,436,5,79,0,0,433,434,5,100,
		0,0,434,436,5,111,0,0,435,431,1,0,0,0,435,433,1,0,0,0,436,110,1,0,0,0,
		437,438,5,68,0,0,438,439,5,79,0,0,439,440,5,87,0,0,440,441,5,78,0,0,441,
		442,5,84,0,0,442,450,5,79,0,0,443,444,5,100,0,0,444,445,5,111,0,0,445,
		446,5,119,0,0,446,447,5,110,0,0,447,448,5,116,0,0,448,450,5,111,0,0,449,
		437,1,0,0,0,449,443,1,0,0,0,450,112,1,0,0,0,451,452,5,83,0,0,452,453,5,
		84,0,0,453,454,5,69,0,0,454,460,5,80,0,0,455,456,5,115,0,0,456,457,5,116,
		0,0,457,458,5,101,0,0,458,460,5,112,0,0,459,451,1,0,0,0,459,455,1,0,0,
		0,460,114,1,0,0,0,461,462,5,80,0,0,462,463,5,82,0,0,463,464,5,73,0,0,464,
		465,5,78,0,0,465,472,5,84,0,0,466,467,5,112,0,0,467,468,5,114,0,0,468,
		469,5,105,0,0,469,470,5,110,0,0,470,472,5,116,0,0,471,461,1,0,0,0,471,
		466,1,0,0,0,472,116,1,0,0,0,473,474,5,82,0,0,474,475,5,69,0,0,475,476,
		5,80,0,0,476,477,5,69,0,0,477,478,5,65,0,0,478,486,5,84,0,0,479,480,5,
		114,0,0,480,481,5,101,0,0,481,482,5,112,0,0,482,483,5,101,0,0,483,484,
		5,97,0,0,484,486,5,116,0,0,485,473,1,0,0,0,485,479,1,0,0,0,486,118,1,0,
		0,0,487,488,5,85,0,0,488,489,5,78,0,0,489,490,5,84,0,0,490,491,5,73,0,
		0,491,498,5,76,0,0,492,493,5,117,0,0,493,494,5,110,0,0,494,495,5,116,0,
		0,495,496,5,105,0,0,496,498,5,108,0,0,497,487,1,0,0,0,497,492,1,0,0,0,
		498,120,1,0,0,0,499,500,5,69,0,0,500,501,5,78,0,0,501,506,5,68,0,0,502,
		503,5,101,0,0,503,504,5,110,0,0,504,506,5,100,0,0,505,499,1,0,0,0,505,
		502,1,0,0,0,506,122,1,0,0,0,507,508,5,66,0,0,508,509,5,82,0,0,509,510,
		5,69,0,0,510,511,5,65,0,0,511,518,5,75,0,0,512,513,5,98,0,0,513,514,5,
		114,0,0,514,515,5,101,0,0,515,516,5,97,0,0,516,518,5,107,0,0,517,507,1,
		0,0,0,517,512,1,0,0,0,518,124,1,0,0,0,519,520,5,67,0,0,520,521,5,79,0,
		0,521,522,5,78,0,0,522,523,5,84,0,0,523,524,5,73,0,0,524,525,5,78,0,0,
		525,526,5,85,0,0,526,536,5,69,0,0,527,528,5,99,0,0,528,529,5,111,0,0,529,
		530,5,110,0,0,530,531,5,116,0,0,531,532,5,105,0,0,532,533,5,110,0,0,533,
		534,5,117,0,0,534,536,5,101,0,0,535,519,1,0,0,0,535,527,1,0,0,0,536,126,
		1,0,0,0,537,538,5,86,0,0,538,539,5,73,0,0,539,540,5,69,0,0,540,546,5,87,
		0,0,541,542,5,118,0,0,542,543,5,105,0,0,543,544,5,101,0,0,544,546,5,119,
		0,0,545,537,1,0,0,0,545,541,1,0,0,0,546,128,1,0,0,0,547,548,5,65,0,0,548,
		549,5,78,0,0,549,554,5,68,0,0,550,551,5,97,0,0,551,552,5,110,0,0,552,554,
		5,100,0,0,553,547,1,0,0,0,553,550,1,0,0,0,554,130,1,0,0,0,555,556,5,79,
		0,0,556,560,5,82,0,0,557,558,5,111,0,0,558,560,5,114,0,0,559,555,1,0,0,
		0,559,557,1,0,0,0,560,132,1,0,0,0,561,562,5,88,0,0,562,563,5,79,0,0,563,
		568,5,82,0,0,564,565,5,120,0,0,565,566,5,111,0,0,566,568,5,114,0,0,567,
		561,1,0,0,0,567,564,1,0,0,0,568,134,1,0,0,0,569,570,5,78,0,0,570,571,5,
		79,0,0,571,576,5,84,0,0,572,573,5,110,0,0,573,574,5,111,0,0,574,576,5,
		116,0,0,575,569,1,0,0,0,575,572,1,0,0,0,576,136,1,0,0,0,577,581,7,0,0,
		0,578,580,7,1,0,0,579,578,1,0,0,0,580,583,1,0,0,0,581,579,1,0,0,0,581,
		582,1,0,0,0,582,138,1,0,0,0,583,581,1,0,0,0,584,586,5,35,0,0,585,587,7,
		2,0,0,586,585,1,0,0,0,587,588,1,0,0,0,588,586,1,0,0,0,588,589,1,0,0,0,
		589,140,1,0,0,0,590,592,7,3,0,0,591,590,1,0,0,0,592,593,1,0,0,0,593,591,
		1,0,0,0,593,594,1,0,0,0,594,601,1,0,0,0,595,597,5,46,0,0,596,598,7,3,0,
		0,597,596,1,0,0,0,598,599,1,0,0,0,599,597,1,0,0,0,599,600,1,0,0,0,600,
		602,1,0,0,0,601,595,1,0,0,0,601,602,1,0,0,0,602,142,1,0,0,0,603,604,5,
		58,0,0,604,605,5,61,0,0,605,144,1,0,0,0,606,607,5,61,0,0,607,146,1,0,0,
		0,608,609,5,59,0,0,609,148,1,0,0,0,610,614,5,34,0,0,611,613,8,4,0,0,612,
		611,1,0,0,0,613,616,1,0,0,0,614,612,1,0,0,0,614,615,1,0,0,0,615,617,1,
		0,0,0,616,614,1,0,0,0,617,618,5,34,0,0,618,150,1,0,0,0,619,621,7,5,0,0,
		620,619,1,0,0,0,621,622,1,0,0,0,622,620,1,0,0,0,622,623,1,0,0,0,623,624,
		1,0,0,0,624,625,6,75,0,0,625,152,1,0,0,0,626,627,5,47,0,0,627,628,5,47,
		0,0,628,632,1,0,0,0,629,631,8,6,0,0,630,629,1,0,0,0,631,634,1,0,0,0,632,
		630,1,0,0,0,632,633,1,0,0,0,633,635,1,0,0,0,634,632,1,0,0,0,635,636,6,
		76,0,0,636,154,1,0,0,0,36,0,305,317,329,339,355,361,371,381,393,401,411,
		423,429,435,449,459,471,485,497,505,517,535,545,553,559,567,575,581,588,
		593,599,601,614,622,632,1,6,0,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}