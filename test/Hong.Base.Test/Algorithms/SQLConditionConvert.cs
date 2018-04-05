using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hong.DAO.QueryCache
{
    /*
	select a.name from t_products as a,t_productmaster as b where a.masterid=b.id;
	select a.name from t_products as a,t_productmaster as b where a.masterid=b.id and a.datetime between '2017/01/01' and '2017/12/31';
	select a.name from t_products as a join t_productmaster as b on a.masterid=b.id;
	select a.typeid,b.name from t_products as a join t_productmaster as b on a.masterid=b.id where a.datetime between '2017/01/01' and '2017/12/31';
    select * from t_products where `datetime` between '2017/01/01' and '2017/12/31';
    select * from t_orders where id in (select orderId from t_orderDetails where ProductId={0});


	where a.masterid=b.id
	on a.masterid=b.id
	where a.datetime between '2017/01/01' and '2017/12/31'
     */
    public class SQLConditionConvert
    {
        static Dictionary<string, Oper> Options = new Dictionary<string, Oper>()
        {
            { " ",Oper.空格 },
            { "=",Oper.等于 },
            { ">=",Oper.大于等于 },
            { "<=",Oper.小于等于 },
            { "!=",Oper.不等于 },
            { ">",Oper.大于 },
            { "<",Oper.小于 },
            { "(",Oper.左括号 },
            { ")",Oper.右括号 },

            { "and",Oper.并且 },
            { "or",Oper.或者 },
            { "between",Oper.区间 },
            { "in",Oper.子查询 },
            { "not",Oper.非 },
            { "like",Oper.模糊查询 },
            { "uninstall",Oper.级联查询 },

            { "as",Oper.别名},
            { ",",Oper.表分隔符}
        };

        static List<string> Keywords = new List<string>()
        {
            "select","from","where","join","left","right","drop","delete","update","drop","on"
        };

        static List<KeyInfo> _keys = new List<KeyInfo>();
        static SQLConditionConvert()
        {
            var g = Options.GetEnumerator();
            while (g.MoveNext())
            {
                var k = new KeyInfo
                {
                    Oper = g.Current.Value,
                    Name = g.Current.Key.ToLower(),
                    Upper = g.Current.Key.ToUpper().ToCharArray(),
                    Lower = g.Current.Key.ToLower().ToCharArray(),
                    Length = (short)g.Current.Key.Length,
                    MustSpace = Regex.IsMatch(g.Current.Key, "[a-zA-Z]")
                };

                _keys.Add(k);
            }

            foreach (var str in Keywords)
            {
                var k = new KeyInfo
                {
                    Oper = Oper.关键词,
                    Name = str.ToLower(),
                    Upper = str.ToUpper().ToCharArray(),
                    Lower = str.ToLower().ToCharArray(),
                    Length = (short)str.Length,
                    MustSpace = true
                };

                _keys.Add(k);
            }
        }

        public SQLConditionConvert(string sql)
        {
            SQL = sql;
            SQLLength = sql.Length;
        }

        public Dictionary<string, List<Expression>> GetTableCondition()
        {
            var result = new Dictionary<string, List<Expression>>();
            var aliaNames = new Dictionary<string, string>();
            var table = string.Empty;
            var type = 0; //1表,2条件
            var e = Split();
            while (e != null)
            {
                e = e.Sub;

                if (e.Expression.Left == "from")
                {
                    type = e.Sub.Expression.Left != "(" ? 1 : 0;
                }
                else if (e.Expression.Left == "where")
                {
                    type = 2;
                }
                else if (e.Expression.Oper == Oper.关键词)
                {
                    type = 0;
                }
                else if (e.Expression.Oper == Oper.别名)
                {
                    if (!aliaNames.ContainsKey(e.Sub.Expression.Left))
                    {
                        aliaNames.Add(e.Sub.Expression.Left, table);
                    }
                }
                else if (e.Expression.Oper == Oper.表分隔符)
                {
                    if (!result.ContainsKey(e.Sub.Expression.Left))
                    {
                        result.Add(e.Expression.Left, new List<Expression>());
                    }
                }
                else if (type == 1)
                {
                    if (!result.ContainsKey(e.Sub.Expression.Left))
                    {
                        result.Add(e.Expression.Left, new List<Expression>());
                    }
                }
                else if (type == 2)
                {
                    var op = e.Expression.Oper;
                    if (op != Oper.空格 && op != Oper.左括号 && op != Oper.右括号 &&
                        op != Oper.并且 && op != Oper.或者 && op != Oper.级联查询)
                    {
                        if (!(op == Oper.子查询 && e.Sub.Expression.Oper == Oper.左括号))
                        {
                            table = e.Expression.Left;
                            if (table.Contains("."))
                            {
                                aliaNames.TryGetValue(table.Split('.')[0], out table);
                            }

                            if (result.TryGetValue(table, out List<Expression> fieds))
                            {
                                fieds.Add(e.Expression);
                            }
                            else
                            {
                                result.Add(table, new List<Expression>() { e.Expression });
                            }
                        }
                    }
                }
            }

            return result;
        }

        public string SQL = string.Empty;
        int SQLLength = 0;

        /// <summary>
        /// SQL语句拆解
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Node Split()
        {
            int currentPosition = 0;
            var e = new Node();
            _Split(ref currentPosition, e);

            return e;
        }

        Node _Split(ref int currentPosition, Node node)
        {
            Oper oper = Oper.None;
            RemoveSpace(ref currentPosition);

            var str = new StringBuilder();
            Oper prevOper = Oper.None;
            var keyword = string.Empty;

            while (currentPosition < SQLLength)
            {
                prevOper = oper;
                oper = ComparToOper1(ref currentPosition, ref keyword);
                switch (oper)
                {
                    case Oper.关键词:
                        node = node.Sub = new Node()
                        {
                            Expression = new Expression
                            {
                                Left = keyword,
                                Oper = oper
                            }
                        };
                        RemoveSpace(ref currentPosition);
                        break;

                    case Oper.None:
                        str.Append(SQL[currentPosition++]);
                        continue;

                    case Oper.空格:
                        node = node.Sub = new Node()
                        {
                            Expression = new Expression
                            {
                                Left = str.ToString(),
                                Oper = oper
                            }
                        };
                        str.Clear();
                        RemoveSpace(ref currentPosition);
                        break;

                    case Oper.左括号:
                        node.Sub = new Node()
                        {
                            Expression = new Expression
                            {
                                Oper = oper
                            }
                        };

                        if (node.Expression.Oper != Oper.子查询)
                        {
                            //函数
                            node.Expression.Right = GetUntilSpace(ref currentPosition);
                            break;
                        }

                        node = _Split(ref currentPosition, node.Sub);
                        break;

                    case Oper.右括号:
                        node = node.Sub = new Node()
                        {
                            Expression = new Expression
                            {
                                Oper = oper
                            }
                        };
                        RemoveSpace(ref currentPosition);
                        return node;

                    case Oper.非:
                        RemoveSpace(ref currentPosition);
                        node = node.Sub = new Node()
                        {
                            Expression = new Expression
                            {
                                Oper = oper,
                                Right = GetUntilSpace(ref currentPosition)
                            }
                        };
                        break;

                    case Oper.并且:
                    case Oper.或者:
                        node = node.Sub = new Node()
                        {
                            Expression = new Expression
                            {
                                Oper = oper,
                            }
                        };
                        break;

                    case Oper.子查询:
                        if (prevOper != Oper.空格)
                        {
                            throw new Exception("语法错误");
                        }

                        node.Expression.Oper = oper;
                        RemoveSpace(ref currentPosition);
                        node = _Split(ref currentPosition, node);
                        break;

                    case Oper.级联查询:
                        node.Sub = new Node()
                        {
                            Expression = new Expression
                            {
                                Oper = oper,
                            }
                        };
                        RemoveSpace(ref currentPosition);
                        node = _Split(ref currentPosition, node.Sub);
                        break;

                    case Oper.模糊查询:
                        RemoveSpace(ref currentPosition);
                        node.Expression.Oper = oper;
                        node.Expression.Right = GetUntilSpace(ref currentPosition);
                        break;

                    default:
                        if (!string.IsNullOrEmpty(node.Expression.Right))
                        {
                            throw new Exception("语法错误");
                        }

                        if (prevOper == Oper.None)
                        {
                            node.Sub = new Node()
                            {
                                Expression = new Expression
                                {
                                    Left = str.ToString()
                                }
                            };
                            str.Clear();
                            node = node.Sub;
                        }

                        node.Expression.Oper = oper;
                        RemoveSpace(ref currentPosition);
                        node.Expression.Right = GetUntilSpace(ref currentPosition);
                        RemoveSpace(ref currentPosition);
                        break;
                }
            }

            if (oper == Oper.None)
            {
                node.Sub = new Node()
                {
                    Expression = new Expression
                    {
                        Left = str.ToString(),
                        Oper = Oper.是
                    }
                };
                node = node.Sub;
            }

            str = null;

            return node;
        }

        void RemoveSpace(ref int currentPosition)
        {
            while (currentPosition < SQLLength && SQL[currentPosition] == ' ') currentPosition++;
        }

        string GetUntilSpace(ref int currentPosition)
        {
            var str = new StringBuilder();

            while (currentPosition < SQLLength && SQL[currentPosition] != ' ' && SQL[currentPosition] != ')')
                str.Append(SQL[currentPosition++]);

            return str.ToString();
        }

        Oper ComparToOper1(ref int currentPosition, ref string keyword)
        {
            var c = SQL[currentPosition];
            var i = 0;

            foreach (var key in _keys)
            {
                if (key.Lower[0] != c && key.Upper[0] != c)
                {
                    continue;
                }

                i = 1;
                for (; i < key.Length && currentPosition + i < SQLLength; i++)
                {
                    if (key.Lower[i] != SQL[currentPosition + i] && key.Upper[i] != SQL[currentPosition + i])
                    {
                        break;
                    }
                }

                if (i == key.Length)
                {
                    if (key.MustSpace)
                    {
                        if (SQL[currentPosition + i] == ' ')
                        {
                            keyword = key.Name;
                            currentPosition = currentPosition + i;
                            return key.Oper;
                        }
                    }
                    else
                    {
                        keyword = key.Name;
                        currentPosition += key.Length;
                        return key.Oper;
                    }
                }
            }

            return Oper.None;
        }

        Oper ComparToOper(ref int currentPosition)
        {
            short i = 1;

            switch (SQL[currentPosition])
            {
                case '=':
                    currentPosition++;
                    return Oper.等于;

                case '>':
                    if (SQL[currentPosition + 1] == '=')
                    {
                        currentPosition++;
                        return Oper.大于等于;
                    }

                    return Oper.大于;
                case '<':
                    if (SQL[currentPosition + 1] == '=')
                    {
                        currentPosition++;
                        return Oper.小于等于;
                    }

                    return Oper.小于;

                case '!':
                    if (SQL[currentPosition + 1] == '=')
                    {
                        currentPosition++;
                        return Oper.不等于;
                    }
                    break;

                case ' ':
                    currentPosition++;
                    return Oper.空格;

                case 'a':
                case 'A':
                    for (; i < 3 && currentPosition + i < SQLLength; i++)
                    {
                        if ("and"[i] != SQL[currentPosition + i] && "AND"[i] != SQL[currentPosition + i])
                        {
                            break;
                        }
                    }

                    if (i == 3 && SQL[currentPosition + i] == ' ')
                    {
                        currentPosition = currentPosition + i;
                        return Oper.并且;
                    }
                    break;

                case 'o':
                case 'O':
                    for (; i < 2 && currentPosition + i < SQLLength; i++)
                    {
                        if ("or"[i] != SQL[currentPosition + i] && "OR"[i] != SQL[currentPosition + i])
                        {
                            break;
                        }
                    }

                    if (i == 2 && SQL[currentPosition + i] == ' ')
                    {
                        currentPosition = currentPosition + i;
                        return Oper.或者;
                    }
                    break;

                case 'l':
                case 'L':
                    for (; i < 4 && currentPosition + i < SQLLength; i++)
                    {
                        if ("like"[i] != SQL[currentPosition + i] && "LIKE"[i] != SQL[currentPosition + i])
                        {
                            break;
                        }
                    }

                    if (i == 4 && SQL[currentPosition + i] == ' ')
                    {
                        currentPosition = currentPosition + i;
                        return Oper.模糊查询;
                    }
                    break;

                case 'n':
                case 'N':
                    for (; i < 3 && currentPosition + i < SQLLength; i++)
                    {
                        if ("not"[i] != SQL[currentPosition + i] && "NOT"[i] != SQL[currentPosition + i])
                        {
                            break;
                        }
                    }

                    if (i == 3 && SQL[currentPosition + i] == ' ')
                    {
                        currentPosition = currentPosition + i;
                        return Oper.非;
                    }
                    break;

                case 'i':
                case 'I':
                    var c = SQL[currentPosition + 1];
                    if ((c == 'n' || c == 'N') && SQL[currentPosition + 2] == ' ')
                    {
                        currentPosition = currentPosition + 2;
                        return Oper.子查询;
                    }
                    break;

                case '(':
                    currentPosition++;
                    return Oper.左括号;

                case ')':
                    currentPosition++;
                    return Oper.右括号;

                case 'u':
                case 'U':
                    for (; i < 9 && currentPosition + i < SQLLength; i++)
                    {
                        if ("uninstall"[i] != SQL[currentPosition + i] && "UNINSTALL"[i] != SQL[currentPosition + i])
                        {
                            break;
                        }
                    }

                    if (i == 9 && SQL[currentPosition + i] == ' ')
                    {
                        currentPosition = currentPosition + i;
                        return Oper.级联查询;
                    }
                    break;
            }

            return Oper.None;
        }

        public class Expression
        {
            public string Left;
            public string Right;

            public Oper Oper;
        }

        public class Node
        {
            public Expression Expression;
            public Node Parent;
            public Node Sub;
        }

        public enum Oper : short
        {
            None, 大于, 小于, 等于, 大于等于, 小于等于, 不等于, 模糊查询,
            区间, 非, 函数, 空格, 并且, 或者, 左括号, 右括号, 子查询, 级联查询, 是, 关键词, 字段,
            表分隔符, 别名
        }

        /// <summary>
        /// 关键词信息
        /// </summary>
        public class KeyInfo
        {
            public Oper Oper;
            public string Name;
            public char[] Upper;
            public char[] Lower;
            public short Length;
            public bool MustSpace;
        }
    }
}
