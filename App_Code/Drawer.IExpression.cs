using System;

namespace Definitif.Data.ObjectSql
{
    public abstract partial class Drawer
    {
        private const string
            AND = "AND",
            OR = "OR";

        // AUTODOC: Drawer.Draw(IExpression Expression)
        private string Draw(IExpression Expression)
        {
            if (Expression is Expression.Object)
                return
                    this.Draw((Expression.Object)Expression);
            else if (Expression is Expression.AND)
                return
                    this.Draw((Expression.AND)Expression);
            else if (Expression is Expression.OR)
                return
                    this.Draw((Expression.OR)Expression);
            else if (Expression is Expression.Equals)
                return
                    this.Draw((Expression.Equals)Expression);
            else if (Expression is Expression.NotEquals)
                return
                    this.Draw((Expression.NotEquals)Expression);
            else if (Expression is Expression.Less)
                return
                    this.Draw((Expression.Less)Expression);
            else if (Expression is Expression.LessOrEquals)
                return
                    this.Draw((Expression.LessOrEquals)Expression);
            else if (Expression is Expression.More)
                return
                    this.Draw((Expression.More)Expression);
            else if (Expression is Expression.MoreOrEquals)
                return
                    this.Draw((Expression.MoreOrEquals)Expression);
            else if (Expression is Expression.Summ)
                return
                    this.Draw((Expression.Summ)Expression);
            else if (Expression is Expression.Subs)
                return
                    this.Draw((Expression.Subs)Expression);
            else if (Expression is Expression.LIKE)
                return
                    this.Draw((Expression.LIKE)Expression);
            else if (Expression is Expression.CONTAINS)
                return
                    this.Draw((Expression.CONTAINS)Expression);
            else
                return
                    this.Except(Expression);
        }

        // AUTODOC: Drawer.Draw(Expression.Object Object)
        protected virtual string Draw(Expression.Object Object)
        {
            if (Object.Container is IColumn)
            {
                return
                    this.Draw((IColumn)Object.Container);
            }
            else if (Object.Container is string)
            {
                return String.Format(
                    "'{0}'",
                    ((string)Object.Container).Replace("'", "''"));
            }
            else if (Object.Container is DateTime)
            {
                return
                    ((DateTime)Object.Container).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                return
                    Object.Container.ToString();
            }
        }

        // AUTODOC: Drawer.Draw(Expression.AND Expression)
        protected virtual string Draw(Expression.AND Expression)
        {
            string result = "";

            for (int i = 0; i < Expression.Container.Length; i++)
            {
                result += String.Format(
                    "{0}{1}",
                    (i > 0) ? " " + AND + " " : "",
                    this.Draw(Expression.Container[i]));
            }

            if (Expression.Container.Length > 1)
                return String.Format(
                    "( {0} )",
                    result);
            else
                return result;
        }

        // AUTODOC: Drawer.Draw(Expression.OR Expression)
        protected virtual string Draw(Expression.OR Expression)
        {
            string result = "";

            for (int i = 0; i < Expression.Container.Length; i++)
            {
                result += String.Format(
                    "{0}{1}",
                    (i > 0) ? " " + OR + " " : "",
                    this.Draw(Expression.Container[i]));
            }

            return String.Format(
                "( {0} )",
                result);
        }

        // AUTODOC: Drawer.Draw(Expression.Equals Expression)
        protected virtual string Draw(Expression.Equals Expression)
        {
            string result = "";

            if (Expression.SecondContainer.Length == 1 && Expression.SecondContainer[0] != null)
            {
                result = String.Format(
                    "{0} = {1}",
                    this.Draw(Expression.FirstContainer[0]),
                    this.Draw(Expression.SecondContainer[0]));
            }
            else if (Expression.SecondContainer.Length == 1 && Expression.SecondContainer[0] == null)
            {
                result = String.Format(
                    "{0} IS NULL",
                    this.Draw(Expression.FirstContainer[0]));
            }
            else if (Expression.SecondContainer.Length > 1)
            {
                for (int i = 0; i < Expression.SecondContainer.Length; i++)
                {
                    if (i > 0) result += ", ";
                    result += this.Draw(Expression.SecondContainer[i]);
                }
                result = String.Format(
                    "{0} IN ( {1} )",
                    this.Draw(Expression.FirstContainer[0]),
                    result);
            }

            return result;
        }

