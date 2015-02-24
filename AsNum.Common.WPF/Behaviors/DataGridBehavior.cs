﻿using AsNum.Common.Extends;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace AsNum.Common.WPF.Behaviors {

    public class DataGridBehavior : Behavior<DataGrid> {

        protected override void OnAttached() {
            AssociatedObject.AutoGeneratingColumn += new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
            AssociatedObject.AutoGeneratedColumns += AssociatedObject_AutoGeneratedColumns;
        }

        void AssociatedObject_AutoGeneratedColumns(object sender, EventArgs e) {
            //数据源只能是 IList 
            var t = AssociatedObject.ItemsSource.GetType().GetGenericArguments()[0];
            var ps = TypeDescriptor.GetProperties(t, null).Cast<PropertyDescriptor>();
            var cc = AssociatedObject.Columns
                .Select(c => (DataGridBoundColumn)c)
                .Where(c => c != null && (Binding)c.Binding != null)
                .Select(c => ((Binding)c.Binding).Path.Path);

            var pom = ps.Select(p => {
                var da = p.Attributes.OfType<DisplayAttribute>().FirstOrDefault();
                if (da != null)
                    return new { p.Name, da.Order };
                else
                    return null;
            }).Where(p => p != null);

            var poms = pom.OrderBy(p => p.Order);
            var pom2 = new Dictionary<string, int>();
            int i = 0;
            foreach (var p in poms) {
                pom2.Add(p.Name, i++);
            }

            ////通过TypeDescriptor注册的 PropertyDescriptor
            ////即排除原生的 Property
            //ps = ps.Where(p => {
            //    var ba = p.Attributes.OfType<BrowsableAttribute>().FirstOrDefault();
            //    if (ba != null) {
            //        return !cc.Any(c => c.Equals(p.Name)) && ba.Browsable;
            //    } else {
            //        return !cc.Any(c => c.Equals(p.Name));
            //    }
            //});

            ////产生通过 TypeDescriptor 注册的 PropertyDescriptor 对应的列
            //DataGrid.GenerateColumns(new ItemProperties(ps)).ToList()
            //    .ForEach(c => {
            //        var col = (DataGridBoundColumn)c;
            //        if (col == null)
            //            AssociatedObject.Columns.Add(c);
            //        else {
            //            AssociatedObject.Columns.Add(col);
            //        }
            //    });

            //设置列的 DisplayIndex
            AssociatedObject.Columns.ToList().ForEach(c => {
                if (c.GetType().IsSubclassOf(typeof(DataGridBoundColumn))) {
                    var col = (DataGridBoundColumn)c;
                    var binding = (Binding)col.Binding;
                    var idx = pom2.Get(binding.Path.Path, -1);
                    if (binding != null && idx != -1)
                        col.DisplayIndex = idx;
                }
            });
        }

        protected override void OnDetaching() {
            AssociatedObject.AutoGeneratingColumn -= new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
            AssociatedObject.AutoGeneratedColumns -= new EventHandler(AssociatedObject_AutoGeneratedColumns);
        }

        protected void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
            var pd = (PropertyDescriptor)e.PropertyDescriptor;
            var ba = pd.Attributes.OfType<BrowsableAttribute>().FirstOrDefault();
            if (ba != null && ba.Browsable == false) {
                e.Cancel = true;
            } else {
                var dna = pd.Attributes.OfType<DisplayAttribute>().FirstOrDefault();
                if (dna != null) {
                    e.Column.Header = dna.Name;
                    //e.Column.DisplayIndex = dna.Order;
                }
            }
        }
    }


    class ItemProperties : IItemProperties {
        private readonly ReadOnlyCollection<ItemPropertyInfo> itemProperties;

        public ItemProperties(IEnumerable<PropertyDescriptor> itemProperties) {
            this.itemProperties = new ReadOnlyCollection<ItemPropertyInfo>(itemProperties.Select(ip => new ItemPropertyInfo(ip.Name, ip.PropertyType, ip)).ToArray());
        }

        ReadOnlyCollection<ItemPropertyInfo> IItemProperties.ItemProperties {
            get { return this.itemProperties; }
        }
    }

}
