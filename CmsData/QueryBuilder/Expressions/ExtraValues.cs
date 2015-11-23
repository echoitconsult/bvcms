﻿/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using UtilityExtensions;

namespace CmsData
{
    public partial class Condition
    {
        internal Expression PeopleExtra()
        {
            Expression<Func<Person, bool>> pred = p =>
                p.PeopleExtras.Any(e => CodeStrIds.Contains(e.FieldValue));
            Expression expr = System.Linq.Expressions.Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression HasPeopleExtraField()
        {
            var sev = ExtraValue.Views.GetViewableNameTypes(db, "People", nocache: true).SingleOrDefault(nn => nn.Name == TextValue);
            if (!db.FromBatch)
                if (sev != null && !sev.CanView)
                    return AlwaysFalse();
            Expression<Func<Person, bool>> pred = p => p.PeopleExtras.Any(e => e.Field == TextValue);
            Expression expr = System.Linq.Expressions.Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression PeopleExtraData()
        {
            var field = Quarters;
            var sev = ExtraValue.Views.GetViewableNameTypes(db, "People", nocache: true).SingleOrDefault(nn => nn.Name == field);
            if (!db.FromBatch)
                if (sev != null && !sev.CanView)
                    return AlwaysFalse();
            Expression<Func<Person, string>> pred = p =>
                p.PeopleExtras.Where(ff => ff.Field == field).Select(ff => ff.Data).SingleOrDefault();
            Expression left = System.Linq.Expressions.Expression.Invoke(pred, parm);
            var right = System.Linq.Expressions.Expression.Constant(TextValue, typeof(string));
            return Compare(left, right);
        }
        internal Expression PeopleExtraInt()
        {
            var field = Quarters;
            var sev = ExtraValue.Views.GetViewableNameTypes(db, "People", nocache: true).SingleOrDefault(nn => nn.Name == field);
            if (!db.FromBatch)
                if (sev != null && !sev.CanView)
                    return AlwaysFalse();
            var value = TextValue.ToInt2();
            if (!value.HasValue)
            {
                Expression<Func<Person, bool>> predint = null;
                switch (op)
                {
                    case CompareType.Equal:
                        predint = p => p.PeopleExtras.All(e => e.Field != field)
                                    || p.PeopleExtras.SingleOrDefault(e => e.Field == field).IntValue == null;
                        return System.Linq.Expressions.Expression.Invoke(predint, parm);
                    case CompareType.NotEqual:
                        predint = p => p.PeopleExtras.SingleOrDefault(e => e.Field == field).IntValue != null;
                        return System.Linq.Expressions.Expression.Invoke(predint, parm);
                    default:
                        return AlwaysFalse();
                }
            }

            Expression<Func<Person, int>> pred = p =>
                p.PeopleExtras.Single(e =>
                    e.Field == field).IntValue ?? 0;
            Expression left = System.Linq.Expressions.Expression.Invoke(pred, parm);
            var right = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(value), left.Type);
            return Compare(left, right);
        }
        internal Expression PeopleExtraDate()
        {
            var field = Quarters;
            var sev = ExtraValue.Views.GetViewableNameTypes(db, "People", nocache: true).SingleOrDefault(nn => nn.Name == field);
            if (!db.FromBatch)
                if (sev != null && !sev.CanView)
                    return AlwaysFalse();
            if (!DateValue.HasValue)
            {
                Expression<Func<Person, bool>> pred = null;
                switch (op)
                {
                    case CompareType.Equal:
                        pred = p => p.PeopleExtras.All(e => e.Field != field)
                              || p.PeopleExtras.SingleOrDefault(e => e.Field == field).DateValue == null;
                        return System.Linq.Expressions.Expression.Invoke(pred, parm);
                    case CompareType.NotEqual:
                        pred = p => p.PeopleExtras.SingleOrDefault(e => e.Field == field).DateValue != null;
                        return System.Linq.Expressions.Expression.Invoke(pred, parm);
                    default:
                        return AlwaysFalse();
                }
            }
            else
            {
                Expression<Func<Person, DateTime>> pred = p => p.PeopleExtras.SingleOrDefault(e => e.Field == field).DateValue.Value;
                Expression left = System.Linq.Expressions.Expression.Invoke(pred, parm);
                var right = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(DateValue), left.Type);
                return Compare(left, right);
            }
        }

        internal Expression RecentPeopleExtraFieldChanged()
        {
            var mindt = Util.Now.AddDays(-Days).Date;
            var sev = ExtraValue.Views.GetViewableNameTypes(db, "People", nocache: true).SingleOrDefault(nn => nn.Name == Quarters);
            if (!db.FromBatch)
                if (sev != null && !sev.CanView)
                    return AlwaysFalse();
            Expression<Func<Person, bool>> pred = p => (
                    from e in p.PeopleExtras
                    where e.Field == Quarters
                    where e.TransactionTime.Date >= mindt
                    select e).Any();
            Expression expr = System.Linq.Expressions.Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }

