using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace CodeGen
{
	/// <summary>
	/// An interface for the statementlines.
	/// </summary>
   public interface IStatementLine
    {
		/// <summary>
		/// A dictionary containing all the Expressions for this StatementLine.
		/// </summary>
        Dictionary<int,IExpressionLine> Expressions { get;}

		/// <summary>
		/// Create a statementLine from all the Expressions.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
        CodeStatement CreateStatement();
    }
}

namespace CodeGen.Statements
{
	/// <summary>
	/// A CommentStatementLine is a class for simply creating comment lines.
	/// </summary>
    public class CommentStatementLine : IStatementLine
    {
		/// <summary>
		/// Is the comment line a document line or a regular comment.
		/// </summary>
        public bool? IsDoc { get; set; }

		/// <summary>
		/// The content of the comment line.
		/// </summary>
        public string Text { get; set; }

		/// <summary>
		/// Implemented from the IStatementLine but not used.
		/// </summary>
        public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// The default contructor for creating a new instance of the CommentStatementLine.
		/// </summary>
	    public CommentStatementLine()
	    {
		    Expressions = new Dictionary<int, IExpressionLine>();
	    }

		/// <summary>
		/// Create a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return IsDoc == null
                ? new CodeCommentStatement(Text)
                : new CodeCommentStatement(Text,
                    IsDoc.Value);
        }
    }

	/// <summary>
	/// A ReturnStatementLine is a class for simply creating a return line.
	/// </summary>
	public class ReturnStatementLine : IStatementLine
    {
		/// <summary>
		/// A dictionary storing the expressions used in the return statementline.
		/// </summary>
        public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// Create a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return new CodeMethodReturnStatement(Expressions[0].CreateExpression());
        }

		/// <summary>
		/// The default contructor for creating a new instance of the ReturnStatementLine.
		/// </summary>
		public ReturnStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}
    }

	/// <summary>
	/// An AssignStatementLine is a class for simply creating a assign line.
	/// </summary>
	public class AssignStatementLine : IStatementLine
    {
		/// <summary>
		/// A dictionary storing the expressions used in the assign statementline.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return new CodeAssignStatement(Expressions[0].CreateExpression(),Expressions[1].CreateExpression());
        }

		/// <summary>
		/// The default contructor for creating a new instance of the AssignStatementLine.
		/// </summary>
		public AssignStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}
    }

	/// <summary>
	/// An AttachEventStatementLine is a class for simply creating an attach event statement line.
	/// </summary>
	public class AttachEventStatementLine : IStatementLine
    {
		/// <summary>
		/// Name of the event that must be attached.
		/// </summary>
        public string EventName { get; set; }

		/// <summary>
		/// A dictionary storing the expressions used in the attach event statementline.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return
                new CodeAttachEventStatement(
                    new CodeEventReferenceExpression(Expressions[0].CreateExpression(), EventName),
                    Expressions[1].CreateExpression());
        }

		/// <summary>
		/// The default contructor for creating a new instance of the AttachEventStatementLine.
		/// </summary>
		public AttachEventStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}
    }

	/// <summary>
	/// A ConditionStatementLine is a class for simply creating a condition statement line.
	/// </summary>
	public class ConditionStatementLine : IStatementLine
    {
		/// <summary>
		/// A dictionary storing the expressions used in the condition statementline.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// An array of statements that must be executed if the condition turns out to be true.
		/// </summary>
        public IStatementLine[] TrueStatementLines { get; set; }

		/// <summary>
		/// An array of statements that must be executed if the condition turns out to be false.
		/// </summary>
		public IStatementLine[] FalseStatementLines { get; set; }

		/// <summary>
		/// The default contructor for creating a new instance of the ConditionStatementLine.
		/// </summary>
		public ConditionStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
	        if (FalseStatementLines != null)
	        {
		        var trueStatements = TrueStatementLines.Select(e => e.CreateStatement()).ToArray();
		        var falseStatements = FalseStatementLines.Select(e => e.CreateStatement()).ToArray();
		        return new CodeConditionStatement(Expressions[0].CreateExpression(), trueStatements, falseStatements);
	        }
	        else
	        {
				var trueStatements = TrueStatementLines.Select( e => e.CreateStatement() ).ToArray();
				return new CodeConditionStatement( Expressions[0].CreateExpression(), trueStatements );
			}
        }
    }

	/// <summary>
	/// An ExpressionStatementLine is a class for simply creating an expression statement line.
	/// </summary>
	public class ExpressionStatementLine : IStatementLine
    {
		/// <summary>
		/// A dictionary storing the expressions used in the expression statementline.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// The default contructor for creating a new instance of the ExpressionStatementLine.
		/// </summary>
		public ExpressionStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return new CodeExpressionStatement(Expressions[0].CreateExpression()); 
        }
    }

	/// <summary>
	/// A GotoStatementLine is a class for simply creating a goto statement line.
	/// </summary>
	public class GotoStatementLine : IStatementLine
    {
		/// <summary>
		/// The name of the label that must be jumped to.
		/// </summary>
        public string Label { get; set; }

		/// <summary>
		/// Implemented from the IStatementLine but not used.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// The default contructor for creating a new instance of the GotoStatementLine.
		/// </summary>
		public GotoStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return new CodeGotoStatement(Label);
        }
    }

	/// <summary>
	/// A LabledStatementLine is a class for simply creating a labled statement line.
	/// </summary>
	public class LabledStatementLine : IStatementLine
    {
		/// <summary>
		/// Name of the generated label statement.
		/// </summary>
        public string Label { get; set; }

		/// <summary>
		/// The statementline that the label contains.
		/// </summary>
        public IStatementLine StatementLine { get; set; }

		/// <summary>
		/// Implemented from the IStatementLine but not used.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// The default contructor for creating a new instance of the LabledStatementLine.
		/// </summary>
		public LabledStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return StatementLine == null
                ? new CodeLabeledStatement(Label)
                : new CodeLabeledStatement(Label, StatementLine.CreateStatement());
        }
    }

	/// <summary>
	/// An IterationStatementLine is a class for simply creating an iteration statement line like a for or while loop.
	/// </summary>
	public class IterationStatementLine : IStatementLine
    {
		/// <summary>
		/// A statement containing the initial value of the iteration.
		/// </summary>
        public IStatementLine Init { get; set; }

		/// <summary>
		/// A statement containing the increment that must take place every iteration.
		/// </summary>
        public IStatementLine Increment { get; set; }

		/// <summary>
		/// The statementlines that must be executed every iteration.
		/// </summary>
        public IStatementLine[] StatementLines { get; set; }

		/// <summary>
		/// A dictionary storing the expressions used in the return statementline.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// The default contructor for creating a new instance of the IterationStatementLine.
		/// </summary>
		public IterationStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return new CodeIterationStatement(Init.CreateStatement(), Expressions[0].CreateExpression(),
                Increment.CreateStatement(), StatementLines.Select(s => s.CreateStatement()).ToArray());
        }
    }

	/// <summary>
	/// A SnippetStatementLine is a class for simply creating a snippet statement line.
	/// </summary>
	public class SnippetStatementLine : IStatementLine
    {
		/// <summary>
		/// The values of the snippet statement.
		/// </summary>
        public string Value { get; set; }

		/// <summary>
		/// Implemented from the IStatementLine but not used.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// The default contructor for creating a new instance of the SnippetStatementLine.
		/// </summary>
		public SnippetStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return new CodeSnippetStatement(Value);
        }
    }

	/// <summary>
	/// A RemoveEventStatementLine is a class for simply creating a remove event statement line.
	/// </summary>
	public class RemoveEventStatementLine: IStatementLine
    {
		/// <summary>
		/// The name of the event that must be removed.
		/// </summary>
        public string EventName { get; set; }

		/// <summary>
		/// A dictionary storing the expressions used in the return statementline.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// The default contructor for creating a new instance of the RemoveEventStatementLine.
		/// </summary>
		public RemoveEventStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return new CodeRemoveEventStatement(new CodeEventReferenceExpression(Expressions[0].CreateExpression(), EventName),
                    Expressions[1].CreateExpression());
        }
    }

	/// <summary>
	/// A ThrowExceptionStatementLine is a class for simply creating a throw exception statement line.
	/// </summary>
	public class ThrowExceptionStatementLine: IStatementLine
    {
		/// <summary>
		/// A dictionary storing the expressions used in the return statementline.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// The default contructor for creating a new instance of the ThrowExceptionStatementLine.
		/// </summary>
		public ThrowExceptionStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return new CodeThrowExceptionStatement(Expressions[0].CreateExpression());
        }
    }

	/// <summary>
	/// A TryCatchFinallyStatementLine is a class for simply creating a try catch statement line.
	/// </summary>
	public class TryCatchFinallyStatementLine: IStatementLine
    {
		/// <summary>
		/// An array of statementlines that is part of the try part of this statement.
		/// </summary>
        public IStatementLine[] Try { get; set; }
		
		/// <summary>
		/// Name of the variable used in the catch block.
		/// </summary>
        public string LocalName { get; set; }

		/// <summary>
		/// Type of variable used in the catch block.
		/// </summary>
        public Type Type { get; set; }

		/// <summary>
		/// An array of statementlines that is part of the catch part of this statement.
		/// </summary>
		public IStatementLine[] Catch { get; set; }

		/// <summary>
		/// An array of statementlines that is part of the finally part of this statement.
		/// </summary>
		public IStatementLine[] Finally { get; set; }

		/// <summary>
		/// Implemented from the IStatementLine but not used.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// The default contructor for creating a new instance of the TryCatchFinallyStatementLine.
		/// </summary>
		public TryCatchFinallyStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            CodeCatchClause codeCatchClause;

            if (Type != null && Catch != null)
                codeCatchClause = new CodeCatchClause(LocalName, new CodeTypeReference(Type),
                    Catch.Select(s => s.CreateStatement()).ToArray());
            else if (Type != null && Catch == null)
                codeCatchClause = new CodeCatchClause(LocalName,new CodeTypeReference(Type));
            else
                codeCatchClause = new CodeCatchClause(LocalName);


            return Finally != null
                ? new CodeTryCatchFinallyStatement(Try.Select(s => s.CreateStatement()).ToArray(), new[] {codeCatchClause},
                    Finally.Select(s => s.CreateStatement()).ToArray())
                : new CodeTryCatchFinallyStatement(Try.Select(s => s.CreateStatement()).ToArray(),
                    new[] {codeCatchClause});
        }
    }

	/// <summary>
	/// A VariableDeclarationStatementLine is a class for simply creating a variable declaration statement line.
	/// </summary>
	public class VariableDeclarationStatementLine: IStatementLine
    {
		/// <summary>
		/// Type of the variable that is being declared.
		/// </summary>
        public Type Type { get; set; }

		/// <summary>
		/// Name of the variable that is being declared.
		/// </summary>
        public string Name { get; set; }

		/// <summary>
		/// A dictionary storing the expressions used in the return statementline.
		/// </summary>
		public Dictionary<int, IExpressionLine> Expressions { get; }

		/// <summary>
		/// The default contructor for creating a new instance of the VariableDeclarationStatementLine.
		/// </summary>
		public VariableDeclarationStatementLine()
	    {
			Expressions = new Dictionary<int, IExpressionLine>();
		}

		/// <summary>
		/// Creates a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
		public CodeStatement CreateStatement()
        {
            return Expressions == null
                ? new CodeVariableDeclarationStatement(Type, Name)
                : new CodeVariableDeclarationStatement(Type, Name, Expressions[0].CreateExpression());

        }
    }
}
