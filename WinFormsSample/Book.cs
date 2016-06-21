using ReactiveProperties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsSample
{
    // Implement INotifyPropertyChanged so it can be used as a ComboBox's DataSource
    public class Book : INotifyPropertyChanged
    {
        public readonly IProperty<string> NameProperty;
        public readonly IProperty<string> AuthorNameProperty;
        public readonly IProperty<string> AuthorLastNameProperty;
        public readonly IProperty<int> RatingProperty;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return NameProperty.Value; }
            set { NameProperty.Value = value; }
        }

        public string AuthorName
        {
            get { return AuthorNameProperty.Value; }
            set { AuthorNameProperty.Value = value; }
        }

        public string AuthorLastName
        {
            get { return AuthorLastNameProperty.Value; }
            set { AuthorLastNameProperty.Value = value; }
        }

        public int Rating
        {
            get { return RatingProperty.Value; }
            set { RatingProperty.Value = value; }
        }

        public Book()
        {
            NameProperty = Property.FromValue<string>("", () => Name, OnPropertyChanged);
            AuthorNameProperty = Property.FromValue<string>("", () => AuthorName, OnPropertyChanged);
            AuthorLastNameProperty = Property.FromValue<string>("", () => AuthorLastName, OnPropertyChanged);
            RatingProperty = Property.FromValue(0, () => Rating, OnPropertyChanged);
        }

        public Book(string name, string authorName, string authorLastName, int rating)
            : this()
        {
            Name = name;
            AuthorName = authorName;
            AuthorLastName = authorLastName;
            Rating = rating;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var evt = PropertyChanged;
            if (evt != null)
                evt(this, e);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
