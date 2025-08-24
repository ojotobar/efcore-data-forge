namespace EFCore.CrudKit.Library.Extensions
{
    public static class StringExtension
    {
        public static string ToQuotedCsv(this string[] values)
        {
            return string.Join(", ", values.Select(v => $"'{v}'"));
        }

        public static string ToCsv(this string[] values)
        {
            return string.Join(", ", values.Select(v => $"{v}"));
        }

        public static string ToQuotedCsv(this List<Guid> values)
        {
            return string.Join(", ", values.Select(v => $"'{v}'"));
        }

        public static string ToCsv(this List<Guid> values)
        {
            return string.Join(", ", values.Select(v => $"{v}"));
        }

        public static string ToCsv(this List<object> values)
        {
            return string.Join(", ", values.Select(v => $"{v}"));
        }

        public static string ToQuotedCsv<T>(this List<T> values)
        {
            return string.Join(" ,", values.Select(v => $"'{v}'"));
        }
    }
}