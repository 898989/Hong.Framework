using System;
using System.Linq.Expressions;
using System.Reflection;
using static Hong.Common.Extendsion.Guard;

namespace Hong.Common.Extendsion
{
    public class Reflection
    {
        public static Func GetMethod<Object, Func>(string methodName) => GetMethod<Object, Func>(typeof(Object).GetMethod(methodName));

        /// <summary>获取执行方法
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <typeparam name="Func"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Func GetMethod<Object, Func>(MethodInfo method)
        {
            NotNull(method, nameof(method));

            var instance = method.IsStatic ? null : Expression.Parameter(typeof(Object), "instance");

            var parameters = method.GetParameters();
            int count = parameters.Length;

            var paramsExp = new ParameterExpression[count + 1];
            paramsExp[0] = instance;

            var argsArrayExp = new Expression[count];

            for (int i = 0; i < count; i++)
            {
                paramsExp[i + 1] = Expression.Parameter(parameters[i].ParameterType, parameters[i].Name);
                argsArrayExp[i] = paramsExp[i + 1];
            }

            var lambdaBodyExp = Expression.Call(instance, method, argsArrayExp);

            return Expression.Lambda<Func>(lambdaBodyExp, paramsExp).Compile();
        }

        /// <summary>创建获取属性值方法
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="propertyName"></param>
        /// <exception cref="ArgumentNullException">如果'<paramref name="propertyName"/>'不存在</exception>
        /// <returns></returns>
        public static Func<Object, ValueType> GetProperty<Object, ValueType>(string propertyName) => GetProperty<Object, ValueType>(typeof(Object).GetProperty(propertyName));
        /// <summary>创建获取属性值方法
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <exception cref="ArgumentNullException">如果'<paramref name="propertyInfo"/>'为null</exception>
        /// <returns></returns>
        public static Func<Object, ValueType> GetProperty<Object, ValueType>(PropertyInfo propertyInfo)
        {
            NotNull(propertyInfo, nameof(propertyInfo));

            Type type = typeof(Object);

            var instance = Expression.Parameter(type, "instance");
            var instanceCast = Expression.Convert(instance, type);
            var propertyAccess = Expression.Property(instanceCast, propertyInfo);
            var getPropertyValue = Expression.Convert(propertyAccess, typeof(ValueType));

            return Expression.Lambda<Func<Object, ValueType>>(getPropertyValue, instance)
                .Compile();
        }

        /// <summary>创建设置属性值方法
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="propertyName"></param>
        /// <exception cref="ArgumentNullException">如果'<paramref name="propertyName"/>'不存在</exception>
        /// <returns></returns>
        public static Action<Object, ValueType> SetProperty<Object, ValueType>(string propertyName) => SetProperty<Object, ValueType>(typeof(Object).GetProperty(propertyName));
        /// <summary>创建设置属性值方法
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <exception cref="ArgumentNullException">如果'<paramref name="propertyInfo"/>'不存在</exception>
        /// <returns></returns>
        public static Action<Object, ValueType> SetProperty<Object, ValueType>(PropertyInfo propertyInfo)
        {
            NotNull(propertyInfo, nameof(propertyInfo));
            Type t = typeof(Object);

            var instance = Expression.Parameter(t, "instance");
            var propertyValue = Expression.Parameter(typeof(ValueType));
            var getValue = Expression.Convert(propertyValue, propertyInfo.PropertyType);
            var setPropertyValue = Expression.Call(instance, propertyInfo.SetMethod, getValue);

            return Expression.Lambda<Action<Object, ValueType>>(setPropertyValue, instance, propertyValue)
                .Compile();
        }

        /// <summary>创建获取字段方法
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="fieldName"></param>
        /// <exception cref="ArgumentNullException">如果'<paramref name="fieldName"/>'为null</exception>
        /// <returns></returns>
        public static Func<Object, ValueType> GetField<Object, ValueType>(string fieldName) => GetField<Object, ValueType>(typeof(Object).GetField(fieldName));
        /// <summary>创建获取字段方法
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <exception cref="ArgumentNullException">如果'<paramref name="fieldInfo"/>'为null</exception>
        /// <returns></returns>
        public static Func<Object, ValueType> GetField<Object, ValueType>(FieldInfo fieldInfo)
        {
            NotNull(fieldInfo, nameof(fieldInfo));

            Type type = typeof(Object);

            var instance = Expression.Parameter(type, "instance");
            var instanceCast = Expression.Convert(instance, type);
            var fieldAccess = Expression.Field(instanceCast, fieldInfo);
            var getFieldValue = Expression.Convert(fieldAccess, typeof(ValueType));

            return Expression.Lambda<Func<Object, ValueType>>(getFieldValue, instance)
                .Compile();
        }

