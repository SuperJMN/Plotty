using Superpower.Display;

namespace Plotty.Parser
{
    public enum AsmToken
    {
        [Token(Category = "keyword", Example = "MOVE")]
        Move,
        [Token(Category = "keyword", Example = "LOAD")]
        Load,
        [Token(Category = "keyword", Example = "STORE")]
        Store,
        [Token(Category = "prefix", Example = "R1", Description = "Register")]
        Register,
        [Token(Category = "keyword", Example = "123")]
        Number,
        [Token(Category = "separator")]
        NewLine,
        [Token(Category = "separator", Example = ",")]
        Comma,
        [Token(Category = "separator", Example = " ")]
        Whitespace,
        [Token(Category = "keyword", Example = "ADD")]
        Add,
        [Token(Category = "separator", Example = "#")]
        Hash,
        [Token(Category = "keyword", Example = "BRANCH")]
        Branch,
        [Token(Category = "text", Example = "abc")]
        Text,
        [Token(Category = "text", Example = ":")]
        Colon,
        [Token(Category = "keyword", Example = "HALT")]
        Halt,
        [Token(Category = "operator", Example = "+")]
        Plus
    }
}