        internal Expression FamilyExtra()
        {
            Expression<Func<Person, bool>> pred = p =>
                p.Family.FamilyExtras.Any(e => CodeStrIds.Contains(e.FieldValue));
            Expression expr = System.Linq.Expressions.Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual || op == CompareType.NotOneOf)
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression HasFamilyExtraField()
        {
            var sev = ExtraValue.Views.GetViewableNameTypes(db, "Family", nocache: true).SingleOrDefault(nn => nn.Name == TextValue);
            if (!db.FromBatch)
                if (sev != null && !sev.CanView)
                    return AlwaysFalse();
            Expression<Func<Person, bool>> pred = null;
            if (!TextValue.HasValue())
                pred = p => !p.Family.FamilyExtras.Any();
            else
                pred = p => p.Family.FamilyExtras.Any(e => e.Field == TextValue);
            Expression expr = System.Linq.Expressions.Expression.Invoke(pred, parm);
            if (op == CompareType.NotEqual)
                expr = System.Linq.Expressions.Expression.Not(expr);
            return expr;
        }
        internal Expression FamilyExtraData()
        {
            var field = Quarters;
            var sev = ExtraValue.Views.GetViewableNameTypes(db, "Family", nocache: true).SingleOrDefault(nn => nn.Name == field);
            if (!db.FromBatch)
                if (sev != null && !sev.CanView)
                    return AlwaysFalse();
            Expression<Func<Person, string>> pred = p =>
                p.Family.FamilyExtras.Where(ff => ff.Field == field).Select(ff => ff.Data).SingleOrDefault();
            Expression left = System.Linq.Expressions.Expression.Invoke(pred, parm);
            var right = System.Linq.Expressions.Expression.Constant(TextValue, typeof(string));
            return Compare(left, right);
        }
        internal Expression FamilyExtraInt()
        {
            var field = Quarters;
            var sev = ExtraValue.Views.GetViewableNameTypes(db, "Family", nocache: true).SingleOrDefault(nn => nn.Name == field);
            if (!db.FromBatch)
                if (sev != null && !sev.CanView)
                    return AlwaysFalse();
            var value = TextValue.ToInt2();
            if (!value.HasValue)
            {
                Expression<Func<Person, bool>> predint = null;
                switch (op)
                {
                    case CompareType.Equal:
                        predint = p => p.Family.FamilyExtras.All(e => e.Field != field)
                                    || p.Family.FamilyExtras.SingleOrDefault(e => e.Field == field).IntValue == null;
                        return System.Linq.Expressions.Expression.Invoke(predint, parm);
                    case CompareType.NotEqual:
                        predint = p => p.Family.FamilyExtras.SingleOrDefault(e => e.Field == field).IntValue != null;
                        return System.Linq.Expressions.Expression.Invoke(predint, parm);
                    default:
                        return AlwaysFalse();
                }
            }

            Expression<Func<Person, int>> pred = p =>
                p.Family.FamilyExtras.Single(e =>
                    e.Field == field).IntValue ?? 0;
            Expression left = System.Linq.Expressions.Expression.Invoke(pred, parm);
            var right = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(value), left.Type);
            return Compare(left, right);
        }
        internal Expression FamilyExtraDate()
        {
            var field = Quarters;
            var sev = ExtraValue.Views.GetViewableNameTypes(db, "Family", nocache: true).SingleOrDefault(nn => nn.Name == field);
            if (!db.FromBatch)
                if (sev != null && !sev.CanView)
                    return AlwaysFalse();
            if (!DateValue.HasValue)
            {
                Expression<Func<Person, bool>> pred = null;
                switch (op)
                {
                    case CompareType.Equal:
                        pred = p => p.Family.FamilyExtras.All(e => e.Field != field)
                              || p.Family.FamilyExtras.SingleOrDefault(e => e.Field == field).DateValue == null;
                        return System.Linq.Expressions.Expression.Invoke(pred, parm);
                    case CompareType.NotEqual:
                        pred = p => p.Family.FamilyExtras.SingleOrDefault(e => e.Field == field).DateValue != null;
                        return System.Linq.Expressions.Expression.Invoke(pred, parm);
                    default:
                        return AlwaysFalse();
                }
            }
            else
            {
                Expression<Func<Person, DateTime>> pred = p => p.Family.FamilyExtras.SingleOrDefault(e => e.Field == field).DateValue.Value;
                Expression left = System.Linq.Expressions.Expression.Invoke(pred, parm);
                var right = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(DateValue), left.Type);
                return Compare(left, right);
            }
        }
    }
}
