using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace restClient
{
    class Program
    {
        public static async Task Main()
        {
            RestApi rest = new RestApi("http://127.0.0.1:3000");

            //functions for the selected options
            void ShowMessages()
            {
                rest.Print();
            }

            void End()
            {
                Console.WriteLine("Ending");
                Environment.Exit(0);
            }

            async void InsertMsg()
            {
                Console.WriteLine("Insert Message:");
                string input = Console.ReadLine();
                await rest.PostRequest(input);
            }

            async void UpdateMsg()
            {
                Console.WriteLine("Update Message:");
                Console.WriteLine("Post ID");
                string input = Console.ReadLine();
                int output;
                if (Int32.TryParse(input, out output))
                {
                    Console.WriteLine("Write updated message:");
                    string msg = Console.ReadLine();
                    await rest.PutRequest(output.ToString(), msg);
                }
                else
                {
                    Console.WriteLine("Failure updating message");
                }
            }

            async void DeleteMsg()
            {
                Console.WriteLine("Delete Message:");
                Console.WriteLine("Delete ID");
                string input = Console.ReadLine();
                int output;
                if (Int32.TryParse(input, out output))
                {
                    await rest.DeleteRequest(output.ToString());
                }
                else
                {
                    Console.WriteLine("Failure deleting message");
                }
            }
            //rest.Print();
            //await rest.PostRequest("http://127.0.0.1:3000/csharpista");
            //await rest.PutRequest("http://127.0.0.1:3000/5", "asdasdasdasd");
            //await rest.DeleteRequest("http://127.0.0.1:3000/5");

            //List<Action<string>> list = new List<Action<string>>();
            //List<Delegate> list = new List<Delegate>();
            //Action EndProgram = () => End();
            //Action<string> PostMsg = async(message) => await rest.PostRequest($"http://127.0.0.1:3000/{message}");
            List<Action> list = new List<Action>();
            list.Add(End);
            list.Add(ShowMessages);
            list.Add(InsertMsg);
            list.Add(UpdateMsg);
            list.Add(DeleteMsg);
            //list.Add(PostMsg);
            //list.Add(ShowMessages);
            //list.Add(()=>InsertMsg("Viesti listalta"));
            //list.Add(async (ls) => await rest.PostRequest(ls));
            //list[0].Invoke();
            //list.Add(rest.DeleteRequest);
            //Console.WriteLine(lista[1].message);

            //Dictionary<int, Action> dict = new Dictionary<int, Action>();
            //Action<int> val = (val) => ;
            Action<string> ln = (line) => Console.WriteLine(line);

            while (true)
            {
                await rest.GetRequest();
                ln("select option");
                ln("Show messages: 1");
                ln("Insert new message: 2");
                ln("Update message: 3");
                ln("Delete message: 4");
                ln("End: 0");
                string line = Console.ReadLine();

                int selection;

                if (Int32.TryParse(line, out selection))
                {
                    if (selection > -1 && selection < list.Count)
                    {
                        list[selection]();
                    }
                    else
                    {
                        ln("Invalid Selection");
                    }
                }
                else
                {
                    ln("Invalid Selection");
                }
            }


        }
    }

    public class Message
    {
        public int id;
        public string message;
        public string created_at;
    }

    static class Jaasson
    {
        public static List<Message> MessageList(string str)
        {
            List<Message> msgList = new List<Message>();
            JArray jsonArray = JArray.Parse(str);

            foreach (JObject obj in jsonArray.Children<JObject>())
            {
                Message msg = obj.ToObject<Message>();
                msgList.Add(msg);
            }
            return msgList;
        }

    }



    class RestApi
    {
        public string url;
        public string responseBody;
        HttpResponseMessage response;
        static readonly HttpClient client = new HttpClient();

        public RestApi(string url)
        {
            this.url = url;
        }
        public async Task<string> GetRequest()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);
                response.Dispose();
                //Console.WriteLine(responseBody);
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return e.ToString();
            }
        }


        public List<Message> GetToList()
        {
            return Jaasson.MessageList(responseBody); ;
        }

        public void Print()
        {
            List<Message> lista = GetToList();

            foreach (Message msg in lista)
            {
                Console.WriteLine($"Message id: {msg.id} | {msg.message} | Timestamp: {msg.created_at}");
            }

        }

        public async Task PostRequest(string msg)
        {
            HttpContent content = new StringContent("", Encoding.UTF8, "application/json");
            await client.PostAsync(url + "/" + msg, content);
        }

        public async Task PutRequest(string id, string msg)
        {
            HttpContent content = new StringContent("", Encoding.UTF8, "application/json");
            await client.PutAsync($"{url}/{id}?message={msg}", content);
        }

        public async Task DeleteRequest(string id)
        {
            await client.DeleteAsync(url + "/" + id);
        }
    }



    /*


    class RestApi
    {
        public string url;
        public WebRequest request;
        HttpWebResponse response;
        public Stream dataStream;
        public StreamReader reader;

        public RestApi(string url)
        {
            this.url = url;
            Init();
        }

        public void Init()
        {
            try
            {
                request = WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public List<Message> GetRequest()
        {
            string responseString = reader.ReadToEnd();
            return Jaasson.MessageList(responseString); ;
        }

        public string PostRequest()
        {
            
            return "asd";
        }
    }
    */
}

