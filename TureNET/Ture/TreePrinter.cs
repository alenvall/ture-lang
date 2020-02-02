using System.Text;
using Ture.Models;

namespace Ture
{
    //public class TreePrinter // : Expr.IVisitor<string>
    //{
    //    public string Print(Expr expr)
    //    {
    //        return expr.Accept(this);
    //    }

    //    public string VisitBinaryExpr(Expr.Binary expr)
    //    {
    //        return Parenthesize(expr.Oper.Lexeme, expr.Left, expr.Right);
    //    }

    //    public string VisitGroupingExpr(Expr.Grouping expr)
    //    {
    //        return Parenthesize("group", expr.Expression);
    //    }

    //    public string VisitLiteralExpr(Expr.Literal expr)
    //    {
    //        if (expr.Value == null)
    //        {
    //            return "null";
    //        }

    //        return expr.Value.ToString();
    //    }

    //    public string VisitUnaryExpr(Expr.Unary expr)
    //    {
    //        return Parenthesize(expr.Oper.Lexeme, expr.Right);
    //    }

    //    private string Parenthesize(string name, params Expr[] exprs)
    //    {
    //        StringBuilder builder = new StringBuilder();

    //        builder.Append("(").Append(name);

    //        foreach (var expr in exprs)
    //        {
    //            builder.Append(" ");
    //            builder.Append(expr.Accept(this));
    //        }
    //        builder.Append(")");

    //        return builder.ToString();
    //    }
    //}
}
