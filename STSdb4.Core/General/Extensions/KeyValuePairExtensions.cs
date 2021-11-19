using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace STSdb4.General.Extensions
{
    public delegate void SetKeyDelegate<TKey, TValue>(ref KeyValuePair<TKey, TValue> kv, TKey key);
    public delegate void SetValueDelegate<TKey, TValue>(ref KeyValuePair<TKey, TValue> kv, TValue value);
    public delegate void SetKeyValueDelegate<TKey, TValue>(ref KeyValuePair<TKey, TValue> kv, TKey key, TValue value);

    public class KeyValuePairHelper<TKey, TValue>
    {
        public static readonly KeyValuePairHelper<TKey, TValue> Instance = new KeyValuePairHelper<TKey, TValue>();

        public readonly SetKeyDelegate<TKey, TValue> SetKey;
        public readonly SetValueDelegate<TKey, TValue> SetValue;
        public readonly SetKeyValueDelegate<TKey, TValue> SetKeyValue;

        public KeyValuePairHelper()
        {
            var setKeyLambda = CreateSetKeyMethod();
            SetKey = setKeyLambda.Compile();

            var setValueLambda = CreateSetValueMethod();
            SetValue = setValueLambda.Compile();

            var setKeyValueLambda = CreateSetKeyValueMethod();
            SetKeyValue = setKeyValueLambda.Compile();
        }

        public Expression<SetKeyDelegate<TKey, TValue>> CreateSetKeyMethod()
        {
            var kv = Expression.Parameter(typeof(KeyValuePair<TKey, TValue>).MakeByRefType(), "kv");
            var key = Expression.Parameter(typeof(TKey), "key");


            //var assign = Expression.Assign(Expression.Field(kv, "key"), key);
            var valueExpression = Expression.Field(kv, "value");
            var newExpression = Expression.New(typeof(KeyValuePair<TKey, TValue>).GetConstructor(new Type[] { typeof(TKey), typeof(TValue) }), key, valueExpression);
            var assign = Expression.Assign(kv, newExpression);
            var block = Expression.Block(valueExpression, newExpression, assign);


            return Expression.Lambda<SetKeyDelegate<TKey, TValue>>(block, kv, key);
        }

        public Expression<SetValueDelegate<TKey, TValue>> CreateSetValueMethod()
        {
            var kv = Expression.Parameter(typeof(KeyValuePair<TKey, TValue>).MakeByRefType(), "kv");
            var value = Expression.Parameter(typeof(TValue), "value");

            var keyExpression = Expression.Field(kv, "key");
            var newExpression = Expression.New(typeof(KeyValuePair<TKey, TValue>).GetConstructor(new Type[] { typeof(TKey), typeof(TValue) }), keyExpression, value);
            var assign = Expression.Assign(kv, newExpression);
            var block = Expression.Block(keyExpression, newExpression, assign);
            //var assign = Expression.Assign(Expression.Field(kv, "value"), value);

            return Expression.Lambda<SetValueDelegate<TKey, TValue>>(block, kv, value);
        }

        public Expression<SetKeyValueDelegate<TKey, TValue>> CreateSetKeyValueMethod()
        {
            var kv = Expression.Parameter(typeof(KeyValuePair<TKey, TValue>).MakeByRefType(), "kv");

            var key = Expression.Parameter(typeof(TKey), "key");
            var value = Expression.Parameter(typeof(TValue), "value");

            //var body = Expression.Block(
            //        Expression.Assign(Expression.Field(kv, "key"), key),
            //        Expression.Assign(Expression.Field(kv, "value"), value)
            //        );
            
            var newExpression = Expression.New(typeof(KeyValuePair<TKey, TValue>).GetConstructor(new Type[] { typeof(TKey), typeof(TValue) }), key, value);
            var assign = Expression.Assign(kv, newExpression);
            var block = Expression.Block( newExpression, assign);

            return Expression.Lambda<SetKeyValueDelegate<TKey, TValue>>(block, kv, key, value);
        }
    }
}