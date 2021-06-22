using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/24 20:06:06
*描述：
*
***********************************************************/
namespace Core.Bases.Models
{
    public class Entity : IEntity
    {
        private int _Id;
        private int? _requestedHashCode;
        /// <summary>
        /// 领域事件
        /// </summary>
        private List<INotification> _domainEvents;

        /// <summary>
        /// 只读领域事件
        /// </summary>
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public virtual int Id
        {
            get
            {
                return _Id;
            }

            set
            {
                _Id = value;
            }
        }

        public void AddDomainEvent(INotification @event)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(@event);
        }

        public void RemoveDomainEvent(INotification @event)
        {
            _domainEvents?.Remove(@event);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();
        }

        private bool IsTransient()
        {
            return _Id == default(int);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;
            if (Object.ReferenceEquals(this, obj))
                return true;
            if (this.GetType() != obj.GetType())
                return false;
            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        public static bool operator == (Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator != (Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}
