using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using MultiTableRepository.Attributes;

namespace MultiTableRepository.DemoApp
{
    class Program
    {
        #region data

        private static readonly string _connectionString = "Data Source=localhost;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private static readonly List<string> Countries = new List<string> { "BE", "FR", "DE" };
        private static readonly List<string> Commodities = new List<string> { "GAS", "POWER" };
        private static readonly List<string> Portfolios = new List<string> { "B2B", "B2C", "GE" };
        private static IDbConnection _connection;

        protected static IDbConnection Connection
        {
            get
            {
                if (_connection != null && _connection.State == ConnectionState.Open)
                {
                    return _connection;
                }

                _connection = SqlClientFactory.Instance.CreateConnection();
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                if (_connection.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection should be open!");
                }

                return _connection;
            }
        }

        private static readonly Random rnd = new Random();

        #endregion

        enum Prop
        {
            GasType,
            Waffle,
            Datetime,
            Volume,
            Unit,
            Id,
            Name,
            Text,
            Country,
            Commodity,
            Portfolio,
        }

        static void Main(string[] args)
        {

            var exProps = new Dictionary<Prop, IEnumerable<string[]>>
            {
                {
                    Prop.Waffle,  new List<string[]>
                    {
                        new[] { "BE", null, "B2B" },
                        new[] { "FR" },
                    }
                },
                {
                    Prop.GasType, new List<string[]>
                    {
                        new[] { null, "GAS" },
                    }
                },
            };

            var igProps = new Dictionary<Prop, IEnumerable<string[]>>
            {
                {
                    Prop.Volume, new List<string[]>
                    {
                        new[]{ "FR", "POWER" },
                    }
                },
                {
                    Prop.Unit, new List<string[]>
                    {
                        new[] { null, "GAS", "GE" },
                        new[] { "DE" },
                    }
                },
            };

            List<string[]> segs = new List<string[]>();
            foreach (var ct in Countries)
            {
                foreach (var co in Commodities)
                {
                    foreach (var p in Portfolios)
                    {
                        segs.Add(new string[] { ct, co, p });
                    }
                }
            }


            var WILDCARD = "-";
            Action<ConsoleColor, Dictionary<Prop, IEnumerable<string[]>>> printAttr = (color, values) =>
            {
                Console.ForegroundColor = color;
                foreach (var ps in values)
                {
                    foreach (var p in ps.Value)
                    {
                        Console.Write(ps.Key.ToString().PadRight(8));
                        Console.Write(" : ");
                        Console.Write("| ");
                        Console.Write(string.Join(" | ", p.Select(s => (string.IsNullOrEmpty(s) ? WILDCARD : s).PadRight(5))));
                        Console.WriteLine(" |");
                    }
                }
                Console.WriteLine();
            };


            Console.WriteLine();
            printAttr(ConsoleColor.Yellow, igProps);
            printAttr(ConsoleColor.Green, exProps);







            Console.WriteLine();
            foreach (var seg in segs)
            {
                var allProps = Enum.GetValues(typeof(Prop)).Cast<Prop>().ToList();

                Console.ForegroundColor = ConsoleColor.Cyan;
                //var sfx = string.Join("_", seg).PadRight(15);
                //Console.Write(sfx);
                Console.Write(seg[0].PadRight(4));
                Console.Write(seg[1].PadRight(7));
                Console.Write(seg[2].PadRight(6));






                var removed = new List<Prop>();

                foreach (Prop prop in allProps)
                {
                    if (igProps.TryGetValue(prop, out var igSegList))
                    {
                        var skipAll = false;

                        foreach (var igSegs in igSegList)
                        {
                            var skip = true;
                            for (int i = 0; i < seg.Length; i++)
                            {
                                if (igSegs.Length <= i)
                                {
                                    break;
                                }

                                if (igSegs[i] == null)
                                {
                                    continue;
                                }

                                skip = skip && (igSegs[i] == seg[i]);
                            }

                            skipAll = skipAll || skip;
                        }

                        if (skipAll)
                            //allProps.Remove(prop);
                            removed.Add(prop);
                    }


                    if (exProps.TryGetValue(prop, out var exSegList))
                    {
                        var keepAll = false;

                        foreach (var exSegs in exSegList)
                        {
                            var keep = true;

                            for (int i = 0; i < seg.Length; i++)
                            {
                                if (exSegs.Length <= i)
                                {
                                    break;
                                }

                                if (exSegs[i] == null)
                                {
                                    continue;
                                }

                                keep = keep && (exSegs[i] == seg[i]);
                            }

                            keepAll = keepAll || keep;
                        }

                        if (!keepAll)
                            //allProps.Remove(prop);
                            removed.Add(prop);
                    }
                }



                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("| ");
                foreach (var p in allProps)
                {
                    Console.ForegroundColor = removed.Contains(p) ? ConsoleColor.Red : ConsoleColor.Gray;
                    Console.Write(p);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(" | ");
                }
                Console.WriteLine();
                Console.WriteLine();









            }













            return;

            ////
            //// THE TABLES
            ////

            //CreateTables();
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine();


            //var model = CreateRandomModel();

            ////
            //// THE CACHE
            ////

            //var tableInfo = MultiTableParser.GetTableInfo(model);


            ////
            //// THE FLUENT SQL SYNTAX
            ////

            //var fluent = FluentSql.CreateFluentSql<MyAwesomeModel>(null, tableInfo);

            //var sql = fluent.Select()
            //                .Alias("X")
            //                .Top(1000)
            //                .WithNoLock()
            //                .AddParam("P1", 12.45)
            //                .Only(25)
            //                .Join("Table2 T2", "X.Id = T2.OtherId")
            //                .Where("X.Value <= @P1")
            //                .Where("T2.DateTime <= GETUTCTIME()")
            //                .Paging(1, 20, "B", "C DESC")
            //                .GetSql();

            //Console.WriteLine(sql);

            //Console.WriteLine();
            //Console.WriteLine();



            ////
            //// THE REPOSITORY
            ////

            //var repo = new MultiRepository<MyAwesomeModel>(_connectionString);
            //IEnumerable<MyAwesomeModel> results;


            //// Get by key
            //model = repo.Get(7, "BE", "GAS", "B2B");

            //model = repo.Get(5, "FR", "POWER", "GE");


            //// Get All
            //var segments = GetRandomSegements();
            //results = repo.GetAll(segments);



            //// Insert One
            //repo.Add(model);


            //// Insert many
            //var randomEntities = new List<MyAwesomeModel>();

            //for (int i = 0; i < 3; i++)
            //{
            //    randomEntities.Add(CreateRandomModel($"ENTITY {i}"));
            //}

            //repo.Add(randomEntities);



            //// Update
            //model = CreateRandomModel();
            //model.Id = 4;
            //repo.Update(model);




            //// Delete
            //model = CreateRandomModel();
            //model.Id = 2;
            //repo.Remove(model);



            //Console.WriteLine(model);
            //Console.WriteLine(results);
            //Console.WriteLine();
        }

