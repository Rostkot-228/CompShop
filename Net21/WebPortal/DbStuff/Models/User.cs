using WebPortal.Enum;
using WebPortal.DbStuff.Models.Notifications;

namespace WebPortal.DbStuff.Models
{
    public class User : BaseModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AvatarUrl { get; set; }
        public int Money { get; set; }
        public Role Role { get; set; }
        public Language Language { get; set; }

        public virtual List<Girl> CreatedGirls { get; set; } = new List<Girl>();
        public virtual List<Girl> FavoriteGirls { get; set; } = new List<Girl>();

        public virtual List<Notification> NotificationCreatedByMe { get; set; } = new List<Notification>();
        public virtual List<Notification> ViewedNotification { get; set; } = new List<Notification>();
    }
}
