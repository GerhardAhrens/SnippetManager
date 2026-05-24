namespace System.Windows
{
    using System;
    using System.Collections.Concurrent;

    public static class Factory
    {
        // Registrierung:
        // (EnumTyp, EnumWert) -> Registrierung
        private static readonly ConcurrentDictionary<(Type EnumType, object Key), Registration> _registrations = new();

        // Singleton Cache
        private static readonly ConcurrentDictionary<(Type EnumType, object Key), Lazy<object>> _singletons = new();

        // Registrierungseintrag
        private sealed class Registration
        {
            public required Func<object> FactoryMethod { get; init; }

            public required Lifetime Lifetime { get; init; }
        }

        public enum Lifetime
        {
            Singleton,
            Transient
        }

        /// <summary>
        /// Registrierung als Singleton
        /// </summary>
        public static void RegisterSingleton<TEnum>(TEnum id, Func<object> factory) where TEnum : struct, Enum
        {
            Register(id, factory, Lifetime.Singleton);
        }

        /// <summary>
        /// Registrierung als Transient
        /// </summary>
        public static void RegisterTransient<TEnum>(TEnum id, Func<object> factory) where TEnum : struct, Enum
        {
            Register(id, factory, Lifetime.Transient);
        }

        private static void Register<TEnum>(TEnum id, Func<object> factory, Lifetime lifetime) where TEnum : struct, Enum
        {
            var key = (typeof(TEnum), (object)id);

            _registrations[key] = new Registration
            {
                FactoryMethod = factory,
                Lifetime = lifetime
            };
        }

        /// <summary>
        /// Liefert eine Instanz
        /// </summary>
        public static T Get<T, TEnum>(TEnum id) where T : class where TEnum : struct, Enum
        {
            var key = (typeof(TEnum), (object)id);

            if (!_registrations.TryGetValue(key, out Registration registration))
            {
                throw new InvalidOperationException($"Keine Registrierung für '{typeof(TEnum).Name}.{id}'.");
            }

            object instance;

            switch (registration.Lifetime)
            {
                case Lifetime.Singleton:

                    Lazy<object> lazy = _singletons.GetOrAdd(key, _ =>
                    {
                        return new Lazy<object>(
                            registration.FactoryMethod,
                            isThreadSafe: true);
                    });

                    instance = lazy.Value;
                    break;

                case Lifetime.Transient:

                    instance = registration.FactoryMethod();
                    break;

                default:
                    throw new NotSupportedException();
            }

            if (instance is not T result)
            {
                throw new InvalidCastException($"Instanz ist nicht vom Typ '{typeof(T).Name}'.");
            }

            return result;
        }
    }
}
