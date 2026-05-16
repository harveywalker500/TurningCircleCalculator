using System.Reflection;

namespace TurningCircleCalculator.Modules;

public class ModuleRegistry
{
    private readonly List<ICalculatorModule> _modules = new();

    public IReadOnlyList<ICalculatorModule> Modules => _modules.AsReadOnly();

    public ModuleRegistry Register(ICalculatorModule module)
    {
        if (_modules.Any(m => m.Id == module.Id))
            throw new InvalidOperationException($"Module '{module.Id}' is already registered.");
        _modules.Add(module);
        return this;
    }

    /// <summary>
    /// Scans an assembly for all non-abstract ICalculatorModule implementations
    /// and registers them in the order they are discovered.
    /// </summary>
    public ModuleRegistry ScanAssembly(Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.IsClass && typeof(ICalculatorModule).IsAssignableFrom(t))
            .OrderBy(t => t.FullName);

        foreach (var type in types)
        {
            var instance = (ICalculatorModule)Activator.CreateInstance(type)!;
            if (_modules.All(m => m.Id != instance.Id))
                _modules.Add(instance);
        }
        return this;
    }
}
