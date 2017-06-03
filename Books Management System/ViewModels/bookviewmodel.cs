using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books_Management_System.ViewModels
{
    class bookviewmodel
    {
        private ObservableCollection<Models.book> allItems = new ObservableCollection<Models.book>();
        public ObservableCollection<Models.book> AllItems { get { return this.allItems; } }

        private Models.book selectedItem = default(Models.book);
        public Models.book SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }
        /// <summary>
        /// 新建一项用来测试
        /// </summary>
        public bookviewmodel()
        {
        }

        public void Add(string bookname, string writer, int totalamount, int currentamount)
        {
            var newbook = new Models.book(bookname, writer, totalamount, currentamount);
            this.allItems.Add(newbook);
        }
        public void Add(string id, string bookname, string writer, int totalamount, int currentamount)
        {
            var newbook = new Models.book(id, bookname, writer, totalamount, currentamount);
            this.allItems.Add(newbook);
        }

        public void RemoveTodoItem(string id)
        {
            this.AllItems.Remove(SelectedItem);
            // set selectedItem to null after remove
            this.selectedItem = null;
        }
        public void RemoveTodoItem()
        {
            this.AllItems.Remove(SelectedItem);
            // set selectedItem to null after remove
            this.selectedItem = null;
        }
        /*
        public void UpdateTodoItem(string id, string title, string description, DateTime date)
        {
            // DIY

            this.SelectedItem.title = title;
            this.SelectedItem.description = description;
            this.SelectedItem.date = date;
            this.selectedItem = null;

        }*/
    }
}
