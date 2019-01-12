using System;

namespace Website.Core.Models.Domain
{
    public class Description : IEquatable<Description>
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int? ProductId { get; set; }
        public int? DescriptionGroupItemId { get; set; }

        public virtual DescriptionGroupItem DescriptionGroupItem { get; set; }
        public virtual Product Product { get; set; }

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
                   DescriptionGroupItemId == other.DescriptionGroupItemId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ProductId.GetHashCode();
                hashCode = (hashCode * 397) ^ DescriptionGroupItemId.GetHashCode();
                return hashCode;
            }
        }
    }
}
