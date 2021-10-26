using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MiniORM
{
    public abstract class DbContext
    {
        protected DbContext(string connectionString)
        {
            connection = new DatabaseConnection(connectionString);

            dbSetProperties = DiscoverDbSets();

            using (new ConnectionManager(connection))
            {
                InitializeDbSets();
            }

            MapAllRelations();
        }

        private readonly DatabaseConnection connection;

        private readonly Dictionary<Type, PropertyInfo> dbSetProperties;

        internal static readonly Type[] AllowedSqlTypes = {
            typeof(int),
            typeof(string),
            typeof(ulong),
            typeof(long),
            typeof(decimal),
            typeof(bool),
            typeof(DateTime)
        };

        public void SaveChanges()
        {
            object[] dbSets = dbSetProperties                   //може би var?
                .Select(p => p.Value.GetValue(this))
                .ToArray();

            foreach (IEnumerable<object> dbSet in dbSets)
            {
                object[] invalidEntities = dbSet
                    .Where(entity => !IsObjectValid(entity))
                    .ToArray();

                if (invalidEntities.Any())
                {
                    throw new InvalidOperationException
                        ($"{invalidEntities.Length} invalid entities found in the {dbSet.GetType().Name}!");
                }
            }

            using (new ConnectionManager(connection))
            {
                using SqlTransaction transaction = connection.StartTransaction();

                foreach (IEnumerable dbSet in dbSets)
                {
                    Type dbSetType = dbSet
                        .GetType()
                        .GetGenericArguments()
                        .First();

                    MethodInfo persistMethod = typeof(DbContext)
                        .GetMethod("Persist", BindingFlags.Instance | BindingFlags.NonPublic)?
                        .MakeGenericMethod(dbSetType);

                    try
                    {
                        persistMethod.Invoke(this, new object?[] { dbSet });
                    }
                    catch (TargetInvocationException e)
                    {
                        throw e.InnerException;
                    }
                    catch (InvalidOperationException e)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    catch (SqlException e)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

                transaction.Commit();
            }
        }

        private void Persist<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            string tableName = GetTableName(typeof(TEntity));

            string[] columns = connection
                .FetchColumnNames(tableName)
                .ToArray();

            if (dbSet.ChangeTracker.Added.Any())
            {
                connection.InsertEntities(dbSet
                    .ChangeTracker
                    .Added, tableName, columns);
            }

            TEntity[] modifiedEntities = dbSet
                .ChangeTracker
                .GetModifiedEntities(dbSet)
                .ToArray();

            if (modifiedEntities.Any())
            {
                connection.UpdateEntities(modifiedEntities, tableName, columns);
            }

            if (dbSet.ChangeTracker
                .Removed
                .Any())
            {
                connection.DeleteEntities(dbSet.ChangeTracker.Added, tableName, columns);
            }
        }

        private void MapRelations<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            MapNavigationProperties(dbSet);

            PropertyInfo[] collections = entityType
                .GetProperties()
                .Where(p => p.PropertyType.IsGenericType &&
                          p.PropertyType.GetGenericTypeDefinition() ==
                          typeof(ICollection<>))
                          .ToArray();

            foreach (PropertyInfo collection in collections)
            {
                Type collectionType =
                    collection.PropertyType
                    .GenericTypeArguments
                    .First();

                MethodInfo mapcollectionGenericMethod = typeof(DbContext)
                    .GetMethod("MapCollection", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entityType, collectionType);

                mapcollectionGenericMethod.Invoke(this, new object?[] { dbSet, collection });
            }
        }

        private void MapCollection<TDbSet, TCollection>(DbSet<TDbSet> dbSet,
            PropertyInfo collectionProperty)
            where TDbSet : class, new()
            where TCollection : class, new()
        {
            Type entityType = typeof(TDbSet);
            Type collectionType = typeof(TCollection);

            PropertyInfo[] primaryKeys = collectionType
                .GetProperties()
                .Where(p => p.HasAttribute<KeyAttribute>())
                .ToArray();

            PropertyInfo primaryKey = primaryKeys.First();

            PropertyInfo foreignKey = entityType
                .GetProperties()
                .First(p => p.HasAttribute<KeyAttribute>());

            bool isManyToMany = primaryKeys.Length >= 2;

            if (isManyToMany)
            {
                primaryKey = collectionType
                    .GetProperties()
                    .First(pi => collectionType
                        .GetProperty(pi.GetCustomAttribute<ForeignKeyAttribute>().Name)
                        .PropertyType == entityType);
            }

            DbSet<TCollection> navigationDbSet = (DbSet<TCollection>)
                dbSetProperties[collectionType].GetValue(this);

            foreach (var entity in dbSet)
            {
                object primaryKeyValue = foreignKey.GetValue(entity);
                var navigationEntities = navigationDbSet
                    .Where(navigationEntity => primaryKey.GetValue(navigationEntity)
                    .Equals(primaryKeyValue))
                    .ToArray();

                ReflectionHelper.ReplaceBackingField(entity, collectionProperty.Name, navigationEntities);
            }
        }

        private void MapNavigationProperties<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            var entityType = typeof(TEntity);

            var foreignKeys = entityType.GetProperties()
                .Where(pi => pi.HasAttribute<ForeignKeyAttribute>())
                .ToArray();

            foreach (var foreignKey in foreignKeys)
            {
                var navigationPropertyName =
                    foreignKey.GetCustomAttribute<ForeignKeyAttribute>()
                    .Name;

                var navigationProperty = entityType.GetProperty(navigationPropertyName);

                var navigationDbSet= dbSetProperties[navigationProperty.PropertyType]
                    .GetValue(this);

                var navigationPrimaryKey = navigationProperty.PropertyType.GetProperties()
                    .First(pi => pi.HasAttribute<KeyAttribute>());

                foreach (var entity in dbSet)
                {
                    var foreignKeyValue = foreignKey.GetValue(entity);

                    var navigationPropertyValue = ((IEnumerable<object>)navigationDbSet)
                        .First(currentNavigationProperty =>
                        navigationPrimaryKey.GetValue(currentNavigationProperty).Equals(foreignKeyValue));

                    navigationProperty.SetValue(entity, navigationPropertyValue);
                }
            }
        }

        private bool IsObjectValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationErrors = new List<ValidationResult>();

            var validationResult =
                Validator.TryValidateObject(entity, validationContext, validationErrors, validateAllProperties: true);

            return validationResult;
        }

        private void InitializeDbSets()
        {
            foreach (KeyValuePair<Type, PropertyInfo> dbSet in dbSetProperties)
            {
                Type dbSetType = dbSet.Key;
                PropertyInfo dbSetProperty = dbSet.Value;

                MethodInfo populateDbSetGeneric = typeof(DbContext)
                    .GetMethod("PopulateDbSet", BindingFlags.Instance |
                    BindingFlags.NonPublic)
                    .MakeGenericMethod(dbSetType);

                populateDbSetGeneric.Invoke(this, new object?[]
                    { dbSetProperty});
            }
        }

        private void PopulateDbSet<TEntity>(PropertyInfo dbSet)
            where TEntity : class, new()
        {
            IEnumerable<TEntity> entities = LoadTableEntities<TEntity>();

            DbSet<TEntity> dbSetInstance = new DbSet<TEntity>(entities);
            ReflectionHelper.ReplaceBackingField(this, dbSet.Name, dbSetInstance);
        }

        private void MapAllRelations()
        {
            foreach (KeyValuePair<Type, PropertyInfo> dbSetProperty in dbSetProperties)
            {
                Type dbSetType = dbSetProperty.Key;

                MethodInfo mapRelationsGenericMethod = typeof(DbContext)
                    .GetMethod("MapRelations", BindingFlags.Instance | BindingFlags.NonPublic)?
                    .MakeGenericMethod(dbSetType);

                object dbSet = dbSetProperty
                    .Value
                    .GetValue(this);

                mapRelationsGenericMethod?.Invoke(this, new object?[] { dbSet });
            }
        }

        private string GetTableName(Type tableType)
        {
            var tableName = ((TableAttribute)Attribute.GetCustomAttribute(tableType, typeof(TableAttribute)))?.Name;

            if (tableName==null)
            {
                tableName = dbSetProperties[tableType].Name;
            }

            return tableName
        }

        private Dictionary<Type, PropertyInfo> DiscoverDbSets()
        {
            var dbSets = this.GetType()
                .GetProperties()
                .Where(pi => pi.PropertyType
                    .GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToDictionary(pi => pi.PropertyType.GetGenericArguments().First(), pi => pi);

            return dbSets;
        }

        private IEnumerable<TEntity> LoadTableEntities<TEntity>()
            where TEntity : class, new()
        {
            Type table = typeof(TEntity);

            string[] columns = GetEntityColumnNames(table);

            string tableName = GetTableName(table);

            TEntity[] fetchedRows = connection
                .FetchResultSet<TEntity>(tableName, columns)
                .ToArray();

            return fetchedRows;
        }

        private string[] GetEntityColumnNames(Type table)
        {
            string tableName = GetTableName(table);
            IEnumerable<string> dbColumns = connection
                .FetchColumnNames(tableName);

            string[] columns = table.GetProperties()
                .Where(pi => dbColumns.Contains(pi.Name) &&
                !pi.HasAttribute<NotMappedAttribute>() &&
                AllowedSqlTypes.Contains(pi.PropertyType))
                .Select(pi => pi.Name)
                .ToArray();

            return columns;
        }
    }
}