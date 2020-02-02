using System.Collections.Generic;

namespace GenerateAst
{
    class Program
    {

        static void Main(string[] args)
        {
            //if (args.Length != 1)
            //{
            //    Console.WriteLine("Usage: generate_ast <output directory>");
            //    Environment.Exit(64);
            //}
            //string outputDir = args[0];

            string outputDir = "../../../../Ture/Models";

            DefineAst(outputDir, "Expr", new List<string>()
            {
                "Assign   : Token name, Expr value",
                "Binary : Expr left, Token oper, Expr right",
                "Call : Expr callee, Token paren, ICollection<Expr> arguments",
                "Grouping : Expr expression",
                "Literal : Object value",
                "Logical : Expr left, Token oper, Expr right",
                "Unary : Token oper, Expr right",
                "Variable : Token name"
            });

            DefineAst(outputDir, "Stmt", new List<string>()
            {
                "Block : ICollection<Stmt> statements",
                "Expression : Expr expr",
                "Function : Token name, IList<Token> parameters, ICollection<Stmt> body",
                "If : Expr condition, Stmt thenBranch, Stmt elseBranch",
                "Print : Expr expr",
                "Return : Token keyword, Expr value",
                "Var : Token name, Expr initializer",
                "While : Expr condition, Stmt body"
            });
        }

        private static void DefineAst(string outputDir, string baseName, ICollection<string> types)
        {
            string path = outputDir + "/" + baseName + ".cs";

            using System.IO.StreamWriter file = new System.IO.StreamWriter(path);
            file.WriteLine("using System;");
            file.WriteLine("using System.Collections.Generic;");
            file.WriteLine("");
            file.WriteLine("namespace Ture.Models");
            file.WriteLine("{");
            file.WriteLine($"    public abstract class {baseName}");
            file.WriteLine("    {");
            DefineVisitor(file, baseName, types);
            file.WriteLine("");
            file.WriteLine("        public abstract R Accept<R>(IVisitor<R> visitor);");
            file.WriteLine("");

            foreach (var type in types)
            {
                string className = type.Split(":")[0].Trim();
                string fieldList = type.Split(":")[1].Trim();
                DefineType(file, baseName, className, fieldList);
            }

            file.WriteLine("    }");
            file.WriteLine("}");
        }

        private static void DefineVisitor(System.IO.StreamWriter file, string baseName, ICollection<string> types)
        {
            file.WriteLine("        public interface IVisitor<R>");
            file.WriteLine("        {");

            foreach (var type in types)
            {
                string typeName = type.Split(":")[0].Trim();
                file.WriteLine($"            public R Visit{Capitalize(typeName)}{baseName}({typeName} {baseName.ToLower()});");
            }
            file.WriteLine("        }");
        }

        private static void DefineType(System.IO.StreamWriter file, string baseName, string className, string fieldList)
        {
            string[] fields = fieldList.Split(", ");


            file.WriteLine($"        public class {className} : {baseName}");
            file.WriteLine("        {");

            foreach (var field in fields)
            {
                string type = field.Split(" ")[0];
                string name = field.Split(" ")[1];
                file.WriteLine($"            public {type} {Capitalize(name)};");
            }

            file.WriteLine("");


            file.WriteLine($"            public {className}({fieldList})");
            file.WriteLine("            {");

            foreach (var field in fields)
            {
                string name = field.Split(" ")[1];
                file.WriteLine($"                {Capitalize(name)} = {name};");
            }

            file.WriteLine("            }");
            file.WriteLine("");
            file.WriteLine("            public override R Accept<R>(IVisitor<R> visitor)");
            file.WriteLine("            {");
            file.WriteLine($"                return visitor.Visit{className}{baseName}(this);");
            file.WriteLine("            }");
            file.WriteLine("        }");
            file.WriteLine("");
        }

        private static string Capitalize(string text)
        {
            return char.ToUpper(text[0]) + text.Substring(1);
        }

    }
}