        // AUTODOC: Drawer.Draw(Expression.NotEquals Expression)
        protected virtual string Draw(Expression.NotEquals Expression)
        {
            string result = "";

            if (Expression.SecondContainer.Length == 1 && Expression.SecondContainer[0] != null)
            {
                result = String.Format(
                    "{0} <> {1}",
                    this.Draw(Expression.FirstContainer[0]),
                    this.Draw(Expression.SecondContainer[0]));
            }
            else if (Expression.SecondContainer.Length == 1 && Expression.SecondContainer[0] == null)
            {
                result = String.Format(
                    "{0} IS NOT NULL",
                    this.Draw(Expression.FirstContainer[0]));
            }
            else if (Expression.SecondContainer.Length > 1)
            {
                for (int i = 0; i < Expression.SecondContainer.Length; i++)
                {
                    if (i > 0) result += ", ";
                    result += this.Draw(Expression.SecondContainer[i]);
                }
                result = String.Format(
                    "{0} NOT IN ( {1} )",
                    this.Draw(Expression.FirstContainer[0]),
                    result);
            }

            return result;
        }

        // AUTODOC: Drawer.Draw(Expression.Less Expression)
        protected virtual string Draw(Expression.Less Expression)
        {
            return String.Format(
                "{0} < {1}",
                this.Draw(Expression.FirstContainer[0]),
                this.Draw(Expression.SecondContainer[0]));
        }

        // AUTODOC: Drawer.Draw(Expression.LessOrEquals Expression)
        protected virtual string Draw(Expression.LessOrEquals Expression)
        {
            return String.Format(
                "{0} <= {1}",
                this.Draw(Expression.FirstContainer[0]),
                this.Draw(Expression.SecondContainer[0]));
        }

        // AUTODOC: Drawer.Draw(Expression.More Expression)
        protected virtual string Draw(Expression.More Expression)
        {
            return String.Format(
                "{0} > {1}",
                this.Draw(Expression.FirstContainer[0]),
                this.Draw(Expression.SecondContainer[0]));
        }

        // AUTODOC: Drawer.Draw(Expression.MoreOrEquals Expression)
        protected virtual string Draw(Expression.MoreOrEquals Expression)
        {
            return String.Format(
                "{0} >= {1}",
                this.Draw(Expression.FirstContainer[0]),
                this.Draw(Expression.SecondContainer[0]));
        }

        // AUTODOC: Drawer.Draw(Expression.Summ Expression)
        protected virtual string Draw(Expression.Summ Expression)
        {
            string result = "";

            for (int i = 0; i < Expression.Container.Length; i++)
            {
                if (i > 0) result += " + ";
                result += this.Draw(Expression.Container[i]);
            }

            return result;
        }

        // AUTODOC: Drawer.Draw(Expression.LIKE Expression)
        protected virtual string Draw(Expression.LIKE Expression)
        {
            return String.Format(
                "{0} LIKE {1}",
                this.Draw(Expression.Container[0]),
                this.Draw(new Expression.Object(Expression.Expression)));
        }

        // AUTODOC: Drawer.Draw(Expression.CONTAINS Expression)
        protected virtual string Draw(Expression.CONTAINS Expression)
        {
            if (Expression.Container == null || Expression.Container.Length == 0)
            {
                return String.Format(
                    "CONTAINS( *, {0} )",
                    this.Draw(new Expression.Object(Expression.Expression)));
            }
            else
            {
                string result = "";

                for (int i = 0; i < Expression.Container.Length; i++)
                {
                    if (i > 0) result += ", ";
                    result += this.Draw(Expression.Container[i]);
                }

                return String.Format(
                    "CONTAINS( {0}, {1} )",
                    result,
                    this.Draw(new Expression.Object(Expression.Expression)));
            }
        }

        // AUTODOC: Drawer.Draw(Expression.Subs Expression)
        protected virtual string Draw(Expression.Subs Expression)
        {
            string result = "";

            for (int i = 0; i < Expression.Container.Length; i++)
            {
                if (i > 0) result += " - ";
                result += this.Draw(Expression.Container[i]);
            }

            return result;
        }
    }
}
