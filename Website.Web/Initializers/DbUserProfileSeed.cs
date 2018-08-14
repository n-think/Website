using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Website.Service.DTO;
using Website.Service.Services;

namespace Website.Web.Initializers
{
    public class DbUserProfileSeed
    {
        public static async Task InitializeAsync(UserManager userManager,
            RoleManager roleManager, DbContext context)
        {
            string email = "email@example.com";
            string password = "123123";
            var names = new[]
            {
                "Демидов Феликс Яковович",
                "Собаков Георгий Евгениевич",
                "Суходолина Лариса Юлиевна",
                "Дуванова Лада Марковна",
                "Соболева Диана Игнатиевна",
                "Кодица Регина Ростиславовна",
                "Галкина Ефросиния Серафимовна",
                "Пронина Ирина Родионовна",
                "Куксилова Алла Святославовна",
                "Бабикова Анна Иосифовна",
                "Эссена Оксана Елизаровна",
                "Стрелков Эммануил Юриевич",
                "Кольцова Екатерина Александровна",
                "Поджио Август Самсонович",
                "Болдаев Мир Евстафиевич",
                "Королева Марфа Семеновна",
                "Эфирова Алина Ильевна",
                "Брант Агафон Агапович",
                "Сомкина Рада Всеволодовна",
                "Тимиряев Александр Пахомович"
            };

            var random = new Random();
            var i = 1;
            foreach (var item in names)
            {
                var name = item.Split();

                var clProf = new ClientProfileDTO()
                {
                    FirstName = name[1],
                    LastName = name[0],
                    PatrName = name[2],
                    City =
                    ((Cities)random.Next(3)).ToString(),
                    RegistrationDate = DateTimeOffset.Now
                };

                var phone = "+7-" + random.Next(100, 999) + "-" + random.Next(100, 999) + "-" + random.Next(10, 99) + "-" + random.Next(10, 99);
                UserDTO user = new UserDTO { Id = Guid.NewGuid().ToString(), Email = i + email, UserName = i + email, ClientProfile = clProf, PhoneNumber = phone, LastActivityDate = DateTimeOffset.Now };
                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, "user");
                i++;
            }
        }

        enum Cities
        {
            Москва,
            Казань,
            Рязань,
            Ростов
        }


    }
}

