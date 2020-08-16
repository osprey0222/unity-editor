﻿using System.Collections.Generic;

namespace Toolbox.Editor
{
    /// <summary>
    /// Utility class to handle Hierarchy Window related data.
    /// </summary>
    internal static class ToolboxHierarchyUtility
    {
        private readonly static List<HierarchyObjectDataItem> rowDataItems = new List<HierarchyObjectDataItem>(4);

        /// <summary>
        /// Settings provided to handle Hierarchy Window overlay.
        /// </summary>
        private static IToolboxHierarchySettings settings;


        internal static void PerformData()
        {
            PerformData(settings);
        }

        internal static void PerformData(IToolboxHierarchySettings settings)
        {
            ToolboxHierarchyUtility.settings = settings;

            ToolboxHierarchyAllowed = settings != null ? settings.UseToolboxHierarchy : false;

            if (settings == null)
            {
                return;
            }

            rowDataItems.Clear();
            for (var i = 0; i < settings.RowDataItemsCount; i++)
            {
                rowDataItems.Add(settings.GetRowDataItemAt(i));
            }

            AreRowDataItemsUpdated = true;
            HorizontalLinesAllowed = settings.DrawHorizontalLines;
        }

        internal static HierarchyObjectDataItem[] GetRowDataItems()
        {
            AreRowDataItemsUpdated = false;
            return rowDataItems.ToArray();
        }


        internal static bool ToolboxHierarchyAllowed { get; private set; }

        internal static bool HorizontalLinesAllowed { get; private set; }

        internal static bool AreRowDataItemsUpdated { get; private set; }
    }
}