        /// <summary>创建字段自增1
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <exception cref="ArgumentNullException">如果'<paramref name="fieldInfo"/>'为null</exception>
        /// <returns></returns>
        public static Func<Object, ValueType> GetFieldAndIncrement<Object, ValueType>(FieldInfo fieldInfo)
        {
            NotNull(fieldInfo, nameof(fieldInfo));

            Type type = typeof(Object);

            var instance = Expression.Parameter(type, "instance");
            var instanceCast = Expression.Convert(instance, type);
            var fieldAccess = Expression.Field(instanceCast, fieldInfo);
            var getFieldValue = Expression.Convert(fieldAccess, fieldInfo.FieldType);
            var incrementValue = Expression.Add(getFieldValue, Expression.Constant(System.Convert.ChangeType(1, fieldInfo.FieldType)));
            var rFieldValue = Expression.Convert(incrementValue, typeof(ValueType));

            return Expression.Lambda<Func<Object, ValueType>>(rFieldValue, instance)
                .Compile();
        }

        /// <summary>创建设置字段方法
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="fieldName"></param>
        /// <exception cref="ArgumentNullException">如果'<paramref name="fieldName"/>'不存在</exception>
        /// <returns></returns>
        public static Action<Object, ValueType> SetField<Object, ValueType>(string fieldName) => SetField<Object, ValueType>(typeof(Object).GetField(fieldName));
        /// <summary>创建设置字段方法
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <exception cref="ArgumentNullException">如果'<paramref name="fieldInfo"/>'为null</exception>
        /// <returns></returns>
        public static Action<Object, ValueType> SetField<Object, ValueType>(FieldInfo fieldInfo)
        {
            NotNull(fieldInfo, nameof(fieldInfo));

            var instance = Expression.Parameter(typeof(Object), "instance");
            var fieldValue = Expression.Parameter(typeof(ValueType));
            var getValue = Expression.Convert(fieldValue, fieldInfo.FieldType);
            var setFieldValue = Expression.Field(instance, fieldInfo);

            return Expression.Lambda<Action<Object, ValueType>>(
                Expression.Assign(setFieldValue, getValue),
                instance, fieldValue)
                .Compile();
        }

        /// <summary>字段值加1
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <exception cref="ArgumentNullException">如果'<paramref name="fieldInfo"/>'为null</exception>
        /// <returns></returns>
        public static Action<Object> Increment<Object>(FieldInfo fieldInfo)
        {
            NotNull(fieldInfo, nameof(fieldInfo));

            var instance = Expression.Parameter(typeof(Object), "instance");
            var fieldAccess = Expression.Field(instance, fieldInfo);
            var getFieldValue = Expression.Convert(fieldAccess, fieldInfo.FieldType);
            var incrementValue = Expression.Increment(getFieldValue);
            var setFieldValue = Expression.Field(instance, fieldInfo);
            var setNewValue = Expression.Assign(setFieldValue, incrementValue);

            return Expression.Lambda<Action<Object>>(
                setNewValue, instance).Compile();
        }

        /// <summary>生成实例化对象方法
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <returns></returns>
        public static Func<Object> CreateInstance<Object>()
        {
            return Expression.Lambda<Func<Object>>(Expression.New(typeof(Object)), null)
                .Compile();
        }

        /// <summary>生成实例化对象方法
        /// </summary>
        /// <typeparam name="ValueType"></typeparam>
        /// <typeparam name="Object"></typeparam>
        /// <returns></returns>
        public static Func<ValueType, Object> CreateInstance<Object, ValueType>()
        {
            var val = Expression.Parameter(typeof(ValueType));
            var constructor = Expression.New(typeof(Object).GetConstructor(new Type[] { typeof(ValueType) }), val);

            return Expression.Lambda<Func<ValueType, Object>>(constructor, val)
                .Compile();
        }

        /// <summary>生成实例化对象方法
        /// </summary>
        /// <typeparam name="ValueType1"></typeparam>
        /// <typeparam name="ValueType2"></typeparam>
        /// <typeparam name="Object"></typeparam>
        /// <returns></returns>
        public static Func<ValueType1, ValueType2, Object> CreateInstance<Object, ValueType1, ValueType2>()
        {
            var value1 = Expression.Parameter(typeof(ValueType));
            var value2 = Expression.Parameter(typeof(ValueType));
            var constructor = Expression.New(typeof(Object).GetConstructor(new Type[] { typeof(ValueType) }), value1, value2);

            return Expression.Lambda<Func<ValueType1, ValueType2, Object>>(constructor, value1, value2)
                .Compile();
        }

        /// <summary>生成实例化对象方法
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static Func<Object> CreateInstance<Object>(Type type)
        {
            return Expression.Lambda<Func<Object>>(Expression.New(type), null)
                .Compile();
        }

        /// <summary>生成实例化对象方法
        /// </summary>
        /// <typeparam name="ValueType"></typeparam>
        /// <typeparam name="Object"></typeparam>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static Func<ValueType, Object> CreateInstance<Object, ValueType>(Type type)
        {
            var val = Expression.Parameter(typeof(ValueType));
            var constructor = Expression.New(type.GetConstructor(new Type[] { typeof(ValueType) }), val);
            return Expression.Lambda<Func<ValueType, Object>>(constructor, val)
                .Compile();
        }
    }
}
