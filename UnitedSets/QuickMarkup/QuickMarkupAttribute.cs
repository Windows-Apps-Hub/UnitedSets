namespace QuickMarkup.SourceGen;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Class)]
class QuickMarkupAttribute(string markup) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.
