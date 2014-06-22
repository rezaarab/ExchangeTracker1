using System;
using System.Linq.Expressions;

namespace ExchangeTracker.Domain
{
    public abstract class Entity : Model
    {
        private Guid _id;

        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref  _id, value); }
        }

        protected void RaisePropertyChanged(Expression<Func<object>> propertySelector)
        {
            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null) return;
            RaisePropertyChanged(memberExpression.Member.Name);
        }
    }
}
