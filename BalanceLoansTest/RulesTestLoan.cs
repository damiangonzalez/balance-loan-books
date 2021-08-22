using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq.Expressions;

// Ref https://stackoverflow.com/questions/6488034/how-to-implement-a-rule-engine)  
namespace BalanceLoans
{
    public class RulesTestLoan
    {
        [Test]
        public void MainTest()
        {
            var loan1 = new Loan
            {
                id = 1,
                amount = 100,
                interest_rate = (float) 0.10,
                default_likelihood = (float) 0.05,
                state = "CA"
            };
            var loan2 = new Loan
            {
                id = 1,
                amount = 100,
                interest_rate = (float) 0.10,
                default_likelihood = (float) 0.10,
                state = "WA"
            };

            var rule_default_lessthan_09 = new Rule("default_likelihood", "LessThan", "0.09");
            Func<Loan, bool> compiledRule1 = CompileRule<Loan>(rule_default_lessthan_09);
            var result1_1 = compiledRule1(loan1);
            result1_1.Should().BeTrue();
            var result1_2 = compiledRule1(loan2);
            result1_2.Should().BeFalse();

            var rule_state_notequal_CA = new Rule("state", "NotEqual", "CA");
            Func<Loan, bool> compiledRule2 = CompileRule<Loan>(rule_state_notequal_CA);
            var result2_1 = compiledRule2(loan1);
            result2_1.Should().BeFalse();
            var result2_2 = compiledRule2(loan2);
            result2_2.Should().BeTrue();
        }

        public class Loan
        {
            public int id { get; set; }
            public int amount { get; set; }
            public float interest_rate { get; set; }
            public float default_likelihood { get; set; }
            public string state { get; set; }
        }

        static Expression BuildExpr<T>(Rule r, ParameterExpression param)
        {
            var left = MemberExpression.Property(param, r.MemberName);
            var tProp = typeof(T).GetProperty(r.MemberName).PropertyType;
            ExpressionType tBinary;
            // is the operator a known .NET operator?
            if (ExpressionType.TryParse(r.Operator, out tBinary))
            {
                var right = Expression.Constant(Convert.ChangeType(r.TargetValue, tProp));
                // use a binary operation, e.g. 'Equal' -> 'u.Age == 15'
                return Expression.MakeBinary(tBinary, left, right);
            }
            else
            {
                var method = tProp.GetMethod(r.Operator);
                var tParam = method?.GetParameters()[0].ParameterType;
                var right = Expression.Constant(Convert.ChangeType(r.TargetValue, tParam));
                // use a method call, e.g. 'Contains' -> 'u.Tags.Contains(some_tag)'
                return Expression.Call(left, method, right);
            }
        }

        public static Func<T, bool> CompileRule<T>(Rule r)
        {
            var paramUser = Expression.Parameter(typeof(Loan));
            Expression expr = BuildExpr<T>(r, paramUser);
            // build a lambda function User->bool and compile it
            return Expression.Lambda<Func<T, bool>>(expr, paramUser).Compile();
        }

        public class Rule
        {
            public string MemberName { get; set; }

            public string Operator { get; set; }

            public string TargetValue { get; set; }

            public Rule(string MemberName, string Operator, string TargetValue)
            {
                this.MemberName = MemberName;
                this.Operator = Operator;
                this.TargetValue = TargetValue;
            }
        }
    }
}