using ProteinCounter.Models;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProteinCounter.Controllers
{
    public class TrackerController : Controller
    {
        public ActionResult Index(long userId, int amount = 0)
        {
            using (IRedisClient client = new RedisClient())
            {
                var userClient = client.As<User>();
                var user = userClient.GetById(userId);
                var historyClient = client.As<int>();
                var historyList = historyClient.Lists["urn:history:" + userId];
                if (amount > 0)
                {
                    user.Total += amount;
                    userClient.Store(user);

                    historyList.Prepend(amount);
                    historyList.Trim(0, 4);

                    client.AddItemToSortedSet("urn:leaderboard", user.Name, user.Total);

                }
                ViewBag.HistoryItems = historyList.GetAll();
                ViewBag.UserName = user.Name;
                ViewBag.Total = user.Total;
                ViewBag.Goal = user.Goal;
                ViewBag.UserId = user.Id;
            }

                return View();
        }
    }
}