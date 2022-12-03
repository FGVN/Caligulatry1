using System;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;

namespace Caligulatry1
{
    class QueueService
    {

        private string JsonFileName(long chatId)
        {
            return Environment.CurrentDirectory.ToString().Split("\\bin")[0] + "\\data\\" + chatId + ".json"; 
        }

        private void CreateJson(string path)
        {
            using (FileStream fs = File.Create(path))
            {

            }

            using var outputStream = File.OpenWrite(path);
            JsonSerializer.Serialize<IEnumerable<Queue>>(
            new Utf8JsonWriter(outputStream, new JsonWriterOptions
            {
                SkipValidation = true,
                Indented = true
            }),
                new List<Queue>()
            );
        }

        public bool AddQueue(Queue queue)
        {
            var data = GetQueues(queue._chatId);

            if (data.Select(x=> x._listName).Contains(queue._listName) || queue._listName == "") {
                return false;
            }

            Console.WriteLine(queue);

            data.Add(queue);

            Console.WriteLine("\nIn list:\n");
            foreach(var item in data)
            {
                Console.WriteLine(item);
            }

            using var outputStream = File.OpenWrite(JsonFileName(queue._chatId));

            JsonSerializer.Serialize<IEnumerable<Queue>>(
                new Utf8JsonWriter(outputStream, new JsonWriterOptions
                {
                    SkipValidation = true,
                    Indented = true
                }),
                data
            );
            return true;
        }
        public List<Queue> GetQueues(long chatId)
        {
            if (!File.Exists(JsonFileName(chatId)))
            {
                CreateJson(JsonFileName(chatId));
            }

            using var jsonFileReader = File.OpenText(JsonFileName(chatId));
            var result = JsonSerializer.Deserialize<List<Queue>>(jsonFileReader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            if (result != null)
                return result;
            return new List<Queue>();
        }

        public bool AddUser(long chatId, string listName, string username)
        {
            List<Queue> data = GetQueues(chatId);

            if (data.FirstOrDefault(x => x._listName == listName) == null || data.FirstOrDefault(x => x._listName == listName)._users.Contains(username))
            {
                return false;
            }

            data.First(x => x._listName == listName)._users.Add(username);

            using var outputStream = File.OpenWrite(JsonFileName(chatId));

            JsonSerializer.Serialize<IEnumerable<Queue>>(
                new Utf8JsonWriter(outputStream, new JsonWriterOptions
                {
                    SkipValidation = true,
                    Indented = true
                }),
                data
            );
            return true;
        }

        public bool RemoveUser(long chatId, string listName, string username)
        {
            List<Queue> data = GetQueues(chatId);

            if (!data.SelectMany(x => x._users).Contains(username) || !data.Select(x => x._listName).Contains(listName))
            {
                return false;
            }

            data.First(x => x._listName == listName)._users.Remove(username);

            File.Delete(JsonFileName(chatId));

            CreateJson(JsonFileName(chatId));

            using var outputStream = File.OpenWrite(JsonFileName(chatId));

            JsonSerializer.Serialize<IEnumerable<Queue>>(
                new Utf8JsonWriter(outputStream, new JsonWriterOptions
                {
                    SkipValidation = true,
                    Indented = true
                }),
                data
            );
            return true;
        }
    }
}
