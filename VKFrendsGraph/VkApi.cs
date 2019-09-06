using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;

namespace VKFrendsGraph
{
    public class VkApi
    {
        //Меню для доступу до прогрес бара  і прапор д
        private readonly MainWindow _main;
        private bool _progressflag;
        //Клас для отримання результату API запиту
        private XDocument _documet;
        //Список
        private readonly Dictionary<NumbersId, List<Frends>> _dictionary = new Dictionary<NumbersId, List<Frends>>();
        //Глибина пошуку
        private readonly int _deep;
        private int _thisdeep = 1;
        //конструктор
        public VkApi(int deep, MainWindow main)
        {
            _main = main;
            _deep = deep;

        }
        // Властивість для доступа до списку
        public Dictionary<NumbersId, List<Frends>> Dictionary
        {
            get { return _dictionary; }
        }

        public bool Progressflag
        {
            set { _progressflag = value; }
        }


        // Формування списку друзів
        public void GetFrends(string userId, string not)
        {
            _documet = XDocument.Load(UrlFriendsGet(userId));

            if (_documet != null)
            {
                if (_documet.Root != null)
                {
                    List<Frends> templist =
                        (
                            from el0 in _documet.Root.Elements("items")
                            from el in el0.Elements("user")
                            where el.Elements("id").First().Value != not
                            select
                                new Frends
                                {
                                    Id = el.Elements("id").First().Value,
                                    Name = el.Elements("first_name").First().Value,
                                    Surname = el.Elements("last_name").First().Value
                                }
                            ).ToList();

                    _dictionary.Add(GetUsers(userId), templist);
                    Progress();
                }
            }
            if (_deep > _thisdeep)
            {
                foreach (var item in _dictionary)
                {
                    if (item.Key.Red == false && item.Key.Deep == _thisdeep)
                    {
                        _thisdeep++;
                        item.Key.Red = true;
                        foreach (var item2 in item.Value)
                        {
                            GetFrends(item2.Id, item.Key.Id);
                        }
                        _thisdeep--;
                        break;
                    }
                }

            }
        }
        // Отримання інформації про користувача і створення об'єкта користувача
        private NumbersId GetUsers(string id)
        {
            _documet = XDocument.Load(UrlUsersGet(id));
            if (_documet.Root != null)
                return _documet.Root.Elements("user").Select(el => new NumbersId
                {
                    Id = id, Deep = _thisdeep, Red = false, Name = el.Elements("first_name").First().Value, Surname = el.Elements("last_name").First().Value
                }).FirstOrDefault();
            return null;
        }
        // Формування строки запиту для отримання списку друзів користувача
        private string UrlFriendsGet(string id)
        {
            // 2019 : https://api.vk.com/method/friends.get.xml?user_id=166780639&fields=nickname&v=5.8&access_token=8630945b8630945b8630945beb865c2e41886308630945bdb42b5e4fcd4adf214bb1d1e
            const string queryFormatString = "https://api.vk.com/method/friends.get.xml?user_id={0}&fields=nickname&v=5.8&access_token=8630945b8630945b8630945beb865c2e41886308630945bdb42b5e4fcd4adf214bb1d1e";
            return string.Format(queryFormatString, id);
        }
        // Формування строки запиту для отримання інформації про користувача
        private string UrlUsersGet(string id)
        {
            const string queryFormatString = "https://api.vk.com/method/users.get.xml?user_ids={0}&v=5.8&access_token=8630945b8630945b8630945beb865c2e41886308630945bdb42b5e4fcd4adf214bb1d1e";
            return string.Format(queryFormatString, id);
        }
        // Функція виводу прогреса
        private void Progress()
        {
            if (_progressflag == false)
            {
                int t = _dictionary.Values.First().Count;
                _main.Bar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => _main.Bar.Visibility = Visibility.Visible));
                _main.Bar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => _main.Bar.Maximum = t));
                _progressflag = true;
            }
            _main.Bar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => _main.Bar.Value = _dictionary.Count));
        }


        private string f = "";

    }
}
