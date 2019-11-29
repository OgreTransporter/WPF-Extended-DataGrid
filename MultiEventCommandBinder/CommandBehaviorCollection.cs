﻿using System;
using System.Collections.Specialized;
using System.Windows;

namespace MultiEventCommandBinder
{
    public class CommandBehaviorCollection
    {
        #region Behaviors

// ReSharper disable StaticFieldInitializersReferesToFieldBelow
        public static readonly DependencyProperty BehaviorsProperty= BehaviorsPropertyKey.DependencyProperty;
// ReSharper restore StaticFieldInitializersReferesToFieldBelow

        /// <summary>
        /// Behaviors Read-Only Dependency Property
        /// As you can see the Attached readonly property has a name registered different (BehaviorsInternal) than the property name, 
        /// this is a tricks os that we can construct the collection as we want
        /// Read more about this here http://wekempf.spaces.live.com/blog/cns!D18C3EC06EA971CF!468.entry
        /// </summary>
        private static readonly DependencyPropertyKey BehaviorsPropertyKey
            = DependencyProperty.RegisterAttachedReadOnly("BehaviorsInternal", typeof(BehaviorBindingCollection),
                                                          typeof(CommandBehaviorCollection),
                                                          new FrameworkPropertyMetadata((BehaviorBindingCollection)null));

        /// <summary>
        /// Gets the Behaviors property.  
        /// Here we initialze the collection and set the Owner property
        /// </summary>
        public static BehaviorBindingCollection GetBehaviors(DependencyObject d)
        {
            if (d == null)
                throw new InvalidOperationException("The dependency object trying to attach to is set to null");

            var collection = d.GetValue(BehaviorsProperty) as BehaviorBindingCollection;
            if (collection == null)
            {
                collection = new BehaviorBindingCollection {Owner = d};
                SetBehaviors(d, collection);
            }
            return collection;
        }

        /// <summary>
        /// Provides a secure method for setting the Behaviors property.  
        /// This dependency property indicates ....
        /// </summary>
        private static void SetBehaviors(DependencyObject d, BehaviorBindingCollection value)
        {
            d.SetValue(BehaviorsPropertyKey, value);
            INotifyCollectionChanged collection = value;
            collection.CollectionChanged += CollectionChanged;
        }

        private static void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var sourceCollection = (BehaviorBindingCollection)sender;
            switch (e.Action)
            {
                //when an item(s) is added we need to set the Owner property implicitly
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                        foreach (BehaviorBinding item in e.NewItems)
                            item.Owner = sourceCollection.Owner;
                    break;
                //when an item(s) is removed we should Dispose the BehaviorBinding
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                        foreach (BehaviorBinding item in e.OldItems)
                            item.Behavior.Dispose();
                    break;

                //here we have to set the owner property to the new item and unregister the old item
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                        foreach (BehaviorBinding item in e.NewItems)
                            item.Owner = sourceCollection.Owner;

                    if (e.OldItems != null)
                        foreach (BehaviorBinding item in e.OldItems)
                            item.Behavior.Dispose();
                    break;

                //when an item(s) is removed we should Dispose the BehaviorBinding
                case NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null)
                        foreach (BehaviorBinding item in e.OldItems)
                            item.Behavior.Dispose();
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// Collection to store the list of behaviors. This is done so that you can intiniate it from XAML
    /// This inherits from freezable so that it gets inheritance context for DataBinding to work
    /// </summary>
    public class BehaviorBindingCollection : FreezableCollection<BehaviorBinding>
    {
        /// <summary>
        /// Gets or sets the Owner of the binding
        /// </summary>
        public DependencyObject Owner { get; set; }
    }
}
