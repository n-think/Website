using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Website.Core.Interfaces.Repositories;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Website.Services.Services;
using Website.Web;
using Image = Website.Core.Models.Domain.Image;

namespace Tests
{
    public static class TestFactory
    {
        public static TestServer GetServer()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(@"../../../"))
                .AddJsonFile("testsettings.json")
                .Build();

            var builder = new WebHostBuilder()
                .UseEnvironment("Testing")
                .UseStartup<Startup>()
                .UseConfiguration(configuration);

            return new TestServer(builder);
        }

        public static T GetRequiredService<T>(this TestServer server)
        {
            return server.Host.Services.GetRequiredService<T>();
        }

        public static
            IShopRepository<Product, Image, ImageBinData, Category, ProductToCategory, DescriptionGroup,
                DescriptionGroupItem, Description, Order> GetShopRepository(this TestServer server)
        {
            return server.Host.Services.GetRequiredService<IShopRepository<Product, Image, ImageBinData, Category,
                ProductToCategory, DescriptionGroup, DescriptionGroupItem, Description, Order>>();
        }
        
        private static Bitmap DrawFilledRectangle(int x, int y)
        {
            Bitmap bmp = new Bitmap(x, y);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, x, y);
                graph.FillRectangle(Brushes.White, ImageSize);
            }

            return bmp;
        }

        public static byte[] GenerateValidImageBytes(int x, int y, ImageFormat format)
        {
            var bmp = DrawFilledRectangle(x, y);
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, format);
                return ms.ToArray();
            }
        }
    }
}