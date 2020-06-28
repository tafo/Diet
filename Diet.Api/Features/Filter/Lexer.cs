using System.Diagnostics;
using System.Net;
using Diet.Api.Infrastructure.ExceptionHandling;

namespace Diet.Api.Features.Filter
{
    /// <summary>
    /// Lexical analysis
    /// See https://en.wikipedia.org/wiki/Lexical_analysis
    /// See https://en.wikibooks.org/wiki/Compiler_Construction/Lexical_analysis
    /// </summary>
    [DebuggerDisplay("Lexer ({_text} @ {_textIndex} [{Token}])")]
    public class Lexer
    {
        private readonly string _text;
        private readonly int _textLength;
        private int _textIndex;
        private char _currentChar;

        public ExpressionToken Token { get; set; }

        public Lexer(string text)
        {
            _text = text;
            _textLength = text.Length;
            Token = new ExpressionToken();
            SetTextIndexAndChar(0); // Move to first char. If we register Lexer as a singleton service. This will be necessary.
            NextToken();
        }

        private void SetTextIndexAndChar(int index)
        {
            _textIndex = index;
            _currentChar = _textIndex < _textLength ? _text[_textIndex] : '\0';
        }

        public ExpressionToken NextToken()
        {
            while (char.IsWhiteSpace(_currentChar))
            {
                NextCharacter();
            }

            var tokenCategory = ExpressionTokenCategory.None;
            var tokenIndex = _textIndex;
            switch (_currentChar)
            {
                case '(':
                    NextCharacter();
                    tokenCategory = ExpressionTokenCategory.OpenParenthesis;
                    break;
                case ')':
                    NextCharacter();
                    tokenCategory = ExpressionTokenCategory.CloseParenthesis;
                    break;
                case '-':
                    NextCharacter();
                    tokenCategory = ExpressionTokenCategory.Minus;
                    break;
                case '=':
                    NextCharacter();
                    tokenCategory = ExpressionTokenCategory.Equal;
                    break;
                case '\'':
                    var quote = _currentChar;
                    do
                    {
                        MoveToNext(quote); // Move to closing quote
                        if (_textIndex == _textLength)
                        {
                            Error("Closing quote is missing at {0}", _textIndex);
                        }
                        NextCharacter();
                    }
                    while (_currentChar == quote);
                    tokenCategory = ExpressionTokenCategory.StringLiteral;
                    break;
                default:
                    if (IsIdentifierStart(_currentChar))
                    {
                        do
                        {
                            NextCharacter();
                        }
                        while (IsIdentifierPart(_currentChar));
                        tokenCategory = ExpressionTokenCategory.Identifier;
                        break;
                    }
                    if (char.IsDigit(_currentChar))
                    {
                        tokenCategory = ExpressionTokenCategory.IntegerLiteral;
                        do
                        {
                            NextCharacter();
                        }
                        while (char.IsDigit(_currentChar));

                        if (_currentChar == '.')// Now, this should be a decimal number.
                        {
                            tokenCategory = ExpressionTokenCategory.DecimalLiteral;
                            NextCharacter();
                            ValidateDigit(); // User must set a fully qualified decimal
                            do
                            {
                                NextCharacter();
                            }
                            while (char.IsDigit(_currentChar));
                        }
                        break;
                    }
                    if (_textIndex == _textLength)
                    {
                        tokenCategory = ExpressionTokenCategory.End;
                        break;
                    }
                    Error("Syntax error at {0}", _textIndex);
                    break;
            }

            Token.Category = tokenCategory;
            Token.Text = _text.Substring(tokenIndex, _textIndex - tokenIndex);
            Token.Index = tokenIndex;

            CheckOperators();

            return Token;
        }

        private void NextCharacter()
        {
            if (_textIndex < _textLength)
            {
                _textIndex++;
            }
            _currentChar = _textIndex < _textLength ? _text[_textIndex] : '\0';
        }

        private void MoveToNext(char character)
        {
            NextCharacter();
            while (_textIndex < _textLength && _currentChar != character)
            {
                NextCharacter();
            }
        }

        /// <summary>
        /// This is used to simplify GetNext method
        /// </summary>
        private void CheckOperators()
        {
            switch (Token.Category)
            {
                case ExpressionTokenCategory.Identifier when Token.Text.ToLower() == "lt":
                    Token.Category = ExpressionTokenCategory.LessThan;
                    break;
                case ExpressionTokenCategory.Identifier when Token.Text.ToLower() == "le":
                    Token.Category = ExpressionTokenCategory.LessThanOrEqual;
                    break;
                case ExpressionTokenCategory.Identifier when Token.Text.ToLower() == "gt":
                    Token.Category = ExpressionTokenCategory.GreaterThan;
                    break;
                case ExpressionTokenCategory.Identifier when Token.Text.ToLower() == "ge":
                    Token.Category = ExpressionTokenCategory.GreaterThanOrEqual;
                    break;
                case ExpressionTokenCategory.Identifier when Token.Text.ToLower() == "eq":
                    Token.Category = ExpressionTokenCategory.Equal;
                    break;
                case ExpressionTokenCategory.Identifier when Token.Text.ToLower() == "ne":
                    Token.Category = ExpressionTokenCategory.NotEqual;
                    break;
                case ExpressionTokenCategory.Identifier when Token.Text.ToLower() == "and":
                    Token.Category = ExpressionTokenCategory.And;
                    break;
                case ExpressionTokenCategory.Identifier when Token.Text.ToLower() == "or":
                    Token.Category = ExpressionTokenCategory.Or;
                    break;
                case ExpressionTokenCategory.Identifier when Token.Text.ToLower() == "not":
                    Token.Category = ExpressionTokenCategory.Not;
                    break;
            }
        }

        private void ValidateDigit()
        {
            if (!char.IsDigit(_currentChar))
            {
                Error("There must be a digit at {0}", _textIndex);
            }
        }

        private bool IsIdentifierStart(char character)
        {
            return char.IsLetter(character);
        }

        private bool IsIdentifierPart(char character)
        {
            return IsIdentifierStart(character) || char.IsDigit(character) || character == '_';
        }

        private void Error(string message, int errorIndex)
        {
            throw new RestException(HttpStatusCode.Conflict, message, string.Format(message, errorIndex));
        }

        public void ValidateTokenCategory(ExpressionTokenCategory category)
        {
            if (Token.Category != category)
                Error("Syntax error at {0}", _textIndex);
        }
    }
}