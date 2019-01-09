using System;

namespace Website.Core.Models.Domain
{
    public class Description : IEquatable<Description>
    {
        public virtual int Id { get; set; }
        public virtual string Value { get; set; }
        public virtual int? ProductId { get; set; }
        public virtual int? DescriptionGroupId { get; set; }
        public virtual Product Product { get; set; }
        public virtual DescriptionGroup DescriptionGroup { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Description) obj);
        }

        public bool Equals(Description other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && string.Equals(Value, other.Value) && ProductId == other.ProductId &&
                   DescriptionGroupId == other.DescriptionGroupId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ProductId.GetHashCode();
                hashCode = (hashCode * 397) ^ DescriptionGroupId.GetHashCode();
                return hashCode;
            }
        }
    }
}
