using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using RestSharp;
using Unit = System.Reactive.Unit;

namespace SwaggerTestNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var rest = new ObservableRest();
            var obs1 = Observable.Interval(TimeSpan.FromMilliseconds(100));

            var sub = new Subject<Unit>();
            obs1.TakeUntil(sub)
                .Subscribe(_ =>
                {
                    var x = rest.Get("http://localhost:3000/math")
                        .Subscribe(y =>
                        {
                            Console.WriteLine(y.Content);
                            if (double.Parse(y.Content) > 8) sub.OnNext(Unit.Default);
                        });
                   

                });

            Console.ReadKey();
        }
    }

    public class ObservableRest
    {
        private RestClient _client;

        public ObservableRest()
        {
            _client = new RestClient();
        }

        public IObservable<IRestResponse> Get(string url)
        {
            _client.BaseUrl = new Uri(url);
            var request = new RestRequest() {Method = Method.GET};
            return Observable.Create<IRestResponse>(o =>
            {
                o.OnNext(_client.Execute(request));

                return () => { };
            });
        }

        public IObservable<IRestResponse> Post(string url, string body = null)
        {
            _client.BaseUrl = new Uri(url);
            var request = new RestRequest() { Method = Method.POST };
            
            if (body != null)
            {
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(body);
            }

            return Observable.Create<IRestResponse>(o =>
            {
                o.OnNext(_client.Execute(request));

                return () => { };
            });
        }
    }
}
