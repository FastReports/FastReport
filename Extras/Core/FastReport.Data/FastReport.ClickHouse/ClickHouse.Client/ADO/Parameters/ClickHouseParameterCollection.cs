using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace ClickHouse.Client.ADO.Parameters
{
    internal class ClickHouseParameterCollection : DbParameterCollection
    {
        private readonly List<ClickHouseDbParameter> parameters = new List<ClickHouseDbParameter>();

        public override int Count => parameters.Count;

        public override object SyncRoot { get; }

        public override int Add(object value)
        {
            parameters.Add((ClickHouseDbParameter)value);
            return parameters.Count - 1;
        }

        public override void AddRange(Array values) => parameters.AddRange(values.Cast<ClickHouseDbParameter>());

        public override void Clear() => parameters.Clear();

        public override bool Contains(object value) => parameters.Contains(value as ClickHouseDbParameter);

        public override bool Contains(string value) => parameters.Any(p => p.ParameterName == value);

        public override void CopyTo(Array array, int index)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                array.SetValue(parameters[i].Value, index + i);
            }
        }

        public override IEnumerator GetEnumerator() => parameters.GetEnumerator();

        public override int IndexOf(object value) => parameters.IndexOf(value as ClickHouseDbParameter);

        public override int IndexOf(string parameterName) => parameters.FindIndex(x => x.ParameterName == parameterName);

        public override void Insert(int index, object value) => parameters.Insert(index, (ClickHouseDbParameter)value);

        public override void Remove(object value) => parameters.Remove(value as ClickHouseDbParameter);

        public override void RemoveAt(int index) => parameters.RemoveAt(index);

        public override void RemoveAt(string parameterName) => parameters.RemoveAll(p => p.ParameterName == parameterName);

        protected override DbParameter GetParameter(int index) => parameters[index];

        protected override DbParameter GetParameter(string parameterName) => parameters[IndexOf(parameterName)];

        protected override void SetParameter(int index, DbParameter value) => parameters[index] = (ClickHouseDbParameter)value;

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);
            if (index < 0)
                Add(value);
            else
                SetParameter(index, value);
        }
    }
}
