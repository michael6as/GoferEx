using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using GoferEx.Core;
using Dapper;

namespace GoferEx.Storage
{
    public class SqlProvider : IDbProvider
    {
        private SqlClientFactory _connFactory;
        private string _connString;

        public SqlProvider(string connString)
        {
            _connFactory = SqlClientFactory.Instance;            
            _connString = connString;
        }

        public bool AddContacts(List<Contact> contacts)
        {
            string sql = "INSERT INTO Contacts (CustomerName) Values (@CustomerName);";
            using (var conn = _connFactory.CreateConnection())
            {
                conn.ConnectionString = _connString;
                conn.Open();
                var added = conn.Execute(sql, contacts);
            }

            return true;
        }

        public Contact GetContact(Guid id)
        {
            string sql = "SELECT * FROM Contacts WHERE Id = @Id";
            using (var conn = _connFactory.CreateConnection())
            {
                conn.ConnectionString = _connString;
                conn.Open();
                var contact = conn.Query<Contact>(sql, new {Id = id.ToString()});
                return contact.FirstOrDefault();
            }
        }

        public List<Contact> GetContacts()
        {
            string sql = "SELECT * FROM Contacts";
            using (var conn = _connFactory.CreateConnection())
            {
                conn.ConnectionString = _connString;
                conn.Open();
                var contacts = conn.Query<Contact>(sql);
                return contacts.ToList();
            }
        }

        public bool RemoveContact(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateContacts(List<Contact> contacts)
        {
            throw new NotImplementedException();
        }
    }
}
