# GoogolDB

A (not yet) comprehensive googology database collecting data about googologisms, notations, and googologists (people studying the art of googology)

## Synopsis

```csharp
namespace GoogolSharp.Database;
// CloudGoogolDB Implements this interface.
public interface IGoogolDB
{
    public string InventorOfNumberWithNameAndDefinition(string name, string definition);
    public string[] NumberNamesFromInventor(string inventor);
    public string[] NumberDefinitionsFromInventor(string inventor);
    public string[] NumberDefinitionsFromNumberName(string name);
}
```