using WebPortal.Models.CompShop.Device;

namespace WebPortal.Models.CompShop
{
    public class ProductInfoViewModel
    {
        public DeviceViewModel DeviceViewModel { get; set; }

        public int ComputerId { get; set; }
        public ComputerViewModel? ComputerViewModel { get; set; }

        /*public int LaptopId { get; set; }
        public LaptopViewModel? LaptopViewModel { get; set; } И так далее */

    }
}
