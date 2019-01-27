using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Website.Core.Models.Domain;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Image = Website.Core.Models.Domain.Image;

namespace Website.Services.Infrastructure
{
    public class RandomProductGenerator
    {
        private readonly string _testImagesFolderPath;

        public RandomProductGenerator(string testImagesFolderPath)
        {
            _testImagesFolderPath = testImagesFolderPath;

            descriptionGroups[0].DescriptionGroupItems.ElementAt(0).DescriptionGroup = descriptionGroups[0];
            descriptionGroups[0].DescriptionGroupItems.ElementAt(1).DescriptionGroup = descriptionGroups[0];
            descriptionGroups[1].DescriptionGroupItems.ElementAt(0).DescriptionGroup = descriptionGroups[1];
            
        }

        private Category[] categories = new[]
        {
            new Category()
            {
                Name = "Категория 1",
                Description = "описание категории 1"
            },
            new Category()
            {
                Name = "Категория 2",
                Description = "описание категории 2"
            },
            new Category()
            {
                Name = "Категория 3",
                Description = "описание категории 3"
            },
            new Category()
            {
                Name = "Категория 4",
                Description = "описание категории 4"
            },
            new Category()
            {
                Name = "Категория 5",
                Description = "описание категории 5"
            }
        };

        private DescriptionGroup[] descriptionGroups = new[]
        {
            new DescriptionGroup()
            {
                Name = "Общие характеристики",
                Description = "описание общих характеристик",
                DescriptionGroupItems = new[]
                {
                    new DescriptionGroupItem()
                    {
                        Name = "Вес"
                    },
                    new DescriptionGroupItem()
                    {
                        Name = "Цвет"
                    },
                }
            },
            new DescriptionGroup()
            {
                Name = "Доп. характеристики",
                Description = "описание доп. характеристик",
                DescriptionGroupItems = new[]
                {
                    new DescriptionGroupItem()
                    {
                        Name = "Рост"
                    },
                }
            }
        };

        //10 имен
        private string[] names = new[]
        {
            "Лошадь", "Медведь", "Акула", "Свинья", "Лев",
            "Тюлень", "Олень", "Корова", "Орел", "Бизон"
        };

        //5 цветов
        private string[] colours = new[] {"красный", "синий", "желтый", "зеленый", "голубой"};
        
        private Random _random = new Random();

        public IEnumerable<Product> GetRandomProducts(int count)
        {
            var productList = new List<Product>();
            var images = getImagesArrays();

            for (int i = 0; i < count; i++)
            {
                var product = new Product();

                var randZeroToNine = _random.Next(10);

                product.Name = names[randZeroToNine] + " " + Path.GetRandomFileName().Substring(0,8);
                product.Images = GenerateProductImages(images[randZeroToNine]);
                product.Code = _random.Next(322, Int32.MaxValue);
                product.Description = "описание " + names[randZeroToNine];
                product.Descriptions = GenerateProductDescriptions();
                product.ProductToCategory = GenerateProductCategories();
                product.Price = _random.Next(100, 10000);
                product.Stock = _random.Next(0, 100);
                product.Created = product.Changed = DateTimeOffset.Now;
                product.Reserved = product.Stock - _random.Next(0, product.Stock);
                product.Available = true;
                
                productList.Add(product);
            }

            return productList;
        }

        private ICollection<Image> GenerateProductImages(byte[][] imageArrays)
        {
            return new List<Image>()
            {
                new Image()
                {
                    Primary = true,
                    Mime = "image/jpeg",
                    BinData = new ImageBinData()
                    {
                        FullData = imageArrays[0],
                        ThumbData = imageArrays[0]
                    }
                },
                new Image()
                {
                    Primary = false,
                    Mime = "image/jpeg",
                    BinData = new ImageBinData()
                    {
                        FullData = imageArrays[1],
                        ThumbData = imageArrays[1]
                    }
                },
            };
        }

        private ICollection<Description> GenerateProductDescriptions()
        {
            var descriptionsList = new List<Description>()
            {
                new Description()
                {
                    Value = $"{_random.Next(322)} кг",
                    DescriptionGroupItem = descriptionGroups[0].DescriptionGroupItems.ElementAt(0),
                    
                },
                new Description()
                {
                    Value = colours[_random.Next(5)],
                    DescriptionGroupItem = descriptionGroups[0].DescriptionGroupItems.ElementAt(1)
                },
                new Description()
                {
                    Value = _random.Next(420).ToString(),
                    DescriptionGroupItem = descriptionGroups[1].DescriptionGroupItems.ElementAt(0)
                }
            };

            return descriptionsList;
        }

        private ICollection<ProductToCategory> GenerateProductCategories()
        {
            
            var max = _random.Next(1,3);
            
            var prodToCatList = new List<ProductToCategory>();
            for (int i = 0; i < max; i++)
            {
                var maxFive = _random.Next(5);
                var prodToCat = new ProductToCategory()
                {
                    Category = new Category()
                    {
                        Name = categories[maxFive].Name,
                        Description = categories[maxFive].Description
                    }
                };
                prodToCatList.Add(prodToCat);
            }

            return prodToCatList.Distinct().ToList();
        }

        private byte[][][] getImagesArrays()
        {
            var array = new byte[10][][];
            for (int i = 0; i < 10; i++)
            {
                array[i] = new byte[2][];

                //try image 1
                try
                {
                    var path = _testImagesFolderPath + $"\\{i+1}-1.jpg";
                    var img = System.Drawing.Image.FromFile(path);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        array[i][0] = ms.ToArray();
                    }

                }
                catch (FileNotFoundException)
                {
                    array[i][0] = GenerateValidImageBytes(1000, 1000, ImageFormat.Jpeg);
                }

                //try image 2
                try
                {
                    
                    var path = _testImagesFolderPath + $"\\{i+1}-1.jpg";
                    var img = System.Drawing.Image.FromFile(path);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, ImageFormat.Jpeg);
                        array[i][1] = ms.ToArray();
                    }
                }
                catch (FileNotFoundException)
                {
                    array[i][1] = GenerateValidImageBytes(1000, 1000, ImageFormat.Jpeg);
                }
            }

            return array;
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