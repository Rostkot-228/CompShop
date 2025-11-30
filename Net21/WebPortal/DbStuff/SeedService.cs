using WebPortal.DbStuff.Models;
using WebPortal.DbStuff.Models.CompShop;
using WebPortal.DbStuff.Models.CompShop.Devices;
using WebPortal.DbStuff.Repositories.CompShop;
using WebPortal.DbStuff.Repositories.Interfaces;

namespace WebPortal.DbStuff
{
    public class SeedService
    {
        private readonly IUserRepositrory _userRepositrory;

        //CompShop
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITypeDeviceRepository _typeDeviceRepository;

        public const string ADMIN_NAME = "Admin";

        public SeedService(
            IUserRepositrory userRepositrory,
            ICategoryRepository categoryRepository,
            ITypeDeviceRepository typeDeviceRepository)
        {
            _userRepositrory = userRepositrory;

            _categoryRepository = categoryRepository;
            _typeDeviceRepository = typeDeviceRepository;
        }

        public void Seed()
        {
            FillUser();
            FillCategories();
            FillTypeDevice();
        }

        private void FillCategories()
        {
            if (_categoryRepository.Any())
            {
                return;
            }

            var categories = new List<Category>
                {
                    new Category
                    {
                        Name = "Компьютер"
                    },
                    new Category
                    {
                        Name = "Ноутбук"
                    },
                    new Category
                    {
                        Name = "Телефон"
                    },
                    new Category
                    {
                        Name = "Запчасти"
                    },
                };
            _categoryRepository.AddRange(categories);
        }

        private void FillTypeDevice()
        {
            if (_typeDeviceRepository.Any())
            {
                return;
            }

            var typeDevices = new List<TypeDevice>
            {
                 new TypeDevice
                 {
                     Name = "Игровой",
                     Description = "Устройство предназначено для игр. Довольно мощный девайс."
                 },

                 new TypeDevice
                 {
                     Name = "Офисный",
                     Description = "Устройство для работы и офисных задач. Зачастую, имеет не самую сильную производительность."
                 },

                 new TypeDevice
                 {
                     Name = "Портативный",
                     Description = "Легкое и мобильное устройство, удобное для использования в дороге."
                 },

                 new TypeDevice
                 {
                     Name = "Бюджетный",
                     Description = "Устройства с браком, поломками или другими проблемами. Продаётся по занижиной цене."
                 }
            };

            _typeDeviceRepository.AddRange(typeDevices);
        }

        private void FillUser()
        {
            var admin = _userRepositrory.GetByName(ADMIN_NAME);
            if (admin == null)
            {
                _userRepositrory.Registration(ADMIN_NAME, ADMIN_NAME);
                admin = _userRepositrory.GetByName(ADMIN_NAME);
                admin.Role = Enum.Role.Admin;
                _userRepositrory.Update(admin);
            }
        }
    }
}
