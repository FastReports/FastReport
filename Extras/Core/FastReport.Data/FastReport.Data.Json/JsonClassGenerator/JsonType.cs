// Copyright © 2010 Xamasoft
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE.txt for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace FastReport.JsonClassGenerator
{
    public class JsonType
    {


        private JsonType(IJsonClassGeneratorConfig generator)
        {
            this.generator = generator;
        }

        public JsonType(IJsonClassGeneratorConfig generator, JToken token)
            : this(generator)
        {

            Type = GetFirstTypeEnum(token);

            if (Type == JsonTypeEnum.Array)
            {
                var array = (JArray)token;
                InternalType = GetCommonType(generator, array.ToArray());
            }
        }

        internal static JsonType GetNull(IJsonClassGeneratorConfig generator)
        {
            return new JsonType(generator, JsonTypeEnum.NullableSomething);
        }

        private IJsonClassGeneratorConfig generator;

        internal JsonType(IJsonClassGeneratorConfig generator, JsonTypeEnum type)
            : this(generator)
        {
            this.Type = type;
        }


        public static JsonType GetCommonType(IJsonClassGeneratorConfig generator, JToken[] tokens)
        {

            if (tokens.Length == 0) return new JsonType(generator, JsonTypeEnum.NonConstrained);

            var common = new JsonType(generator, tokens[0]).MaybeMakeNullable(generator);

            for (int i = 1; i < tokens.Length; i++)
            {
                var current = new JsonType(generator, tokens[i]);
                common = common.GetCommonType(current);
            }

            return common;

        }

        internal JsonType MaybeMakeNullable(IJsonClassGeneratorConfig generator)
        {
            if (!generator.AlwaysUseNullableValues) return this;
            return this.GetCommonType(JsonType.GetNull(generator));
        }


        public JsonTypeEnum Type { get; private set; }
        public JsonType InternalType { get; private set; }
        public string AssignedName { get; private set; }


        public void AssignName(string name)
        {
            AssignedName = name;
        }



        public bool MustCache
        {
            get
            {
                switch (Type)
                {
                    case JsonTypeEnum.Array: return true;
                    case JsonTypeEnum.Object: return true;
                    case JsonTypeEnum.Anything: return true;
                    case JsonTypeEnum.Dictionary: return true;
                    case JsonTypeEnum.NonConstrained: return true;
                    default: return false;
                }
            }
        }

        public string GetReaderName()
        {
            if (Type == JsonTypeEnum.Anything || Type == JsonTypeEnum.NullableSomething || Type == JsonTypeEnum.NonConstrained)
            {
                return "ReadObject";
            }
            if (Type == JsonTypeEnum.Object)
            {
                return string.Format("ReadStronglyTypedObject<{0}>", AssignedName);
            }
            else if (Type == JsonTypeEnum.Array)
            {
                return string.Format("ReadArray<{0}>", InternalType.GetTypeName());
            }
            else
            {
                return string.Format("Read{0}", Enum.GetName(typeof(JsonTypeEnum), Type));
            }
        }

        public JsonType GetInnermostType()
        {
            if (Type != JsonTypeEnum.Array) throw new InvalidOperationException();
            if (InternalType.Type != JsonTypeEnum.Array) return InternalType;
            return InternalType.GetInnermostType();
        }


        public string GetTypeName()
        {
            return generator.CodeWriter.GetTypeName(this, generator);
        }

        public string GetJTokenType()
        {
            switch (Type)
            {
                case JsonTypeEnum.Boolean:
                case JsonTypeEnum.Integer:
                case JsonTypeEnum.Long:
                case JsonTypeEnum.Float:
                case JsonTypeEnum.Date:
                case JsonTypeEnum.NullableBoolean:
                case JsonTypeEnum.NullableInteger:
                case JsonTypeEnum.NullableLong:
                case JsonTypeEnum.NullableFloat:
                case JsonTypeEnum.NullableDate:
                case JsonTypeEnum.String:
                    return "JValue";
                case JsonTypeEnum.Array:
                    return "JArray";
                case JsonTypeEnum.Dictionary:
                    return "JObject";
                case JsonTypeEnum.Object:
                    return "JObject";
                default:
                    return "JToken";

            }
        }

        public JsonType GetCommonType(JsonType type2)
        {
            var commonType = GetCommonTypeEnum(this.Type, type2.Type);

            if (commonType == JsonTypeEnum.Array)
            {
                if (type2.Type == JsonTypeEnum.NullableSomething) return this;
                if (this.Type == JsonTypeEnum.NullableSomething) return type2;
                var commonInternalType = InternalType.GetCommonType(type2.InternalType).MaybeMakeNullable(generator);
                if (commonInternalType != InternalType) return new JsonType(generator, JsonTypeEnum.Array) { InternalType = commonInternalType };
            }


            //if (commonType == JsonTypeEnum.Dictionary)
            //{
            //    var commonInternalType = InternalType.GetCommonType(type2.InternalType);
            //    if (commonInternalType != InternalType) return new JsonType(JsonTypeEnum.Dictionary) { InternalType = commonInternalType };
            //}


            if (this.Type == commonType) return this;
            return new JsonType(generator, commonType).MaybeMakeNullable(generator);
        }


        private static bool IsNull(JsonTypeEnum type)
        {
            return type == JsonTypeEnum.NullableSomething;
        }



        private JsonTypeEnum GetCommonTypeEnum(JsonTypeEnum type1, JsonTypeEnum type2)
        {
            if (type1 == JsonTypeEnum.NonConstrained) return type2;
            if (type2 == JsonTypeEnum.NonConstrained) return type1;

            switch (type1)
            {
                case JsonTypeEnum.Boolean:
                    if (IsNull(type2)) return JsonTypeEnum.NullableBoolean;
                    if (type2 == JsonTypeEnum.Boolean) return type1;
                    break;
                case JsonTypeEnum.NullableBoolean:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Boolean) return type1;
                    break;
                case JsonTypeEnum.Integer:
                    if (IsNull(type2)) return JsonTypeEnum.NullableInteger;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.Float;
                    if (type2 == JsonTypeEnum.Long) return JsonTypeEnum.Long;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    break;
                case JsonTypeEnum.NullableInteger:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Long) return JsonTypeEnum.NullableLong;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    break;
                case JsonTypeEnum.Float:
                    if (IsNull(type2)) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Float) return type1;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    if (type2 == JsonTypeEnum.Long) return type1;
                    break;
                case JsonTypeEnum.NullableFloat:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Float) return type1;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    if (type2 == JsonTypeEnum.Long) return type1;
                    break;
                case JsonTypeEnum.Long:
                    if (IsNull(type2)) return JsonTypeEnum.NullableLong;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.Float;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    break;
                case JsonTypeEnum.NullableLong:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    if (type2 == JsonTypeEnum.Long) return type1;
                    break;
                case JsonTypeEnum.Date:
                    if (IsNull(type2)) return JsonTypeEnum.NullableDate;
                    if (type2 == JsonTypeEnum.Date) return JsonTypeEnum.Date;
                    break;
                case JsonTypeEnum.NullableDate:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Date) return type1;
                    break;
                case JsonTypeEnum.NullableSomething:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.String) return JsonTypeEnum.String;
                    if (type2 == JsonTypeEnum.Integer) return JsonTypeEnum.NullableInteger;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Long) return JsonTypeEnum.NullableLong;
                    if (type2 == JsonTypeEnum.Boolean) return JsonTypeEnum.NullableBoolean;
                    if (type2 == JsonTypeEnum.Date) return JsonTypeEnum.NullableDate;
                    if (type2 == JsonTypeEnum.Array) return JsonTypeEnum.Array;
                    if (type2 == JsonTypeEnum.Object) return JsonTypeEnum.Object;
                    break;
                case JsonTypeEnum.Object:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Object) return type1;
                    if (type2 == JsonTypeEnum.Dictionary) throw new ArgumentException();
                    break;
                case JsonTypeEnum.Dictionary:
                    throw new ArgumentException();
                //if (IsNull(type2)) return type1;
                //if (type2 == JsonTypeEnum.Object) return type1;
                //if (type2 == JsonTypeEnum.Dictionary) return type1;
                //  break;
                case JsonTypeEnum.Array:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Array) return type1;
                    break;
                case JsonTypeEnum.String:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.String) return type1;
                    break;
            }

            return JsonTypeEnum.Anything;

        }

        private static bool IsNull(JTokenType type)
        {
            return type == JTokenType.Null || type == JTokenType.Undefined;
        }



        private static JsonTypeEnum GetFirstTypeEnum(JToken token)
        {
            var type = token.Type;
            if (type == JTokenType.Integer)
            {
                if ((long)((JValue)token).Value < int.MaxValue) return JsonTypeEnum.Integer;
                else return JsonTypeEnum.Long;

            }
            switch (type)
            {
                case JTokenType.Array: return JsonTypeEnum.Array;
                case JTokenType.Boolean: return JsonTypeEnum.Boolean;
                case JTokenType.Float: return JsonTypeEnum.Float;
                case JTokenType.Null: return JsonTypeEnum.NullableSomething;
                case JTokenType.Undefined: return JsonTypeEnum.NullableSomething;
                case JTokenType.String: return JsonTypeEnum.String;
                case JTokenType.Object: return JsonTypeEnum.Object;
                case JTokenType.Date: return JsonTypeEnum.Date;

                default: return JsonTypeEnum.Anything;

            }
        }


        public IList<FieldInfo> Fields { get; internal set; }
        public bool IsRoot { get; internal set; }
    }
}
