// Copyright © 2010 Xamasoft
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE.txt for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.JsonClassGenerator
{
    public enum JsonTypeEnum
    {
        Anything,
        String,
        Boolean,
        Integer,
        Long,
        Float,
        Date,
        NullableInteger,
        NullableLong,
        NullableFloat,
        NullableBoolean,
        NullableDate,
        Object,
        Array,
        Dictionary,
        NullableSomething,
        NonConstrained


    }
}
