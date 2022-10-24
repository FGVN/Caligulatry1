using System;
using System.Collections.Generic;
using System.Text;

namespace Caligulatry1
{
    class Queue
    {
        public long _chatId { get; set; }
        public string _creator { get; set; }
        public string _listName { get; set; }
        public DateTime _date { get; set; }
        public List<string> _users { get; set; }

        public Queue(long chatId, string creator, string listName, DateTime date)
        {
            _chatId = chatId;
            _creator = creator;
            _listName = listName;
            _date = date;
            _users = new List<string>();
        }

        public Queue()
        {
            _chatId = 0;
            _creator = null;
            _listName = null;
            _date = DateTime.MinValue;
            _users = new List<string>();
        }

        public override string ToString()
        {
            string usersStr = "\n";
            for(int i = 0; i < _users.Count; i++)
            {
                usersStr +=  i +1 + " - " + _users[i].ToString() + "\n";
            }
            return _listName + " by " + _creator + "\n" + "Due to: " + _date + "\n" + usersStr;
        }
    }
}
