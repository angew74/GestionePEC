using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delta.Utilities.LinqExtensions
{
    public class HierarchyNode<T> where T : class
    {
        public T Entity { get; set; }
        public IEnumerable<HierarchyNode<T>> ChildNodes { get; set; }
        public int Depth { get; set; }
        public T Parent { get; set; }
    }

    public static class LinqExtensionMethods
    {
        private static IEnumerable<HierarchyNode<TEntity>> CreateHierarchy<TEntity, TProperty>(
            IEnumerable<TEntity> allItems,
            TEntity parentItem,
            Func<TEntity, TProperty> idProperty,
            Func<TEntity, TProperty> parentIdProperty,
            Func<TEntity, TProperty> orderIdProperty,
            object rootItemId,
            int maxDepth,
            int depth) where TEntity : class
        {
            IEnumerable<TEntity> childs;

            if (rootItemId != null)
            {
                childs = allItems.Where(i => idProperty(i).Equals(rootItemId));
            }
            else
            {
                if (parentItem == null)
                    childs = allItems.Where(i => parentIdProperty(i).Equals(default(TProperty)));
                else
                    childs = allItems.Where(i => parentIdProperty(i).Equals(idProperty(parentItem)));
            }

            if (childs.Count() > 0)
            {
                if (orderIdProperty != null)
                    childs = childs.OrderBy(i => orderIdProperty(i)).AsEnumerable();
                depth++;

                if ((depth <= maxDepth) || (maxDepth == 0))
                {
                    foreach (var item in childs)
                        yield return new HierarchyNode<TEntity>()
                        {
                            Entity = item,
                            ChildNodes = CreateHierarchy<TEntity, TProperty>(allItems.AsEnumerable(), item, idProperty, parentIdProperty, orderIdProperty, null, maxDepth, depth),
                            Depth = depth,
                            Parent = parentItem
                        };
                }
            }
        }

        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
            this IEnumerable<TEntity> allItems,
            Func<TEntity, TProperty> idProperty,
            Func<TEntity, TProperty> parentIdProperty)
            where TEntity : class
        {
            return CreateHierarchy<TEntity, TProperty>(allItems, default(TEntity),
                idProperty, parentIdProperty, null, null, 0, 0);
        }

        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
            this IEnumerable<TEntity> allItems,
            Func<TEntity, TProperty> idProperty,
            Func<TEntity, TProperty> parentIdProperty,
            Func<TEntity, TProperty> orderIdProperty)
            where TEntity : class
        {
            return CreateHierarchy<TEntity, TProperty>(allItems, default(TEntity),
                idProperty, parentIdProperty, orderIdProperty, null, 0, 0);
        }

        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
            this IEnumerable<TEntity> allItems,
            Func<TEntity, TProperty> idProperty,
            Func<TEntity, TProperty> parentIdProperty,
            object rootItemId) where TEntity : class
        {
            return CreateHierarchy<TEntity, TProperty>(allItems, default(TEntity),
                idProperty, parentIdProperty, null, rootItemId, 0, 0);
        }

        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
            this IEnumerable<TEntity> allItems,
            Func<TEntity, TProperty> idProperty,
            Func<TEntity, TProperty> parentIdProperty,
            Func<TEntity, TProperty> orderIdProperty,
            object rootItemId) where TEntity : class
        {
            return CreateHierarchy<TEntity, TProperty>(allItems, default(TEntity),
                idProperty, parentIdProperty, orderIdProperty, rootItemId, 0, 0);
        }

        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
            this IEnumerable<TEntity> allItems,
            Func<TEntity, TProperty> idProperty,
            Func<TEntity, TProperty> parentIdProperty,
            object rootItemId,
            int maxDepth) where TEntity : class
        {
            return CreateHierarchy<TEntity, TProperty>(allItems, default(TEntity),
                idProperty, parentIdProperty, null, rootItemId, maxDepth, 0);
        }

        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
            this IEnumerable<TEntity> allItems,
            Func<TEntity, TProperty> idProperty,
            Func<TEntity, TProperty> parentIdProperty,
            Func<TEntity, TProperty> orderIdProperty,
            object rootItemId,
            int maxDepth) where TEntity : class
        {
            return CreateHierarchy<TEntity, TProperty>(allItems, default(TEntity),
                idProperty, parentIdProperty, orderIdProperty, rootItemId, maxDepth, 0);
        }
    }
}