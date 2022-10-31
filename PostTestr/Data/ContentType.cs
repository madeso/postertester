using System.Runtime.Serialization;
namespace PostTestr.Data;

public enum ContentType
{
    [EnumMember(Value = "json")]
    Json,

    [EnumMember(Value = "text")]
    Text,
}
