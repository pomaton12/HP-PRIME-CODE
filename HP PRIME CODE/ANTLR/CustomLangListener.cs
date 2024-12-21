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

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="CustomLangParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.2")]
[System.CLSCompliant(false)]
public interface ICustomLangListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProgram([NotNull] CustomLangParser.ProgramContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProgram([NotNull] CustomLangParser.ProgramContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.viewStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterViewStatement([NotNull] CustomLangParser.ViewStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.viewStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitViewStatement([NotNull] CustomLangParser.ViewStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.pragma"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPragma([NotNull] CustomLangParser.PragmaContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.pragma"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPragma([NotNull] CustomLangParser.PragmaContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.pragmaSetting"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPragmaSetting([NotNull] CustomLangParser.PragmaSettingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.pragmaSetting"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPragmaSetting([NotNull] CustomLangParser.PragmaSettingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.separatorValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSeparatorValue([NotNull] CustomLangParser.SeparatorValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.separatorValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSeparatorValue([NotNull] CustomLangParser.SeparatorValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.integerType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIntegerType([NotNull] CustomLangParser.IntegerTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.integerType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIntegerType([NotNull] CustomLangParser.IntegerTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Asignar_Valor</c>
	/// labeled alternative in <see cref="CustomLangParser.globalVariableDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAsignar_Valor([NotNull] CustomLangParser.Asignar_ValorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Asignar_Valor</c>
	/// labeled alternative in <see cref="CustomLangParser.globalVariableDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAsignar_Valor([NotNull] CustomLangParser.Asignar_ValorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Declarar_Funcion</c>
	/// labeled alternative in <see cref="CustomLangParser.functionDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDeclarar_Funcion([NotNull] CustomLangParser.Declarar_FuncionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Declarar_Funcion</c>
	/// labeled alternative in <see cref="CustomLangParser.functionDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDeclarar_Funcion([NotNull] CustomLangParser.Declarar_FuncionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.functionDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionDefinition([NotNull] CustomLangParser.FunctionDefinitionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.functionDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionDefinition([NotNull] CustomLangParser.FunctionDefinitionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.functionVariable"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionVariable([NotNull] CustomLangParser.FunctionVariableContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.functionVariable"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionVariable([NotNull] CustomLangParser.FunctionVariableContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] CustomLangParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] CustomLangParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.variableDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariableDeclaration([NotNull] CustomLangParser.VariableDeclarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.variableDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariableDeclaration([NotNull] CustomLangParser.VariableDeclarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.variableDeclarator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariableDeclarator([NotNull] CustomLangParser.VariableDeclaratorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.variableDeclarator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariableDeclarator([NotNull] CustomLangParser.VariableDeclaratorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.assignLocal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignLocal([NotNull] CustomLangParser.AssignLocalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.assignLocal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignLocal([NotNull] CustomLangParser.AssignLocalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.assignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignment([NotNull] CustomLangParser.AssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.assignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignment([NotNull] CustomLangParser.AssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.complexExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComplexExpression([NotNull] CustomLangParser.ComplexExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.complexExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComplexExpression([NotNull] CustomLangParser.ComplexExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.caseStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCaseStatement([NotNull] CustomLangParser.CaseStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.caseStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCaseStatement([NotNull] CustomLangParser.CaseStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.ifStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatement([NotNull] CustomLangParser.IfStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.ifStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatement([NotNull] CustomLangParser.IfStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.iferrStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIferrStatement([NotNull] CustomLangParser.IferrStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.iferrStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIferrStatement([NotNull] CustomLangParser.IferrStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.forStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForStatement([NotNull] CustomLangParser.ForStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.forStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForStatement([NotNull] CustomLangParser.ForStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.whileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileStatement([NotNull] CustomLangParser.WhileStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.whileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileStatement([NotNull] CustomLangParser.WhileStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.repeatStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRepeatStatement([NotNull] CustomLangParser.RepeatStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.repeatStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRepeatStatement([NotNull] CustomLangParser.RepeatStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>PrintConParentesis</c>
	/// labeled alternative in <see cref="CustomLangParser.printStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrintConParentesis([NotNull] CustomLangParser.PrintConParentesisContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>PrintConParentesis</c>
	/// labeled alternative in <see cref="CustomLangParser.printStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrintConParentesis([NotNull] CustomLangParser.PrintConParentesisContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>PrintSinParentesis</c>
	/// labeled alternative in <see cref="CustomLangParser.printStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrintSinParentesis([NotNull] CustomLangParser.PrintSinParentesisContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>PrintSinParentesis</c>
	/// labeled alternative in <see cref="CustomLangParser.printStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrintSinParentesis([NotNull] CustomLangParser.PrintSinParentesisContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.functionCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionCall([NotNull] CustomLangParser.FunctionCallContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.functionCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionCall([NotNull] CustomLangParser.FunctionCallContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.subFunctionCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubFunctionCall([NotNull] CustomLangParser.SubFunctionCallContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.subFunctionCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubFunctionCall([NotNull] CustomLangParser.SubFunctionCallContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.killStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterKillStatement([NotNull] CustomLangParser.KillStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.killStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitKillStatement([NotNull] CustomLangParser.KillStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturnStatement([NotNull] CustomLangParser.ReturnStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturnStatement([NotNull] CustomLangParser.ReturnStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.breakStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBreakStatement([NotNull] CustomLangParser.BreakStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.breakStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBreakStatement([NotNull] CustomLangParser.BreakStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.continueStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterContinueStatement([NotNull] CustomLangParser.ContinueStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.continueStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitContinueStatement([NotNull] CustomLangParser.ContinueStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.storeStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStoreStatement([NotNull] CustomLangParser.StoreStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.storeStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStoreStatement([NotNull] CustomLangParser.StoreStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Suma_de_lista</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSuma_de_lista([NotNull] CustomLangParser.Suma_de_listaContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Suma_de_lista</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSuma_de_lista([NotNull] CustomLangParser.Suma_de_listaContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ParentesisExpression</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParentesisExpression([NotNull] CustomLangParser.ParentesisExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ParentesisExpression</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParentesisExpression([NotNull] CustomLangParser.ParentesisExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ComplexStructures</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComplexStructures([NotNull] CustomLangParser.ComplexStructuresContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ComplexStructures</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComplexStructures([NotNull] CustomLangParser.ComplexStructuresContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Sumatoria</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSumatoria([NotNull] CustomLangParser.SumatoriaContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Sumatoria</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSumatoria([NotNull] CustomLangParser.SumatoriaContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>UnaryLogical</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryLogical([NotNull] CustomLangParser.UnaryLogicalContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>UnaryLogical</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryLogical([NotNull] CustomLangParser.UnaryLogicalContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>BasicValues</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBasicValues([NotNull] CustomLangParser.BasicValuesContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>BasicValues</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBasicValues([NotNull] CustomLangParser.BasicValuesContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>SingleRelational</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSingleRelational([NotNull] CustomLangParser.SingleRelationalContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>SingleRelational</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSingleRelational([NotNull] CustomLangParser.SingleRelationalContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Producto_de_lista</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProducto_de_lista([NotNull] CustomLangParser.Producto_de_listaContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Producto_de_lista</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProducto_de_lista([NotNull] CustomLangParser.Producto_de_listaContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Integracion</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIntegracion([NotNull] CustomLangParser.IntegracionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Integracion</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIntegracion([NotNull] CustomLangParser.IntegracionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Primeras_diferencias_de_lista</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrimeras_diferencias_de_lista([NotNull] CustomLangParser.Primeras_diferencias_de_listaContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Primeras_diferencias_de_lista</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrimeras_diferencias_de_lista([NotNull] CustomLangParser.Primeras_diferencias_de_listaContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Exponentiation</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExponentiation([NotNull] CustomLangParser.ExponentiationContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Exponentiation</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExponentiation([NotNull] CustomLangParser.ExponentiationContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Derivada</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDerivada([NotNull] CustomLangParser.DerivadaContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Derivada</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDerivada([NotNull] CustomLangParser.DerivadaContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>RaizCuadrada</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRaizCuadrada([NotNull] CustomLangParser.RaizCuadradaContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>RaizCuadrada</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRaizCuadrada([NotNull] CustomLangParser.RaizCuadradaContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>LogicalExpression</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicalExpression([NotNull] CustomLangParser.LogicalExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>LogicalExpression</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicalExpression([NotNull] CustomLangParser.LogicalExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>AdditionOrSubtraction</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAdditionOrSubtraction([NotNull] CustomLangParser.AdditionOrSubtractionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>AdditionOrSubtraction</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAdditionOrSubtraction([NotNull] CustomLangParser.AdditionOrSubtractionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>UnaryNegation</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryNegation([NotNull] CustomLangParser.UnaryNegationContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>UnaryNegation</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryNegation([NotNull] CustomLangParser.UnaryNegationContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>MultiplicationOrDivision</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultiplicationOrDivision([NotNull] CustomLangParser.MultiplicationOrDivisionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>MultiplicationOrDivision</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultiplicationOrDivision([NotNull] CustomLangParser.MultiplicationOrDivisionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>FunctionAccess</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionAccess([NotNull] CustomLangParser.FunctionAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>FunctionAccess</c>
	/// labeled alternative in <see cref="CustomLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionAccess([NotNull] CustomLangParser.FunctionAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.array"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArray([NotNull] CustomLangParser.ArrayContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.array"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArray([NotNull] CustomLangParser.ArrayContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.matrix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMatrix([NotNull] CustomLangParser.MatrixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.matrix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMatrix([NotNull] CustomLangParser.MatrixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterList([NotNull] CustomLangParser.ListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitList([NotNull] CustomLangParser.ListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.complex"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComplex([NotNull] CustomLangParser.ComplexContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.complex"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComplex([NotNull] CustomLangParser.ComplexContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.integralExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIntegralExpression([NotNull] CustomLangParser.IntegralExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.integralExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIntegralExpression([NotNull] CustomLangParser.IntegralExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>SimpleSummation</c>
	/// labeled alternative in <see cref="CustomLangParser.summationExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSimpleSummation([NotNull] CustomLangParser.SimpleSummationContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>SimpleSummation</c>
	/// labeled alternative in <see cref="CustomLangParser.summationExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSimpleSummation([NotNull] CustomLangParser.SimpleSummationContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>MultiSummation</c>
	/// labeled alternative in <see cref="CustomLangParser.summationExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultiSummation([NotNull] CustomLangParser.MultiSummationContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>MultiSummation</c>
	/// labeled alternative in <see cref="CustomLangParser.summationExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultiSummation([NotNull] CustomLangParser.MultiSummationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.expressionList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpressionList([NotNull] CustomLangParser.ExpressionListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.expressionList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpressionList([NotNull] CustomLangParser.ExpressionListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.idList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIdList([NotNull] CustomLangParser.IdListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.idList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIdList([NotNull] CustomLangParser.IdListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.derivativeExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDerivativeExpression([NotNull] CustomLangParser.DerivativeExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.derivativeExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDerivativeExpression([NotNull] CustomLangParser.DerivativeExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.sqrtExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSqrtExpression([NotNull] CustomLangParser.SqrtExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.sqrtExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSqrtExpression([NotNull] CustomLangParser.SqrtExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.listSum"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterListSum([NotNull] CustomLangParser.ListSumContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.listSum"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitListSum([NotNull] CustomLangParser.ListSumContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.listProduct"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterListProduct([NotNull] CustomLangParser.ListProductContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.listProduct"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitListProduct([NotNull] CustomLangParser.ListProductContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.firstDifferences"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFirstDifferences([NotNull] CustomLangParser.FirstDifferencesContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.firstDifferences"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFirstDifferences([NotNull] CustomLangParser.FirstDifferencesContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.factorialExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFactorialExpression([NotNull] CustomLangParser.FactorialExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.factorialExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFactorialExpression([NotNull] CustomLangParser.FactorialExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.basetoReal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBasetoReal([NotNull] CustomLangParser.BasetoRealContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.basetoReal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBasetoReal([NotNull] CustomLangParser.BasetoRealContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.relationalOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRelationalOperator([NotNull] CustomLangParser.RelationalOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.relationalOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRelationalOperator([NotNull] CustomLangParser.RelationalOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.logicalOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicalOperator([NotNull] CustomLangParser.LogicalOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.logicalOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicalOperator([NotNull] CustomLangParser.LogicalOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.matrixAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMatrixAccess([NotNull] CustomLangParser.MatrixAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.matrixAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMatrixAccess([NotNull] CustomLangParser.MatrixAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomLangParser.listAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterListAccess([NotNull] CustomLangParser.ListAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomLangParser.listAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitListAccess([NotNull] CustomLangParser.ListAccessContext context);
}