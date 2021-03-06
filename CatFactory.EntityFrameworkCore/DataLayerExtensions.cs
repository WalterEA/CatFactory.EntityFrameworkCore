﻿using System.Collections.Generic;
using System.IO;
using CatFactory.Collections;
using CatFactory.EntityFrameworkCore.Definitions.Extensions;
using CatFactory.NetCore.CodeFactory;
using CatFactory.NetCore.ObjectOrientedProgramming;
using CatFactory.ObjectRelationalMapping;

namespace CatFactory.EntityFrameworkCore
{
    public static class DataLayerExtensions
    {
        public static EntityFrameworkCoreProject ScaffoldDataLayer(this EntityFrameworkCoreProject project)
        {
            ScaffoldMappings(project);
            ScaffoldDbContext(project);
            ScaffoldDataContracts(project);
            ScaffoldDataRepositories(project);
            ScaffoldReadMe(project);

            return project;
        }

        private static void ScaffoldMappings(EntityFrameworkCoreProject project)
        {
            var projectSelection = project.GlobalSelection();

            if (!projectSelection.Settings.UseDataAnnotations)
            {
                foreach (var table in project.Database.Tables)
                {
                    if (project.Database.HasDefaultSchema(table))
                        CSharpCodeBuilder.CreateFiles(project.OutputDirectory, project.GetDataLayerConfigurationsDirectory(), projectSelection.Settings.ForceOverwrite, project.GetEntityConfigurationClassDefinition(table));
                    else
                        CSharpCodeBuilder.CreateFiles(project.OutputDirectory, project.GetDataLayerConfigurationsDirectory(table.Schema), projectSelection.Settings.ForceOverwrite, project.GetEntityConfigurationClassDefinition(table));
                }

                foreach (var view in project.Database.Views)
                {
                    if (project.Database.HasDefaultSchema(view))
                        CSharpCodeBuilder.CreateFiles(project.OutputDirectory, project.GetDataLayerConfigurationsDirectory(), projectSelection.Settings.ForceOverwrite, project.GetEntityTypeConfigurationClassDefinition(view));
                    else
                        CSharpCodeBuilder.CreateFiles(project.OutputDirectory, project.GetDataLayerConfigurationsDirectory(view.Schema), projectSelection.Settings.ForceOverwrite, project.GetEntityTypeConfigurationClassDefinition(view));
                }
            }
        }

        private static void ScaffoldDbContext(EntityFrameworkCoreProject project)
        {
            var projectSelection = project.GlobalSelection();

            foreach (var projectFeature in project.Features)
            {
                CSharpCodeBuilder
                    .CreateFiles(project.OutputDirectory, project.GetDataLayerDirectory(), projectSelection.Settings.ForceOverwrite, projectFeature.GetDbContextClassDefinition(projectSelection));
            }
        }

        private static void ScaffoldDataLayerContract(EntityFrameworkCoreProject project, CSharpInterfaceDefinition interfaceDefinition)
            => CSharpCodeBuilder.CreateFiles(project.OutputDirectory, project.GetDataLayerContractsDirectory(), project.GlobalSelection().Settings.ForceOverwrite, interfaceDefinition);

        private static void ScaffoldRepositoryInterface(EntityFrameworkCoreProject project)
            => CSharpCodeBuilder.CreateFiles(project.OutputDirectory, project.GetDataLayerContractsDirectory(), project.GlobalSelection().Settings.ForceOverwrite, project.GetRepositoryInterfaceDefinition());

        private static void ScaffoldBaseRepositoryClassDefinition(EntityFrameworkCoreProject project)
            => CSharpCodeBuilder.CreateFiles(project.OutputDirectory, project.GetDataLayerRepositoriesDirectory(), project.GlobalSelection().Settings.ForceOverwrite, project.GetRepositoryBaseClassDefinition());

        private static void ScaffoldRepositoryExtensionsClassDefinition(EntityFrameworkCoreProject project)
            => CSharpCodeBuilder.CreateFiles(project.OutputDirectory, project.GetDataLayerRepositoriesDirectory(), project.GlobalSelection().Settings.ForceOverwrite, project.GetRepositoryExtensionsClassDefinition());

        private static void ScaffoldDataContracts(EntityFrameworkCoreProject project)
        {
            foreach (var table in project.Database.Tables)
            {
                var selection = project.GetSelection(table);

                if (!selection.Settings.EntitiesWithDataContracts)
                    continue;

                var classDefinition = project.GetDataContractClassDefinition(table);

                CSharpCodeBuilder.CreateFiles(project.OutputDirectory, project.GetDataLayerDataContractsDirectory(), selection.Settings.ForceOverwrite, classDefinition);
            }
        }

        private static void ScaffoldDataRepositories(EntityFrameworkCoreProject project)
        {
            var projectSelection = project.GlobalSelection();

            if (!string.IsNullOrEmpty(projectSelection.Settings.ConcurrencyToken))
            {
                projectSelection.Settings.InsertExclusions.Add(projectSelection.Settings.ConcurrencyToken);
                projectSelection.Settings.UpdateExclusions.Add(projectSelection.Settings.ConcurrencyToken);
            }

            ScaffoldRepositoryInterface(project);
            ScaffoldBaseRepositoryClassDefinition(project);
            ScaffoldRepositoryExtensionsClassDefinition(project);

            foreach (var projectFeature in project.Features)
            {
                var repositoryClassDefinition = projectFeature.GetRepositoryClassDefinition();

                var repositoryInterfaceDefinition = repositoryClassDefinition.RefactInterface();

                repositoryInterfaceDefinition.Namespace = project.GetDataLayerContractsNamespace();
                repositoryInterfaceDefinition.Implements.Add("IRepository");

                ScaffoldDataLayerContract(project, repositoryInterfaceDefinition);

                CSharpCodeBuilder.CreateFiles(project.OutputDirectory, project.GetDataLayerRepositoriesDirectory(), projectSelection.Settings.ForceOverwrite, repositoryClassDefinition);
            }
        }

        private static void ScaffoldReadMe(this EntityFrameworkCoreProject project)
        {
            var lines = new List<string>
            {
                "CatFactory: Scaffolding Made Easy",
                string.Empty,

                "How to use this code on your ASP.NET Core Application:",
                string.Empty,

                "1. Install EntityFrameworkCore.SqlServer package",
                string.Empty,

                "2. Register your DbContext and repositories in ConfigureServices method (Startup class):",
                string.Format("   services.AddDbContext<{0}>(options => options.UseSqlServer(\"ConnectionString\"));", project.GetDbContextName(project.Database)),

                "   services.AddScoped<IDboRepository, DboRepository>();",
                string.Empty,

                "Happy scaffolding!",
                string.Empty,

                "You can check the guide for this package in:",
                "https://www.codeproject.com/Articles/1160615/Scaffolding-Entity-Framework-Core-with-CatFactory",
                string.Empty,
                "Also you can check source code on GitHub:",
                "https://github.com/hherzl/CatFactory.EntityFrameworkCore",
                string.Empty,
                "CatFactory Development Team ==^^=="
            };

            File.WriteAllText(Path.Combine(project.OutputDirectory, "CatFactory.EntityFrameworkCore.ReadMe.txt"), lines.ToStringBuilder().ToString());
        }
    }
}
