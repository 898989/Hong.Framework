using Hong.Common.Extendsion;
using Hong.Test.Public;
using System;
using System.Reflection;
using Xunit;

namespace Hong.Test.Function
{
    public class TestReflection
    {
        [Fact]
        public void GetPropertyPerformance()
        {
            ReflectionObject o = new ReflectionObject();

            var name = typeof(ReflectionObject).GetTypeInfo().GetProperty("Name");
            //var getName = Reflection.GetProperty<ReflectionObject, string>("Name");
            var getName = Reflection.GetProperty<ReflectionObject, object>("Name");

            var time1 = Time.UserTime(() =>
            {
                string s = null;
                for (var index = 0; index < 1000000; index++)
                {
                    s = (string)getName(o);
                }

                Assert.Equal(s, o.Name);
            });

            var time2 = Time.UserTime(() =>
            {
                string s = null;
                for (var index = 0; index < 1000000; index++)
                {
                    s = (string)name.GetValue(o);
                }
                Assert.Equal(s, o.Name);
            });
        }

        [Fact]
        public void GetObjectClassPerformance()
        {
            ReflectionObject o = new ReflectionObject();
            SpeedExample speedExample = new SpeedExample(o);

            var name = typeof(SpeedExample).GetTypeInfo().GetProperty("Reflect");
            //var getName = Reflection.GetProperty<ReflectionObject, string>("Name");
            var getReflection = Reflection.GetProperty<SpeedExample, object>("Reflect");

            var time1 = Time.UserTime(() =>
            {
                ReflectionObject s = null;
                for (var index = 0; index < 1000000; index++)
                {
                    s = (ReflectionObject)getReflection(speedExample);
                }
                Assert.NotNull(s);
            });

            var time2 = Time.UserTime(() =>
            {
                ReflectionObject s = null;
                for (var index = 0; index < 1000000; index++)
                {
                    s = (ReflectionObject)name.GetValue(speedExample);
                }

                Assert.NotNull(s);
            });
        }

        [Fact]
        public void SetPropertyPerformance()
        {
            ReflectionObject o = new ReflectionObject();

            var name = typeof(ReflectionObject).GetTypeInfo().GetProperty("Name");
            var setName = Reflection.SetProperty<ReflectionObject, object>("Name");

            const string value1 = "sdfljslfwe";
            const string value2 = "sdfljslfwf";

            var time1 = Time.UserTime(() =>
            {
                for (var index = 0; index <= 1000000; index++)
                {
                    setName(o, value1 + index);
                }

                Assert.Equal(o.Name, value1 + "1000000");
            });

            var time2 = Time.UserTime(() =>
            {
                for (var index = 0; index <= 1000000; index++)
                {
                    name.SetValue(o, value2 + index);
                }

                Assert.Equal(o.Name, value2 + "1000000");
            });
        }

        [Fact]
        public void GetFieldPerformance()
        {
            ReflectionObject o = new ReflectionObject();

            var counterField = typeof(ReflectionObject).GetTypeInfo().GetField("CounterField");
            var getCounterField = Reflection.GetField<ReflectionObject, object>("CounterField");

            var time1 = Time.UserTime(() =>
            {
                long s = 0;
                for (var index = 0; index < 1000000; index++)
                {
                    s = (long)getCounterField(o);
                }
                Assert.Equal(s, o.CounterField);
            });

            var time2 = Time.UserTime(() =>
            {
                long s = 0;
                for (var index = 0; index < 1000000; index++)
                {
                    s = (long)counterField.GetValue(o);
                }

                Assert.Equal(s, o.CounterField);
            });
        }

        [Fact]
        public void SetFieldperformance()
        {
            ReflectionObject o = new ReflectionObject();

            var counterField = typeof(ReflectionObject).GetTypeInfo().GetField("CounterField");
            var setCounterField = Reflection.SetField<ReflectionObject, object>("CounterField");

            const long value1 = 352342;
            const long value2 = 352341;

            var time1 = Time.UserTime(() =>
            {
                for (var index = 0; index < 1000000; index++)
                {
                    setCounterField(o, value1);
                }

                Assert.Equal(o.CounterField, value1);
            });

            var time2 = Time.UserTime(() =>
            {
                for (var index = 0; index < 1000000; index++)
                {
                    counterField.SetValue(o, value2);
                }

                Assert.Equal(o.CounterField, value2);
            });
        }

        [Fact]
        public void MethodPerformance()
        {
            ReflectionObject o = new ReflectionObject();

            var vokeMethod = o.GetType().GetTypeInfo().GetMethod("VokeTest");
            var vokeTest = Reflection.GetMethod<ReflectionObject, Func<ReflectionObject, int, string, string>>(vokeMethod);

            var time1 = Time.UserTime(() =>
            {
                string s = null;
                for (var index = 0; index <= 1000000; index++)
                {
                    s = vokeTest(o, index, index.ToString());
                }

                Assert.Equal(s, o.VokeTest(1000000, "1000000"));
            });

            var time2 = Time.UserTime(() =>
            {
                string s = null;
                for (var index = 0; index <= 1000000; index++)
                {
                    s = (string)vokeMethod.Invoke(o, new object[] { index, index.ToString() });
                }

                Assert.Equal(s, o.VokeTest(1000000, "1000000"));
            });

            var time3 = Time.UserTime(() =>
            {
                string s = null;
                for (var index = 0; index < 1000000; index++)
                {
                    s = o.VokeTest(index, index.ToString());
                }
            });
        }