        private static string[] GetRandomSegements()
        {
            return new string[]
            {
                Countries[rnd.Next(0, Countries.Count)],
                Commodities[rnd.Next(0, Commodities.Count)],
                Portfolios[rnd.Next(0, Portfolios.Count)]
            };
        }

        private static MyAwesomeModel CreateRandomModel(string someText = null)
        {
            return new MyAwesomeModel
            {
                Country = Countries[rnd.Next(0, Countries.Count)],
                Commodity = Commodities[rnd.Next(0, Commodities.Count)],
                Portfolio = Portfolios[rnd.Next(0, Portfolios.Count)],
                CreatedOn = DateTime.Now,
                GasType = rnd.NextDouble() <= 0.5 ? "L" : "H",
                SomeText = string.IsNullOrEmpty(someText) ? $"RND-{rnd.Next()}" : someText,
                Value = rnd.NextDouble() * 3
            };
        }

        private static void CreateTables()
        {
            using var conn = SqlClientFactory.Instance.CreateConnection();

            conn.ConnectionString = _connectionString;
            conn.Open();

            var random = new Random(2543);

            var baseName = typeof(MyAwesomeModel).GetCustomAttribute<MultiTableAttribute>().TableNamePrefix;

            foreach (var country in Countries)
            {
                foreach (var commodity in Commodities)
                {
                    foreach (var portfolio in Portfolios)
                    {
                        var parameters = new DynamicParameters();
                        var sql = CreateTableSql<MyAwesomeModel>(baseName, parameters, country, commodity, portfolio);

                        Console.WriteLine(sql.drop);
                        Console.WriteLine();
                        conn.Execute(sql.drop);

                        Console.WriteLine(sql.create);
                        Console.WriteLine();
                        conn.Execute(sql.create);



                        for (int i = 0; i < 10; i++)
                        {
                            var dummy = new MyAwesomeModel
                            {
                                Country = country,
                                Commodity = commodity,
                                Portfolio = portfolio,
                                CreatedOn = DateTime.Now.AddSeconds(random.Next(0, 100000)),
                                GasType = random.NextDouble() < 0.5 ? "L" : "H",
                                Value = random.NextDouble() * 10,
                                SomeText = Guid.NewGuid().ToString()
                            };

                            conn.Execute(sql.insert, dummy);
                        }
                    }
                }
            }
        }

