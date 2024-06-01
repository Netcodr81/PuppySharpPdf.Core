namespace PuppySharpPdf.Core.Common.Exceptions;

public sealed record ValidationError(string PropertyName, string ErrorMessage);
