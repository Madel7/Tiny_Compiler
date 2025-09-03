using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JASON_Compiler;
namespace Tiny_Compiler
{

    public class Node
    {
        public List<Node> Children = new List<Node>();
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }

    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;
       
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(FunctionStatements());
            program.Children.Add(MainFunction());
            MessageBox.Show("Success");
            return program;
        }
        Node MainFunction()
        {
            Node n = new Node("MainFunction");
            n.Children.Add(match(Token_Class.Integer));
            n.Children.Add(match(Token_Class.main));
            n.Children.Add(match(Token_Class.LParanthesis));
            n.Children.Add(match(Token_Class.RParanthesis));
            n.Children.Add(FunctionBody());
            return n;
        }
        Node FunctionStatements()
        {
            if (IsDataType())
            {
                Node n = new Node("FunctionStatements");
                if (!(InputPointer + 1 < TokenStream.Count &&
                   TokenStream[InputPointer].token_type == Token_Class.Integer &&
                   TokenStream[InputPointer + 1].token_type == Token_Class.main))
                {
                    n.Children.Add(FunctionStatement());
                    n.Children.Add(FunctionStatements());
                }
                return n;
            }
            return null;
        }
        Node FunctionStatement()
        {
            Node n = new Node("FunctionStatement");
            n.Children.Add(FunctionDeclaration());
            n.Children.Add(FunctionBody());
            return n;
        }
        Node FunctionDeclaration()
        {
            Node n = new Node("FunctionDeclaration");
            n.Children.Add(Datatype());
            n.Children.Add(FunctionName());
            n.Children.Add(match(Token_Class.LParanthesis));
            n.Children.Add(FunctionParameters());
            n.Children.Add(match(Token_Class.RParanthesis));
            return n;
        }
        Node Datatype()
        {
            Node n = new Node("Datatype");
            if (IsvalidToken(Token_Class.Integer))
                n.Children.Add(match(Token_Class.Integer));
            else if (IsvalidToken(Token_Class.Float))
                n.Children.Add(match(Token_Class.Float));
            else if (IsvalidToken(Token_Class.String))
                n.Children.Add(match(Token_Class.String));
            else
                return null;
            return n;
        }
        Node FunctionParameters()
        {
            Node n = new Node("FunctionParameters");
            if (IsDataType())
            {
                n.Children.Add(Parameter());
                Node moreParams = MoreFunctionParameters();
                if (moreParams != null)
                    n.Children.Add(moreParams);
            }
            else                      
                return null;
            return n;
        }
        Node Parameter()
        {
            Node node = new Node("Parameter");
            node.Children.Add(Datatype());
            node.Children.Add(match(Token_Class.Idenifier));
            return node;
        }
        Node MoreFunctionParameters()
        {
            if (IsvalidToken(Token_Class.Comma))
            {
                Node n = new Node("MoreFunctionParameters");
                n.Children.Add(match(Token_Class.Comma));
                n.Children.Add(Parameter());
                Node nextParams = MoreFunctionParameters();
                if (nextParams != null)
                    n.Children.Add(nextParams);
                return n;
            }
            return null;
        }
        Node FunctionName()
        {
            Node n = new Node("FunctionName");
             n.Children.Add(match(Token_Class.Idenifier));
            return n;
        }
        Node FunctionBody()
        {
            Node n = new Node("FunctionBody");
            n.Children.Add(match(Token_Class.LeftBrace));
            n.Children.Add(Statements());
            n.Children.Add(ReturnStatement());
            n.Children.Add(match(Token_Class.RightBrace));
            return n;
        }
        Node Statements()
        {
            if ((IsvalidToken(Token_Class.Comment) ||
                   IsDataType() ||
                   IsvalidToken(Token_Class.Idenifier) ||
                   IsvalidToken(Token_Class.Write) ||
                   IsvalidToken(Token_Class.Read) ||
                   IsvalidToken(Token_Class.If) ||
                   IsvalidToken(Token_Class.Repeat)) || IsvalidToken(Token_Class.Semicolon))
            {
                Node n = new Node("Statements");
                Node statement = Statement();
                if (statement != null)
                    n.Children.Add(statement);
                n.Children.Add(Statements());
                return n;
            }
            return null;
        }
        Node Statement()
        {
            Node n = new Node("Statement");
            Token currentToken = TokenStream[InputPointer];
            if (IsvalidToken(Token_Class.Comment))
                n.Children.Add(CommentStatement());
            else if (IsDataType())
                n.Children.Add(DeclarationStatement());
            else if (IsvalidToken(Token_Class.Idenifier))
                n.Children.Add(AssignmentStatement());
            else if (IsvalidToken(Token_Class.Write))
                n.Children.Add(WriteStatement());
            else if (IsvalidToken(Token_Class.Read))
                n.Children.Add(ReadStatement());
            else if (IsvalidToken(Token_Class.If))
                n.Children.Add(IfStatement());
            else if (IsvalidToken(Token_Class.Repeat))
                n.Children.Add(RepeatStatement());
            else if (IsvalidToken(Token_Class.Semicolon))
            {
                InputPointer++;
            }
            else
            {
                return null;
            }
                if (n.Children.Count > 0 && n.Children[0] != null)
                    return n;
            return null;
        }
        Node DeclarationStatement()
        {
            Node n = new Node("DeclarationStatement");
            n.Children.Add(Datatype());
            n.Children.Add(match(Token_Class.Idenifier));
            Node declareRest = DeclareRest();
            if (declareRest != null)
                n.Children.Add(declareRest);
            n.Children.Add(match(Token_Class.Semicolon));
            return n;
        }
        Node DeclareRest()
        {
            Node n = new Node("DeclareRest");

            if (IsvalidToken(Token_Class.Comma))
            {
                n.Children.Add(match(Token_Class.Comma));
                n.Children.Add(match(Token_Class.Idenifier));
                Node nextRest = DeclareRest();
                if (nextRest != null)
                    n.Children.Add(nextRest);
            }
            else if (IsvalidToken(Token_Class.AssignOp))
            {
                n.Children.Add(match(Token_Class.AssignOp));
                n.Children.Add(Expression());
            }
            else
                return null;
            return n;
        }
        Node FunctionCall()
        {
            Node n = new Node("FunctionCall");
            n.Children.Add(match(Token_Class.Idenifier));
            n.Children.Add(match(Token_Class.LParanthesis));
            n.Children.Add(Parameters());
            n.Children.Add(match(Token_Class.RParanthesis));
            return n;
        }
        Node AssignmentStatement()
        {
            Node n = new Node("AssignmentStatement");
            n.Children.Add(match(Token_Class.Idenifier));
            n.Children.Add(match(Token_Class.AssignOp));
            Node e = Expression();
            if (e != null)            
                n.Children.Add(e);            
            n.Children.Add(match(Token_Class.Semicolon));
            return n;
        }
        Node WriteStatement()
        {
            Node n = new Node("WriteStatement");
            n.Children.Add(match(Token_Class.Write));
            n.Children.Add(WriteRest());
            return n;
        }
        Node WriteRest()
        {
            Node n = new Node("WriteRest");
            if (IsvalidToken(Token_Class.endl))
            {
                n.Children.Add(match(Token_Class.endl));
                n.Children.Add(match(Token_Class.Semicolon));
            }
            else
            {
                n.Children.Add(Expression());
                n.Children.Add(match(Token_Class.Semicolon));
            }
            return n;
        }
        Node ReadStatement()
        {
            Node n = new Node("ReadStatement");
            n.Children.Add(match(Token_Class.Read));
            n.Children.Add(match(Token_Class.Idenifier));
            n.Children.Add(match(Token_Class.Semicolon));
            return n;
        }
        Node IfStatement()
        {
            Node n = new Node("If_Statement");
            n.Children.Add(match(Token_Class.If));
            Node condition = ConditionStatement();
            if (condition == null)
            {
                while (InputPointer < TokenStream.Count &&
                       !IsvalidToken(Token_Class.Then) &&
                       !IsvalidToken(Token_Class.End))
                {
                    InputPointer++;
                }
            }
            else
            {
                n.Children.Add(condition);
            }
            if (!IsvalidToken(Token_Class.Then)) return n;
            n.Children.Add(match(Token_Class.Then));
            Node stmt;
            while ((stmt = Statements()) != null)
            {
                n.Children.Add(stmt);
            }
            n.Children.Add(ElseStatement());
            if (!IsvalidToken(Token_Class.End)) return n;
            n.Children.Add(match(Token_Class.End));
            return n;
        }
        Node ElseStatement()
        {
            if (IsvalidToken(Token_Class.ElseIf))
            {
                Node n = new Node("ElseIfStatement");
                n.Children.Add(match(Token_Class.ElseIf));
                n.Children.Add(ConditionStatement());
                n.Children.Add(match(Token_Class.Then));
                n.Children.Add(Statements());
                n.Children.Add(ElseStatement());
                return n;
            }
            else if (IsvalidToken(Token_Class.Else))
            {
                Node n = new Node("ElseStatement");
                n.Children.Add(match(Token_Class.Else));
                n.Children.Add(Statements());
                n.Children.Add(match(Token_Class.End));
                return n;
            }
            return null;
        }
        Node RepeatStatement()
        {
            Node n = new Node("RepeatStatement");
            n.Children.Add(match(Token_Class.Repeat));
            Node stmt = null;
            do
            {
                stmt = Statements();
                if (stmt != null)
                    n.Children.Add(stmt);
            } while (stmt != null); 
            n.Children.Add(match(Token_Class.Until));
            Node condition = ConditionStatement();
            if (condition != null)
                n.Children.Add(condition);
            return n;
        }
        Node Parameters()
        {
            Node n = new Node("Parameters");
            Node expression = Expression();  
            if (expression == null) return null;
            n.Children.Add(expression);
            n.Children.Add(MoreParameters());
            return n;
        }
        Node MoreParameters()
        {
            Node n = new Node("MoreParameters");
            if (IsvalidToken(Token_Class.Comma))
            {
                n.Children.Add(match(Token_Class.Comma));
                n.Children.Add(Parameters());
                return n;
            }
            return n;
        }
        Node Expression()
        {
            Node n = new Node("Expression");

            if (IsvalidToken(Token_Class.String))
            {
                n.Children.Add(match(Token_Class.String));
            }
            else if (IsvalidToken(Token_Class.Constant) ||
                   IsvalidToken(Token_Class.Idenifier) ||
                   IsvalidToken(Token_Class.LParanthesis))
            {
                Node equation = Equation();
                if (equation != null)
                    n.Children.Add(equation);
            }
            else
            {
                Node term = Term();
                if (term != null)
                    n.Children.Add(term);
            }
            return n;
        }
        Node Equation()
        {
            Node n = new Node("Equation");
            Node f = Factor();
            if (f == null)
            {
                while (InputPointer < TokenStream.Count &&
                       !(IsvalidToken(Token_Class.PlusOp) ||
                   IsvalidToken(Token_Class.MinusOp) ||
                IsvalidToken(Token_Class.MultiplyOp) ||
                   IsvalidToken(Token_Class.DivideOp)) &&
                       !IsvalidToken(Token_Class.Semicolon))
                {
                    InputPointer++;
                }
                return null;
            }
            n.Children.Add(f);
            n.Children.Add(MoreEqn());
            return n;
        }
        Node MoreEqn()
        {
            Node n = new Node("MoreEqn");
            if (IsvalidToken(Token_Class.PlusOp) ||
                   IsvalidToken(Token_Class.MinusOp) ||
                IsvalidToken(Token_Class.MultiplyOp) ||
                   IsvalidToken(Token_Class.DivideOp))
            {
                Node ArithOp = ArithmeticOp();
                n.Children.Add(ArithOp);
                Node Eqn = Equation();
                if (Eqn == null)
                {
                    return null;
                }
                n.Children.Add(Eqn);
                return n;
            }
            return n;
        }
        Node Term()
        {
            Node node = new Node("Term");
            Node f = Factor();
            if (f == null)
            {
                while (InputPointer < TokenStream.Count &&
                       !(IsvalidToken(Token_Class.PlusOp) ||
                   IsvalidToken(Token_Class.MinusOp) ||
                IsvalidToken(Token_Class.MultiplyOp) ||
                   IsvalidToken(Token_Class.DivideOp)) &&
                       !IsvalidToken(Token_Class.Semicolon))
                {
                    InputPointer++;
                }
                return null;
            }

            node.Children.Add(f);
            node.Children.Add(Term_Tail());

            return node;
        }
        Node Term_Tail()
        {
            Node node = new Node("Term_Tail");
            if (IsvalidToken(Token_Class.PlusOp) ||
                   IsvalidToken(Token_Class.MinusOp) ||
                IsvalidToken(Token_Class.MultiplyOp) ||
                   IsvalidToken(Token_Class.DivideOp))
            {
                Node ArithOp = ArithmeticOp();
                node.Children.Add(ArithOp);
                Node term = Term();
                if (term == null)                
                    return null;                
                node.Children.Add(term);
                return node;
            }
            return node;
        }
        Node Factor()
        {
            Node node = new Node("Term");
            if (IsvalidToken(Token_Class.Constant))
            {
                node.Children.Add(match(Token_Class.Constant));
            }
            else if (IsvalidToken(Token_Class.Idenifier))
            {
                if (InputPointer + 1 < TokenStream.Count &&TokenStream[InputPointer].token_type == Token_Class.Idenifier &&TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                {
                    node.Children.Add(FunctionCall());
                }
                else
                {
                    node.Children.Add(match(Token_Class.Idenifier));
                }
            }
            else if (IsvalidToken(Token_Class.LParanthesis))
            {
                node.Children.Add(match(Token_Class.LParanthesis));
                node.Children.Add(Equation());
                node.Children.Add(match(Token_Class.RParanthesis));
            }
            else
            {
                return null; 
            }
            return node;
        }
        Node ConditionStatement()
        {
            Node n = new Node("ConditionStatement");
                Node firstCondition = Condition();
                if (firstCondition == null)
                return null;              
                n.Children.Add(firstCondition);
                Node more = MoreConditions();
                if (more != null)
                n.Children.Add(more);               
                return n;           
        }
        Node MoreConditions()
        {
            if (!(IsvalidToken(Token_Class.AndOp) || IsvalidToken(Token_Class.OrOp)))            
                return null;            
            Node n = new Node("MoreConditions");
            {
                n.Children.Add(BooleanOperator());
                Node nextCondition = Condition();
                if (nextCondition == null)
                {
                    return null;
                }
                n.Children.Add(nextCondition);
                Node additionalClauses = MoreConditions();
                if (additionalClauses != null)
                    n.Children.Add(additionalClauses);
                return n;
            }
        }
        Node Condition()
        {
            Node n = new Node("Condition");
            n.Children.Add(match(Token_Class.Idenifier));
            n.Children.Add(ConditionOp());
            n.Children.Add(Term());
            return n;
        }
        Node ArithmeticOp()
        {
            Node n = new Node("ArithmeticOp");
            if (IsvalidToken(Token_Class.PlusOp))
                n.Children.Add(match(Token_Class.PlusOp));
            else if (IsvalidToken(Token_Class.MinusOp))
                n.Children.Add(match(Token_Class.MinusOp));
            else if (IsvalidToken(Token_Class.MultiplyOp))
                n.Children.Add(match(Token_Class.MultiplyOp));
            else
                n.Children.Add(match(Token_Class.DivideOp));
            return n;
        }
        Node ConditionOp()
        {
            Node n = new Node("ConditionOp");
            if (IsvalidToken(Token_Class.LessThanOp))
                n.Children.Add(match(Token_Class.LessThanOp));
            else if (IsvalidToken(Token_Class.GreaterThanOp))
                n.Children.Add(match(Token_Class.GreaterThanOp));
            else if (IsvalidToken(Token_Class.EqualOp))
                n.Children.Add(match(Token_Class.EqualOp));
            else
                n.Children.Add(match(Token_Class.NotEqualOp));
            return n;
        }
        Node BooleanOperator()
        {
            Node n = new Node("BooleanOp");
            if (IsvalidToken(Token_Class.AndOp))
                n.Children.Add(match(Token_Class.AndOp));
            else
                n.Children.Add(match(Token_Class.OrOp));
            return n;
        }
        Node ReturnStatement()
        {
            Node n = new Node("ReturnStatement");
            n.Children.Add(match(Token_Class.Return));
            Node exp = Expression();
            n.Children.Add(exp);
            n.Children.Add(match(Token_Class.Semicolon));
            return n;
        }
        Node CommentStatement()
        {
            Node n = new Node("CommentStatement");
            if (IsvalidToken(Token_Class.Comment))
            {
                n.Children.Add(match(Token_Class.Comment));
            }
            else
            {
                return null;
            }
            return n;
        }
        private bool IsDataType()
        {
            return IsvalidToken(Token_Class.Integer) ||
                   IsvalidToken(Token_Class.Float) ||
                   IsvalidToken(Token_Class.String);
        }
        private bool IsvalidToken(Token_Class token)
        {
            return (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == token);
        }
        public Node match(Token_Class ExpectedToken)
        {
            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    Node newNode = new Node(TokenStream[InputPointer].token_type.ToString());
                    InputPointer++;
                    return newNode;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        public static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}