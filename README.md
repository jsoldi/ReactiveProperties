
# ReactiveProperties

Observable properties for easy binding. It includes a tiny sample WinForms application to give you a quick idea of what it does.


# Quick start

ReactiveProperties consists of two interfaces: `IPropertySource<T>` and `IProperty<T>`. 

`IPropertySource<T>` represents a readonly property that can be subscribed to, just like you'd subscribe to `IObservable<T>` if you were using [Reactive Extensions][1]:

    public interface IPropertySource<out T>
    {
	    T Value { get; }
    	IDisposable RawSubscribe(Action rawObserver);
    }

`IProperty<T>` implements `IPropertySource<T>`, but it also has a setter:

    public interface IProperty<T> : IPropertySource<T>
    {
    	new T Value { get; set; }
    }

(To subscribe, don't use `RawSubstribe` but the [Subscribe][2] extension method.)

It's easy to create a property from any standard property that either has an associated `[PropertyName]Changed` event, or whose declaring class implements [INotifyPropertyChanged][3]. For instance, to create a property given a `TextBox` that represents its text, just use the [FromProperty][4] method:

    IProperty<string> textBoxTextProperty = Property.FromProperty(() => textBox1.Text);

This property can be then observed or it's value retrieved:

    string value = textBoxTextProperty.Value;
    IDisposable subscription = textBoxTextProperty.Subscribe(val => { /* Do something with the value */ });

Every subscription method returns an `IDisposable` that must be disposed as soon as we're done observing it. An easy way to deal with all the disposables is to use the [DisposableSet][5] utility class and add all the disposables in a single call to the [AddRange][6] method (see the [sample application][7]).


# Extension Methods

Since `IPropertySource<T>` is a monad, some Linq operators can be implemented (although I've implemented just a few of them), all of which return another `IPropertySource<T>`. For instance, say we have a `ComboBox` that contains a list of items of class `Person`, which in turn have a property called `NameProperty`. If so, we could create an `IPropertySource<T>` that represents the name of the selected person using the [As][8], [SelectMany][9] and [Return][10] methods:

    IPropertySource<string> currentName = Property
    	.FromProperty(() => comboBox1.SelectedValue)
    	.As<Person>()
    	.SelectMany(person => person == null ? PropertySource.Return("") : person.NameProperty);

In this example, `currentName` will contain the name of the currently selected person, or an empty string if no person is selected, and will notify whenever either the current person or the current person's name changes. By convention, I like to call the property version of a C# property `[Name]Property`, such as in this sample class:

	public class Person
	{
		public readonly IProperty<string> NameProperty;
	
		public string Name
		{
			get { return NameProperty.Value; }
			set { NameProperty.Value = value; }
		}
	}

Properties can also be merged using the [Merge][11] method to make composite properties:

    public class Vector
    {
        public readonly IProperty<double> XProperty;
        public readonly IProperty<double> YProperty;
        public readonly IPropertySource<double> MagnitudeProperty;

        public double X
        {
            get { return XProperty.Value; }
            set { XProperty.Value = value; }
        }

        public double Y
        {
            get { return YProperty.Value; }
            set { YProperty.Value = value; }
        }

        public double Magnitude
        {
            get { return MagnitudeProperty.Value; }
        }

        public Vector()
        {
            XProperty = Property.FromValue(0.0);
            YProperty = Property.FromValue(0.0);
            MagnitudeProperty = XProperty.Merge(YProperty, (x, y) => Math.Sqrt(x * x + y * y));
        }
	}

In this example, changing any of the X or Y properties will cause (if the actual value of the magnitude changes) `MagnitudeProperty` to notify a subscriber, if any. Alternatively, the `Magnitude` property can also have a setter if `MagnitudeProperty` is an `IProperty<double>` defined using Property's [Create][12] method:

    MagnitudeProperty = Property.Create(
        XProperty.Merge(YProperty, (x, y) => Math.Sqrt(x * x + y * y)), 
        mag =>
        {
            var magnitude = Magnitude;
            X *= mag / magnitude;
            Y *= mag / magnitude;
        }
    );

`IProperty<T>` also has a [SelectMany][13] and a variation of the [Select][14] method (which also take a backwards selector), but since it cannot have a Return method (it wouldn't make any sense), it is not a monad and therefore not as extensible as `IPropertySource<T>`. 


  [1]: https://msdn.microsoft.com/en-us/data/gg577609
  [2]: https://github.com/jsoldi/ReactiveProperties/blob/355dcfd27824d9f90a2e40539efc6f0343132dd0/Core/PropertySourceExtensions.cs#L33
  [3]: https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
  [4]: https://github.com/jsoldi/ReactiveProperties/blob/355dcfd27824d9f90a2e40539efc6f0343132dd0/Assignable/Property.cs#L102
  [5]: https://github.com/jsoldi/ReactiveProperties/blob/355dcfd27824d9f90a2e40539efc6f0343132dd0/Utils/DisposableSet.cs
  [6]: https://github.com/jsoldi/ReactiveProperties/blob/355dcfd27824d9f90a2e40539efc6f0343132dd0/Utils/DisposableSet.cs#L35
  [7]: https://github.com/jsoldi/ReactiveProperties/blob/7c121a1329a499d58a9bc460a071c9b6b0d87fbc/WinFormsSample/Form1.cs#L45
  [8]: https://github.com/jsoldi/ReactiveProperties/blob/355dcfd27824d9f90a2e40539efc6f0343132dd0/Core/PropertySourceLinq.cs#L146
  [9]: https://github.com/jsoldi/ReactiveProperties/blob/355dcfd27824d9f90a2e40539efc6f0343132dd0/Core/PropertySourceLinq.cs#L95
  [10]: https://github.com/jsoldi/ReactiveProperties/blob/355dcfd27824d9f90a2e40539efc6f0343132dd0/Core/PropertySourceLinq.cs#L12
  [11]: https://github.com/jsoldi/ReactiveProperties/blob/355dcfd27824d9f90a2e40539efc6f0343132dd0/Core/PropertySourceLinq.cs#L154
  [12]: https://github.com/jsoldi/ReactiveProperties/blob/355dcfd27824d9f90a2e40539efc6f0343132dd0/Assignable/Property.cs#L10
  [13]: https://github.com/jsoldi/ReactiveProperties/blob/355dcfd27824d9f90a2e40539efc6f0343132dd0/Assignable/PropertyExtensions.cs#L12
  [14]: https://github.com/jsoldi/ReactiveProperties/blob/574eb022526bdaad520fcbcca947be9bc469423d/Assignable/PropertyExtensions.cs#L20
