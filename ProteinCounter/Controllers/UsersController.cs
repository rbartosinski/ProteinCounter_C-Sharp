using ProteinCounter.Models;
using ServiceStack.Redis;
using System.Web.Mvc;

namespace ProteinCounter.Controllers
{
    public class UsersController : Controller
    {
        public ActionResult NewUser()
        {
            return View();
        }

        public ActionResult Save(string userName, int goal, long? userId)
        {
            using (IRedisClient client = new RedisClient())
            {
                var userClient = client.As<User>();

                User user;
                if (userId != null)
                {
                    user = userClient.GetById(userId);
                    client.RemoveItemFromSortedSet("urn:leaderboard", user.Name);
                }
                else
                {
                    user = new User
                    {
                        Id = userClient.GetNextSequence()
                    };
                }

                user.Goal = goal;
                user.Name = userName;
                userClient.Store(user);
                userId = user.Id;
                client.AddItemToSortedSet("urn:leaderboard", userName, user.Total);
            }

            return RedirectToAction("Index", "Tracker", new { userId });
        }

        public ActionResult Edit(int userId)
        {
            using (IRedisClient client = new RedisClient())
            {
                var userClient = client.As<User>();
                var user = userClient.GetById(userId);
                ViewBag.UserName = user.Name;
                ViewBag.Goal = user.Goal;
                ViewBag.UserId = user.Id;
            }
            return View("NewUser");
        }
    }
}