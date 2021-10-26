using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MiniORM
{
    class ChangeTracker<T>
        where T : class, new()
    {
        private readonly List<T> allEntities;
        private readonly List<T> removed;
        private readonly List<T> added;

        public ChangeTracker(IEnumerable<T> entities)
        {
            added = new List<T>();
            removed = new List<T>();

            allEntities = CloneEntities(entities);
        }

        public IReadOnlyCollection<T> AllEntities => allEntities;
        public IReadOnlyCollection<T> Added => added;
        public IReadOnlyCollection<T> Removed => removed;

        private static List<T> CloneEntities(IEnumerable<T> entities)
        {
            var clonedEntities = new List<T>();

            PropertyInfo[] propertiesToClone = typeof(T)
                .GetProperties()
                .Where(x => DbContext.AllowedSqlTypes.Contains(x.PropertyType))
                .ToArray();

            foreach (T e in entities)
            {
                T clonedEntity = Activator.CreateInstance<T>();

                foreach (PropertyInfo p in propertiesToClone)
                {
                    object value = p.GetValue(e);
                    p.SetValue(clonedEntities, value);
                }

                clonedEntities.Add(clonedEntity);
            }

            return clonedEntities;
        }

        public void Add(T entity) => added.Add(entity);
        public void Remove(T entity) => removed.Add(entity);

        public IEnumerable<T> GetModifiedEntities(DbSet<T> dbSet)
        {
            var modifiedEntities = new List<T>();

            PropertyInfo[] primaryKeys = typeof(T)
                .GetProperties()
                .Where(x => x.HasAttribute<KeyAttribute>())
                .ToArray();

            foreach (T proxyEntity in allEntities)
            {
                object[] primaryKeyValues = GetPrimaryKeyValues(primaryKeys, proxyEntity)
                    .ToArray();

                T dbEntity = dbSet
                    .Entities
                    .Single(e => GetPrimaryKeyValues(primaryKeys, e)
                        .SequenceEqual(primaryKeyValues));

                bool isModified = IsModified(proxyEntity, dbEntity);

                if (isModified)
                {
                    modifiedEntities.Add(dbEntity);
                }
            }

            return modifiedEntities;
        }

        private static bool IsModified(T proxyEntity, T dbEntity)
        {
            PropertyInfo[] monitoredProperties=typeof(T)
                .GetProperties()
                .Where(p=>DbContext.AllowedSqlTypes.Contains(p.PropertyType))
                .ToArray();

            PropertyInfo[] modifiedProperties = monitoredProperties
                .Where(p => !Equals(p.GetValue(proxyEntity), p.GetValue(dbEntity)))
                .ToArray();

            return modifiedProperties.Any();
        }
        private static IEnumerable<object> GetPrimaryKeyValues
            (IEnumerable<PropertyInfo> primaryKeys, T entity) => primaryKeys.Select(pk => pk.GetValue(entity));
    }
}