using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant, main, Return, Repeat, ElseIf, endl, Comment
    //Added tokens
    , Float, String, StringLiteral, AndOp, OrOp, AssignOp
    , RightBrace, LeftBrace
}
namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("IF", Token_Class.If);
            ReservedWords.Add("BEGIN", Token_Class.Begin);
            ReservedWords.Add("CALL", Token_Class.Call);
            ReservedWords.Add("DECLARE", Token_Class.Declare);
            ReservedWords.Add("END", Token_Class.End);
            ReservedWords.Add("DO", Token_Class.Do);
            ReservedWords.Add("ELSE", Token_Class.Else);
            ReservedWords.Add("ENDIF", Token_Class.EndIf);
            ReservedWords.Add("ENDUNTIL", Token_Class.EndUntil);
            ReservedWords.Add("ENDWHILE", Token_Class.EndWhile);
            ReservedWords.Add("INTEGER", Token_Class.Integer);
            ReservedWords.Add("PARAMETERS", Token_Class.Parameters);
            ReservedWords.Add("PROCEDURE", Token_Class.Procedure);
            ReservedWords.Add("PROGRAM", Token_Class.Program);
            ReservedWords.Add("READ", Token_Class.Read);
            ReservedWords.Add("REAL", Token_Class.Real);
            ReservedWords.Add("SET", Token_Class.Set);
            ReservedWords.Add("THEN", Token_Class.Then);
            ReservedWords.Add("UNTIL", Token_Class.Until);
            ReservedWords.Add("WHILE", Token_Class.While);
            ReservedWords.Add("WRITE", Token_Class.Write);

            //Added ReservedWords
            ReservedWords.Add("FLOAT", Token_Class.Float);
            ReservedWords.Add("STRING", Token_Class.String);
            ReservedWords.Add("INT", Token_Class.Integer);
            ReservedWords.Add("MAIN", Token_Class.main);
            ReservedWords.Add("RETURN", Token_Class.Return);
            ReservedWords.Add("REPEAT", Token_Class.Repeat);
            ReservedWords.Add("ELSEIF", Token_Class.ElseIf);
            ReservedWords.Add("ENDL", Token_Class.endl);
            ReservedWords.Add("COMMENT", Token_Class.Comment);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            //Added operators
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add(":=", Token_Class.AssignOp);
            Operators.Add("{", Token_Class.LeftBrace);
            Operators.Add("}", Token_Class.RightBrace);



        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (SourceCode[j] >= 'A' && SourceCode[j] <= 'z') //Identifier lexeme
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        if (SourceCode[j] >= '0' && SourceCode[j] <= '9' || SourceCode[j] >= 'A' && SourceCode[j] <= 'z')
                        {
                            CurrentLexeme += SourceCode[j].ToString();
                        }
                        else { break; }
                        j++;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                    continue;
                }
                else if (SourceCode[j] >= '0' && SourceCode[j] <= '9')
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        if (SourceCode[j] >= '0' && SourceCode[j] <= '9' || SourceCode[j] == '.')
                            CurrentLexeme += SourceCode[j].ToString();
                        else
                        { break; }
                        j++;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                    continue;
                }
                else if (CurrentChar == '"') //String literal lexeme
                {
                    bool closedquote = false;
                    j++;
                    try
                    {
                        while (j < SourceCode.Length)
                        {
                            CurrentLexeme += SourceCode[j].ToString();
                            j++;
                            if (j == SourceCode.Length && SourceCode[j - 1] == '"' && SourceCode[j - 2] == '"')
                            {
                                closedquote = true;
                                break;
                            }
                            if (SourceCode[j] == '"')
                            {

                                if (SourceCode[j - 1] != '\\')
                                {
                                    CurrentLexeme += SourceCode[j].ToString();
                                    closedquote = true;
                                    break;
                                }
                            }
                        }

                    }
                    catch (IndexOutOfRangeException)
                    {
                        Errors.Error_List.Add("error");
                        i = j;
                        continue;
                    }
                    if (!closedquote)
                    {
                        Errors.Error_List.Add("Unclosed quote");
                        i = j;
                        continue;
                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                        i = j;
                        continue;
                    }
                }
                else if (CurrentChar == '/') // Comment lexeme
                {
                    bool closedcomment = false;
                    j++;
                    if (j < SourceCode.Length && SourceCode[j] == '*')
                    {
                        CurrentLexeme += SourceCode[j].ToString();
                        j++;
                        try
                        {
                            while (j < SourceCode.Length)
                            {
                                CurrentLexeme += SourceCode[j].ToString();
                                j++;
                                if (SourceCode[j - 1] == '*' && SourceCode[j] == '/')
                                {
                                    CurrentLexeme += SourceCode[j].ToString();
                                    closedcomment = true;
                                    i = j;
                                    break;
                                }
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Errors.Error_List.Add("Unclosed comment");
                            i = j;
                            continue;
                        }
                        if (!closedcomment)
                        {
                            Errors.Error_List.Add("Unclosed comment");
                            i = j;
                            continue;
                        }

                        Token Tok = new Token
                        {
                            lex = CurrentLexeme,
                            token_type = Token_Class.Comment
                        };
                        Tokens.Add(Tok);
                        continue;
                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                        continue;
                    }
                }

                else if (SourceCode[j] == '&')
                {
                    j++;
                    if (j < SourceCode.Length && SourceCode[j] == '&')
                    {
                        CurrentLexeme += SourceCode[j].ToString();
                        j++;
                        i = j;
                        i--;
                    }
                    FindTokenClass(CurrentLexeme);
                    continue;
                }
                else if (SourceCode[j] == '|')
                {
                    j++;
                    if (j < SourceCode.Length && SourceCode[j] == '|')
                    {
                        CurrentLexeme += SourceCode[j].ToString();
                        j++;
                        i = j;
                        i--;
                    }
                    FindTokenClass(CurrentLexeme);
                    continue;
                }
                else if (SourceCode[j] == '<')
                {
                    j++;
                    if (j < SourceCode.Length && SourceCode[j] == '=' || SourceCode[j] == '>')
                    {
                        CurrentLexeme += SourceCode[j].ToString();
                        j++;
                        i = j;
                        i--;
                    }

                    FindTokenClass(CurrentLexeme);
                    continue;
                }
                else if (SourceCode[j] == '>')
                {
                    j++;
                    if (j < SourceCode.Length && SourceCode[j] == '=')
                    {
                        CurrentLexeme += SourceCode[j].ToString();
                        j++;
                        i = j;
                        i--;
                    }

                    FindTokenClass(CurrentLexeme);
                    continue;
                }
                else if (SourceCode[j] == ':')
                {
                    j++;
                    if (j < SourceCode.Length && SourceCode[j] == '=')
                    {
                        CurrentLexeme += SourceCode[j].ToString();
                        j++;
                        i = j;
                        i--;
                    }

                    FindTokenClass(CurrentLexeme);
                    continue;
                }
                else
                {
                    FindTokenClass(CurrentLexeme);
                }
            }

            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Tok.lex.ToUpper()))
            {
                TC = ReservedWords[Tok.lex.ToUpper()];
                Tok.token_type = TC;
                Tokens.Add(Tok);
                return;
            }


            //Is it an identifier?
            if (isIdentifier(Lex))
            {
                TC = Token_Class.Idenifier;
                Tok.token_type = TC;
                Tokens.Add(Tok);
                return;
            }
            //Is it a Constant?
            if (isConstant(Lex))
            {
                TC = Token_Class.Constant;
                Tok.token_type = TC;
                Tokens.Add(Tok);
                return;
            }
            //Is it an operator?
            if (Operators.ContainsKey(Tok.lex))
            {
                TC = Operators[Tok.lex];
                Tok.token_type = TC;
                Tokens.Add(Tok);
                return;
            }
            //Is it a string?
            if (isString(Lex))
            {
                TC = Token_Class.StringLiteral;
                Tok.token_type = TC;
                Tokens.Add(Tok);
                return;
            }
            //Is it an undefined?
            if (Lex.Trim().Length == 0)
            {
                return;
            }
            Errors.Error_List.Add("Undefined lexeme: " + Lex);
        }



        bool isIdentifier(string lex)
        {
            // Check if the lex is an identifier or not.
            Regex re = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$");
            if (re.IsMatch(lex))
                return true;
            else
                return false;
        }
        bool isConstant(string lex)
        {
            // Check if the lex is a constant (Number) or not.
            Regex re = new Regex(@"^[0-9]+(\.[0-9]+)?$");
            if (re.IsMatch(lex))
                return true;
            else
                return false;
        }
        bool isString(string lex)
        {

            Regex re = new Regex(@"^""[a-zA-Z0-9!@#$%[ \]{}()=<>?&_\- \\ \\"" /|]*""$");
            if (re.IsMatch(lex.Trim()))
                return true;
            else
                return false;
        }
    }
}
