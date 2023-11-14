using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Common.Util
{
    public partial class NewsGenerator<T>
    {
        // 위임형 선언 
        public delegate void NewsReportEventHandler(object sender, NewsReportEventArgs<T> args);

        // 이벤트 인자 정의
        public class NewsReportEventArgs<Type> : EventArgs
        {
            public NewsReportEventArgs(Type news)
            {
                this._News = news;
            }

            // 이벤트 인자 내에서 쓸 목록 
            public readonly Type _News;
        }        

        // 이벤트 정의 
        private NewsReportEventHandler _newsReport = null;

        // 이벤트 등록 메쏘드
        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public void add_NewsReportEventHandler(NewsReportEventHandler handler)
        {
            this._newsReport = (NewsReportEventHandler)Delegate.Combine(this._newsReport, handler);
        }

        // 이벤트 등록해제 메쏘드 
        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public void remove_NewsReportEventHandler(NewsReportEventHandler handler)
        {
            this._newsReport = (NewsReportEventHandler)Delegate.Remove(this._newsReport, handler);
        }

        // 이벤트를 발생시키는 함수 
        protected virtual void OnNewsReport(NewsReportEventArgs<T> e)
        {
            if (this._newsReport != null)
            {
                this._newsReport(this, e);
            }
        }

        // 이벤트 발생을 위해 테스트용 함수 
        public void SimulateNewsEvent(T news)
        {
            NewsReportEventArgs<T> e = new NewsReportEventArgs<T>(news);
            this.OnNewsReport(e);
        }
    }
    
    public class NewsCollector<T>
    {
        private List<NewsGenerator<T>> _NewsGeneratorList = null;

        public NewsCollector()
        {
            this._NewsGeneratorList = new List<NewsGenerator<T>>();
        }

        ~NewsCollector()
        {
            if (this._NewsGeneratorList != null)
            {
                this._NewsGeneratorList.Clear();
                this._NewsGeneratorList = null;
            }
        }

        public void Add_NewsGenerator(NewsGenerator<T> newsGenerator, NewsGenerator<T>.NewsReportEventHandler eventHandler)
        {   
            newsGenerator.add_NewsReportEventHandler(eventHandler);
            this._NewsGeneratorList.Add(newsGenerator);
        }

        public void Remove_NewsGenerator(NewsGenerator<T> newsGenerator, NewsGenerator<T>.NewsReportEventHandler eventHandler)
        {
            newsGenerator.remove_NewsReportEventHandler(eventHandler);
            this._NewsGeneratorList.Remove(newsGenerator);
        }
    }

    public partial class ClassTemp
    {
        [STAThread]
        public static void Main(string[] args)
        {
            NewsGenerator<string> newsGenerator1 = new NewsGenerator<string>();
            NewsCollector<string> newsCollector1 = new NewsCollector<string>();
            newsCollector1.Add_NewsGenerator(newsGenerator1, newsCollectorCallback1);

            NewsGenerator<int> newsGenerator2 = new NewsGenerator<int>();
            NewsGenerator<int> newsGenerator3 = new NewsGenerator<int>();
            NewsCollector<int> newsCollector2 = new NewsCollector<int>();
            newsCollector2.Add_NewsGenerator(newsGenerator2, newsCollectorCallback2);
            newsCollector2.Add_NewsGenerator(newsGenerator3, newsCollectorCallback2);

            newsGenerator1.SimulateNewsEvent("SimulateNewsEvent Test"); // Input Event

            newsGenerator2.SimulateNewsEvent(1); // Input Event
            newsGenerator3.SimulateNewsEvent(2); // Input Event
        }
        
        public static void newsCollectorCallback1(object sender, NewsGenerator<string>.NewsReportEventArgs<string> e)
        {
            Console.WriteLine("{0}", e._News);
        }

        public static void newsCollectorCallback2(object sender, NewsGenerator<int>.NewsReportEventArgs<int> e)
        {
            Console.WriteLine("{0}", e._News);
        }
    }
}