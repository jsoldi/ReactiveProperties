using ReactiveProperties;
using ReactiveProperties.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsSample
{
    public partial class Form1 : Form
    {
        private readonly BindingList<Book> Books = new BindingList<Book>();
        private readonly DisposableSet DisposableSet = new DisposableSet();
        private readonly IProperty<Book> CurrentBook;

        public Form1()
        {
            InitializeComponent();

            // A dummy book to be used when the book list is empty.
            var dummyBook = new Book();

            // Create an IProperty<Book> that represents the book currently selected in the booksComboBox, or dummyBook of none is selected.
            CurrentBook = Property.Create(
                PropertySource.FromProperty(() => booksComboBox.SelectedValue).Select(book => book as Book ?? dummyBook),
                book => booksComboBox.SelectedItem = book
            );

            // Bind booksComboBox to the Books list. If I don't use a BindingSource it'll fail to raise SelectedValueChanged when the data source changes for some reason.
            var booksBindingSource = new BindingSource();
            booksBindingSource.DataSource = Books;
            booksComboBox.DataSource = booksBindingSource;
            booksComboBox.DisplayMember = "Name";

            // Read / Write properties that represent each of the the currently selected book's properties.
            var currentName = CurrentBook.SelectMany(book => book.NameProperty);
            var currentAuthorName = CurrentBook.SelectMany(book => book.AuthorNameProperty);
            var currentAuthorLastName = CurrentBook.SelectMany(book => book.AuthorLastNameProperty);
            var currentRating = CurrentBook.SelectMany(book => book.RatingProperty);

            // Hook everything up
            DisposableSet.AddRange(

                // Adding this just because it also needs to be disposed
                booksBindingSource,

                // Disable input controls if the current book is dummyBook (which means np book is currently selected because the book list is empty)
                CurrentBook.Select(book => object.ReferenceEquals(book, dummyBook)).Subscribe(isDummy =>
                    nameTextBox.Enabled = authorNameTextBox.Enabled = authorLastNameTextBox.Enabled = ratingTrackBar.Enabled = removeButton.Enabled = !isDummy
                ),

                // Bind two way all of the input controls with their respective properties. 
                // When the input controls change, they'll update the current book's corresponding properties.
                // When the current book changes, the current values will change and update the input controls.
                // If the current book is dummyBook they'll be disabled so dummyBook will never be edited.
                Property.FromProperty(() => nameTextBox.Text).TwoWayBind(currentName),
                Property.FromProperty(() => authorNameTextBox.Text).TwoWayBind(currentAuthorName),
                Property.FromProperty(() => authorLastNameTextBox.Text).TwoWayBind(currentAuthorLastName),
                Property.FromProperty(() => ratingTrackBar.Value).TwoWayBind(currentRating),

                // Bind the bookNameLabel's Text to the current book's name.
                currentName.Subscribe(name => bookNameLabel.Text = name),

                // Bind the authorNameLabel's text to a combination of the current author name and last name.
                currentAuthorName
                    .Merge(currentAuthorLastName, (l, r) => string.Format("By {0} {1}", l, r))
                    .Subscribe(full => authorNameLabel.Text = full),

                // Bind the current rating image to the current book's rating.                    
                currentRating.Subscribe(rating => starPictureBox.Image = starsImageList.Images[rating])
            );

            // Add some sample books.
            Books.Add(new Book("To Kill A Mockingbird", "Harper", "Lee", 5));
            Books.Add(new Book("A Tale Of Two Cities", "Charles", "Dickens", 4));
            Books.Add(new Book("Nineteen Eighty-four", "George", "Orwell", 4));
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var book = new Book();
            Books.Add(book);
            CurrentBook.Value = book;
            nameTextBox.Focus();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            Books.Remove(CurrentBook.Value);
        }
    }
}
