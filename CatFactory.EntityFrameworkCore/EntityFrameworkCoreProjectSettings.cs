﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using CatFactory.CodeFactory.Scaffolding;

namespace CatFactory.EntityFrameworkCore
{
    public class EntityFrameworkCoreProjectSettings : ProjectSettings
    {
        public EntityFrameworkCoreProjectSettings()
        {
        }

        public bool ForceOverwrite { get; set; }

        public bool SimplifyDataTypes { get; set; } = true;

        public bool UseAutomaticPropertiesForEntities { get; set; } = true;

        public bool EnableDataBindings { get; set; }

        public bool UseDataAnnotations { get; set; }

        [Obsolete("Temporarily disabled")]
        public bool UseMefForEntitiesMapping { get; set; } = true;

        public bool DeclareDbSetPropertiesInDbContext { get; } = true;

        public bool DeclareNavigationProperties { get; set; } = true;

        public bool DeclareNavigationPropertiesAsVirtual { get; set; }

        public string NavigationPropertyEnumerableNamespace { get; set; } = "System.Collections.ObjectModel";

        public string NavigationPropertyEnumerableType { get; set; } = "Collection";

        public string ConcurrencyToken { get; set; }

        public string EntityInterfaceName { get; set; } = "IEntity";

        public AuditEntity AuditEntity { get; set; }

        public bool EntitiesWithDataContracts { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<string> m_backingFields;

        public List<string> BackingFields
        {
            get
            {
                return m_backingFields ?? (m_backingFields = new List<string>());
            }
            set
            {
                m_backingFields = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<string> m_insertExclusions;

        public List<string> InsertExclusions
        {
            get
            {
                return m_insertExclusions ?? (m_insertExclusions = new List<string>());
            }
            set
            {
                m_insertExclusions = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<string> m_updateExclusions;

        public List<string> UpdateExclusions
        {
            get
            {
                return m_updateExclusions ?? (m_updateExclusions = new List<string>());
            }
            set
            {
                m_updateExclusions = value;
            }
        }
    }
}
