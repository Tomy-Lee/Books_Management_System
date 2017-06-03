using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books_Management_System.Models
{
    class book
    {
        private string id;
        public string bookname { get; set; }

        public string writer { get; set; }

        public int totalamount { get; set; }
        public int currentamount { get; set; }

        public DateTime date { get; set; }

        public string getid()
        {
            return id;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bookname"></param>
        /// <param name="writer"></param>
        /// <param name="totalamount"></param>
        /// <param name="currentamount"></param>
        public book( string bookname, string writer, int totalamount, int currentamount)
        {
            this.id = Guid.NewGuid().ToString(); 
            this.bookname = bookname;
            this.writer= writer;
            this.totalamount = totalamount;
            this.currentamount = currentamount;
            // this.completed = false; //默认为未完成
            var db = App.conn;
            try
            {
                using (var book = db.Prepare("INSERT INTO book ( bookname, writer, totalamount,currentamount) VALUES (?, ?, ?, ?)"))
                {
                    book.Bind(1, this.bookname);
                    book.Bind(2, this.writer);
                    book.Bind(3, this.totalamount);
                    book.Bind(4, this.currentamount);
                    book.Step();
                }
            }
            catch (Exception)
            { }

        }
        public book(string id,string bookname, string writer, int totalamount, int currentamount)
        {
            this.id = Guid.NewGuid().ToString();
            this.bookname = bookname;
            this.writer = writer;
            this.totalamount = totalamount;
            this.currentamount = currentamount;
           

        }
    }
}