        static (string create, string insert, string drop) CreateTableSql<T>(string baseName, DynamicParameters p, string country = null, string commodity = null, string portfolio = null)
        {
            var tableName = $"{baseName}" +
                            (string.IsNullOrEmpty(country) ? "" : $"_{country}") +
                            (string.IsNullOrEmpty(commodity) ? "" : $"_{commodity}") +
                            (string.IsNullOrEmpty(portfolio) ? "" : $"_{portfolio}");

            var sql = new StringBuilder();
            var props = typeof(T).GetProperties();

            var drop = $"IF OBJECT_ID('dbo.{tableName}') IS NOT NULL DROP TABLE dbo.{tableName};";
            var cols = new List<string>();
            var ins = $"INSERT INTO {tableName} ("; // + string.Join(",", cols) + ") VALUES (@" + string.Join(", @", cols) + ")" ;

            sql.AppendLine($"CREATE TABLE dbo.{tableName} (");
            foreach (var prop in props)
            {
                if (prop.Name == nameof(MyAwesomeModel.GasType) && commodity != "GAS") //TODO: Read attribute instead
                {
                    continue;
                }


               sql.Append("    ").Append(prop.Name);


                switch (prop.Name)
                {
                    case nameof(MyAwesomeModel.Country):
                        sql.Append($" AS '{country}'");
                        break;

                    case nameof(MyAwesomeModel.Commodity):
                        sql.Append($" AS'{commodity}'");
                        break;

                    case nameof(MyAwesomeModel.Portfolio):
                        sql.Append($" AS '{portfolio}'");
                        break;

                    default:


                        {
                            var colType = prop.PropertyType;
                            if (colType == typeof(int))
                            {
                                sql.Append(" INT");
                                p.Add(prop.Name, dbType: DbType.Int32);
                            }
                            else if (colType == typeof(long))
                            {
                                sql.Append(" BIGINT");
                                p.Add(prop.Name, dbType: DbType.Int64);
                            }
                            else if (colType == typeof(string))
                            {
                                sql.Append(" VARCHAR(MAX)");
                                p.Add(prop.Name, dbType: DbType.String);
                            }
                            else if (colType == typeof(DateTime))
                            {
                                sql.Append(" DATETIME");
                                p.Add(prop.Name, dbType: DbType.DateTime);
                            }
                            else if (colType == typeof(double))
                            {
                                sql.Append(" FLOAT");
                                p.Add(prop.Name, dbType: DbType.Double);
                            }

                            if (prop.GetCustomAttribute<KeyAttribute>() != null)
                            {
                                sql.Append($" IDENTITY(1,1) NOT NULL PRIMARY KEY");
                            }
                            else
                            {
                                cols.Add(prop.Name);
                            }
                        }


                        break;
                }

                sql.AppendLine(",");
            }
            sql.Remove(sql.Length - 3, 1);
            sql.AppendLine(")");

            ins = ins + string.Join(",", cols) + ") VALUES (@" + string.Join(", @", cols) + ")" ;

            return (sql.ToString(), ins, drop);
        }
    }
}
