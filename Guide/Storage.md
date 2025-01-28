# Storage

Foster come with built-in data storage system, which itself is a top-layer from the storage API of SDL3. 

## Game data and Assets
```csharp
Storage.OpenTitleStorage("path/to/data", (Storage storage) =>
{
    Log.Info("Data is now available");
});
```