# IPromise
A simple promise library for .NET

## Usage

### Typical promise
```csharp
var promise = new Promise<string>((fulfill, failure) => 
{
  var response, error = API.Call("");
  if (error != null)
  {
    failure(error);
  }
  else 
  {
    fulfill(response);
  }
});
```

### Completed Promise
```csharp
Promise<int> promise = Promise.Of(3);
```

### Pending Promise
```csharp
Promise<int> promise = Promise.Pending<int>();
if (pass)
  promise.Fullfil(3);
else
  promise.Reject(new Exception());
```

### Then Chaining
```csharp
Promise.Of(3)
  .Then(x => x++)
  .Then(x => x++)
  .Then(x => x++);
```

### Catching Exceptions
```
Promise.Of("Three")
  .Then(x => double.Parse(x))
  .Catch(e => Console.WriteLine(e.Message);
```

## Future Features
* Timeout
* `All`
* `Always`
* `Race`
* Support `Then` and `Catch` on a `PromiseQueue`
