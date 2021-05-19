using System;
using System.Linq.Expressions;
using System.Reflection;
using Xamarin.Forms;

namespace MorrallaExpress.Helpers
{
    public abstract class ExtendedBindableObject : BindableObject
    {
        public void RaisePropertyChanged<T>(Expression<Func<T>> property) =>
            OnPropertyChanged(GetMemberInfo(property).Name);
        

        private MemberInfo GetMemberInfo(Expression expression)
        {
            MemberExpression operand;
            LambdaExpression lambdaExpression = (LambdaExpression)expression;
            if (lambdaExpression.Body as UnaryExpression != null)
            {
                UnaryExpression body = (UnaryExpression)lambdaExpression.Body;
                operand = (MemberExpression)body.Operand;
            }
            else
                operand = (MemberExpression)lambdaExpression.Body;
            return operand.Member;
        }
    }
}