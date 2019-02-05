using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Website.Core.Infrastructure;
using Website.Core.Models.Domain;

namespace Website.Core.Interfaces.Repositories
{
    //default implementation
    public interface IShopRepository : IShopRepository<Product, Image, ImageBinData, Category, ProductToCategory, DescriptionGroup,
        DescriptionGroupItem, Description, Order>
    {
    }
    
    public interface
        IShopRepository<TProduct, TImage, TImageData, TCategory, TProductCategory, TDescriptionGroup,
            TDescriptionGroupItem, TDescription, TOrder> :
            IDisposable,
            IProductRepository<TProduct>,
            IImageRepository<TImage, TImageData>,
            ICategoryRepository<TCategory, TProductCategory>,
            IDescriptionsRepository<TDescriptionGroup, TDescriptionGroupItem, TDescription>,
            IOrderRepository<TOrder>
    
        where TProduct : class
        where TImage : class
        where TImageData : class
        where TCategory : class
        where TProductCategory : class
        where TDescriptionGroup : class
        where TDescription : class
        where TOrder : class
    {
        IDbContextTransaction BeginTransaction(IsolationLevel iLevel = IsolationLevel.Serializable);
        void JoinTransaction(IDbContextTransaction tran);

       
    }
}