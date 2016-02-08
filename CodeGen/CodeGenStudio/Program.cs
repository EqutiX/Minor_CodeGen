using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeGen;
using CodeGen.Expressions;
using CodeGen.Statements;

namespace CodeGenStudio
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			var cb = new ClassBuilder("EntryPoint", TypeAttributes.Public, false);
			var pi = new ParameterItem();
			pi.Name = "s";
			pi.Type = typeof (string);
			
			IStatementLine[] Statements = new IStatementLine[]
			{
				new ExpressionStatementLine(),
				new ReturnStatementLine()
			};

			var methodInvoke = new MethodInvokeExpressionLine();
			methodInvoke.MethodName = "WriteLine";
			var test = new TypeReferenceExpressionLine();
			test.Type = "System.Console";
			methodInvoke.TargetObject = test;
			var methodInvokeParam = new VariableReferenceExpressionLine();
			methodInvokeParam.VariableName = "s";
			methodInvoke.Parameters = new IExpressionLine[] {methodInvokeParam};
			Statements[1].Expressions.Add(0, new VariableReferenceExpressionLine() { VariableName = "s"});

			Statements[0].Expressions.Add( 0, methodInvoke );
			cb.AddMethod<string>("Print", new ParameterItem[] { pi }, MemberAttributes.Public | MemberAttributes.Static, Statements);

			var cub = new CompileUnitBuilder("Bla");
			var typeDecl = cb.GetDeclaration();
			cub.AddClass( typeDecl );
			cub.PublishCode( "c:\\Users\\Sytse\\Documents\\GitHubVisualStudio\\Minor_CodeGen\\CodeGen\\test.cs", "CSharp" );

			Console.WriteLine("Hello");
			Console.ReadKey();
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new CodeGenStudio());
		}
	}
}
