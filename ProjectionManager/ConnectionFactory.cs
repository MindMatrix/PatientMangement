﻿using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace ProjectionManager
{
    public class ConnectionFactory
    {
        private readonly IDocumentStore _store;

        public ConnectionFactory(string database)
        {
            _store = new DocumentStore
            {
                Urls = new string[] { "http://localhost:8080/" },
                Database = database
            };
            _store.Initialize();
        }

        public IDocumentSession Connect()
        {
            return _store.OpenSession();
        }
    }
}