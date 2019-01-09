using System;

namespace Website.Core.Models.Domain
{
    public class ProductToCategory : IEquatable<ProductToCategory>
    {
        public virtual int ProductId { get; set; }
        public virtual int CategoryId { get; set; }


        // Nav property for Product
        public virtual Product Product { get; set; }
        // Nav property for Category
        public virtual Category Category { get; set; }

        public bool Equals(ProductToCategory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ProductId == other.ProductId && CategoryId == other.CategoryId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProductToCategory) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ProductId * 397) ^ CategoryId;
            }
        }
    }
}