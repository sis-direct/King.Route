﻿namespace King.Route.Demo.MVC.DataLayer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using King.Azure.Data;
    using King.Route.Demo.MVC.Models;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage.Table;

    [RouteAlias("CompanyStorage")]
    public class CompanyStorage
    {
        private static readonly string connection = CloudConfigurationManager.GetSetting("StorageAccount");
        private readonly ITableStorage storage = new TableStorage("companyroute", connection);

        public async Task Save(Company company)
        {
            var c = new TableEntity()
            {
                RowKey = company.Identifier.ToString(),
                PartitionKey = company.Name,
            };

            await storage.InsertOrReplace(c);
        }

        public Company Search(Guid id, string name)
        {
            if (Guid.Empty != id && !string.IsNullOrWhiteSpace(name))
            {
                var task = this.storage.QueryByPartitionAndRow(name, id.ToString());

                task.Wait();
                var cs = task.Result;

                return new Company()
                {
                    Identifier = Guid.Parse(cs[TableStorage.RowKey] as string),
                    Name = cs[TableStorage.PartitionKey] as string
                };
            }
            //else if (Guid.Empty != id)
            //{
            //    return await this.storage.QueryByRow<Company>(id.ToString());
            //}
            //else if (!string.IsNullOrWhiteSpace(name))
            //{
            //    return await this.storage.QueryByPartition<Company>(name);
            //}
            else
            {
                return null;
            }
        }
    }
}