
# ReactiveProperties

Observable properties for easy data binding. It includes a tiny sample WinForms application to give you a quick idea of what it does.


# Quick start

ReactiveProperties consists of two interfaces: `IPropertySource<T>` and `IProperty<T>`. 

`IPropertySource<T>` represents a readonly property that can be subscribed to, just like you'd subscribe to `IObservable<T>` if you were using [Reactive Extensions][1]:

```csharp
public interface IPropertySource<out T>
{
    T Value { get; }
    IDisposable RawSubscribe(Action rawObserver);
}
```

`IProperty<T>` implements `IPropertySource<T>`, but it also has a setter:

```csharp
public interface IProperty<T> : IPropertySource<T>
{
    new T Value { get; set; }
}
```

(To subscribe, don't use `RawSubstribe` but the [Subscribe][2] extension method.)

It's easy to create a property from any standard property that either has an associated `[PropertyName]Changed` event, or whose declaring class implements [INotifyPropertyChanged][3]. For instance, to create a property given a `TextBox` that represents its text, just use the [FromProperty][4] method:

```csharp
IProperty<string> textBoxTextProperty = Property.FromProperty(() => textBox1.Text);
```
This property can be then observed or it's value retrieved:

```csharp
string value = textBoxTextProperty.Value;
IDisposable subscription = textBoxTextProperty.Subscribe(val => { /* Do something with the value */ });
```

Every subscription method returns an `IDisposable` that must be disposed as soon as we're done observing it. An easy way to deal with all the disposables is to use the [DisposableSet][5] utility class and add all the disposables in a single call to the [AddRange][6] method (see the [sample application][7]).


# Extension Methods

Since `IPropertySource<T>` is a [monad][8], some Linq operators can be implemented (although I've implemented just a few of them), all of which return another `IPropertySource<T>`. For instance, say we have a `ComboBox` that contains a list of items of class `Person`, which in turn have a property called `NameProperty`. If so, we could create an `IPropertySource<T>` that represents the name of the selected person using the [As][9], [SelectMany][10] and [Return][11] methods:

```csharp
IPropertySource<string> currentName = Property
    .FromProperty(() => comboBox1.SelectedValue)
    .As<Person>()
    .SelectMany(person => person == null ? PropertySource.Return("") : person.NameProperty);
```

In this example, `currentName` will contain the name of the currently selected person, or an empty string if no person is selected, and will notify whenever either the current person or the current person's name changes. By convention, I like to call the property version of a C# property `[Name]Property`, such as in this sample class:

```csharp
public class Person
{
	public readonly IProperty<string> NameProperty;

	public string Name
	{
		get { return NameProperty.Value; }
		set { NameProperty.Value = value; }
	}
}
```

Properties can also be merged using the [Merge][12] method to make composite properties:

```csharp
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
```

In this example, changing any of the X or Y properties will cause (if the actual value of the magnitude changes) `MagnitudeProperty` to notify a subscriber, if any. Alternatively, the `Magnitude` property can also have a setter if `MagnitudeProperty` is an `IProperty<double>` defined using Property's [Create][13] method:

```csharp
MagnitudeProperty = Property.Create(
	XProperty.Merge(YProperty, (x, y) => Math.Sqrt(x * x + y * y)), 
	newMagnitude =>
	{
		var scale = newMagnitude / Magnitude;
		X *= scale;
		Y *= scale;
	}
);
```

`IProperty<T>` also has a [SelectMany][14] and a variation of the [Select][15] method (which also take a backwards selector), but since it cannot have a Return method (it wouldn't make any sense), it is not a monad and therefore not as extensible as `IPropertySource<T>`. 


  [1]: https://msdn.microsoft.com/en-us/data/gg577609
  [2]: https://github.com/jsoldi/ReactiveProperties/blob/8075dbe0d18e375c98fef80d8acc6df64aa39598/Core/PropertySourceExtensions.cs#L55
  [3]: https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
  [4]: https://github.com/jsoldi/ReactiveProperties/blob/8075dbe0d18e375c98fef80d8acc6df64aa39598/WinForms/Property.cs#L44
  [5]: https://github.com/jsoldi/ReactiveProperties/blob/master/Utils/DisposableSet.cs
  [6]: https://github.com/jsoldi/ReactiveProperties/blob/8075dbe0d18e375c98fef80d8acc6df64aa39598/Utils/DisposableSet.cs#L50
  [7]: https://github.com/jsoldi/ReactiveProperties/blob/8075dbe0d18e375c98fef80d8acc6df64aa39598/WinFormsSample/Form1.cs#L47
  [8]: http://stackoverflow.com/questions/2704652/monad-in-plain-english-for-the-oop-programmer-with-no-fp-background/2704795#2704795
  [9]: https://github.com/jsoldi/ReactiveProperties/blob/8075dbe0d18e375c98fef80d8acc6df64aa39598/Core/PropertySourceLinq.cs#L207
  [10]: https://github.com/jsoldi/ReactiveProperties/blob/8075dbe0d18e375c98fef80d8acc6df64aa39598/Core/PropertySourceLinq.cs#L136
  [11]: https://github.com/jsoldi/ReactiveProperties/blob/8075dbe0d18e375c98fef80d8acc6df64aa39598/Core/PropertySourceLinq.cs#L18
  [12]: https://github.com/jsoldi/ReactiveProperties/blob/8075dbe0d18e375c98fef80d8acc6df64aa39598/Core/PropertySourceLinq.cs#L225
  [13]: https://github.com/jsoldi/ReactiveProperties/blob/8075dbe0d18e375c98fef80d8acc6df64aa39598/Assignable/Property.cs#L17
  [14]: https://github.com/jsoldi/ReactiveProperties/blob/8075dbe0d18e375c98fef80d8acc6df64aa39598/Assignable/PropertyExtensions.cs#L22
  [15]: https://github.com/jsoldi/ReactiveProperties/blob/8075dbe0d18e375c98fef80d8acc6df64aa39598/Assignable/PropertyExtensions.cs#L39
