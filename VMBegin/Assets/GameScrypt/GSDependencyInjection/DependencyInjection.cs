using Game.Main;
using Game.Shared.Enums;
using Game.Shared.Interfaces;
using System;
using System.Collections.Generic;

namespace GameScrypt.GSDependencyInjection
{
    public static class DependencyInjection
    {
        public static Dictionary<Dependency, Action> DependencyRequest
            = new Dictionary<Dependency, Action>();
        private static Dictionary<Dependency, Action<Dependency, IDependency>> _injectDependency
            = new Dictionary<Dependency, Action<Dependency, IDependency>>();
        private static Dictionary<Dependency, bool> _dependencies
            = new Dictionary<Dependency, bool>();
        private static bool _initialized;

        public static void Reset()
        {
            _initialized = false;
            DependencyRequest.Clear();

            _dependencies.Clear();
            _injectDependency.Clear();
        }

        public static void Init()
        {
            _initialized = true;
            Main._.OnGameQuit = Reset;
        }

        public static void DeclareDependencies(List<Dependency> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                if (_dependencies.ContainsKey(dependency)) { continue; }

                _dependencies.Add(dependency, true);
                Main._.CreateDependency(dependency);
            }

            if (_initialized) { return; }
            Init();
        }

        public static void ListenForDependencies(List<Dependency> dependencies, Action<Dependency, IDependency> injectDependency)
        {
            foreach (var dependency in dependencies)
            {
                if (_injectDependency.ContainsKey(dependency) == false)
                {
                    _injectDependency.Add(dependency, injectDependency);
                }
                else
                {
                    _injectDependency[dependency] += injectDependency;
                }
            }
        }

        public static void UnsubscribeFromDependency(Dependency dependency, Action<Dependency, IDependency> injectDependency)
        {
            _injectDependency[dependency] -= injectDependency;
        }

        public static void RequestDependencies(List<Dependency> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                if (DependencyRequest.ContainsKey(dependency))
                    DependencyRequest[dependency]?.Invoke();
            }
        }

        public static void InjectDependency(Dependency dependency, IDependency dependencyInstance)
        {
            _injectDependency[dependency]?.Invoke(dependency, dependencyInstance);
        }
    }
}
