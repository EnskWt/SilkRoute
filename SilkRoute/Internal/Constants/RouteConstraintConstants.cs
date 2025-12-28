namespace SilkRoute.Internal.Constants;

internal static class RouteConstraintConstants
{
    public const string Int = "int";
    public const string Bool = "bool";
    public const string DateTime = "datetime";
    public const string Decimal = "decimal";
    public const string Double = "double";
    public const string Float = "float";
    public const string Guid = "guid";
    public const string Long = "long";
    
    public const string MinLength = "minlength";
    public const string MaxLength = "maxlength";
    public const string Length = "length";
    public const string Min = "min";
    public const string Max = "max";
    public const string Range = "range";
    public const string Alpha = "alpha";
    public const string Regex = "regex";
    public const string Required = "required";
    
    public const string NonFile = "nonfile";
    public const string File = "file";

    public static readonly IReadOnlySet<string> TypeConstraints =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Int, Bool, DateTime, Decimal, Double, Float, Guid, Long
        };

    public static readonly IReadOnlyDictionary<Type, string> TypeToConstraint =
        new Dictionary<Type, string>
        {
            [typeof(bool)] = Bool,
            [typeof(Guid)] = Guid,
            [typeof(DateTime)] = DateTime,
            [typeof(DateTimeOffset)] = DateTime,
            [typeof(decimal)] = Decimal,
            [typeof(double)] = Double,
            [typeof(float)] = Float,

            [typeof(long)] = Long,
            [typeof(ulong)] = Long,
            
            [typeof(int)] = Int,
            [typeof(uint)] = Int,
            [typeof(short)] = Int,
            [typeof(ushort)] = Int,
            [typeof(byte)] = Int,
            [typeof(sbyte)] = Int,
        };
}