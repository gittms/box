﻿using System;
using System.Collections.Generic;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Expression
{
    /// <summary>
    /// Represents - expression:
    /// [IExpression] - [IExpression]
    /// </summary>
    public class Subs : SingleContainer
    {
        public Subs(
            params IExpression[] Expressions)
        {
            this.container = Expressions;
        }
    }
}