namespace KB.SharpCore.Algorithms;

public static class TopologicalSorter<T>
    where T : notnull
{
    public static IReadOnlyList<T> Sort(IEnumerable<T> roots, Func<T, bool> exists, Func<T, IEnumerable<T>> getDependencies)
    {
        ArgumentNullException.ThrowIfNull(roots);
        ArgumentNullException.ThrowIfNull(exists);
        ArgumentNullException.ThrowIfNull(getDependencies);

        List<T> ordered = new();
        Dictionary<T, VisitState> states = new();

        foreach (T root in roots)
        {
            Visit(root, exists, getDependencies, states, ordered);
        }

        return ordered;
    }

    private static void Visit(
        T node,
        Func<T, bool> exists,
        Func<T, IEnumerable<T>> getDependencies,
        Dictionary<T, VisitState> states,
        List<T> ordered)
    {
        if (!exists(node))
        {
            throw new InvalidOperationException($"NodeNotFound: '{node}'.");
        }

        if (states.TryGetValue(node, out VisitState state))
        {
            if (state == VisitState.Visiting)
            {
                throw new InvalidOperationException($"CircularDependency: '{node}'.");
            }

            return;
        }

        states[node] = VisitState.Visiting;
        foreach (T dependency in getDependencies(node))
        {
            Visit(dependency, exists, getDependencies, states, ordered);
        }

        states[node] = VisitState.Visited;
        ordered.Add(node);
    }

    private enum VisitState
    {
        Visiting,
        Visited,
    }
}