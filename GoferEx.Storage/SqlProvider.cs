using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using GoferEx.Core;
using Dapper;
using System.Threading.Tasks;

namespace GoferEx.Storage
{
    public class SqlProvider : IDbProvider
    {
        private SqlConnection _sqlConn;
        private string _connString;

        public SqlProvider(string connString)
        {
            _sqlConn = new SqlConnection(connString);
        }

        public async Task<bool> AddContacts(List<Contact> contacts)
        {
            string sql = "INSERT INTO Contacts (Id, FirstName, LastName, Username, EmailAddress, BirthDate, PhoneNumber, Password) Values (@Id, @FirstName, @LastName, @Username, @EmailAddress, @BirthDate, @PhoneNumber, @Password);";
            await _sqlConn.OpenAsync();
            var added = _sqlConn.Execute(sql, contacts);
            _sqlConn.Close();
            if (added == contacts.Count)
            {
                return true;
            }

            return false;
        }

        public async Task<Contact> GetContact(Guid id)
        {
            using (var sq = new SqlConnection("aaa"))
            {

            }
            string sql = "SELECT * FROM Contacts WHERE Id = @Id";
            await _sqlConn.OpenAsync();
            var contact = _sqlConn.Query<Contact>(sql, new { Id = id.ToString() });
            _sqlConn.Close();
            return contact.FirstOrDefault();
        }

        public async Task<IEnumerable<Contact>> GetContacts()
        {
            string sql = "SELECT * FROM Contacts";
            await _sqlConn.OpenAsync();
            var contacts = _sqlConn.Query<Contact>(sql);
            _sqlConn.Close();
            return contacts;
        }

        public async Task<bool> RemoveContact(Guid id)
        {
            string sql = "DELETE * FROM Contacts WHERE Id = @id";
            await _sqlConn.OpenAsync();
            _sqlConn.Execute(sql, new { Id = id.ToString() });
            _sqlConn.Close();
            return true;
        }

        public Task<bool> UpdateContacts(List<Contact> contacts)
        {
            throw new NotImplementedException();
        }
    }
}
