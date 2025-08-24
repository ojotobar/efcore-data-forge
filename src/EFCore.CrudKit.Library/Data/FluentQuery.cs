using EFCore.CrudKit.Library.Extensions;
using System.Text;

namespace EFCore.CrudKit.Library.Data
{
    public class FluentQuery
    {
        private readonly StringBuilder _query = new();

        public FluentQuery(string query)
        {
            _query.Append(query);
        }

        public FluentQuery() { }

        public FluentQuery Select(string columns)
        {
            _query.Append($"SELECT ").Append(columns);
            return this;
        }

        public FluentQuery Select(bool isQuoted = false, params string[] columns)
        {
            if (isQuoted)
            {
                _query.Append($"SELECT ")
                    .Append(columns.ToArray().ToQuotedCsv());
            }
            else
            {
                _query.Append($"SELECT ")
                    .Append(columns.ToArray().ToCsv());
            }
            return this;
        }

        public FluentQuery From(string table)
        {
            _query.Append($" FROM ")
                .Append(table);
            return this;
        }

        public FluentQuery And(string statement)
        {
            _query.Append($" AND ")
                .Append(statement);
            return this;
        }

        public FluentQuery AndIn(string column, List<Guid> guids)
        {
            if (guids != null && guids.Count > 0)
            {
                _query.Append(" AND ");
                this.In(column, guids);
            }
            return this;
        }

        public FluentQuery AndNotIn(string column, List<Guid> guids)
        {
            if (guids != null && guids.Count > 0)
            {
                _query.Append(" AND ");
                this.NotIn(column, guids);
            }
            return this;
        }

        public FluentQuery AndIn(string column, List<object> values)
        {
            if (values != null && values.Count > 0)
            {
                _query.Append(" AND ");
                this.In(column, values);
            }
            return this;
        }

        public FluentQuery AndNotIn(string column, List<object> values)
        {
            if (values != null && values.Count > 0)
            {
                _query.Append(" AND ");
                this.NotIn(column, values);
            }
            return this;
        }

        public FluentQuery Or(string statement)
        {
            _query.Append($" OR ")
                .Append(statement);
            return this;
        }

        public FluentQuery Where(string statement)
        {
            _query.Append($" WHERE ")
                .Append(statement);
            return this;
        }

        public FluentQuery WhereIn(string column, List<Guid> guids)
        {
            if (guids != null && guids.Count > 0)
            {
                _query.Append(" WHERE ");
                this.In(column, guids);
            }
            return this;
        }

        public FluentQuery WhereNotIn(string column, List<Guid> guids)
        {
            if (guids != null && guids.Count > 0)
            {
                _query.Append(" WHERE ");
                this.In(column, guids);
            }
            return this;
        }

        public FluentQuery WhereIn(string column, List<object> values)
        {
            if (values != null && values.Count > 0)
            {
                _query.Append(" WHERE ");
                this.In(column, values);
            }
            return this;
        }

        public FluentQuery WhereNotIn(string column, List<object> values)
        {
            if (values != null && values.Count > 0)
            {
                _query.Append(" WHERE ");
                this.In(column, values);
            }
            return this;
        }

        public FluentQuery Join(string statement)
        {
            _query.Append($" JOIN ").Append(statement);
            return this;
        }

        public FluentQuery RightJoin(string statement)
        {
            _query.Append($" RIGHT JOIN ").Append(statement);
            return this;
        }

        public FluentQuery LeftJoin(string statement)
        {
            _query.Append($" LEFT JOIN ").Append(statement);
            return this;
        }

        public FluentQuery On(string statement)
        {
            _query.Append($" ON ").Append(statement);
            return this;
        }

        public FluentQuery OrderBy(string column, bool ascending)
        {
            var direction = ascending ? " ASC " : " DESC ";
            _query.Append(" ORDER BY ").Append(column).Append(direction);
            return this;
        }

        public FluentQuery GroupBy(string column)
        {
            _query.Append(" GROUP BY ").Append(column);
            return this;
        }

        public FluentQuery Limit(int skip, int take)
        {
            _query.Append(" LIMIT ").Append(skip).Append(',').Append(take);
            return this;
        }

        public FluentQuery Between(string start, string end)
        {
            _query.Append(" BETWEEN ").Append(start).Append(" AND ").Append(end);
            return this;
        }

        public FluentQuery Columns(params string[] columns)
        {
            _query.Append('(').Append(columns.ToQuotedCsv()).Append(")");
            return this;
        }

        public FluentQuery Values(params string[] columns)
        {
            _query.Append("VALUES").Append('(').Append(columns.ToCsv()).Append(")");
            return this;
        }

        public string ToQuery() =>
            _query.ToString();

        private FluentQuery In(string column, List<Guid> guids)
        {
            if (guids.Count == 1)
            {
                _query.Append(column).Append(" = ")
                    .Append('\'').Append(guids.First()).Append('\'');
            }
            else
            {
                _query.Append($" AND ").Append(column).Append(" IN ")
                    .Append('(').Append(guids.ToQuotedCsv()).Append(')');
            }
            return this;
        }

        private FluentQuery In(string column, List<object> values)
        {
            if (values.Count == 1)
            {
                _query.Append(column).Append(" = ")
                    .Append('\'').Append(values.First()).Append('\'');
            }
            else
            {
                _query.Append(column).Append(" IN ")
                    .Append('(').Append(values.ToQuotedCsv()).Append(')');
            }
            return this;
        }

        private FluentQuery NotIn(string column, List<Guid> guids)
        {
            if (guids.Count == 1)
            {
                _query.Append(column).Append(" = ")
                    .Append('\'').Append(guids.First()).Append('\'');
            }
            else
            {
                _query.Append(column).Append(" IN ")
                    .Append('(').Append(guids.ToQuotedCsv()).Append(')');
            }
            return this;
        }

        private FluentQuery NotIn<T>(string column, List<T> values)
        {
            if (values.Count == 1)
            {
                _query.Append(column).Append(" = ")
                    .Append('\'').Append(values.First()).Append('\'');
            }
            else
            {
                _query.Append(column).Append(" IN ")
                    .Append('(').Append(values.ToQuotedCsv<T>()).Append(')');
            }
            return this;
        }
    }
}