        [Fact]
        public void CreateInstancePerformance()
        {
            var creator = Reflection.CreateInstance<ReflectionObject>();
            ReflectionObject o = null;

            var time1 = Time.UserTime(() =>
            {
                for (var i = 0; i < 1000000; i++)
                {
                    o = creator();
                }
            });

            var time2 = Time.UserTime(() =>
            {
                for (var i = 0; i < 1000000; i++)
                {
                    o = new ReflectionObject();
                }
            });

            var type = typeof(ReflectionObject);
            var time3 = Time.UserTime(() =>
            {
                for (var i = 0; i < 1000000; i++)
                {
                    o = (ReflectionObject)System.Activator.CreateInstance(type);
                }
            });

            var time4 = Time.UserTime(() =>
            {
                var creator1 = Reflection.CreateInstance<ReflectionObject, long>();
                for (var i = 0; i < 1000000; i++)
                {
                    o = creator1(i);
                }
            });

            var time5 = Time.UserTime(() =>
            {
                for (var i = 0; i < 1000000; i++)
                {
                    o = (ReflectionObject)System.Activator.CreateInstance(type, i);
                }
            });
        }

        [Fact]
        public void PropertyVisitPerformance()
        {
            ReflectionObject model = new ReflectionObject();
            SpeedExample example = new SpeedExample(model);

            var time1 = Time.UserTime(() =>
            {
                for (var index = 0; index < 1000000; index++)
                {
                    model.Counter = index;
                    model.Counter++;
                }
            });

            var time2 = Time.UserTime(() =>
            {
                for (var index = 0; index < 1000000; index++)
                {
                    example.CounterField = index;
                    example.CounterField++;
                }
            });

            var time3 = Time.UserTime(() =>
            {
                for (var index = 0; index < 1000000; index++)
                {
                    example.Counter = index;
                    example.Counter++;
                }
            });

            var time4 = Time.UserTime(() =>
            {
                for (var index = 0; index < 1000000; index++)
                {
                    model.CounterField = index;
                    model.CounterField++;
                }
            });

            var time5 = Time.UserTime(() =>
            {
                for (var index = 0; index < 1000000; index++)
                {
                    model.Counter1 = index;
                    model.Counter1++;
                }
            });

            var time6 = Time.UserTime(() =>
            {
                for (var index = 0; index < 1000000; index++)
                {
                    model.SetCounter2(index);
                    model.SetCounter2(model.GetCounter2() + 1);
                }
            });
        }

        [Fact]
        public void GetAttributePerformance()
        {
            Hong.Model.Order order = new Hong.Model.Order();
            var p = order.GetType().GetTypeInfo().GetProperty("RecipientMobile");

            var time = Time.UserTime(() =>
            {
                for (var i = 0; i < 1000000; i++)
                {
                    p.GetCustomAttribute<Hong.DAO.Core.DBFieldAttribute>();
                }
            });
        }

        [Fact]
        public void Increment()
        {
            ReflectionObject o = new ReflectionObject();
            var count = o.CounterField + 1;
            var increment = Reflection.Increment<ReflectionObject>(o.GetType().GetTypeInfo().GetField("CounterField"));
            increment(o);
            Assert.Equal(count, o.CounterField);
        }

        [Fact]
        public void GetFieldIncrement()
        {
            ReflectionObject o = new ReflectionObject();
            var count = o.CounterField;
            var increment = Reflection.GetFieldAndIncrement<ReflectionObject, long>(o.GetType().GetTypeInfo().GetField("CounterField"));
            var newValue = increment(o);
            Assert.Equal(newValue, o.CounterField + 1);
        }

        public class ReflectionObject
        {
            public string Name { get; set; } = "cccc";

            public long CounterField = 111;
            public long Counter { get; set; }

            private long c = 0;
            public long Counter1
            {
                get
                {
                    return c;
                }
                set
                {
                    c = value;
                }
            }

            private long d = 0;
            public long GetCounter2()
            {
                return d;
            }
            public long SetCounter2(long v)
            {
                return d = v;
            }

            public string VokeTest(int input1, string input2)
            {
                return "result->" + input1.ToString() + "|" + input2;
            }

            public ReflectionObject()
            {

            }

            public ReflectionObject(long id)
            {
                CounterField = id;
            }
        }

        public class SpeedExample
        {
            ReflectionObject _model = null;

            public string Name
            {
                get
                {
                    return _model.Name;
                }
                set
                {
                    _model.Name = value;
                }
            }

            public long Counter
            {
                get
                {
                    return _model.Counter;
                }
                set
                {
                    _model.Counter = value;
                }
            }

            public long CounterField
            {
                get { return _model.CounterField; }
                set { _model.CounterField = value; }
            }

            public SpeedExample(ReflectionObject model)
            {
                _model = model;
            }

            public ReflectionObject Reflect
            {
                get
                {
                    return _model;
                }
            }
        }
    }
}
